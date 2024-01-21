using NUnit.Framework;
using UnityEngine;
using PlayerScripts;
using AdditionalScripts;

[TestFixture]
public class PlayerControllerTurnintoBig
{
    private PlayerController _playerController;
    private GameObject _playerGameObject;
    
    [SetUp]
    public void SetUp()
    {
        // Create a new game object and add the PlayerController component to it
        _playerGameObject = new GameObject();
        _playerController = _playerGameObject.AddComponent<PlayerController>();

        // Initialize necessary components that ChangeAnim() might rely on
        _playerController.bigPlayer = new GameObject("bigPlayer");
        _playerController.smallPlayer = new GameObject("smallPlayer");
        _playerController.bigPlayerCollider = new GameObject("bigPlayerCollider");
        _playerController.smallPlayerCollider = new GameObject("smallPlayerCollider");

        // Ensure the GameObject has the components that PlayerController expects to interact with
        _playerGameObject.AddComponent<Rigidbody2D>();
        _playerGameObject.AddComponent<Animator>();
        
        // Assuming ToolController is a static class that the PlayerController depends on.
        // Make sure it's in the expected initial state.
        ToolController.PlayerTag = "Player";
        ToolController.IsBigPlayer = false;
        ToolController.IsFirePlayer = false;
    }

    [Test]
    public void TurnIntoBigPlayer_WhenCalled_ChangesPlayerToBigPlayer()
    {
        // Arrange is already done in SetUp method.

        // Act
        _playerController.TurnIntoBigPlayer();

        // Assert
        Assert.IsTrue(_playerGameObject.CompareTag("BigPlayer") || _playerGameObject.CompareTag("UltimateBigPlayer"), "Player tag is not set to BigPlayer or UltimateBigPlayer.");
        Assert.IsTrue(ToolController.IsBigPlayer, "ToolController.IsBigPlayer should be true after turning into a big player.");
        // Add further assertions here if needed.
    }

    [TearDown]
    public void TearDown()
    {
        // Cleanup code if necessary.
        Object.Destroy(_playerGameObject);
    }
}
