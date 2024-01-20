using UnityEngine;
using AdditionalScripts;

namespace EnemyScripts
{
    /// <summary>
    /// This script manages the behavior when the enemy's head is hit by the player.
    /// </summary>
    public class EnemyHead : MonoBehaviour
    {
        // Reference to the enemy controller script for the enemy.
        private EnemyController _enemyController;

        // Reference to the enemy GameObject.
        public GameObject enemy;

        // Audio source for playing the sound effects.
        private AudioSource _enemyAudio;

        // Sound clip to play when the player hits the enemy's head.
        public AudioClip hitByPlayerSound;

        private void Awake()
        {
            // Initialize the audio source and the enemy controller reference.
            _enemyAudio = GetComponent<AudioSource>();
            _enemyController = enemy.GetComponent<EnemyController>();
        }

        /// <summary>
        /// Handle collision events with other objects.
        /// </summary>
        /// <param name="other">The collision data.</param>
        private void OnCollisionEnter2D(Collision2D other)
        {
            // If the collision is with the Player or BigPlayer, manage the enemy's death and play the hit sound.
            if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("BigPlayer"))
            {
                ToolController.IsEnemyDieOrCoinEat = true;
                
                // Play the sound effect for hitting the enemy.
                _enemyAudio.PlayOneShot(hitByPlayerSound);
                
                // Apply an upward force to the object that collided (the player).
                other.rigidbody.AddForce(new Vector2(0f, _enemyController.pushForce));
                
                // Stop the enemy's movement.
                _enemyController.speed = 0;
                
                // Execute the die behavior for the enemy.
                _enemyController.Die();
            }
        }
    }
}
