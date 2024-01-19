using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor;

using PlayerScripts;


public class FireBallControllerTests
{
    private FireBallController _fireBallController;
    private GameObject _gameObject;

    [SetUp]
    public void Setup()
    {
        // Create a GameObject and add FireBallController component
        _gameObject = new GameObject();
        _fireBallController = _gameObject.AddComponent<FireBallController>();

        // Add Rigidbody2D and Animator components
        _gameObject.AddComponent<Rigidbody2D>();
        _gameObject.AddComponent<Animator>();
    }


//This test verifies that the InitializeComponents method of the FireBallController class correctly assigns the Rigidbody2D
// and Animator components to the _fireBallRb and _fireBallAnim member variables.
//The test is done by calling the InitializeComponents method and then checking whether the Rigidbody2D and Animator components
// exist on _gameObject (the GameObject created in the test).
    [Test]
    public void InitializeComponents_AssignsRigidbodyAndAnimator()
    {
        // Call the method
        _fireBallController.InitializeComponents();

        // Assert that Rigidbody and Animator are assigned
        Assert.IsNotNull(_fireBallController.GetComponent<Rigidbody2D>());
        Assert.IsNotNull(_fireBallController.GetComponent<Animator>());
    }

    //This test verifies that the FireBallController class's SetInitialVelocity method correctly sets the fireball's initial velocity.
    //The test first sets the speed property of _fireBallController and then calls the InitializeComponents method and SetInitialVelocity
    //method. Finally, the test verifies that the velocity of the Rigidbody2D component on the GameObject is 
    //correctly set to the expected value (Vector2(5f, 0) in this case).
   
    [Test]
    public void SetInitialVelocity_SetsCorrectVelocity()
    {
        // Arrange
        _fireBallController.speed = 5f; //set expected speed
        _fireBallController.InitializeComponents(); // initialize component

        // Act
        _fireBallController.SetInitialVelocity();

        // Assert
        var rigidbody2D = _fireBallController.GetComponent<Rigidbody2D>();
        Assert.AreEqual(new Vector2(5f, 0), rigidbody2D.velocity); // check if the speed is setted correctedly
    }



}


