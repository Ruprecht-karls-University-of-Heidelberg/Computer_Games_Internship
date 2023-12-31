﻿using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using AdditionalScripts;

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
            SetScore(playerHighScoreText, ToolController._highScore);
            _gameStatusAudio = GetComponent<AudioSource>();
            _pauseTrigger = false;
        }

        private void Update()
        {
            if (ToolController.IsStageClear)
            {
                _gameStatusAudio.PlayOneShot(stageClearSound);
                ToolController.IsStageClear = false;
            }

            if (ToolController.IsShowMessage)
            {
                StartCoroutine(DisplayFirstMessage());
            }

            if (ToolController.IsEnemyDieOrCoinEat)
            {
                ToolController.IsEnemyDieOrCoinEat = false;
                UpdateScorePopup(score200Prefab);
            }

            if (ToolController.IsPowerUpEat)
            {
                ToolController.IsPowerUpEat = false;
                UpdateScorePopup(score1000Prefab);
            }

            if (ToolController.Score > ToolController._highScore)
            {
                ToolController._highScore = ToolController.Score;
            }

            SetCoin();
            SetLevel();
            SetScore(playerScoreText, ToolController.Score);
            SetLive();
            Pause();
        }

        private void SetScore(TextMeshProUGUI scoreText, int score)
        {
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

        private void SetCoin()
        {
            if (ToolController.CollectedCoin > 0)
            {
                collectedCoinText.SetText($"x0{ToolController.CollectedCoin}");
                if (ToolController.CollectedCoin <= 9) return;
                collectedCoinText.SetText($"x{ToolController.CollectedCoin}");
                if (ToolController.CollectedCoin > 99)
                {
                    collectedCoinText.SetText("x00");
                }
            }
            else
            {
                collectedCoinText.SetText("x00");
            }
        }

        public void SetTime(float second)
        {
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

        private void SetLevel()
        {
            levelText.SetText(SceneManager.GetActiveScene().name);
        }

        private void SetLive()
        {
            livesText.SetText($"x {ToolController.Live.ToString()}");
        }

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

        public void StartGame()
        {
            SceneManager.LoadScene(1);
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