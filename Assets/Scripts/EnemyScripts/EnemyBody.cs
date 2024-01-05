using System.Collections;
using UnityEngine;

namespace EnemyScripts
{
    /// <summary>
    /// Manages the behavior and interactions of an enemy entity.
    /// </summary>
    public class EnemyBody : MonoBehaviour
    {
        /// <summary>
        /// Reference to the enemy's main controller.
        /// </summary>
        private EnemyController _enemyController;

        /// <summary>
        /// Reference to the main enemy GameObject this body component belongs to.
        /// </summary>
        public GameObject enemy;

        /// <summary>
        /// AudioSource to play enemy-related sounds.
        /// </summary>
        private AudioSource _enemyAudio;

        /// <summary>
        /// Sound to play when the enemy hits the player.
        /// </summary>
        public AudioClip hitPlayerSound;

        /// <summary>
        /// Sound to play when a big player turns small after enemy interaction.
        /// </summary>
        public AudioClip turnSmallPlayerSound;

        /// <summary>
        /// Initialization of the enemy body component.
        /// </summary>
        private void Awake()
        {
            _enemyAudio = GetComponent<AudioSource>();
            
            if (enemy != null)
            {
                _enemyController = enemy.GetComponent<EnemyController>();
            }
        }

        /// <summary>
        /// Update is called once per frame to manage enemy body state.
        /// </summary>
        private void Update()
        {
            if (_enemyController != null && _enemyController.isTouchByPlayer)
            {
                GetComponent<BoxCollider2D>().offset = Vector2.zero;
                GetComponent<BoxCollider2D>().size = new Vector2(1, 0.01f);
            }
        }

        /// <summary>
        /// Handles collisions with other game objects.
        /// </summary>
        /// <param name="other">The collision data.</param>
        private void OnCollisionEnter2D(Collision2D other)
        {
            PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
            
            if (other.gameObject.CompareTag("Player"))
            {
                if (!playerController.isInvulnerable)
                {
                    _enemyAudio.PlayOneShot(hitPlayerSound);
                    GameStatusController.IsDead = true;
                    GameStatusController.Live -= 1;
                    playerController.GetComponent<Rigidbody2D>().isKinematic = true;
                    playerController.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                }
                else
                {
                    Physics2D.IgnoreCollision(GetComponent<Collider2D>(), playerController.smallPlayerCollider.GetComponent<Collider2D>());
                }
            }
            else if (other.gameObject.CompareTag("BigPlayer"))
            {
                _enemyAudio.PlayOneShot(turnSmallPlayerSound);
                GameStatusController.IsBigPlayer = false;
                GameStatusController.IsFirePlayer = false;
                GameStatusController.PlayerTag = "Player";
                playerController.gameObject.tag = GameStatusController.PlayerTag;
                playerController.ChangeAnim();
                playerController.isInvulnerable = true;
            }
        }

        /// <summary>
        /// Coroutine to apply a force to the player (for future implementation).
        /// </summary>
        /// <param name="playerGameObject">The player game object.</param>
        /// <returns>IEnumerator for coroutine.</returns>
        IEnumerator Die(GameObject playerGameObject)
        {
            yield return new WaitForSeconds(1);
            playerGameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 4000));
            playerGameObject.GetComponent<Rigidbody2D>().gravityScale = 25;
        }
    }
}
