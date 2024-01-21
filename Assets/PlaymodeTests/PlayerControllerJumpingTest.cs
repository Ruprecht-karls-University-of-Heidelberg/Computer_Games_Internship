using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class SimplePlayerController : MonoBehaviour
{
    public Rigidbody2D _playerRb;
    public bool _isOnGround;
    public float jumpForce = 795f;

    public void HandleJumping()
    {
        if (_isOnGround)
        {
            _isOnGround = false;
            _playerRb.AddForce(new Vector2(0f, jumpForce));
        }
    }
}

public class SimplePlayerControllerJumpingTest
{
    private SimplePlayerController simplePlayerController;
    private GameObject playerGameObject;

    [SetUp]
    public void Setup()
    {
        // Create a SimplePlayerController instance
        playerGameObject = new GameObject();
        simplePlayerController = playerGameObject.AddComponent<SimplePlayerController>();

        // Add required components
        var rigidbody2D = playerGameObject.AddComponent<Rigidbody2D>(); // Add Rigidbody2D component
        
        // Set SimplePlayerController's properties
        simplePlayerController._playerRb = rigidbody2D;

        // Set the player as "on the ground"
        simplePlayerController._isOnGround = true;
    }

    [UnityTest]
    public IEnumerator TestJumpingPhysically()
    {
        // Manually simulate the effects of a jump
        simplePlayerController.HandleJumping();

        // Wait for a short duration to simulate time passing
        yield return new WaitForSeconds(0.1f);

        // Check that the Rigidbody2D's velocity has changed in the Y-axis
        Assert.AreNotEqual(0f, simplePlayerController._playerRb.velocity.y, "Rigidbody2D's vertical velocity should change after jumping");

        // Check that _isOnGround is set to false
        Assert.IsFalse(simplePlayerController._isOnGround, "Player should not be on ground after jumping");
    }

    [TearDown]
    public void Teardown()
    {
        // Destroy the game object created for the test
        Object.DestroyImmediate(playerGameObject);
    }
}
