using SystemScripts;
using PlayerScripts;
using UnityEngine;

namespace EnemyScripts
{
    /// <summary>
    /// Manages the behavior of the Koopa's shell when it's kicked by the player.
    /// This script should be attached to the Koopa shell GameObject.
    /// </summary>
    public class KoopaShell : MonoBehaviour
    {
        /// <summary>
        /// Reference to the Koopa GameObject.
        /// </summary>
        public GameObject koopa;

        /// <summary>
        /// Determines the movement direction of the Koopa shell.
        /// </summary>
        private bool _isMoveRight;

        /// <summary>
        /// Determines if the Koopa shell is currently moving.
        /// </summary>
        private bool _isMove;

        /// <summary>
        /// Flag to check if the player can be killed by the Koopa shell.
        /// </summary>
        private bool _isPlayerKillable;

        /// <summary>
        /// Movement speed of the Koopa shell.
        /// </summary>
        public float speed;

        /// <summary>
        /// Audio source for playing sound effects.
        /// </summary>
        private AudioSource _enemyAudio;

        /// <summary>
        /// Sound clip to play when the player hits the Koopa shell.
        /// </summary>
        public AudioClip hitPlayerSound;

        /// <summary>
        /// Sound clip to play when the Koopa shell is kicked.
        /// </summary>
        public AudioClip kickSound;

        /// <summary>
        /// Sound clip to play when a big player turns small after being hit by the Koopa shell.
        /// </summary>
        public AudioClip turnSmallPlayerSound;

        /// <summary>
        /// Initializes the audio source component.
        /// </summary>
        private void Awake()
        {
            _enemyAudio = GetComponent<AudioSource>();
        }

        /// <summary>
        /// Update is called once per frame to handle Koopa shell movement.
        /// </summary>
        private void Update()
        {
            if (_isMove)
            {
                Move();
            }
        }

        /// <summary>
        /// Handles collisions with other objects.
        /// </summary>
        /// <param name="other">The collision data.</param>
        private void OnCollisionEnter2D(Collision2D other)
        {
            // Play kick sound when colliding with objects other than the player.
            if (!other.gameObject.CompareTag("Player"))
            {
                _enemyAudio.PlayOneShot(kickSound);
            }

            // Logic for when the Koopa shell is not set to kill the player.
            if (!_isPlayerKillable)
            {
                // Check for collisions with the player or BigPlayer.
                if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("BigPlayer"))
                {
                    // Update Koopa's tag to KoopaShell.
                    koopa.tag = "KoopaShell";

                    // Determine the angle between the Koopa shell and the other object (the player).
                    Vector3 relative = transform.InverseTransformPoint(other.transform.position);
                    float angle = Mathf.Atan2(relative.x, relative.y) * Mathf.Rad2Deg;

                    // Set movement and direction based on the collision angle.
                    _isMove = true;
                    _isMoveRight = angle <= 0;
                    if (other.gameObject.CompareTag("BigPlayer"))
                    {
                        _isMoveRight = angle > 0;
                    }

                    // Now, the Koopa shell can potentially kill the player.
                    _isPlayerKillable = true;
                }
            }
            else
            {
                // Logic for when the Koopa shell can kill the player.
                PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
                if (other.gameObject.CompareTag("Player"))
                {
                    speed = 0;
                    if (!playerController.isInvulnerable)
                    {
                        _enemyAudio.PlayOneShot(hitPlayerSound);
                        GameStatusController.IsDead = true;
                    }
                    else
                    {
                        // Ignore collisions with the player's small collider when invulnerable.
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
        }

        /// <summary>
        /// Move the Koopa shell based on the set direction.
        /// </summary>
        private void Move()
        {
            if (_isMoveRight)
            {
                koopa.transform.Translate(speed * Time.deltaTime * Vector3.right);
            }
            else
            {
                koopa.transform.Translate(-speed * Time.deltaTime * Vector3.right);
            }
        }
    }
}
