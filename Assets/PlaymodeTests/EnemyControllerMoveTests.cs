using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using EnemyScripts;

public class EnemyControllerMoveTests
{
    private GameObject enemyObject;
    private EnemyController enemyController;
    private float initialPositionX;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        // Create a new GameObject with the required components for the Enemy
        enemyObject = new GameObject("Enemy");
        enemyController = enemyObject.AddComponent<EnemyController>();
        enemyObject.AddComponent<Animator>();

        // Initialize the EnemyController
        enemyController.Awake();
        enemyController.speed = 2; // Set the speed to a known value

        initialPositionX = enemyObject.transform.position.x;

        yield return null;  // Wait for the next frame (optional)
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        // Clean up
        Object.Destroy(enemyObject);
        yield return null;
    }

    [UnityTest]
    public IEnumerator Move_MovesEnemyLeft()
    {
        // Arrange
        var moveDuration = 1f; // The duration for which the enemy will move
        var expectedPositionX = initialPositionX - enemyController.speed * moveDuration;

        // Act
        var startTime = Time.time;
        while (Time.time - startTime < moveDuration)
        {
            enemyController.Move(); // Call Move method
            yield return null;     // Wait for the next frame
        }

        // Assert
        Assert.Less(enemyObject.transform.position.x, expectedPositionX, "Enemy should move left.");
    }
}

