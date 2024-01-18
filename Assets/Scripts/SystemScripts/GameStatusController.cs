using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using AdditionalScripts;
//after changing

namespace SystemScripts
{
    public class GameStatusController : MonoBehaviour
    {
        public TextMeshProUGUI playerScoreText;
        public TextMeshProUGUI playerHighScoreText;
        public TextMeshProUGUI collectedCoinText;
        public TextMeshProUGUI levelText;
        public TextMeshProUGUI secondsText;
        public TextMeshProUGUI livesText;
        public GameObject score200Prefab;
        public GameObject score1000Prefab;
        public GameObject pausePopup;
        public GameObject instructionPopup;
        public GameObject creditPopup;
        public GameObject firstMessagePopup;
        public GameObject secondMessagePopup;
        public Transform scoreParent;
        private AudioSource _gameStatusAudio;

        public AudioClip pauseSound;
        public AudioClip stageClearSound;

        private bool _pauseTrigger;

        private float _second;

        private void Awake()
        {
            InitializeHighScoreDisplay();
            PrepareAudioSource();
            ResetPauseTrigger();
        }

        // Sets the high score on the UI at the start.
        private void InitializeHighScoreDisplay()
        {
            SetScore(playerHighScoreText, ToolController._highScore);
        }

        // Prepares the audio source component for use.
        private void PrepareAudioSource()
        {
            _gameStatusAudio = GetComponent<AudioSource>();
        }

        // Resets the pause trigger to its default state.
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

        // Checks and handles the stage clearing event.
        private void CheckAndHandleStageClear()
        {
            if (ToolController.IsStageClear)
            {
                PlayStageClearAudio();
                ToolController.IsStageClear = false;
            }
        }

        // Plays audio for stage clearance.
        private void PlayStageClearAudio()
        {
            _gameStatusAudio.PlayOneShot(stageClearSound);
        }

        // Displays a message when required.
        private void DisplayMessageIfNeeded()
        {
            if (ToolController.IsShowMessage)
            {
                StartCoroutine(DisplayFirstMessage());
            }
        }

        // Processes events related to enemy defeat or coin collection.
        private void HandleEnemyDeathOrCoinCollection()
        {
            if (ToolController.IsEnemyDieOrCoinEat)
            {
                ToolController.IsEnemyDieOrCoinEat = false;
                UpdateScorePopup(score200Prefab);
            }
        }

        // Responds to power-up consumption.
        private void HandlePowerUpConsumption()
        {
            if (ToolController.IsPowerUpEat)
            {
                ToolController.IsPowerUpEat = false;
                UpdateScorePopup(score1000Prefab);
            }
        }

        // Updates high score if current score exceeds it.
        private void UpdateHighScoreIfNeeded()
        {
            if (ToolController.Score > ToolController._highScore)
            {
                ToolController._highScore = ToolController.Score;
            }
        }

        // Updates various UI elements.
        private void UpdateUIElements()
        {

            SetCoin();
            SetLevel();
            SetScore(playerScoreText, ToolController.Score);
            SetLive();
            Pause();
        }


        // Updates the score display with appropriate formatting.
        private void SetScore(TextMeshProUGUI scoreText, int score)
        {
            string formattedScore = FormatScore(score);
            scoreText.SetText(formattedScore);
        }

        // Formats the score as a string with leading zeros.
        private string FormatScore(int score)
        {
            return score.ToString("D6");
        }

        // Sets the display for collected coins.
        private void SetCoin()
        {
            string coinDisplay = FormatCoinDisplay(ToolController.CollectedCoin);
            collectedCoinText.SetText(coinDisplay);
        }

        // Formats the coin count for display.
        private string FormatCoinDisplay(int coinCount)
        {
            if (coinCount > 99)
                return "x00";
            
            return coinCount > 0 ? $"x{coinCount:D2}" : "x00";
        }

        // Adjusts the timer display based on the given seconds.
        public void SetTime(float second)
        {
            _second = second;
            secondsText.SetText(FormatTime(_second));
        }

        // Formats the time display with leading zeros.
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

        // Manages the game pause functionality.
        private void Pause()
        {
            if (CanPauseGame())
            {
                TogglePauseState();
            }
        }

        // Checks if the game can be paused based on the current scene.
        private bool CanPauseGame()
        {
            return SceneManager.GetActiveScene().buildIndex > 1 && Input.GetKeyDown(KeyCode.P);
        }

        // Toggles the pause state of the game.
        private void TogglePauseState()
        {
            _gameStatusAudio.PlayOneShot(pauseSound);
            _pauseTrigger = !_pauseTrigger;
            pausePopup.SetActive(_pauseTrigger);
            Time.timeScale = _pauseTrigger ? 0 : 1;
        }

        // Initiates a new game session.
        public void StartGame()
        {
            LoadFirstLevel();
            ResetGameStats();
        }

        // Loads the first level scene.
        private void LoadFirstLevel()
        {
            SceneManager.LoadScene(1);
        }

        // Resets the game statistics to their initial state.
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