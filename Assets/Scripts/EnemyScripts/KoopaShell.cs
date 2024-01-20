using PlayerScripts;
using UnityEngine;
using AdditionalScripts;

/// <summary>
/// Namespace for enemy-related scripts.
/// </summary>
namespace EnemyScripts
{
    /// <summary>
    /// Manages the behavior of the Koopa's shell when it's kicked by the player.
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
        /// Sound effect played when the Koopa shell hits the player.
        /// </summary>
        public AudioClip hitPlayerSound;

        /// <summary>
        /// Sound effect played when the Koopa shell is kicked.
        /// </summary>
        public AudioClip kickSound;

        /// <summary>
        /// Sound effect played when the player turns small after being hit by the Koopa shell.
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
        /// Moves the Koopa shell if it's set to move.
        /// </summary>
        void Update()
        {
            if (_isMove)
            {
                Move();
            }
        }

        /// <summary>
        /// Handles collisions with other objects. Manages the behavior when the Koopa shell collides
        /// with players or other game objects.
        /// </summary>
        /// <param name="other">The collision data associated with this collision.</param>
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!other.gameObject.CompareTag("Player"))
            {
                _enemyAudio.PlayOneShot(kickSound);
            }

            if (!_isPlayerKillable)
            {
                if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("BigPlayer"))
                {
                    koopa.tag = "KoopaShell";
                    Vector3 relative = transform.InverseTransformPoint(other.transform.position);
                    float angle = Mathf.Atan2(relative.x, relative.y) * Mathf.Rad2Deg;

                    _isMove = true;
                    if (other.gameObject.CompareTag("Player"))
                    {
                        _isMoveRight = angle <= 0;
                    }
                    else if (other.gameObject.CompareTag("BigPlayer"))
                    {
                        _isMoveRight = angle > 0;
                    }

                    _isPlayerKillable = true;
                }
            }
            else
            {
                PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
                if (other.gameObject.CompareTag("Player"))
                {
                    speed = 0;
                    if (!playerController.isInvulnerable)
                    {
                        _enemyAudio.PlayOneShot(hitPlayerSound);
                        ToolController.IsDead = true;
                    }
                    else
                    {
                        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), playerController.smallPlayerCollider.GetComponent<Collider2D>());
                    }
                }
                else if (other.gameObject.CompareTag("BigPlayer"))
                {
                    _enemyAudio.PlayOneShot(turnSmallPlayerSound);
                    ToolController.IsBigPlayer = false;
                    ToolController.IsFirePlayer = false;
                    ToolController.PlayerTag = "Player";
                    playerController.gameObject.tag = ToolController.PlayerTag;
                    playerController.ChangeAnim();
                    playerController.isInvulnerable = true;
                }
            }
        }

        /// <summary>
        /// Moves the Koopa shell based on the set direction (_isMoveRight).
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
