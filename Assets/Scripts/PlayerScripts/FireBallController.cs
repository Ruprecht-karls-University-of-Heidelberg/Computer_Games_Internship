using System.Collections;
using SystemScripts;
using UnityEngine;

namespace PlayerScripts
{
    /// <summary>
    /// Controls the behavior of the fireball in the game.
    /// This script should be attached to the fireball GameObject.
    /// </summary>
    public class FireBallController : MonoBehaviour
    {
        /// <summary>
        /// Defines the speed of the fireball.
        /// </summary>
        public float speed;

        /// <summary>
        /// Rigidbody2D component of the fireball for physics interactions.
        /// </summary>
        private Rigidbody2D _fireBallRb;

        /// <summary>
        /// Animator component of the fireball for animations.
        /// </summary>
        private Animator _fireBallAnim;

        /// <summary>
        /// Converts the string "Destroy_t" to a hash for better performance with the animator.
        /// </summary>
        private static readonly int DestroyT = Animator.StringToHash("Destroy_t");

        /// <summary>
        /// Start is called before the first frame update.
        /// Initializes the animator and rigidbody components and sets the initial velocity of the fireball.
        /// </summary>
        void Start()
        {
            _fireBallAnim = GetComponent<Animator>();
            _fireBallRb = GetComponent<Rigidbody2D>();
            _fireBallRb.velocity = transform.right * speed;
        }

        /// <summary>
        /// Called when the fireball collides with another object.
        /// Handles the interaction of the fireball with different objects in the game.
        /// </summary>
        /// <param name="other">The collision data.</param>
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("Stone"))
            {
                _fireBallRb.AddForce(new Vector2(0, 180));

                if (Mathf.RoundToInt(_fireBallRb.velocity.x) == 0)
                {
                    _fireBallAnim.SetTrigger(DestroyT);
                    StartCoroutine(Destroy());
                }
            }
            else
            {
                _fireBallAnim.SetTrigger(DestroyT);
                StartCoroutine(Destroy());
            }

            if (other.gameObject.CompareTag("Piranha"))
            {
                GameStatusController.IsEnemyDieOrCoinEat = true;
                Destroy(other.gameObject);
            }
        }

        /// <summary>
        /// Coroutine to destroy the fireball after a short delay.
        /// </summary>
        /// <returns>IEnumerator for coroutine execution.</returns>
        private IEnumerator Destroy()
        {
            yield return new WaitForSeconds(0.02f);
            Destroy(gameObject);
        }
    }
}
