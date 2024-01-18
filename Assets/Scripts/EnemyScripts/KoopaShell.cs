//using SystemScripts;
using PlayerScripts;
using UnityEngine;
using AdditionalScripts;

namespace EnemyScripts
{
    // This script manages the behavior of the Koopa's shell when it's kicked by the player.
    public class KoopaShell : MonoBehaviour
    {
        // Reference to the Koopa GameObject.
        public GameObject koopa;

        // Determine the movement direction of the Koopa shell.
        private bool _isMoveRight;

        // Determine if the Koopa shell is currently moving.
        private bool _isMove;

        // Flag to check if the player can be killed by the Koopa shell.
        private bool _isPlayerKillable;

        // Movement speed of the Koopa shell.
        public float speed;

        // Audio source for playing sound effects.
        private AudioSource _enemyAudio;

        // Sound clips.
        public AudioClip hitPlayerSound;
        public AudioClip kickSound;
        public AudioClip turnSmallPlayerSound;

        private void Awake()
        {
            // Initialize the audio source component.
            _enemyAudio = GetComponent<AudioSource>();
        }

        // Update is called once per frame.
        void Update()
        {
            // If the Koopa shell is set to move, move it.
            if (_isMove)
            {
                Move();
            }
        }

        // Handle collisions with other objects.
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
                    if (other.gameObject.CompareTag("Player"))
                    {
                        _isMoveRight = angle <= 0;
                    }
                    else if (other.gameObject.CompareTag("BigPlayer"))
                    {
                        _isMoveRight = angle > 0;
                    }

                    // Now, the Koopa shell can potentially kill the player.
                    _isPlayerKillable = true;
                }
            }
            // Logic for when the Koopa shell can kill the player.
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
                        // Ignore collisions with the player's small collider when invulnerable.
                        Physics2D.IgnoreCollision(GetComponent<Collider2D>(),
                            playerController.smallPlayerCollider.GetComponent<Collider2D>());
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

        // Move the Koopa shell based on the set direction.
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
