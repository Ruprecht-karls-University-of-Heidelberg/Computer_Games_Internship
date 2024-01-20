using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using AdditionalScripts;

/// <summary>
/// Namespace containing system scripts for game status control.
/// </summary>
namespace SystemScripts
{
    /// <summary>
    /// Class that controls the game status and UI elements.
    /// </summary>
    public class GameStatusController : MonoBehaviour
    {
        /// <summary>
        /// Reference to the player score text UI element.
        /// </summary>
        public TextMeshProUGUI playerScoreText;

        /// <summary>
        /// Reference to the player high score text UI element.
        /// </summary>
        public TextMeshProUGUI playerHighScoreText;

        /// <summary>
        /// Reference to the collected coin text UI element.
        /// </summary>
        public TextMeshProUGUI collectedCoinText;

        /// <summary>
        /// Reference to the level text UI element.
        /// </summary>
        public TextMeshProUGUI levelText;

        /// <summary>
        /// Reference to the seconds text UI element.
        /// </summary>
        public TextMeshProUGUI secondsText;

        /// <summary>
        /// Reference to the lives text UI element.
        /// </summary>
        public TextMeshProUGUI livesText;

        /// <summary>
        /// Prefab for the score 200 popup.
        /// </summary>
        public GameObject score200Prefab;

        /// <summary>
        /// Prefab for the score 1000 popup.
        /// </summary>
        public GameObject score1000Prefab;

        /// <summary>
        /// Reference to the pause popup UI element.
        /// </summary>
        public GameObject pausePopup;

        /// <summary>
        /// Reference to the instruction popup UI element.
        /// </summary>
        public GameObject instructionPopup;

        /// <summary>
        /// Reference to the credit popup UI element.
        /// </summary>
        public GameObject creditPopup;

        /// <summary>
        /// Reference to the first message popup UI element.
        /// </summary>
        public GameObject firstMessagePopup;

        /// <summary>
        /// Reference to the second message popup UI element.
        /// </summary>
        public GameObject secondMessagePopup;

        /// <summary>
        /// Parent transform for the score popups.
        /// </summary>
        public Transform scoreParent;

        private AudioSource _gameStatusAudio;

        /// <summary>
        /// Audio clip for the pause sound.
        /// </summary>
        public AudioClip pauseSound;

        /// <summary>
        /// Audio clip for the stage clear sound.
        /// </summary>
        public AudioClip stageClearSound;

        private bool _pauseTrigger;

        private float _second;

        private void Awake()
        {
            InitializeHighScoreDisplay();
            PrepareAudioSource();
            ResetPauseTrigger();
        }

        /// <summary>
        /// Sets the high score on the UI at the start.
        /// </summary>
        private void InitializeHighScoreDisplay()
        {
            SetScore(playerHighScoreText, ToolController._highScore);
        }

        /// <summary>
        /// Prepares the audio source component for use.
        /// </summary>
        private void PrepareAudioSource()
        {
            _gameStatusAudio = GetComponent<AudioSource>();
        }

        /// <summary>
        /// Resets the pause trigger to its default state.
        /// </summary>
        private void ResetPauseTrigger()
        {
            _pauseTrigger = false;
        }

        private void Update()
        {
            CheckAndHandleStageClear();
            DisplayMessageIfNeeded();
            HandleEnemyDeathOrCoinCollection();
            HandlePowerUpConsumption();
            UpdateHighScoreIfNeeded();
            UpdateUIElements();
        }

        /// <summary>
        /// Checks and handles the stage clearing event.
        /// </summary>
        private void CheckAndHandleStageClear()
        {
            if (ToolController.IsStageClear)
            {
                PlayStageClearAudio();
                ToolController.IsStageClear = false;
            }
        }

        /// <summary>
        /// Plays audio for stage clearance.
        /// </summary>
        private void PlayStageClearAudio()
        {
            _gameStatusAudio.PlayOneShot(stageClearSound);
        }

        /// <summary>
        /// Displays a message when required.
        /// </summary>
        private void DisplayMessageIfNeeded()
        {
            if (ToolController.IsShowMessage)
            {
                StartCoroutine(DisplayFirstMessage());
            }
        }

        /// <summary>
        /// Processes events related to enemy defeat or coin collection.
        /// </summary>
        private void HandleEnemyDeathOrCoinCollection()
        {
            if (ToolController.IsEnemyDieOrCoinEat)
            {
                ToolController.IsEnemyDieOrCoinEat = false;
                UpdateScorePopup(score200Prefab);
            }
        }

        /// <summary>
        /// Responds to power-up consumption.
        /// </summary>
        private void HandlePowerUpConsumption()
        {
            if (ToolController.IsPowerUpEat)
            {
                ToolController.IsPowerUpEat = false;
                UpdateScorePopup(score1000Prefab);
            }
        }

