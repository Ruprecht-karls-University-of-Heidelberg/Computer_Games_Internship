using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

public class PowerUpsControllerMovementPlayModeTests
{
    private GameObject powerUpObject;
    private PowerUpsController powerUpsController;
    private Rigidbody2D powerUpRigidbody;
    private BoxCollider2D powerUpCollider;
    private Vector2 initialPosition;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        // Create a new GameObject with the required components for the PowerUp
        powerUpObject = new GameObject("PowerUp");
        powerUpsController = powerUpObject.AddComponent<PowerUpsController>();
        powerUpObject.AddComponent<AudioSource>();
        powerUpRigidbody = powerUpObject.AddComponent<Rigidbody2D>();
        powerUpCollider = powerUpObject.AddComponent<BoxCollider2D>();

        // Initialize the PowerUp (mimicking the Awake function)
        powerUpsController.Awake();
        powerUpsController.speedRight = 1;
        powerUpsController.speedUp = 1;

        initialPosition = powerUpObject.transform.position;

        // 创建一个带有 AudioListener 的 GameObject，以避免 "没有音频监听器" 的错误
        new GameObject("AudioListener").AddComponent<AudioListener>();

        // 确保 PowerUp 对象的运动不受物理引擎影响
        powerUpRigidbody.isKinematic = true;

        yield return null;  // Wait for the next frame (optional)
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        // Clean up
        Object.Destroy(powerUpObject);
        yield return null;
    }

    [UnityTest]
    public IEnumerator PowerUpsController_HandlePowerUpMovement_MovesRight_WhenIsMoving()
    {
        // Arrange
        powerUpsController.isTouchByPlayer = true;
        powerUpsController.isMoving = true;
        powerUpObject.tag = "BigMushroom";  // Simulate as a BigMushroom

        // Act
        float initialXPosition = powerUpObject.transform.position.x;
        float timeToMove = 2.0f;
        yield return new WaitForSeconds(timeToMove);  // Wait for the PowerUp to move

        // Assert
        Assert.Greater(powerUpObject.transform.position.x, initialXPosition, "PowerUp should move right when isMoving is true.");
    }

    [UnityTest]
    public IEnumerator PowerUpsController_HandlePowerUpMovement_MovesUp_WhenTouchedByPlayer()
    {
        // Arrange
        powerUpsController.isTouchByPlayer = true;
        powerUpsController.isMoving = false;
        powerUpObject.tag = "Untagged";  // Use 'Untagged' to avoid the tag not defined error

        // 重置 PowerUp 对象的位置
        powerUpObject.transform.position = initialPosition;

        // Act
        float initialYPosition = powerUpObject.transform.position.y;
        float timeToMove = 2.0f;
        
        // 在指定时间内模拟 Update 方法的调用
        for (float t = 0; t < timeToMove; t += Time.deltaTime)
        {
            powerUpsController.HandlePowerUpMovement();
            yield return null;  // 等待一帧
        }

        // Assert
        Assert.Greater(powerUpObject.transform.position.y, initialYPosition, "PowerUp should move up when touched by player and not a coin.");
    }

    // Add more tests here as needed
}
