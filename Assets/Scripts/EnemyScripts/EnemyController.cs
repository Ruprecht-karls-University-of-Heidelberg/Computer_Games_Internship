using System.Collections;
using System.Collections.Generic;
//using SystemScripts;
using UnityEngine;
using AdditionalScripts;

namespace EnemyScripts
{
    public class EnemyController : MonoBehaviour
    {
        public int speed = 2;
        public float pushForce = 500;
        public bool isTouchByPlayer;
        public List<Collider2D> deadDisableColliders;
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

        private void Move()
        {
            transform.Translate(speed * Time.deltaTime * Vector3.left);
        }

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

        private void ReverseDirectionForKoopaShell(Collision2D collision)
        {
            if (!collision.gameObject.CompareTag("Player") && !collision.gameObject.CompareTag("Ground") &&
                !collision.gameObject.CompareTag("Brick") && !collision.gameObject.CompareTag("ScreenBorder") &&
                !collision.gameObject.CompareTag("Goomba") && !collision.gameObject.CompareTag("Koopa"))
            {
                transform.Rotate(0, 180, 0);
            }
        }

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

