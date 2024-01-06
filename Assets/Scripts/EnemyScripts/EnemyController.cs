using System.Collections;
using System.Collections.Generic;
// using SystemScripts;
using UnityEngine;
using AdditionalScripts;

namespace EnemyScripts
{
    public class EnemyController : MonoBehaviour
    {
        // Enemy movement speed.
        public int speed = 2;

        // Force to apply when the enemy pushes an object.
        public float pushForce = 500;

        // Indicates if the enemy has been touched by the player.
        public bool isTouchByPlayer;

        // Reference to the enemy's animator.
        private Animator _enemyAnim;

        // List of colliders to disable when the enemy dies.
        public List<Collider2D> deadDisableCollider;

        // Single collider to enable when the enemy dies.
        public Collider2D deadEnableCollider;

        // Animator's parameter hash to optimize performance.
        private static readonly int DieB = Animator.StringToHash("Die_b");

        private void Awake()
        {
            // Initialize the animator component.
            _enemyAnim = GetComponent<Animator>();
        }

        private void Update()
        {
            Move();
        }

        // Moves the enemy.
        private void Move()
        {
            transform.Translate(speed * Time.deltaTime * Vector3.left);
        }

        // Handles the death of the enemy.
        public void Die()
        {
            isTouchByPlayer = true;
            ToolController.Score += 200;

            // Disable all colliders in the deadDisableCollider list.
            for (var i = 0; i < deadDisableCollider.Count; i++)
            {
                deadDisableCollider[i].enabled = false;
            }

            // Enable the collider if it's assigned.
            if (deadEnableCollider != null)
            {
                deadEnableCollider.enabled = true;
            }

            // Change animation to the "death" animation.
            _enemyAnim.SetBool(DieB, true);
            
            // If the enemy is a Goomba, start the destroy coroutine.
            if (CompareTag("Goomba"))
            {
                StartCoroutine(Destroy());
            }
        }

        // Handles collisions with other objects.
        private void OnCollisionEnter2D(Collision2D other)
        {
            // Checks and handles collisions for KoopaShell.
            if (CompareTag("KoopaShell"))
            {
                // Reverse direction for specific collision cases.
                if (!other.gameObject.CompareTag("Player") && !other.gameObject.CompareTag("Ground") &&
                    !other.gameObject.CompareTag("Brick") && !other.gameObject.CompareTag("ScreenBorder") &&
                    !other.gameObject.CompareTag("Goomba") && !other.gameObject.CompareTag("Koopa"))
                {
                    transform.Rotate(0, 180, 0);
                }
            }
            else
            {
                // Reverse direction for other collision cases.
                if (!other.gameObject.CompareTag("Player") && !other.gameObject.CompareTag("Ground") &&
                    !other.gameObject.CompareTag("Brick") && !other.gameObject.CompareTag("ScreenBorder"))
                {
                    transform.Rotate(0, 180, 0);
                }
            }

            // Handles the enemy's interaction with KoopaShell or Fireball.
            if (other.gameObject.CompareTag("KoopaShell") || other.gameObject.CompareTag("Fireball"))
            {
                ToolController.Score += 200;
                ToolController.IsEnemyDieOrCoinEat = true;
                Destroy(gameObject);
            }
        }

        // Coroutine to destroy the enemy after a short delay.
        IEnumerator Destroy()
        {
            yield return new WaitForSeconds(0.3f);
            Destroy(gameObject);
        }
    }
}
