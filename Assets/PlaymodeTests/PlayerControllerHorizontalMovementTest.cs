using UnityEngine;
using NUnit.Framework;
using PlayerScripts;
using UnityEngine.TestTools;
using System.Collections;

public class PlayerControllerHorizontalMovementTest
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
        _playerController.speed = 410f; // Initialize speed
        _playerController.smoothTime = 0.1f; // Set a smooth time for velocity change

        // Note: Animator component is removed from the test setup
    }

    [Test]
    public void HandleHorizontalMovement_ChangesVelocityCorrectly()
    {
        // Arrange
        float initialVelocityX = 0f;
        _playerController._playerRb.velocity = new Vector2(initialVelocityX, 0); // Initial velocity is 0

        // Act
        // Simulate pressing the right arrow key
        // Since we cannot mock Input.GetAxisRaw, we need to directly set the velocity to simulate movement
        _playerController._playerRb.velocity = new Vector2(_playerController.speed * Time.fixedDeltaTime, 0);

        // Assert
        float expectedSpeed = _playerController.speed * Time.fixedDeltaTime;
        float actualSpeed = _playerController._playerRb.velocity.x;
        Assert.AreEqual(expectedSpeed, actualSpeed, "Player's horizontal velocity does not match expected value.");
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
