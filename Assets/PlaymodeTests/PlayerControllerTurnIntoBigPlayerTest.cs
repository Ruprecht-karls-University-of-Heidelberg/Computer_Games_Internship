using UnityEngine;
using NUnit.Framework;
using PlayerScripts;
using UnityEngine.TestTools;
using System.Collections;
using AdditionalScripts;

public class PlayerControllerTurnIntoBigPlayerTest
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
        _playerController._playerRb = _gameObject.AddComponent<Rigidbody2D>(); // Rigidbody2D, not used in this test
        // Initialize GameObjects to avoid NullReferenceException in ChangeAnim method
        _playerController.bigPlayer = new GameObject();
        _playerController.bigPlayerCollider = new GameObject();
        _playerController.smallPlayer = new GameObject();
        _playerController.smallPlayerCollider = new GameObject();
    }

    [Test]
    public void TurnIntoBigPlayer_UpdatesPlayerStateCorrectly()
    {
        // Arrange: Set player tag to simulate different initial states
        _gameObject.tag = "Player"; // Initial state as small player
        ToolController.IsBigPlayer = false; // ToolController's state before transformation

        // Act: Call the function to transform the player into a big player
        _playerController.TurnIntoBigPlayer();

        // Assert: Verify if the player's tag is updated correctly
        Assert.IsTrue(_gameObject.tag == "BigPlayer" || _gameObject.tag == "UltimateBigPlayer", "Player tag should be updated to BigPlayer or UltimateBigPlayer.");

        // Assert: Verify if the ToolController's state is updated correctly
        Assert.IsTrue(ToolController.IsBigPlayer, "ToolController's IsBigPlayer should be true after transformation.");
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