        /// <summary>
        /// Updates high score if current score exceeds it.
        /// </summary>
        private void UpdateHighScoreIfNeeded()
        {
            if (ToolController.Score > ToolController._highScore)
            {
                ToolController._highScore = ToolController.Score;
            }
        }

        /// <summary>
        /// Updates various UI elements.
        /// </summary>
        private void UpdateUIElements()
        {
            SetCoin();
            SetLevel();
            SetScore(playerScoreText, ToolController.Score);
            SetLive();
            Pause();
        }

        /// <summary>
        /// Updates the score display with appropriate formatting.
        /// </summary>
        private void SetScore(TextMeshProUGUI scoreText, int score)
        {
            string formattedScore = FormatScore(score);
            scoreText.SetText(formattedScore);
        }

        /// <summary>
        /// Formats the score as a string with leading zeros.
        /// </summary>
        private string FormatScore(int score)
        {
            return score.ToString("D6");
        }

        /// <summary>
        /// Sets the display for collected coins.
        /// </summary>
        private void SetCoin()
        {
            string coinDisplay = FormatCoinDisplay(ToolController.CollectedCoin);
            collectedCoinText.SetText(coinDisplay);
        }

        /// <summary>
        /// Formats the coin count for display.
        /// </summary>
        private string FormatCoinDisplay(int coinCount)
        {
            if (coinCount > 99)
                return "x00";
            
            return coinCount > 0 ? $"x{coinCount:D2}" : "x00";
        }

        /// <summary>
        /// Adjusts the timer display based on the given seconds.
        /// </summary>
        public void SetTime(float second)
        {
            _second = second;
            secondsText.SetText(FormatTime(_second));
        }

        /// <summary>
        /// Formats the time display with leading zeros.
        /// </summary>
        private string FormatTime(float time)
        {
            if (time <= 0)
                return "000";

            int roundedTime = Mathf.RoundToInt(time);
            return roundedTime > 99 ? roundedTime.ToString() : roundedTime.ToString("D3");
        }

        private void SetLevel()
        {
            levelText.SetText(SceneManager.GetActiveScene().name);
        }

        private void SetLive()
        {
            livesText.SetText($"x {ToolController.Live.ToString()}");
        }

        /// <summary>
        /// Manages the game pause functionality.
        /// </summary>
        private void Pause()
        {
            if (CanPauseGame())
            {
                TogglePauseState();
            }
        }

        /// <summary>
        /// Checks if the game can be paused based on the current scene.
        /// </summary>
        private bool CanPauseGame()
        {
            return SceneManager.GetActiveScene().buildIndex > 1 && Input.GetKeyDown(KeyCode.P);
        }

        /// <summary>
        /// Toggles the pause state of the game.
        /// </summary>
        private void TogglePauseState()
        {
            _gameStatusAudio.PlayOneShot(pauseSound);
            _pauseTrigger = !_pauseTrigger;
            pausePopup.SetActive(_pauseTrigger);
            Time.timeScale = _pauseTrigger ? 0 : 1;
        }

        /// <summary>
        /// Initiates a new game session.
        /// </summary>
        public void StartGame()
        {
            LoadFirstLevel();
            ResetGameStats();
        }

        /// <summary>
        /// Loads the first level scene.
        /// </summary>
        private void LoadFirstLevel()
        {
            SceneManager.LoadScene(1);
        }

        /// <summary>
        /// Resets the game statistics to their initial state.
        /// </summary>
        private void ResetGameStats()
        {
            ToolController.CurrentLevel = 2;
            ToolController.Live = 3;
            ToolController.Score = 0;
            ToolController.CollectedCoin = 0;
            ToolController.PlayerTag = "Player";
        }

        public void OpenInstructionPopup()
        {
            instructionPopup.SetActive(true);
        }

        public void OpenCreditPopup()
        {
            creditPopup.SetActive(true);
        }

        public void CloseInstructionPopup()
        {
            instructionPopup.SetActive(false);
        }

        public void CloseCreditPopup()
        {
            creditPopup.SetActive(false);
        }

        public void ExitGame()
        {
            SceneManager.LoadScene(0);
            Time.timeScale = 1;
        }

        private void UpdateScorePopup(GameObject scorePrefab)
        {
            GameObject score = Instantiate(scorePrefab, scoreParent);
            StartCoroutine(DestroyScorePrefab(score));
        }

        private IEnumerator DestroyScorePrefab(GameObject prefab)
        {
            yield return new WaitForSeconds(1);
            Destroy(prefab);
        }

        private IEnumerator DisplayFirstMessage()
        {
            yield return new WaitForSeconds(1);
            firstMessagePopup.SetActive(true);
            StartCoroutine(DisplaySecondMessage());
        }

        private IEnumerator DisplaySecondMessage()
        {
            yield return new WaitForSeconds(1.5f);
            secondMessagePopup.SetActive(true);
        }
    }
}