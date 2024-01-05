using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SystemScripts
{
    /// <summary>
    /// Manages the game's status, including player score, high score, coins collected,
    /// level information, and various UI elements.
    /// </summary>
    public class GameStatusController : MonoBehaviour
    {
        public TextMeshProUGUI playerScoreText; // Text component for the player's score.
        public TextMeshProUGUI playerHighScoreText; // Text component for the player's high score.
        public TextMeshProUGUI collectedCoinText; // Text component for the coins collected.
        public TextMeshProUGUI levelText; // Text component for the current level.
        public TextMeshProUGUI secondsText; // Text component for the time remaining in seconds.
        public TextMeshProUGUI livesText; // Text component for the player's lives.
        public GameObject score200Prefab; // Prefab for displaying a 200 score.
        public GameObject score1000Prefab; // Prefab for displaying a 1000 score.
        public GameObject pausePopup; // Popup for the pause menu.
        public GameObject instructionPopup; // Popup for instructions.
        public GameObject creditPopup; // Popup for credits.
        public GameObject firstMessagePopup; // Popup for the first message.
        public GameObject secondMessagePopup; // Popup for the second message.
        public Transform scoreParent; // Parent transform for score display.
        private AudioSource _gameStatusAudio; // AudioSource for playing game status related sounds.

        public AudioClip pauseSound; // Sound clip for pause action.
        public AudioClip stageClearSound; // Sound clip for stage clear.

        private bool _pauseTrigger; // Flag to check if the game is paused.

        public static int CollectedCoin; // Total number of coins collected.
        public static int Score; // Current player score.
        private static int _highScore; // Highest score achieved.
        public static int Live; // Number of lives the player has.
        public static int CurrentLevel; // Current game level.
        public static bool IsDead; // Flag to check if the player is dead.
        public static bool IsGameOver; // Flag to check if the game is over.
        public static bool IsStageClear; // Flag to check if the stage is cleared.
        public static bool IsBigPlayer; // Flag to check if the player is in a "big" state.
        public static bool IsFirePlayer; // Flag to check if the player is in a "fire" state.
        public static bool IsBossBattle; // Flag to check if the boss battle is active.
        public static bool IsGameFinish; // Flag to check if the game is finished.
        public static bool IsEnemyDieOrCoinEat; // Flag for enemy defeat or coin collection.
        public static bool IsPowerUpEat; // Flag to check if a power-up is eaten.
        public static bool IsShowMessage; // Flag to check if a message should be shown.
        public static string PlayerTag; // Tag for the player GameObject.
        private float _second; // Internal counter for seconds.

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// Performs initial setup for the game status controller.
        /// </summary>
        private void Awake()
        {
            SetScore(playerHighScoreText, _highScore);
            _gameStatusAudio = GetComponent<AudioSource>();
            _pauseTrigger = false;
        }

        /// <summary>
        /// Update is called once per frame and handles various game status updates.
        /// </summary>
        private void Update()
        {
            // Play stage clear sound when the stage is cleared.
            if (IsStageClear)
            {
                _gameStatusAudio.PlayOneShot(stageClearSound);
                IsStageClear = false;
            }

            // Display first message if required.
            if (IsShowMessage)
            {
                StartCoroutine(DisplayFirstMessage());
            }

            // Update score popup for enemy defeat or coin collection.
            if (IsEnemyDieOrCoinEat)
            {
                IsEnemyDieOrCoinEat = false;
                UpdateScorePopup(score200Prefab);
            }

            // Update score popup for power-up collection.
            if (IsPowerUpEat)
            {
                IsPowerUpEat = false;
                UpdateScorePopup(score1000Prefab);
            }

            // Update the high score if the current score exceeds it.
            if (Score > _highScore)
            {
                _highScore = Score;
            }

            // Update UI elements for coins, level, score, and lives.
            SetCoin();
            SetLevel();
            SetScore(playerScoreText, Score);
            SetLive();
            Pause();
        }


        /// <summary>
        /// Sets the score text with leading zeros based on the score value.
        /// </summary>
        /// <param name="scoreText">The TextMeshProUGUI component to update.</param>
        /// <param name="score">The score value to display.</param>
        private void SetScore(TextMeshProUGUI scoreText, int score)
        {
            // Formatting the score display based on the length of the score value.
            switch (score.ToString().Length)
            {
                case 0:
                    scoreText.SetText("000000");
                    break;
                case 3:
                    scoreText.SetText($"000{score}");
                    break;
                case 4:
                    scoreText.SetText($"00{score}");
                    break;
                case 5:
                    scoreText.SetText($"0{score}");
                    break;
                case 6:
                    scoreText.SetText($"{score}");
                    break;
            }
        }

        /// <summary>
        /// Updates the collected coin display.
        /// </summary>
        private void SetCoin()
        {
            // Display the coin count with leading zero if needed.
            if (CollectedCoin > 0)
            {
                collectedCoinText.SetText($"x0{CollectedCoin}");
                if (CollectedCoin <= 9) return;
                collectedCoinText.SetText($"x{CollectedCoin}");
                if (CollectedCoin > 99)
                {
                    collectedCoinText.SetText("x00");
                }
            }
            else
            {
                collectedCoinText.SetText("x00");
            }
        }

        /// <summary>
        /// Sets the remaining time display.
        /// </summary>
        /// <param name="second">The remaining time in seconds.</param>
        public void SetTime(float second)
        {
            // Formatting the time display based on the amount of time left.
            _second = second;
            if (_second > 0)
            {
                if (_second > 99.5f)
                {
                    secondsText.SetText(Mathf.RoundToInt(_second).ToString());
                }
                else if (_second > 9.5f)
                {
                    secondsText.SetText($"0{Mathf.RoundToInt(_second).ToString()}");
                }
                else
                {
                    secondsText.SetText($"00{Mathf.RoundToInt(_second).ToString()}");
                }
            }
            else
            {
                secondsText.SetText("000");
            }
        }

        /// <summary>
        /// Sets the current level text to the active scene's name.
        /// </summary>
        private void SetLevel()
        {
            levelText.SetText(SceneManager.GetActiveScene().name);
        }

        /// <summary>
        /// Updates the lives display based on the current number of lives.
        /// </summary>
        private void SetLive()
        {
            livesText.SetText($"x {Live.ToString()}");
        }

        /// <summary>
        /// Toggles the pause state of the game.
        /// </summary>
        private void Pause()
        {
            if (SceneManager.GetActiveScene().buildIndex > 1)
            {
                if (Input.GetKeyDown(KeyCode.P))
                {
                    _gameStatusAudio.PlayOneShot(pauseSound);
                    _pauseTrigger = !_pauseTrigger;
                    pausePopup.SetActive(_pauseTrigger);
                    Time.timeScale = _pauseTrigger ? 0 : 1;
                }
            }
        }

        /// <summary>
        /// Starts the game by loading the first level.
        /// </summary>
        public void StartGame()
        {
            SceneManager.LoadScene(1);
            CurrentLevel = 2;
            Live = 3;
            Score = 0;
            CollectedCoin = 0;
            PlayerTag = "Player";
        }
        /// <summary>
        /// Opens the instruction popup.
        /// </summary>
        public void OpenInstructionPopup()
        {
            instructionPopup.SetActive(true);
        }

        /// <summary>
        /// Opens the credit popup.
        /// </summary>
        public void OpenCreditPopup()
        {
            creditPopup.SetActive(true);
        }

        /// <summary>
        /// Closes the instruction popup.
        /// </summary>
        public void CloseInstructionPopup()
        {
            instructionPopup.SetActive(false);
        }

        /// <summary>
        /// Closes the credit popup.
        /// </summary>
        public void CloseCreditPopup()
        {
            creditPopup.SetActive(false);
        }

        /// <summary>
        /// Exits the game and returns to the main menu.
        /// </summary>
        public void ExitGame()
        {
            SceneManager.LoadScene(0);
            Time.timeScale = 1;
        }

        /// <summary>
        /// Instantiates a score popup and starts a coroutine to destroy it.
        /// </summary>
        /// <param name="scorePrefab">The score popup prefab to be instantiated.</param>
        private void UpdateScorePopup(GameObject scorePrefab)
        {
            GameObject score = Instantiate(scorePrefab, scoreParent);
            StartCoroutine(DestroyScorePrefab(score));
        }

        /// <summary>
        /// Coroutine to destroy a score popup after a delay.
        /// </summary>
        /// <param name="prefab">The score popup GameObject to be destroyed.</param>
        private IEnumerator DestroyScorePrefab(GameObject prefab)
        {
            yield return new WaitForSeconds(1);
            Destroy(prefab);
        }

        /// <summary>
        /// Coroutine to display the first message popup.
        /// </summary>
        private IEnumerator DisplayFirstMessage()
        {
            yield return new WaitForSeconds(1);
            firstMessagePopup.SetActive(true);
            StartCoroutine(DisplaySecondMessage());
        }

        /// <summary>
        /// Coroutine to display the second message popup after a delay.
        /// </summary>
        private IEnumerator DisplaySecondMessage()
        {
            yield return new WaitForSeconds(1.5f);
            secondMessagePopup.SetActive(true);
        }
    }
}