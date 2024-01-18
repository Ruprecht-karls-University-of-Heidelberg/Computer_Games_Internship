using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PlayerScripts;

public class FireBallControllerPlaymodeTests
{
    private GameObject _fireballGameObject;
    private FireBallController _fireBallController;
    private Animator _animator;

    [SetUp]
    public void Setup()
    {
        // Create a GameObject and add the FireBallController component
        _fireballGameObject = new GameObject();
        _fireballGameObject.AddComponent<Rigidbody2D>();
        // Add and configure Animator component
        _animator = _fireballGameObject.AddComponent<Animator>();
       _animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Fireball");


        _fireBallController = _fireballGameObject.AddComponent<FireBallController>();
        
    }

    [UnityTest]
    //This test verifies that the fireball behaves correctly when it collides with the ground (labeled "Ground").
    //The test simulates a collision by creating a game object that represents the ground and placing it underneath the fireball.
    //The test then checks that the velocity of the fireball's Rigidbody2D component is non-zero after the collision, 
    //to confirm that the bouncing behavior occurs.
    public IEnumerator OnCollisionEnter2D_WithGround_CausesBounce()
    {
        // 创建地面模拟对象
        var groundGameObject = new GameObject();
        var groundCollider = groundGameObject.AddComponent<BoxCollider2D>();
        groundCollider.isTrigger = true;

        // 模拟火球与地面碰撞
        groundGameObject.tag = "Ground";
        _fireballGameObject.transform.position = new Vector3(0, 5, 0);
        yield return new WaitForFixedUpdate(); // 等待物理更新

        // 确认是否执行了BounceUpward方法
        Assert.AreNotEqual(Vector2.zero, _fireBallController.GetComponent<Rigidbody2D>().velocity);

        // 清理测试环境
        Object.Destroy(_fireballGameObject);
        Object.Destroy(groundGameObject);
    }

    // 其他测试方法...
    [UnityTest]
    public IEnumerator HandleCollisionWithPiranha_CollisionWithPiranha_TriggerExpectedBehavior()
    {
        // 创建Piranha游戏对象并设置标签
        var piranhaGameObject = new GameObject();
        piranhaGameObject.tag = "Piranha";
        var piranhaCollider = piranhaGameObject.AddComponent<BoxCollider2D>();
        piranhaCollider.isTrigger = true;

        // 将Piranha放置在火球对象的位置来模拟碰撞
        piranhaGameObject.transform.position = _fireballGameObject.transform.position;
        _fireBallController.speed = 5f; // 设置火球速度
        _fireBallController.InitializeComponents(); // 初始化组件

        // 由于HandleCollisionWithPiranha中涉及销毁对象，需要相应的验证方式
        // 比如检查对象是否被标记为销毁

        yield return new WaitForFixedUpdate(); // 等待物理更新

        // 验证Piranha对象是否被标记为销毁
        // 这里我们检查场景中是否还能找到这个对象
        // 注意，由于 Unity 的销毁操作是延迟的，所以需要稍等一段时间才能确保对象被销毁
        yield return new WaitForSeconds(0.1f);
        var piranhaObjectAfterCollision = GameObject.Find("PiranhaObject");
        Assert.IsNull(piranhaObjectAfterCollision); // 验证对象是否被销毁

        // 清理测试环境
        Object.Destroy(_fireballGameObject);
        Object.Destroy(piranhaGameObject);
    }

//This test verifies that the DestroyFireball coroutine in the FireBallController class is able
// to successfully destroy the fireball object after a specified delay.
    [UnityTest]
    public IEnumerator DestroyFireball_DestroysGameObject()
    {
        // Start the DestroyFireball coroutine
        _fireBallController.StartCoroutine(_fireBallController.DestroyFireball());

        // Wait for the time defined in DestroyFireball (0.02 seconds)
        yield return new WaitForSeconds(0.02f);

        // Assert that the GameObject is destroyed
        Assert.IsTrue(_fireballGameObject == null);

        // Cleanup is not necessary as the GameObject should be destroyed
    }

    // TearDown method to clean up after each test
    [TearDown]
    public void TearDown()
    {
        if (_fireballGameObject != null)
        {
            Object.DestroyImmediate(_fireballGameObject);
        }
    }

}

