using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.TestTools.Utils;

public class PlayerControllerTests
{
    [UnityTest]
    public IEnumerator TestFireballInstantiationDirectly()
    {
        // 创建一个fireBallPrefab实例
        var fireBallPrefab = new GameObject();

        // 创建一个fireBallParent对象
        var fireBallParent = new GameObject().transform;

        // 实例化一个新的火球对象
        var instantiatedFireball = Object.Instantiate(fireBallPrefab, fireBallParent);

        // 等待一个小的时间段来处理实例化
        yield return new WaitForSeconds(0.1f);

        // 检查fireBallParent下是否有一个新的子对象
        Assert.AreEqual(1, fireBallParent.childCount, "A fireball should have been instantiated");

        // 清理
        Object.DestroyImmediate(fireBallPrefab);
        Object.DestroyImmediate(fireBallParent.gameObject);
        Object.DestroyImmediate(instantiatedFireball);
    }
}
