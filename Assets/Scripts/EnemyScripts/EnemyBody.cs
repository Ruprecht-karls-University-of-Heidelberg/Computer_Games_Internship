using System.Collections;
// using SystemScripts;
using PlayerScripts;
using UnityEngine;
using AdditionalScripts;

namespace EnemyScripts
{
    public class EnemyBody : MonoBehaviour
    {
        // Reference to the enemy's main controller.
        private EnemyController _enemyController;

        // Reference to the main enemy GameObject this body component belongs to.
        public GameObject enemy;

        // AudioSource to play enemy-related sounds.
        private AudioSource _enemyAudio;

        // Sounds to play on certain interactions.
        public AudioClip hitPlayerSound;         // Sound when the enemy hits the player.
        public AudioClip turnSmallPlayerSound;   // Sound when a big player turns small after enemy interaction.

        private void Awake()
        {
            // Initialize the audio source component.
            _enemyAudio = GetComponent<AudioSource>();
            
            // Get the enemy controller if an enemy reference exists.
            if (enemy != null)
            {
                _enemyController = enemy.GetComponent<EnemyController>();
            }
        }

        private void Update()
        {
            // Adjust the collider if the enemy has been touched by the player.
            if (_enemyController != null && _enemyController.isTouchByPlayer)
            {
                GetComponent<BoxCollider2D>().offset = Vector2.zero;
                GetComponent<BoxCollider2D>().size = new Vector2(1, 0.01f);
            }
        }

        public void OnCollisionEnter2D(Collision2D other)
        {
            // Get PlayerController from the colliding object (if exists).
            PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
            
            if (other.gameObject.CompareTag("Player"))
            {
                // If the player is not invulnerable, the enemy hits the player.
                if (!playerController.isInvulnerable)
                {
                    _enemyAudio.PlayOneShot(hitPlayerSound);
                    ToolController.IsDead = true;
                    ToolController.Live -= 1;
                    // Freeze the player by making it kinematic and setting velocity to zero.
                    playerController.GetComponent<Rigidbody2D>().isKinematic = true;
                    playerController.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                }
                else
                {
                    // If player is invulnerable, ignore collisions between this enemy and the player.
                    Physics2D.IgnoreCollision(GetComponent<Collider2D>(),
                        playerController.smallPlayerCollider.GetComponent<Collider2D>());
                }
            }
            else if (other.gameObject.CompareTag("BigPlayer"))
            {
                // If a big player collides with the enemy, the player turns small.
                _enemyAudio.PlayOneShot(turnSmallPlayerSound);
                ToolController.IsBigPlayer = false;
                ToolController.IsFirePlayer = false;
                ToolController.PlayerTag = "Player";
                playerController.gameObject.tag = ToolController.PlayerTag;
                // Change the player's animation to reflect its smaller size.
                playerController.ChangeAnim();
                playerController.isInvulnerable = true;
                // The die method can be used for future implementation.
                // StartCoroutine(Die(other.gameObject));
            }
        }

        // Coroutine to apply a force to the player (possibly to simulate a knock-back effect).
        IEnumerator Die(GameObject playerGameObject)
        {
            yield return new WaitForSeconds(1);
            playerGameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 4000));
            playerGameObject.GetComponent<Rigidbody2D>().gravityScale = 25;
        }
    }
}
