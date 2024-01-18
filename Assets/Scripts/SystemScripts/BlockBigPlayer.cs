using UnityEngine;
using AdditionalScripts;

namespace SystemScripts
{
    public class BlockBigPlayer : MonoBehaviour
    {
        /// <summary>
        /// This method is called when another object collides with the attached object's collider.
        /// </summary>
        /// <param name="other">The object that this script's object collided with.</param>
        private void OnCollisionEnter2D(Collision2D other)
        {
            // If the colliding object isn't tagged as "BigPlayer", make this object's collider a trigger.
            // This means objects can move through it without physical interaction.
            if (!other.gameObject.CompareTag("BigPlayer"))
            {
                GetComponent<BoxCollider2D>().isTrigger = true;
            }
        }

        /// <summary>
        /// This method is called when another object enters the attached object's trigger collider.
        /// </summary>
        /// <param name="other">The object that entered this script's object's trigger collider.</param>
        private void OnTriggerEnter2D(Collider2D other)
        {
            // If the object that entered the trigger is tagged as "BigPlayer", make this object's collider solid again.
            // This means objects will now physically interact with it.
            if (other.gameObject.CompareTag("BigPlayer"))
            {
                GetComponent<BoxCollider2D>().isTrigger = false;
            }
        }
    }
}
