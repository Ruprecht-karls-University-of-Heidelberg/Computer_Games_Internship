using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SystemScripts
{
    /// <summary>
    /// Manages the loading of different scenes based on the game's status, such as repeating a level,
    /// proceeding to the next level, or handling the game over scenario.
    /// </summary>
    public class LoadingSceneToLevelScene : MonoBehaviour
    {
        public GameObject liveStat;           // GameObject representing player's live statistics.
        public GameObject gameOverPopup;      // Popup displayed when the game is over.
        private AudioSource _loadSceneAudio;  // AudioSource component for this script.
        public AudioClip gameOverSound;       // AudioClip for the game over sound.

        /// <summary>
        /// Invoked at the start of the scene. Determines which scene to load next based on game status.
        /// </summary>
        private void Start()
        {
            _loadSceneAudio = GetComponent<AudioSource>();

            // Check and handle game over condition.
            if (GameStatusController.Live < 1)
            {
                GameStatusController.IsGameOver = true;
            }

            // Determine next scene based on whether the game is over or the player is dead.
            if (!GameStatusController.IsGameOver)
            {
                if (GameStatusController.IsDead)
                {
                    StartCoroutine(RepeatLevelScene());
                }
                else
                {
                    StartCoroutine(LevelScene());
                }
            }
            else
            {
                liveStat.SetActive(false);
                gameOverPopup.SetActive(true);
                _loadSceneAudio.PlayOneShot(gameOverSound);
                StartCoroutine(StartingScene());
            }
        }

        /// <summary>
        /// Coroutine to load the next level scene after a delay.
        /// </summary>
        private static IEnumerator LevelScene()
        {
            yield return new WaitForSeconds(2);
            SceneManager.LoadScene(GameStatusController.CurrentLevel);
            GameStatusController.CurrentLevel += 1;
        }

        /// <summary>
        /// Coroutine to repeat the current level scene after a delay.
        /// </summary>
        private static IEnumerator RepeatLevelScene()
        {
            yield return new WaitForSeconds(2);
            SceneManager.LoadScene(GameStatusController.CurrentLevel - 1);
            GameStatusController.IsDead = false;
        }

        /// <summary>
        /// Coroutine to load the starting scene after the game is over.
        /// </summary>
        private static IEnumerator StartingScene()
        {
            yield return new WaitForSeconds(4.5f);
            SceneManager.LoadScene(0);

            // Reset game parameters to initial states.
            GameStatusController.Live = 3;
            GameStatusController.Score = 0;
            GameStatusController.CollectedCoin = 0;
            GameStatusController.IsGameOver = false;
            GameStatusController.IsDead = false;
        }
    }
}
