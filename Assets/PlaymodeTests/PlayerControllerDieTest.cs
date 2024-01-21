using NUnit.Framework;
using UnityEngine;
using PlayerScripts;
using AdditionalScripts;

[TestFixture]
public class PlayerControllerDieTest
{
    private PlayerController _playerController;
    private GameObject _playerGameObject;
    private AudioSource _audioSource;
    
    [SetUp]
    public void SetUp()
    {
        // Create a new game object and add the PlayerController component to it
        _playerGameObject = new GameObject();
        _playerController = _playerGameObject.AddComponent<PlayerController>();
        _audioSource = _playerGameObject.AddComponent<AudioSource>();

        // Ensure the GameObject has the components that PlayerController expects to interact with
        _playerGameObject.AddComponent<Rigidbody2D>();
        _playerGameObject.AddComponent<Animator>();

        // Initialize necessary components that Die() might rely on
        _playerController.playerCol = new GameObject("playerCol");
        _playerController.dieSound = AudioClip.Create("dieSound", 44100, 1, 44100, false);

        // Set AudioSource for playing sounds
        _playerController._playerAudio = _audioSource;

        // Assuming ToolController is a static class that the PlayerController depends on.
        // Make sure it's in the expected initial state.
        ToolController.IsDead = false;
    }

    [Test]
    public void Die_WhenCalled_PerformsDeathOperations()
    {
        // Arrange is already done in SetUp method.
        
        // Act
        _playerController.Die();

        // Assert
        Assert.IsTrue(ToolController.IsDead, "ToolController.IsDead should be true after dying.");
        // Add further assertions here if needed, like checking if playerCol is deactivated, etc.
    }

    [TearDown]
    public void TearDown()
    {
        // Cleanup code if necessary.
        Object.Destroy(_playerGameObject);
    }
}
