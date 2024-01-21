// using System.Collections;
// using NUnit.Framework;
// using UnityEngine;
// using UnityEngine.TestTools;
// using UnityEngine.SceneManagement;

// public class PowerUpsControllerCollisionPlayModeTests
// {
//     private GameObject powerUpObject;
//     private GameObject colliderObject;
//     private PowerUpsController powerUpsController;
//     private Rigidbody2D powerUpRigidbody;
//     private Rigidbody2D colliderRigidbody;

//     [UnitySetUp]
//     public IEnumerator SetUp()
//     {
//         // Load your test scene here if you have one
//         // SceneManager.LoadScene("YourTestScene", LoadSceneMode.Single);
//         yield return null;  // Wait for the scene to load

//         // Create a new GameObject with the required components for the PowerUp
//         powerUpObject = new GameObject("PowerUp");
//         powerUpsController = powerUpObject.AddComponent<PowerUpsController>();
//         powerUpObject.AddComponent<AudioSource>();
//         powerUpRigidbody = powerUpObject.AddComponent<Rigidbody2D>();
//         powerUpObject.AddComponent<BoxCollider2D>();

//         // Create another GameObject to simulate the collider object
//         colliderObject = new GameObject("Collider");
//         colliderObject.AddComponent<BoxCollider2D>();
//         colliderRigidbody = colliderObject.AddComponent<Rigidbody2D>();

//         // Initialize the PowerUp (mimicking the Awake function)
//         powerUpsController.Awake();

//         // Set the collider object's position to ensure a collision will occur
//         colliderObject.transform.position = powerUpObject.transform.position;

//         // Configure the physics for the test
//         powerUpRigidbody.isKinematic = false;
//         colliderRigidbody.isKinematic = false;

//         // set initial speedRight value
//         powerUpsController.speedRight = 10; // Set a non-zero initial value

//         // Add an AudioListener to the scene
//         var listener = new GameObject("Listener");
//         listener.AddComponent<AudioListener>();
//     }

//     [UnityTearDown]
//     public IEnumerator TearDown()
//     {
//         // Clean up
//         Object.Destroy(powerUpObject);
//         Object.Destroy(colliderObject);
//         // Optionally destroy the listener if you created one
//         var listener = GameObject.Find("Listener");
//         if (listener != null) {
//             Object.Destroy(listener);
//         }
//         yield return null;
//     }

//     [UnityTest]
//     public IEnumerator PowerUpsController_OnCollisionEnter2D_ChangesSpeedRight_WhenCollidingWithStone()
//     {
//         // Arrange
//         int initialSpeedRight = powerUpsController.speedRight;
//         colliderObject.tag = "Stone";  // Simulate colliding with a stone
//         powerUpRigidbody.AddForce(Vector2.right * 10, ForceMode2D.Impulse); // Add force to ensure a collision occurs

//         // Act
//         // Wait for the physics engine to process the collision
//         yield return new WaitForSeconds(1);

//         // Assert
//         Assert.AreNotEqual(initialSpeedRight, powerUpsController.speedRight, "speedRight should change after collision with a stone.");
//     }

//     // Add more tests here as needed
// }
