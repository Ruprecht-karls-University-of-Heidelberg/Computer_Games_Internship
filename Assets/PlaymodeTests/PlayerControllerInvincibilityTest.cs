using UnityEngine;
using NUnit.Framework;
using PlayerScripts;
using UnityEngine.TestTools;
using System.Collections;

public class PlayerControllerInvincibilityTest
{
    private PlayerController _playerController;
    private GameObject _gameObject;

    [SetUp]
    public void SetUp()
    {
        // Create a new GameObject and add the PlayerController
        _gameObject = new GameObject();
        _playerController = _gameObject.AddComponent<PlayerController>();

        // Mock components to avoid errors related to their absence
        _playerController._playerRb = _gameObject.AddComponent<Rigidbody2D>();
        _playerController._playerAnim = new GameObject().AddComponent<Animator>(); // Mock Animator

        // Initialize necessary fields
        _playerController.isInvincible = true;
        _playerController._startInvincible = Time.time;
    }

    [Test]
    public void HandleInvincibility_CorrectlyManagesInvincibility()
    {
        // Arrange: Prepare the conditions for the test
        float invincibleDuration = 5.0f; // Example duration, adjust as needed

        // Act: Simulate some time passing
        _playerController._startInvincible = Time.time - invincibleDuration; // Simulate that invincibility started 'invincibleDuration' seconds ago
        _playerController.HandleInvincibility();

        // Assert: Check if the invincibility is being handled correctly
        bool expectedInvincibilityStatus = invincibleDuration < 10; // Assuming invincibility lasts for 10 seconds
        Assert.AreEqual(expectedInvincibilityStatus, _playerController.isInvincible, "Player invincibility status does not match expected value.");
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
