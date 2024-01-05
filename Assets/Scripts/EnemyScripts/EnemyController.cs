using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyScripts
{
    /// <summary>
    /// Controls the behavior and interactions of an enemy character.
    /// </summary>
    public class EnemyController : MonoBehaviour
    {
        /// <summary>
        /// Movement speed of the enemy.
        /// </summary>
        public int speed = 2;

        /// <summary>
        /// Force to apply when the enemy pushes an object.
        /// </summary>
        public float pushForce = 500;

        /// <summary>
        /// Indicates if the enemy has been touched by the player.
        /// </summary>
        public bool isTouchByPlayer;

        /// <summary>
        /// Reference to the enemy's animator.
        /// </summary>
        private Animator _enemyAnim;

        /// <summary>
        /// List of colliders to disable when the enemy dies.
        /// </summary>
        public List<Collider2D> deadDisableCollider;

        /// <summary>
        /// Single collider to enable when the enemy dies.
        /// </summary>
        public Collider2D deadEnableCollider;

        /// <summary>
        /// Animator's parameter hash to optimize performance.
        /// </summary>
        private static readonly int DieB = Animator.StringToHash("Die_b");

        /// <summary>
        /// Initialization of the enemy controller component.
        /// </summary>
        private void Awake()
        {
            _enemyAnim = GetComponent<Animator>();
        }

        /// <summary>
        /// Update is called once per frame to manage enemy movement.
        /// </summary>
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
            GameStatusController.Score += 200;

            foreach (var collider in deadDisableCollider)
            {
                collider.enabled = false;
            }

            if (deadEnableCollider != null)
            {
                deadEnableCollider.enabled = true;
            }

            _enemyAnim.SetBool(DieB, true);

            if (CompareTag("Goomba"))
            {
                StartCoroutine(Destroy());
            }
        }

        /// <summary>
        /// Handles collisions with other objects.
        /// </summary>
        /// <param name="other">The collision data.</param>
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (CompareTag("KoopaShell"))
            {
                HandleKoopaShellCollision(other);
            }
            else
            {
                HandleGeneralCollision(other);
            }
        }

        /// <summary>
        /// Coroutine to destroy the enemy after a delay.
        /// </summary>
        /// <returns>IEnumerator for coroutine.</returns>
        IEnumerator Destroy()
        {
            yield return new WaitForSeconds(0.3f);
            Destroy(gameObject);
        }

        /// <summary>
        /// Handles collision behavior specific to KoopaShell.
        /// </summary>
        /// <param name="other">The collision data.</param>
        private void HandleKoopaShellCollision(Collision2D other)
        {
            if (!other.gameObject.CompareTag("Player") && !other.gameObject.CompareTag("Ground") &&
                !other.gameObject.CompareTag("Brick") && !other.gameObject.CompareTag("ScreenBorder") &&
                !other.gameObject.CompareTag("Goomba") && !other.gameObject.CompareTag("Koopa"))
            {
                transform.Rotate(0, 180, 0);
            }
        }

        /// <summary>
        /// Handles general collision behavior for enemies.
        /// </summary>
        /// <param name="other">The collision data.</param>
        private void HandleGeneralCollision(Collision2D other)
        {
            if (!other.gameObject.CompareTag("Player") && !other.gameObject.CompareTag("Ground") &&
                !other.gameObject.CompareTag("Brick") && !other.gameObject.CompareTag("ScreenBorder"))
            {
                transform.Rotate(0, 180, 0);
            }

            if (other.gameObject.CompareTag("KoopaShell") || other.gameObject.CompareTag("Fireball"))
            {
                GameStatusController.Score += 200;
                GameStatusController.IsEnemyDieOrCoinEat = true;
                Destroy(gameObject);
            }
        }
        /// <summary>
        /// Coroutine to destroy the enemy after a short delay.
        /// </summary>
        /// <returns>IEnumerator for coroutine.</returns>
        IEnumerator Destroy()
        {
            // Wait for 0.3 seconds.
            yield return new WaitForSeconds(0.3f);

            // Destroy the game object this script is attached to.
            Destroy(gameObject);
        }
    }
}
