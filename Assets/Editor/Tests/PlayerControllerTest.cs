using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using AdditionalScripts;
using PlayerScripts;
using System.Reflection;
using UnityEditor.Animations;


public class PlayerControllerTests
{
    private PlayerController _playerController;
    private GameObject _gameObject;
    private GameObject fireBallPrefab;
    private AudioSource audioSource;

    private GameObject playerObject;

    private Animator animator;
    private AnimatorController animatorController;

    [SetUp]
    public void Setup()
    {
        // 创建一个 GameObject 并添加 PlayerController 组件
        _gameObject = new GameObject();
        _playerController = _gameObject.AddComponent<PlayerController>();

        // 添加实际的 AudioSource 和 Rigidbody2D 组件
        _playerController._playerAudio = _gameObject.AddComponent<AudioSource>();
        _playerController._playerRb = _gameObject.AddComponent<Rigidbody2D>();
        _playerController._playerAnim = _gameObject.AddComponent<Animator>();
        

        // 创建一个火球预制体并赋值给 PlayerController
        fireBallPrefab = new GameObject();
        // 确保火球预制体有一个 Renderer 或 SpriteRenderer 组件
        fireBallPrefab.AddComponent<SpriteRenderer>();
        _playerController.fireBallPrefab = fireBallPrefab;
        _playerController.fireBallParent = new GameObject().transform;

        // 设置标签以便于测试时找到实例化的火球
        fireBallPrefab.tag = "Fireball";

        // Setting up the ToolController
        ToolController.IsFirePlayer = true;
    }


    [Test]
    public void Awake_InitializesCorrectly()
    {
        // 调用 Awake 方法
        _playerController.Awake();

        // 验证是否正确设置了初始值
        Assert.IsTrue(_playerController._isFacingRight);
        Assert.IsFalse(_playerController.isInvulnerable);

        // 验证是否获取了所需的组件
        Assert.IsNotNull(_playerController._playerAudio);
        Assert.IsNotNull(_playerController._playerRb);
        Assert.IsNotNull(_playerController._playerAnim);

        // 验证其他初始状态
        Assert.IsFalse(_playerController._isFinish);
        Assert.IsTrue(_playerController._isOnGround);
        Assert.IsFalse(_playerController.isInCastle);

        // 如果使用了 ToolController, 还可以验证相关逻辑
        // Assert.AreEqual(expectedValue, _playerController.tag);
    }
    [UnityTest]
    public IEnumerator TestHandleFireballShooting()
    {
        ToolController.IsFirePlayer = true; // 确保玩家可以射击火球

        // 调用 HandleFireballShooting 方法
        _playerController.HandleFireballShooting();

        // 等待一帧以允许实例化发生
        yield return null;

        // 检查是否实例化了火球
        GameObject instantiatedFireball = GameObject.FindWithTag("Fireball");
        Assert.IsNotNull(instantiatedFireball, "Fireball was not instantiated.");

        // 如果有必要进行额外的清理
        if (instantiatedFireball != null)
        {
            Object.DestroyImmediate(instantiatedFireball);
        }
    }
    

    [TearDown]
    public void Teardown()
    {
        // 清理工作
        Object.DestroyImmediate(_gameObject);
    }
}
