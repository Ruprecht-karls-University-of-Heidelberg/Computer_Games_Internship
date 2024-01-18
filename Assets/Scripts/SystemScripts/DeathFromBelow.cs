using UnityEngine;
using AdditionalScripts;

namespace SystemScripts
{
    /// <summary>
    /// Represents a behavior where non-player objects that collide with this object will be destroyed.
    /// </summary>
    public class DeathFromBelow : MonoBehaviour
    {
        /// <summary>
        /// Called when a 2D collision occurs. If the colliding object isn't any variant of the player,
        /// it gets destroyed.
        /// </summary>
        /// <param name="other">The colliding object's data.</param>
        private void OnCollisionEnter2D(Collision2D other)
        {
            // Check if the colliding object is NOT any of the player variants.
            // If true, destroy the colliding object.
            if (!other.gameObject.CompareTag("Player") && 
                !other.gameObject.CompareTag("BigPlayer") &&
                !other.gameObject.CompareTag("UltimateBigPlayer") && 
                !other.gameObject.CompareTag("UltimatePlayer"))
            {
                Destroy(other.gameObject);
            }
        }
    }
}
