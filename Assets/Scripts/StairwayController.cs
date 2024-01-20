using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the movement of a stairway object.
/// </summary>
public class StairwayController : MonoBehaviour
{
    /// <summary>
    /// Denotes if the stairway operates in a vertical or horizontal manner.
    /// </summary>
    public bool isVerticalStairway;

    /// <summary>
    /// Defines the velocity of the stairway's movement.
    /// </summary>
    public float moveSpeed;

    /// <summary>
    /// Update is invoked every frame.
    /// </summary>
    void Update()
    {
        if (isVerticalStairway)
        {
            // Execute the method to vertically maneuver the stairway.
            MoveStairwayVertically();
        }
        else
        {
            // Check and reverse the stairway's direction at certain horizontal positions.
            ReverseDirectionAtBoundaries();

            // Execute the method to horizontally maneuver the stairway.
            MoveStairwayHorizontally();
        }
    }

    /// <summary>
    /// Triggered upon collision with another collider.
    /// </summary>
    /// <param name="other">The collider that the stairway collided with.</param>
    private void OnCollisionEnter2D(Collision2D other)
    {
        // Reverse the stairway's direction upon collision with a stone object.
        if (other.gameObject.CompareTag("Stone"))
        {
            ReverseDirection();
        }
    }

    /// <summary>
    /// Method for downward vertical movement of the stairway.
    /// </summary>
    private void MoveStairwayVertically()
    {
        transform.Translate(Time.deltaTime * moveSpeed * Vector3.down);
    }

    /// <summary>
    /// Method for leftward horizontal movement of the stairway.
    /// </summary>
    private void MoveStairwayHorizontally()
    {
        transform.Translate(Time.deltaTime * moveSpeed * Vector3.left);
    }

    /// <summary>
    /// Reverses the direction of movement when reaching specified horizontal positions.
    /// </summary>
    private void ReverseDirectionAtBoundaries()
    {
        if (transform.position.x < 59 || transform.position.x > 65)
        {
            ReverseDirection();
        }
    }

    /// <summary>
    /// Reverses the current direction of movement.
    /// </summary>
    private void ReverseDirection()
    {
        moveSpeed = -moveSpeed;
    }
}
