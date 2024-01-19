using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using AdditionalScripts;

// Your test class
[TestFixture]
public class PowerUpsControllerTests
{
    private GameObject powerUpObject;
    private PowerUpsController powerUpsController;
    
    // Setup before each test
    [SetUp]
    public void Setup()
    {
        // Create a new GameObject with the required components
        powerUpObject = new GameObject();
        powerUpsController = powerUpObject.AddComponent<PowerUpsController>();
        powerUpObject.AddComponent<AudioSource>();
        powerUpObject.AddComponent<Rigidbody2D>();
        powerUpObject.AddComponent<BoxCollider2D>();

    }

    // Teardown after each test
    [TearDown]
    public void Teardown()
    {
            Object.DestroyImmediate(powerUpObject);
            
    }

    // Test for Awake method
    [Test]
    public void PowerUpsController_Awake_InitializesCorrectly()
    {
        // Act - call the Awake method
        powerUpsController.Awake();

        // Assert - check if the conditions after Awake are met
        Assert.IsNotNull(powerUpsController.GetComponent<AudioSource>(), "AudioSource component should be assigned.");
        Assert.IsTrue(Physics2D.GetIgnoreLayerCollision(9, 10), "Layer collision should be ignored between layers 9 and 10.");
        Assert.AreEqual(powerUpsController.transform.position.y, powerUpsController.GetType().GetField("_firstYPos", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(powerUpsController), "First Y position should be set to the current transform's y position.");
    }

        // Test for OnCollisionEnter2D method
}
