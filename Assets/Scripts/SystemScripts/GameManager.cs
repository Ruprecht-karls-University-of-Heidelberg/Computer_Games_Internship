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

        // Initializes game elements upon object awakening.
        private void Awake()
        {
            InitializeStairwaySpawning();
            InitializeGameTime();
        }

        // Prepares stairway spawning if the level requires it.
        private void InitializeStairwaySpawning()
        {
            if (isStairLevel)
            {
                InvokeRepeating(nameof(SpawnStairway), 0, 3);
            }
        }

        // Sets the initial game time in the status controller.
        private void InitializeGameTime()
        {
            if (gameStatusController != null)
            {
                gameStatusController.SetTime(time);
            }
        }

        // Regularly updates game elements each frame.
        private void Update()
        {
            if (player != null)
            {
                PerformPlayerRelatedUpdates();
            }
        }

        // Conducts updates related to player actions and states.
        public void PerformPlayerRelatedUpdates()
        {
            HaltEnemiesOnPlayerDeath();
            ActivateEnemiesUponPlayerDetection();
            EliminateOutOfBoundsEnemies();
            CountDownTime();
            ExecuteUltimateDestruction();

            CaptureFinalTimeOnTimeStop();
        }

        // Stops enemies from moving if the player has died.
        public void HaltEnemiesOnPlayerDeath()
        {
            StopEnemiesFromMovingWhenPlayerDie();
        }

        // Activates enemies when they detect the player.
        public void ActivateEnemiesUponPlayerDetection()
        {
            SetActiveEnemiesWhenSeePlayer();
        }

        // Removes enemies that have moved out of bounds.
        public void EliminateOutOfBoundsEnemies()
        {
            DestroyEnemiesOutOfBound();
        }

        // Updates the remaining game time.
        public void CountDownTime()
        {
            UpdateTime();
        }

        // Executes a complete destruction under certain conditions.
        public void ExecuteUltimateDestruction()
        {
            UltimateDestroyAll();
        }

        // Records the final time when time-stop is activated by the player.
        public void CaptureFinalTimeOnTimeStop()
        {
            if (player.isStopTime)
            {
                finalTime = time;
                player.isStopTime = false;
            }
        }


        // Halts the movement of all enemies if the player has been defeated.
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

        // Freezes the enemy's movement and sets it as kinematic.
        private void FreezeEnemy(EnemyController enemyController)
        {
            enemyController.speed = 0;
            Rigidbody2D enemyRigidbody = enemyController.gameObject.GetComponent<Rigidbody2D>();
            enemyRigidbody.velocity = Vector2.zero;
            enemyRigidbody.isKinematic = true;
        }


// Activates enemies within a certain distance of the player.
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

        // Activates an enemy game object if it is close to the player.
        private void ActivateEnemyIfCloseToPlayer(GameObject enemyGameObject)
        {
            float distanceToPlayer = Mathf.Abs(enemyGameObject.transform.position.x - player.transform.position.x);
            if (distanceToPlayer < 12)
            {
                enemyGameObject.SetActive(true);
            }
        }



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

        // Manages the countdown of game time and updates game state based on time.
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

        // Determines if time should be decremented based on game conditions.
        private bool ShouldDecrementTime()
        {
            return !ToolController.IsDead && !player.isWalkingToCastle && !player.isInCastle && !ToolController.IsGameFinish;
        }

        // Decrements the game time.
        private void DecrementTime()
        {
            gameStatusController.SetTime(time -= Time.deltaTime * 2);
        }

        // Checks if time has run out and sets the player as dead.
        private void CheckTimeForDeath()
        {
            if (time < 0)
            {
                time = 0;
                ToolController.IsDead = true;
            }
        }

        // Manages time updates when the player is in the castle.
        private void UpdateCastleTime()
        {
            DecrementCastleTime();
            AwardScoreForTimeInCastle();
        }

        // Decrements time faster when the player is in the castle.
        private void DecrementCastleTime()
        {
            gameStatusController.SetTime(time -= Time.deltaTime * 60);
        }

        // Awards score based on time spent in the castle and checks for level completion.
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

        // Awards score for each second spent in the castle.
        private void AwardScorePerSecond()
        {
            if (finalTime - time >= 1f)
            {
                ToolController.Score += 50;
                finalTime = time;
            }
        }


        // Executes the ultimate destruction of enemies based on player's state.
        private void UltimateDestroyAll()
        {
            if (IsPlayerInUltimateState())
            {
                ProcessEnemiesForUltimateDestruction();
            }
        }

        // Determines if the player is in an ultimate state.
        private bool IsPlayerInUltimateState()
        {
            return player.CompareTag("UltimatePlayer") || player.CompareTag("UltimateBigPlayer");
        }

        // Processes each enemy for potential destruction in the ultimate state.
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

        // Evaluates and destroys the enemy if conditions are met.
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


        // Eliminates and removes enemies based on proximity to the player.
        private void KillAndRemoveEnemies(int i, float distance)
        {
            if (IsEnemyWithinVerticalDistance(i, distance))
            {
                IncrementScoreAndTriggerEffects(i);
            }
        }

        // Checks if the enemy is within a specified vertical distance from the player.
        private bool IsEnemyWithinVerticalDistance(int index, float distance)
        {
            float verticalDistance = enemyControllers[index].gameObject.transform.position.y - player.transform.position.y;
            return verticalDistance < distance && verticalDistance > -distance;
        }

        // Increments score, triggers effects, and removes the enemy.
        private void IncrementScoreAndTriggerEffects(int index)
        {
            ToolController.Score += 200;
            ToolController.IsEnemyDieOrCoinEat = true;
            enemyControllers[index].Die();
            enemyControllers.RemoveAt(index);
        }


        private void SpawnStairway()
        {
            GameObject stairwayDown = Instantiate(stairwayPrefab, stairwayDownParent);
            GameObject stairwayUp = Instantiate(stairwayPrefab, stairwayUpParent);
            StartCoroutine(DestroyStair(stairwayDown));
            StartCoroutine(DestroyStair(stairwayUp));
        }

        private static IEnumerator NextLevel()
        {
            yield return new WaitForSeconds(3);
            SceneManager.LoadScene(1);
        }

        private IEnumerator DestroyStair(GameObject stair)
        {
            yield return new WaitForSeconds(5.3f);
            Destroy(stair);
        }
    }
}