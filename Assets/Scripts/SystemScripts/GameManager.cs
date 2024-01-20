using System;
using System.Collections;
using System.Collections.Generic;
using EnemyScripts;
using PlayerScripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using AdditionalScripts;
//after changing

namespace SystemScripts
{
    /// <summary>
    /// Manages the game logic and elements.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public PlayerController player;
        public GameStatusController gameStatusController;
        public List<EnemyController> enemyControllers;
        public List<GameObject> enemyGameObjects;
        public GameObject stairwayPrefab;
        public Transform stairwayDownParent;
        public Transform stairwayUpParent;
        public bool isStairLevel;

        public float time = 400;
        public float finalTime;

        /// <summary>
        /// Initializes game elements upon object awakening.
        /// </summary>
        private void Awake()
        {
            InitializeStairwaySpawning();
            InitializeGameTime();
        }

        /// <summary>
        /// Prepares stairway spawning if the level requires it.
        /// </summary>
        private void InitializeStairwaySpawning()
        {
            if (isStairLevel)
            {
                InvokeRepeating(nameof(SpawnStairway), 0, 3);
            }
        }

        /// <summary>
        /// Sets the initial game time in the status controller.
        /// </summary>
        private void InitializeGameTime()
        {
            if (gameStatusController != null)
            {
                gameStatusController.SetTime(time);
            }
        }

        /// <summary>
        /// Regularly updates game elements each frame.
        /// </summary>
        private void Update()
        {
            if (player != null)
            {
                PerformPlayerRelatedUpdates();
            }
        }

        /// <summary>
        /// Conducts updates related to player actions and states.
        /// </summary>
        private void PerformPlayerRelatedUpdates()
        {
            HaltEnemiesOnPlayerDeath();
            ActivateEnemiesUponPlayerDetection();
            EliminateOutOfBoundsEnemies();
            CountDownTime();
            ExecuteUltimateDestruction();

            CaptureFinalTimeOnTimeStop();
        }

        /// <summary>
        /// Stops enemies from moving if the player has died.
        /// </summary>
        private void HaltEnemiesOnPlayerDeath()
        {
            StopEnemiesFromMovingWhenPlayerDie();
        }

        /// <summary>
        /// Activates enemies when they detect the player.
        /// </summary>
        private void ActivateEnemiesUponPlayerDetection()
        {
            SetActiveEnemiesWhenSeePlayer();
        }

        /// <summary>
        /// Removes enemies that have moved out of bounds.
        /// </summary>
        private void EliminateOutOfBoundsEnemies()
        {
            DestroyEnemiesOutOfBound();
        }

        /// <summary>
        /// Updates the remaining game time.
        /// </summary>
        private void CountDownTime()
        {
            UpdateTime();
        }

        /// <summary>
        /// Executes a complete destruction under certain conditions.
        /// </summary>
        private void ExecuteUltimateDestruction()
        {
            UltimateDestroyAll();
        }

        /// <summary>
        /// Records the final time when time-stop is activated by the player.
        /// </summary>
        private void CaptureFinalTimeOnTimeStop()
        {
            if (player.isStopTime)
            {
                finalTime = time;
                player.isStopTime = false;
            }
        }

