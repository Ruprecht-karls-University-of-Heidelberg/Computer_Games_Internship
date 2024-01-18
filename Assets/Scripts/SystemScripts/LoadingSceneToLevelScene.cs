using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using AdditionalScripts;

//after changing

namespace SystemScripts
{
    public class LoadingSceneToLevelScene : MonoBehaviour
    {
        public GameObject liveStat;           // Represents statistics of player lives.
        public GameObject gameOverPopup;      // Shows up when the player loses all lives.
        private AudioSource _loadSceneAudio;  // Handles audio for scene transitions.
        public AudioClip gameOverSound;       // Sound effect for game over event.

        // Called when the scene begins.
        private void Start()
        {
            // Acquire the AudioSource component attached to this object.
            _loadSceneAudio = GetComponent<AudioSource>();

            // Check the player's remaining lives.
            CheckPlayerLives();

            // Process scene loading based on the game's state.
            ProcessSceneLoading();
        }

        // Checks and updates the player's live status.
        private void CheckPlayerLives()
        {
            if (ToolController.Live < 1)
            {
                ToolController.IsGameOver = true;  // Flag the game as over.
            }
        }

        // Decides which scene to load next based on the game state.
        private void ProcessSceneLoading()
        {
            if (!ToolController.IsGameOver)
            {
                LoadLevelOrRepeat();  // Load appropriate level scene.
            }
            else
            {
                HandleGameOver();  // Process game over scenario.
            }
        }

        // Loads the next level or repeats the current one.
        private void LoadLevelOrRepeat()
        {
            if (ToolController.IsDead)
            {
                StartCoroutine(RepeatLevelScene());  // Reload the current level.
            }
            else
            {
                StartCoroutine(LevelScene());  // Proceed to the next level.
            }
        }

        // Handles the game over situation.
        private void HandleGameOver()
        {
            liveStat.SetActive(false);
            gameOverPopup.SetActive(true);
            _loadSceneAudio.PlayOneShot(gameOverSound);
            StartCoroutine(StartingScene());  // Transition to the starting scene.
        }

        // Coroutine to load the next level.
        private static IEnumerator LevelScene()
        {
            yield return new WaitForSeconds(2);
            SceneManager.LoadScene(ToolController.CurrentLevel);
            ToolController.CurrentLevel += 1;  // Advance to the next level.
        }

        // Coroutine to repeat the current level.
        private static IEnumerator RepeatLevelScene()
        {
            yield return new WaitForSeconds(2);
            SceneManager.LoadScene(ToolController.CurrentLevel - 1);
            ToolController.IsDead = false;  // Reset player's death status.
        }

        // Coroutine for transitioning to the start scene after game over.
        private static IEnumerator StartingScene()
        {
            yield return new WaitForSeconds(4.5f);
            SceneManager.LoadScene(0);  // Return to the start scene.

            // Reset game settings to initial values.
            ResetGameParameters();
        }

        // Resets game parameters to their default states.
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
