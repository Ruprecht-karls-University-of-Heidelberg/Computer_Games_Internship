using UnityEngine;
using NUnit.Framework;
using PlayerScripts;
using AdditionalScripts;

public class PlayerControllerGameFinishStateTest
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
    }

    [Test]
    public void HandleGameFinishState_PlayerMovesCorrectlyOnGameFinish()
    {
        // Set the game to the finished state
        ToolController.IsGameFinish = true;

        // Get the initial position of the player
        Vector3 initialPosition = _playerController.transform.position;

        // Call the HandleGameFinishState function
        _playerController.HandleGameFinishState();

        // Get the new position of the player
        Vector3 newPosition = _playerController.transform.position;

        // Verify that the player has moved
        Assert.AreNotEqual(initialPosition, newPosition, "Player should have moved after game finish.");

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
