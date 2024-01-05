using System;
using System.Collections;
using System.Collections.Generic;
using EnemyScripts;
using PlayerScripts;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SystemScripts
{
    /// <summary>
    /// Manages overall game mechanics, interactions, and the progression of levels.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public PlayerController player; // Reference to the player controller.
        public GameStatusController gameStatusController; // Reference to the game status controller.
        public List<EnemyController> enemyControllers; // List of enemy controllers in the game.
        public List<GameObject> enemyGameObjects; // List of enemy game objects in the game.
        public GameObject stairwayPrefab; // Prefab for the stairway in the game.
        public Transform stairwayDownParent; // Parent transform for stairways going down.
        public Transform stairwayUpParent; // Parent transform for stairways going up.
        public bool isStairLevel; // Flag to determine if the current level is a stairway level.

        public float time = 400; // Total time for the level.
        public float finalTime; // Final time when the level is completed.

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// Handles initial setup for the game level.
        /// </summary>
        private void Awake()
        {
            // Spawn stairways periodically if the level is a stair level.
            if (isStairLevel)
            {
                InvokeRepeating(nameof(SpawnStairway), 0, 3);
            }

            // Set the initial time for the game status controller.
            if (gameStatusController != null)
            {
                gameStatusController.SetTime(time);
            }
        }

        /// <summary>
        /// Update is called once per frame and handles the main game logic.
        /// </summary>
        private void Update()
        {
            if (player != null)
            {
                // Handle various game updates and checks.
                StopEnemiesFromMovingWhenPlayerDie();
                SetActiveEnemiesWhenSeePlayer();
                DestroyEnemiesOutOfBound();
                UpdateTime();
                UltimateDestroyAll();

                // Update final time when the player stops the time.
                if (player.isStopTime)
                {
                    finalTime = time;
                    player.isStopTime = false;
                }
            }
        }
        /// <summary>
        /// Stops all enemies from moving when the player is dead.
        /// </summary>
        private void StopEnemiesFromMovingWhenPlayerDie()
        {
            // Check if the player is dead.
            if (!GameStatusController.IsDead) return;

            // Stop each enemy in the list.
            for (var i = 0; i < enemyControllers.Count; i++)
            {
                if (enemyControllers[i] != null)
                {
                    enemyControllers[i].speed = 0;
                    var enemyRb = enemyControllers[i].gameObject.GetComponent<Rigidbody2D>();
                    enemyRb.velocity = Vector2.zero;
                    enemyRb.isKinematic = true;
                }
                else
                {
                    enemyControllers.Remove(enemyControllers[i]);
                }
            }
        }

        /// <summary>
        /// Activates enemies when they come within a certain distance of the player.
        /// </summary>
        private void SetActiveEnemiesWhenSeePlayer()
        {
            // Activate each enemy when close to the player.
            for (var i = 0; i < enemyGameObjects.Count; i++)
            {
                if (enemyGameObjects[i] != null)
                {
                    if (enemyGameObjects[i].transform.position.x - player.transform.position.x < 12)
                    {
                        enemyGameObjects[i].SetActive(true);
                    }
                }
                else
                {
                    enemyGameObjects.Remove(enemyGameObjects[i]);
                }
            }
        }

        /// <summary>
        /// Destroys enemies that are out of bounds relative to the player.
        /// </summary>
        private void DestroyEnemiesOutOfBound()
        {
            // Destroy each enemy that is far enough behind the player.
            for (var i = 0; i < enemyGameObjects.Count; i++)
            {
                if (enemyGameObjects[i] != null)
                {
                    if (enemyGameObjects[i].transform.position.x - player.transform.position.x < -15)
                    {
                        Destroy(enemyGameObjects[i]);
                    }
                }
                else
                {
                    enemyGameObjects.Remove(enemyGameObjects[i]);
                }
            }
        }
        /// <summary>
        /// Updates the game time and handles time-based game events.
        /// </summary>
        private void UpdateTime()
        {
            // Handle time countdown and check for time-based game over.
            if (!GameStatusController.IsDead && !player.isWalkingToCastle && !player.isInCastle &&
                !GameStatusController.IsGameFinish)
            {
                gameStatusController.SetTime(time -= Time.deltaTime * 2);
                if (time < 0)
                {
                    time = 0;
                    GameStatusController.IsDead = true;
                }
            }
            else if (player.isInCastle)
            {
                // Handle time countdown in the castle.
                gameStatusController.SetTime(time -= Time.deltaTime * 60);

                if (time < 0)
                {
                    time = 0;
                    StartCoroutine(NextLevel());
                }
                else
                {
                    // Award score based on time.
                    if (finalTime - time >= 1f)
                    {
                        GameStatusController.Score += 50;
                        finalTime = time;
                    }
                }
            }
        }

        /// <summary>
        /// Destroys all enemies in proximity when the player is in an ultimate state.
        /// </summary>
        private void UltimateDestroyAll()
        {
            // Check if the player is in an ultimate state.
            if (player.CompareTag("UltimatePlayer") || player.CompareTag("UltimateBigPlayer"))
            {
                for (var i = 0; i < enemyControllers.Count; i++)
                {
                    if (enemyControllers[i] != null)
                    {
                        // Determine if enemies are in range to be destroyed.
                        if (Mathf.RoundToInt(enemyControllers[i].gameObject.transform.position.x -
                                             player.transform.position.x) == 0 && player.CompareTag("UltimatePlayer"))
                        {
                            KillAndRemoveEnemies(i, 0.2f);
                        }
                        else if (Mathf.RoundToInt(enemyControllers[i].gameObject.transform.position.x -
                                                  player.transform.position.x) == 0 &&
                                 player.CompareTag("UltimateBigPlayer"))
                        {
                            KillAndRemoveEnemies(i, 0.7f);
                        }
                    }
                    else
                    {
                        enemyControllers.Remove(enemyControllers[i]);
                    }
                }
            }
        }

        /// <summary>
        /// Kills and removes enemies based on their proximity to the player.
        /// </summary>
        /// <param name="i">Index of the enemy in the enemyControllers list.</param>
        /// <param name="distance">Proximity distance for the enemy to be affected.</param>
        private void KillAndRemoveEnemies(int i, float distance)
        {
            // Check if the enemy is within a specific vertical distance from the player.
            if (enemyControllers[i].gameObject.transform.position.y - player.transform.position.y <
                distance &&
                enemyControllers[i].gameObject.transform.position.y - player.transform.position.y >
                -distance)
            {
                GameStatusController.Score += 200;
                GameStatusController.IsEnemyDieOrCoinEat = true;
                enemyControllers[i].Die();
                enemyControllers.Remove(enemyControllers[i]);
            }
        }


        /// <summary>
        /// Spawns stairway objects at designated parent transforms.
        /// </summary>
        private void SpawnStairway()
        {
            // Instantiate stairway objects and start coroutine to destroy them after a set time.
            GameObject stairwayDown = Instantiate(stairwayPrefab, stairwayDownParent);
            GameObject stairwayUp = Instantiate(stairwayPrefab, stairwayUpParent);
            StartCoroutine(DestroyStair(stairwayDown));
            StartCoroutine(DestroyStair(stairwayUp));
        }

        /// <summary>
        /// Coroutine to load the next level after a delay.
        /// </summary>
        private static IEnumerator NextLevel()
        {
            yield return new WaitForSeconds(3);
            SceneManager.LoadScene(1);
        }

        /// <summary>
        /// Coroutine to destroy a stair object after a delay.
        /// </summary>
        /// <param name="stair">The stair GameObject to be destroyed.</param>
        private IEnumerator DestroyStair(GameObject stair)
        {
            yield return new WaitForSeconds(5.3f);
            Destroy(stair);
        }
    }
}