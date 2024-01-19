using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using EnemyScripts;

public class EnemyControllerEditModeTests
{
    [Test]
    public void EnemyController_Awake_InitializesAnimator()
    {
        // Arrange
        var gameObject = new GameObject();
        var enemyController = gameObject.AddComponent<EnemyController>();
        var animator = gameObject.AddComponent<Animator>();

        // Act
        enemyController.Awake();

        // Assert
        Assert.IsNotNull(enemyController.GetComponent<Animator>(), "Animator should be initialized in Awake.");
    }
}
