using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdditionalScripts;

namespace EnemyScripts
{
    /// <summary>
    /// Controls the behavior of an enemy in the game.
    /// </summary>
    public class EnemyController : MonoBehaviour
    /// <summary>
    /// Controls the behavior of an enemy in the game.
    /// </summary>
    public class EnemyController : MonoBehaviour
    {
        /// <summary>
        /// The speed at which the enemy moves.
        /// </summary>
        public int speed = 2;

        /// <summary>
        /// The force applied when the enemy is pushed.
        /// </summary>
        public float pushForce = 500;

        /// <summary>
        /// Indicates whether the enemy is touched by the player.
        /// </summary>
        public bool isTouchByPlayer;

        /// <summary>
        /// The colliders that should be disabled when the enemy dies.
        /// </summary>
        public List<Collider2D> deadDisableColliders;

        /// <summary>
        /// The collider that should be enabled when the enemy dies.
        /// </summary>
        public Collider2D deadEnableCollider;

        private Animator _enemyAnimator;
        private static readonly int DieHash = Animator.StringToHash("Die_b");

        private void Awake()
        {
            _enemyAnimator = GetComponent<Animator>();
        }

        private void Update()
        {
            Move();
        }

        /// <summary>
        /// Moves the enemy.
        /// </summary>
        private void Move()
        {
            transform.Translate(speed * Time.deltaTime * Vector3.left);
        }

        /// <summary>
        /// Handles the death of the enemy.
        /// </summary>
        public void Die()
        {
            isTouchByPlayer = true;
            ToolController.Score += 200;
            foreach (var collider in deadDisableColliders)
            {
                collider.enabled = false;
            }

            if (deadEnableCollider != null)
            {
                deadEnableCollider.enabled = true;
            }

            _enemyAnimator.SetBool(DieHash, true);

            if (CompareTag("Goomba"))
            {
                StartCoroutine(DestroyEnemy());
            }
        }
        /// <summary>
        /// Handles the collision of the enemy with other objects.  
        /// </summary>
        /// <param name="other"></param>
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (CompareTag("KoopaShell"))
            {
                ReverseDirectionForKoopaShell(other);
            }
            else
            {
                ReverseDirectionForOtherCollisions(other);
            }

            if (other.gameObject.CompareTag("KoopaShell") || other.gameObject.CompareTag("Fireball"))
            {
                ToolController.Score += 200;
                ToolController.IsEnemyDieOrCoinEat = true;
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Handles the reverse direction of the enemy when it collides with other objects.
        /// </summary>
        /// <param name="collision"></param>

        private void ReverseDirectionForKoopaShell(Collision2D collision)
        {
            if (!collision.gameObject.CompareTag("Player") && !collision.gameObject.CompareTag("Ground") &&
                !collision.gameObject.CompareTag("Brick") && !collision.gameObject.CompareTag("ScreenBorder") &&
                !collision.gameObject.CompareTag("Goomba") && !collision.gameObject.CompareTag("Koopa"))
            {
                transform.Rotate(0, 180, 0);
            }
        }

        /// <summary>
        /// Handles the reverse direction of the enemy when it collides with other objects.
        /// </summary>
        /// <param name="collision"></param>
        private void ReverseDirectionForOtherCollisions(Collision2D collision)
        {
            if (!collision.gameObject.CompareTag("Player") && !collision.gameObject.CompareTag("Ground") &&
                !collision.gameObject.CompareTag("Brick") && !collision.gameObject.CompareTag("ScreenBorder"))
            {
                transform.Rotate(0, 180, 0);
            }
        }
        
        IEnumerator DestroyEnemy()
        {
            yield return new WaitForSeconds(0.3f);
            Destroy(gameObject);
        }
    }
}

