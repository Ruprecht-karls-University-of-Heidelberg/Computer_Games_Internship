using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the movement of a stairway object within the game, allowing for both vertical and horizontal movement.
/// </summary>
public class StairwayController : MonoBehaviour
{
    public bool isVerticalStairway;  // Indicates if the stairway moves vertically or horizontally.
    public float moveSpeed;          // Speed at which the stairway moves.

    /// <summary>
    /// Update is called once per frame to handle stairway movement.
    /// </summary>
    void Update()
    {
        // Check if the stairway should move vertically.
        if (isVerticalStairway)
        {
            MoveVertical();
        }
        else
        {
            // Reverse the direction of horizontal movement at specific positions.
            if (transform.position.x < 59)
            {
                moveSpeed = -moveSpeed;
            }
            else if (transform.position.x > 65)
            {
                moveSpeed = -moveSpeed;
            }

            MoveHorizontal();
        }
    }

    /// <summary>
    /// Called when another collider makes contact with the stairway's collider.
    /// </summary>
    /// <param name="other">The collision data.</param>
    private void OnCollisionEnter2D(Collision2D other)
    {
        // Reverse movement direction upon collision with a stone.
        if (other.gameObject.CompareTag("Stone"))
        {
            moveSpeed = -moveSpeed;
        }
    }

    /// <summary>
    /// Moves the stairway vertically in the downward direction.
    /// </summary>
    private void MoveVertical()
    {
        transform.Translate(Time.deltaTime * moveSpeed * Vector3.down);
    }

    /// <summary>
    /// Moves the stairway horizontally in the left direction.
    /// </summary>
    private void MoveHorizontal()
    {
        transform.Translate(Time.deltaTime * moveSpeed * Vector3.left);
    }
}
