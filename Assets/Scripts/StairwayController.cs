using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//after changing

public class StairwayController : MonoBehaviour
{
    public bool isVerticalStairway;  // Denotes if the stairway operates in a vertical or horizontal manner.
    public float moveSpeed;          // Defines the velocity of the stairway's movement.

    // Update is invoked every frame.
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

    // Triggered upon collision with another collider.
    private void OnCollisionEnter2D(Collision2D other)
    {
        // Reverse the stairway's direction upon collision with a stone object.
        if (other.gameObject.CompareTag("Stone"))
        {
            ReverseDirection();
        }
    }

    // Method for downward vertical movement of the stairway.
    private void MoveStairwayVertically()
    {
        transform.Translate(Time.deltaTime * moveSpeed * Vector3.down);
    }

    // Method for leftward horizontal movement of the stairway.
    private void MoveStairwayHorizontally()
    {
        transform.Translate(Time.deltaTime * moveSpeed * Vector3.left);
    }

    // Reverses the direction of movement when reaching specified horizontal positions.
    private void ReverseDirectionAtBoundaries()
    {
        if (transform.position.x < 59 || transform.position.x > 65)
        {
            ReverseDirection();
        }
    }

    // Reverses the current direction of movement.
    private void ReverseDirection()
    {
        moveSpeed = -moveSpeed;
    }
}
