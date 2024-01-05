using SystemScripts;
using UnityEngine;

namespace EnemyScripts
{
    /// <summary>
    /// Manages the behavior when the enemy's head is hit by the player.
    /// This script should be attached to the head part of the enemy GameObject.
    /// </summary>
    public class EnemyHead : MonoBehaviour
    {
        /// <summary>
        /// Reference to the enemy controller script for the enemy.
        /// </summary>
        private EnemyController _enemyController;

        /// <summary>
        /// Reference to the enemy GameObject.
        /// </summary>
        public GameObject enemy;

        /// <summary>
        /// Audio source for playing sound effects.
        /// </summary>
        private AudioSource _enemyAudio;

        /// <summary>
        /// Sound clip to play when the player hits the enemy's head.
        /// </summary>
        public AudioClip hitByPlayerSound;

        /// <summary>
        /// Initialize the audio source and the enemy controller reference.
        /// </summary>
        private void Awake()
        {
            _enemyAudio = GetComponent<AudioSource>();
            _enemyController = enemy.GetComponent<EnemyController>();
        }

        /// <summary>
        /// Handle collision events with other objects.
        /// Specifically, this method handles the interactions when the player hits the enemy's head.
        /// </summary>
        /// <param name="other">The collision data.</param>
        private void OnCollisionEnter2D(Collision2D other)
        {
            // Check if the collision is with the Player or BigPlayer.
            if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("BigPlayer"))
            {
                GameStatusController.IsEnemyDieOrCoinEat = true;

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
