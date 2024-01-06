using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using AdditionalScripts;

namespace SystemScripts
{
    public class LoadingSceneToLevelScene : MonoBehaviour
    {
        public GameObject liveStat;           // GameObject representing player's live statistics.
        public GameObject gameOverPopup;      // Popup displayed when the game is over.
        private AudioSource _loadSceneAudio;  // AudioSource component for this script.
        public AudioClip gameOverSound;       // AudioClip for the game over sound.

        // Invoked at the start of the scene.
        private void Start()
        {
            // Retrieve the AudioSource component.
            _loadSceneAudio = GetComponent<AudioSource>();

            // Check if player's live count is less than 1.
            if (ToolController.Live < 1)
            {
                ToolController.IsGameOver = true;  // Set game over status to true.
            }

            // If the game is not over, decide which scene to load based on player's death status.
            if (!ToolController.IsGameOver)
            {
                if (ToolController.IsDead)
                {
                    StartCoroutine(RepeatLevelScene());  // Load the current level again.
                }
                else
                {
                    StartCoroutine(LevelScene());  // Load the next level.
                }
            }
            // If the game is over, play the game over sound and show game over popup.
            else
            {
                liveStat.SetActive(false);
                gameOverPopup.SetActive(true);
                _loadSceneAudio.PlayOneShot(gameOverSound);
                StartCoroutine(StartingScene());  // Load the starting scene after some delay.
            }
        }

        // Coroutine to load the next level after a delay.
        private static IEnumerator LevelScene()
        {
            yield return new WaitForSeconds(2);
            SceneManager.LoadScene(ToolController.CurrentLevel);
            ToolController.CurrentLevel += 1;  // Increment the current level index for future loading.
        }

        // Coroutine to repeat the current level after a delay.
        private static IEnumerator RepeatLevelScene()
        {
            yield return new WaitForSeconds(2);
            SceneManager.LoadScene(ToolController.CurrentLevel - 1);  // Load the current level again.
            ToolController.IsDead = false;  // Reset the death status of the player.
        }

        // Coroutine to load the starting scene after game over.
        private static IEnumerator StartingScene()
        {
            yield return new WaitForSeconds(4.5f);
            SceneManager.LoadScene(0);  // Load the starting scene.

            // Reset game parameters to their initial states.
            ToolController.Live = 3;
            ToolController.Score = 0;
            ToolController.CollectedCoin = 0;
            ToolController.IsGameOver = false;
            ToolController.IsDead = false;
        }
    }
}
