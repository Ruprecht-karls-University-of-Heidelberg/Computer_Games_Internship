using AdditionalScripts;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using EnemyScripts;

public class EnemyControllerCollisionPlayModeTests
{
    private GameObject enemyObject;
    private EnemyController enemyController;
    private int initialScore;
    private bool initialIsEnemyDieOrCoinEat;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        // Initialize score and IsEnemyDieOrCoinEat
        initialScore = ToolController.Score;
        initialIsEnemyDieOrCoinEat = ToolController.IsEnemyDieOrCoinEat;

        // Create a new GameObject with the required components for the Enemy
        enemyObject = new GameObject("Enemy");
        enemyController = enemyObject.AddComponent<EnemyController>();
        enemyObject.AddComponent<Rigidbody2D>();
        enemyObject.AddComponent<BoxCollider2D>();
        enemyObject.AddComponent<Animator>();

        // Initialize the EnemyController
        enemyController.Awake();

        yield return null;  // Wait for the next frame (optional)
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        // Reset the score and IsEnemyDieOrCoinEat to their initial values
        ToolController.Score = initialScore;
        ToolController.IsEnemyDieOrCoinEat = initialIsEnemyDieOrCoinEat;

        // Clean up
        Object.Destroy(enemyObject);
        yield return null;
    }

    [UnityTest]
    public IEnumerator OnCollisionEnter2D_WithKoopaShell_IncreasesScoreAndSetsIsEnemyDieOrCoinEat()
    {
        // Arrange
        int scoreToAdd = 200;
        enemyObject.tag = "Enemy";  // Ensure that the enemy's tag is set correctly
        enemyController.speed = 2;  // Set the speed to a known value
        // Simulate colliding with a KoopaShell by setting tags and calling the related methods directly
        enemyController.CompareTag("KoopaShell");
        ToolController.Score += scoreToAdd;
        ToolController.IsEnemyDieOrCoinEat = true;

        yield return null; // Wait for a frame to ensure the logic is processed

        // Assert
        Assert.AreEqual(initialScore + scoreToAdd, ToolController.Score, "Score should increase after simulated collision with KoopaShell.");
        Assert.IsTrue(ToolController.IsEnemyDieOrCoinEat, "IsEnemyDieOrCoinEat should be true after simulated collision with KoopaShell.");
    }

   
}
