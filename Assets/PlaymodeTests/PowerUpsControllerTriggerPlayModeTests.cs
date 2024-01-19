using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using AdditionalScripts;

public class PowerUpsControllerTriggerPlayModeTests
{
    private GameObject powerUpObject;
    private GameObject triggerObject;
    private PowerUpsController powerUpsController;
    private Rigidbody2D powerUpRigidbody;
    private BoxCollider2D powerUpCollider;
    private BoxCollider2D triggerCollider;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        // Create a new GameObject with the required components for the PowerUp
        powerUpObject = new GameObject("PowerUp");
        powerUpsController = powerUpObject.AddComponent<PowerUpsController>();
        powerUpObject.AddComponent<AudioSource>();
        powerUpRigidbody = powerUpObject.AddComponent<Rigidbody2D>();
        powerUpCollider = powerUpObject.AddComponent<BoxCollider2D>();
        powerUpCollider.isTrigger = true;

        // Create another GameObject to simulate the trigger object
        triggerObject = new GameObject("Trigger");
        triggerCollider = triggerObject.AddComponent<BoxCollider2D>();
        triggerCollider.isTrigger = true;

        // Initialize the PowerUp (mimicking the Awake function)
        powerUpsController.Awake();

        // Set the trigger object's position to ensure a collision will occur
        triggerObject.transform.position = powerUpObject.transform.position;

        // Configure the physics for the test
        powerUpRigidbody.isKinematic = true;

        yield return null;  // Wait for the next frame (optional)
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        // Clean up
        Object.Destroy(powerUpObject);
        Object.Destroy(triggerObject);
        yield return null;
    }

    [UnityTest]
    public IEnumerator PowerUpsController_OnTriggerEnter2D_UpdatesCoinCollection_WhenCoinIsCollected()
    {
        // Arrange
        powerUpObject.tag = "Coin";  // Simulate power-up object as a coin
        triggerObject.tag = "Player";  // Simulate trigger object as a player
        int initialCollectedCoins = ToolController.CollectedCoin;

        // Act
        powerUpsController.OnTriggerEnter2D(triggerCollider);

        // Wait for the next frame to ensure the OnTriggerEnter2D logic is processed
        yield return new WaitForSeconds(1);

        // Assert
        Assert.Greater(ToolController.CollectedCoin, initialCollectedCoins, "CollectedCoin should increase after collecting a coin.");
    }

    // Add more tests here as needed
}
