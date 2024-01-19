using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using EnemyScripts;
using System.Collections.Generic;
using AdditionalScripts;

public class EnemyControllerDieTest
{
    private GameObject enemyObject;
    private EnemyController enemyController;
    private BoxCollider2D[] mockDeadDisableColliders;
    private BoxCollider2D mockDeadEnableCollider;
    private Animator animator;  // 引用 Animator
    private int initialScore;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        // Initialize score
        initialScore = ToolController.Score;

        // Create a new GameObject with the required components for the Enemy
        enemyObject = new GameObject("Enemy");
        enemyController = enemyObject.AddComponent<EnemyController>();
        animator = enemyObject.AddComponent<Animator>();  // 添加 Animator 组件
        enemyController.Awake(); // Initialize EnemyController

        // Setup mock colliders - 使用数组初始化
        mockDeadEnableCollider = enemyObject.AddComponent<BoxCollider2D>();
        mockDeadDisableColliders = new BoxCollider2D[] {
            enemyObject.AddComponent<BoxCollider2D>(),
            enemyObject.AddComponent<BoxCollider2D>()
        };
        enemyController.deadEnableCollider = mockDeadEnableCollider;
        enemyController.deadDisableColliders = new List<Collider2D>(mockDeadDisableColliders);

        yield return null;
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        // Reset the score to its initial value
        ToolController.Score = initialScore;

        // Clean up
        Object.Destroy(enemyObject);
        yield return null;
    }

    [UnityTest]
    public IEnumerator Die_SetsIsTouchByPlayer_IncreasesScore_DisablesDeadColliders_EnablesDeadCollider()
    {
        // Arrange
        int scoreToAdd = 200;

        // Act
        enemyController.Die();
        yield return null; // Wait for a frame to ensure the Die method logic is processed

        // Assert
        Assert.IsTrue(enemyController.isTouchByPlayer, "isTouchByPlayer should be true after enemy dies.");
        Assert.AreEqual(initialScore + scoreToAdd, ToolController.Score, "Score should increase by the scoreToAdd.");
        foreach (var collider in mockDeadDisableColliders)
        {
            Assert.IsFalse(collider.enabled, "Dead disable colliders should be disabled after enemy dies.");
        }
        Assert.IsTrue(mockDeadEnableCollider.enabled, "Dead enable collider should be enabled after enemy dies.");
    }

    // Add more tests here as needed
}

