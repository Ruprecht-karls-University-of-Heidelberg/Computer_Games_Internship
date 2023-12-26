using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairwayController : MonoBehaviour
{
    public bool isVerticalStairway;  // Indicates if the stairway moves vertically or horizontally.
    public float moveSpeed;          // Speed at which the stairway moves.

    // Update is called once per frame.
    void Update()
    {
        if (isVerticalStairway)
        {
            // Call the function to move the stairway vertically.
            MoveVertical();
        }
        else
        {
            // Reverse the direction of the stairway's movement when it reaches specific positions.
            if (transform.position.x < 59)
            {
                moveSpeed = -moveSpeed;
            }
            else if (transform.position.x > 65)
            {
                moveSpeed = -moveSpeed;
            }

            // Call the function to move the stairway horizontally.
            MoveHorizontal();
        }
    }

    // Called when another collider makes contact with the stairway's collider.
    private void OnCollisionEnter2D(Collision2D other)
    {
        // If the stairway collides with a stone, reverse its movement direction.
        if (other.gameObject.CompareTag("Stone"))
        {
            moveSpeed = -moveSpeed;
        }
    }

    // Function to move the stairway vertically in the downward direction.
    private void MoveVertical()
    {
        transform.Translate(Time.deltaTime * moveSpeed * Vector3.down);
    }

    // Function to move the stairway horizontally in the left direction.
    private void MoveHorizontal()
    {
        transform.Translate(Time.deltaTime * moveSpeed * Vector3.left);
    }
}
