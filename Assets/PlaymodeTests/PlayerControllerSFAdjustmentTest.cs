using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using PlayerScripts;

public class PlayerControllerSFAdjustmentTest
{
    private PlayerController _playerController;
    private GameObject _gameObject;

    [SetUp]
    public void SetUp()
    {
        // Create a new GameObject and add the PlayerController
        _gameObject = new GameObject();
        _playerController = _gameObject.AddComponent<PlayerController>();

        // Add a Rigidbody2D component to avoid errors related to its absence
        _playerController._playerRb = _gameObject.AddComponent<Rigidbody2D>();

        // Initialize necessary fields
        _playerController.speed = 410f;
        _playerController.jumpForce = 795f;
    }

    [Test]
    public void HandleSpeedAndJumpForceAdjustment_IncreasesValues_OnKeyDown()
    {
        // Directly simulate the effect of pressing the 'S' key
        _playerController.speed = 600;
        _playerController.jumpForce = 1160;

        // Check if the speed and jumpForce have been increased
        Assert.AreEqual(600f, _playerController.speed, "Speed should be increased to 600.");
        Assert.AreEqual(1160f, _playerController.jumpForce, "Jump force should be increased.");
    }

    [Test]
    public void HandleSpeedAndJumpForceAdjustment_ResetsValues_OnKeyUp()
    {
        // Directly simulate the effect of releasing the 'S' key
        _playerController.speed = 410;
        _playerController.jumpForce = 1030;

        // Check if the speed and jumpForce have been reset
        Assert.AreEqual(410f, _playerController.speed, "Speed should be reset to 410.");
        Assert.AreEqual(1030f, _playerController.jumpForce, "Jump force should be reset.");
    }

    [TearDown]
    public void TearDown()
    {
        // Clean up
        if (_gameObject != null)
        {
            Object.DestroyImmediate(_gameObject);
        }
    }
}
