using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using AdditionalScripts;

namespace SystemScripts
{
    /// <summary>
    /// Represents a script for loading the level scene from the loading scene.
    /// </summary>
    public class LoadingSceneToLevelScene : MonoBehaviour
    {
        /// <summary>
        /// Represents the GameObject that displays the statistics of player lives.
        /// </summary>
        public GameObject liveStat;

        /// <summary>
        /// Represents the GameObject that shows up when the player loses all lives.
        /// </summary>
        public GameObject gameOverPopup;

        /// <summary>
        /// Represents the AudioSource component for handling audio during scene transitions.
        /// </summary>
        private AudioSource _loadSceneAudio;

        /// <summary>
        /// Represents the sound effect for the game over event.
        /// </summary>
        public AudioClip gameOverSound;

        /// <summary>
        /// Called when the scene starts.
        /// </summary>
        private void Start()
        {
            _loadSceneAudio = GetComponent<AudioSource>();
            CheckPlayerLives();
            ProcessSceneLoading();
        }

        /// <summary>
        /// Checks and updates the player's live status.
        /// </summary>
        private void CheckPlayerLives()
        {
            if (ToolController.Live < 1)
            {
                ToolController.IsGameOver = true;
            }
        }

        /// <summary>
        /// Decides which scene to load next based on the game state.
        /// </summary>
        private void ProcessSceneLoading()
        {
            if (!ToolController.IsGameOver)
            {
                LoadLevelOrRepeat();
            }
            else
            {
                HandleGameOver();
            }
        }

        /// <summary>
        /// Loads the next level or repeats the current one.
        /// </summary>
        private void LoadLevelOrRepeat()
        {
            if (ToolController.IsDead)
            {
                StartCoroutine(RepeatLevelScene());
            }
            else
            {
                StartCoroutine(LevelScene());
            }
        }

        /// <summary>
        /// Handles the game over situation.
        /// </summary>
        private void HandleGameOver()
        {
            liveStat.SetActive(false);
            gameOverPopup.SetActive(true);
            _loadSceneAudio.PlayOneShot(gameOverSound);
            StartCoroutine(StartingScene());
        }

        /// <summary>
        /// Coroutine to load the next level.
        /// </summary>
        private static IEnumerator LevelScene()
        {
            yield return new WaitForSeconds(2);
            SceneManager.LoadScene(ToolController.CurrentLevel);
            ToolController.CurrentLevel += 1;
        }

        /// <summary>
        /// Coroutine to repeat the current level.
        /// </summary>
        private static IEnumerator RepeatLevelScene()
        {
            yield return new WaitForSeconds(2);
            SceneManager.LoadScene(ToolController.CurrentLevel - 1);
            ToolController.IsDead = false;
        }

        /// <summary>
        /// Coroutine for transitioning to the start scene after game over.
        /// </summary>
        private static IEnumerator StartingScene()
        {
            yield return new WaitForSeconds(4.5f);
            SceneManager.LoadScene(0);
            ResetGameParameters();
        }

        /// <summary>
        /// Resets game parameters to their default states.
        /// </summary>
        private static void ResetGameParameters()
        {
            ToolController.Live = 3;
            ToolController.Score = 0;
            ToolController.CollectedCoin = 0;
            ToolController.IsGameOver = false;
            ToolController.IsDead = false;
        }
    }
}
