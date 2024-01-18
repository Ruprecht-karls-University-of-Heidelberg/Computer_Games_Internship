using UnityEngine;
using AdditionalScripts;

namespace SystemScripts
{
    public class BlockPlayer : MonoBehaviour
    {
        /// <summary>
        /// Called when a 2D collision occurs. If the colliding object isn't a "Player", 
        /// the current object's BoxCollider2D is set to trigger mode.
        /// </summary>
        /// <param name="other">The colliding object's data.</param>
        private void OnCollisionEnter2D(Collision2D other)
        {
            // If the colliding object is not the "Player", set this object's collider to trigger mode.
            // This means objects can pass through without physical interaction.
            if (!other.gameObject.CompareTag("Player"))
            {
                GetComponent<BoxCollider2D>().isTrigger = true;
            }
        }

        /// <summary>
        /// Called when another object enters this object's trigger zone. If the triggering object is a "Player",
        /// the current object's BoxCollider2D is set back to a solid state (not a trigger).
        /// </summary>
        /// <param name="other">The triggering object's data.</param>
        private void OnTriggerEnter2D(Collider2D other)
        {
            // If the object entering the trigger zone is the "Player", set this object's collider back to solid mode.
            // This means it will physically block objects again.
            if (other.gameObject.CompareTag("Player"))
            {
                GetComponent<BoxCollider2D>().isTrigger = false;
            }
        }
    }
}