        /// <summary>
        /// Halts the movement of all enemies if the player has been defeated.
        /// </summary>
        private void StopEnemiesFromMovingWhenPlayerDie()
        {
            // Exit if the player is not dead.
            if (!ToolController.IsDead) return;

            // Iterate over the list of enemy controllers.
            for (int i = enemyControllers.Count - 1; i >= 0; i--)
            {
                if (enemyControllers[i] != null)
                {
                    // Halt individual enemy movement.
                    FreezeEnemy(enemyControllers[i]);
                }
                else
                {
                    // Remove null references from the list.
                    enemyControllers.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Freezes the enemy's movement and sets it as kinematic.
        /// </summary>
        private void FreezeEnemy(EnemyController enemyController)
        {
            enemyController.speed = 0;
            Rigidbody2D enemyRigidbody = enemyController.gameObject.GetComponent<Rigidbody2D>();
            enemyRigidbody.velocity = Vector2.zero;
            enemyRigidbody.isKinematic = true;
        }

        /// <summary>
        /// Activates enemies within a certain distance of the player.
        /// </summary>
        private void SetActiveEnemiesWhenSeePlayer()
        {
            // Reverse iteration to safely modify the list during iteration.
            for (int i = enemyGameObjects.Count - 1; i >= 0; i--)
            {
                if (enemyGameObjects[i] != null)
                {
                    ActivateEnemyIfCloseToPlayer(enemyGameObjects[i]);
                }
                else
                {
                    // Remove null entries from the list.
                    enemyGameObjects.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Activates an enemy game object if it is close to the player.
        /// </summary>
        private void ActivateEnemyIfCloseToPlayer(GameObject enemyGameObject)
        {
            float distanceToPlayer = Mathf.Abs(enemyGameObject.transform.position.x - player.transform.position.x);
            if (distanceToPlayer < 12)
            {
                enemyGameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Removes enemies that have moved out of bounds.
        /// </summary>
        private void DestroyEnemiesOutOfBound()
        {
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
        /// Manages the countdown of game time and updates game state based on time.
        /// </summary>
        private void UpdateTime()
        {
            if (ShouldDecrementTime())
            {
                DecrementTime();
                CheckTimeForDeath();
            }
            else if (player.isInCastle)
            {
                UpdateCastleTime();
            }
        }

        /// <summary>
        /// Determines if time should be decremented based on game conditions.
        /// </summary>
        private bool ShouldDecrementTime()
        {
            return !ToolController.IsDead && !player.isWalkingToCastle && !player.isInCastle && !ToolController.IsGameFinish;
        }

        /// <summary>
        /// Decrements the game time.
        /// </summary>
        private void DecrementTime()
        {
            gameStatusController.SetTime(time -= Time.deltaTime * 2);
        }

        /// <summary>
        /// Checks if time has run out and sets the player as dead.
        /// </summary>
        private void CheckTimeForDeath()
        {
            if (time < 0)
            {
                time = 0;
                ToolController.IsDead = true;
            }
        }

        /// <summary>
        /// Manages time updates when the player is in the castle.
        /// </summary>
        private void UpdateCastleTime()
        {
            DecrementCastleTime();
            AwardScoreForTimeInCastle();
        }

        /// <summary>
        /// Decrements time faster when the player is in the castle.
        /// </summary>
        private void DecrementCastleTime()
        {
            gameStatusController.SetTime(time -= Time.deltaTime * 60);
        }

        /// <summary>
        /// Awards score based on time spent in the castle and checks for level completion.
        /// </summary>
        private void AwardScoreForTimeInCastle()
        {
            if (time < 0)
            {
                time = 0;
                StartCoroutine(NextLevel());
            }
            else
            {
                AwardScorePerSecond();
            }
        }

        /// <summary>
        /// Awards score for each second spent in the castle.
        /// </summary>
        private void AwardScorePerSecond()
        {
            if (finalTime - time >= 1f)
            {
                ToolController.Score += 50;
                finalTime = time;
            }
        }

        /// <summary>
        /// Executes the ultimate destruction of enemies based on player's state.
        /// </summary>
        private void UltimateDestroyAll()
        {
            if (IsPlayerInUltimateState())
            {
                ProcessEnemiesForUltimateDestruction();
            }
        }

        /// <summary>
        /// Determines if the player is in an ultimate state.
        /// </summary>
        private bool IsPlayerInUltimateState()
        {
            return player.CompareTag("UltimatePlayer") || player.CompareTag("UltimateBigPlayer");
        }

        /// <summary>
        /// Processes each enemy for potential destruction in the ultimate state.
        /// </summary>
        private void ProcessEnemiesForUltimateDestruction()
        {
            for (int i = enemyControllers.Count - 1; i >= 0; i--)
            {
                if (enemyControllers[i] != null)
                {
                    EvaluateAndDestroyEnemy(i);
                }
                else
                {
                    enemyControllers.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Evaluates and destroys the enemy if conditions are met.
        /// </summary>
        private void EvaluateAndDestroyEnemy(int index)
        {
            float distance = Mathf.RoundToInt(enemyControllers[index].gameObject.transform.position.x - player.transform.position.x);
            if (distance == 0)
            {
                float delay = player.CompareTag("UltimatePlayer") ? 0.2f : (player.CompareTag("UltimateBigPlayer") ? 0.7f : 0f);
                if (delay > 0)
                {
                    KillAndRemoveEnemies(index, delay);
                }
            }
        }

        /// <summary>
        /// Eliminates and removes enemies based on proximity to the player.
        /// </summary>
        private void KillAndRemoveEnemies(int i, float distance)
        {
            if (IsEnemyWithinVerticalDistance(i, distance))
            {
                IncrementScoreAndTriggerEffects(i);
            }
        }

        /// <summary>
        /// Checks if the enemy is within a specified vertical distance from the player.
        /// </summary>
        private bool IsEnemyWithinVerticalDistance(int index, float distance)
        {
            float verticalDistance = enemyControllers[index].gameObject.transform.position.y - player.transform.position.y;
            return verticalDistance < distance && verticalDistance > -distance;
        }

        /// <summary>
        /// Increments score, triggers effects, and removes the enemy.
        /// </summary>
        private void IncrementScoreAndTriggerEffects(int index)
        {
            ToolController.Score += 200;
            ToolController.IsEnemyDieOrCoinEat = true;
            enemyControllers[index].Die();
            enemyControllers.RemoveAt(index);
        }

        /// <summary>
        /// Spawns stairways for the level.
        /// </summary>
        private void SpawnStairway()
        {
            GameObject stairwayDown = Instantiate(stairwayPrefab, stairwayDownParent);
            GameObject stairwayUp = Instantiate(stairwayPrefab, stairwayUpParent);
            StartCoroutine(DestroyStair(stairwayDown));
            StartCoroutine(DestroyStair(stairwayUp));
        }

        /// <summary>
        /// Loads the next level after a delay.
        /// </summary>
        private static IEnumerator NextLevel()
        {
            yield return new WaitForSeconds(3);
            SceneManager.LoadScene(1);
        }

        /// <summary>
        /// Destroys the stairway after a delay.
        /// </summary>
        private static IEnumerator DestroyStair(GameObject stairway)
        {
            yield return new WaitForSeconds(10);
            Destroy(stairway);
        }
    }
}