using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using EnemyScripts;

public class EnemyControllerDestroyEnemyPlayModeTests
{
    private GameObject enemyObject;
    private EnemyController enemyController;
    
    [UnitySetUp]
    public IEnumerator SetUp()
    {
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
        // Clean up
        if (enemyObject != null)
        {
            Object.Destroy(enemyObject);
        }
        yield return null;
    }

    [UnityTest]
    public IEnumerator DestroyEnemy_DestroysGameObjectAfterDelay()
    {
        // Arrange
        float delay = 0.3f;  // The delay after which the enemy should be destroyed

        // Act
        enemyController.StartCoroutine(enemyController.DestroyEnemy());
        yield return new WaitForSeconds(delay + 0.1f);  // Wait for longer than the delay to ensure the enemy has been destroyed

        // Assert
        Assert.IsTrue(enemyObject == null, "Enemy GameObject should be destroyed after the delay.");
    }
}
