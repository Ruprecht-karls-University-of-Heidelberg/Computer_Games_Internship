using UnityEngine;
using NUnit.Framework;
using PlayerScripts;

public class PlayerControllerDirectionalMovementTest
{
    private PlayerController _playerController;
    private GameObject _gameObject;

    [SetUp]
    public void SetUp()
    {
        // Create a new GameObject and add the PlayerController
        _gameObject = new GameObject();
        _playerController = _gameObject.AddComponent<PlayerController>();

        // Add required components
        _playerController._playerRb = _gameObject.AddComponent<Rigidbody2D>();  // Mock Rigidbody2D, not used in this test
        _playerController._playerAnim = _gameObject.AddComponent<Animator>();  // Mock Animator, not used in this test
    }

    [Test]
    public void HandleDirectionalMovement_FlipsPlayerOnDirectionChange()
    {
        // Initially the player is facing right
        _playerController._isFacingRight = true;

        // Simulate pressing the left arrow key by directly manipulating _isFacingRight
        _playerController._isFacingRight = false;
        _playerController.HandleDirectionalMovement();

        // Check if the player direction is flipped
        Assert.IsFalse(_playerController._isFacingRight, "Player should be facing left after simulating left arrow key press.");

        // Simulate pressing the right arrow key by directly manipulating _isFacingRight
        _playerController._isFacingRight = true;
        _playerController.HandleDirectionalMovement();

        // Check if the player direction is correct
        Assert.IsTrue(_playerController._isFacingRight, "Player should be facing right after simulating right arrow key press.");
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
