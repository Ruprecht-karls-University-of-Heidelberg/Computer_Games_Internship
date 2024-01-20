using System.Collections;
using UnityEngine;
using AdditionalScripts;

/// <summary>
/// Namespace for player-related scripts.
/// </summary>
namespace PlayerScripts
{
    /// <summary>
    /// Controls the behavior of fireballs launched by the player.
    /// </summary>
    public class FireBallController : MonoBehaviour
    {
        /// <summary>
        /// The speed at which the fireball moves.
        /// </summary>
        public float speed;

        private Rigidbody2D _fireBallRb;
        private Animator _fireBallAnim;

        /// <summary>
        /// Hash for the destroy animation trigger.
        /// </summary>
        private static readonly int DestroyT = Animator.StringToHash("Destroy_t");

        /// <summary>
        /// Initializes components and sets the initial velocity of the fireball.
        /// </summary>
        void Start()
        {
            InitializeComponents();
            SetInitialVelocity();
        }

        /// <summary>
        /// Initializes the animator and rigidbody components.
        /// </summary>
        public void InitializeComponents()
        {
            _fireBallAnim = GetComponent<Animator>();
            _fireBallRb = GetComponent<Rigidbody2D>();
        }

        /// <summary>
        /// Sets the initial velocity of the fireball.
        /// </summary>
        public void SetInitialVelocity()
        {
            _fireBallRb.velocity = transform.right * speed;
        }

        /// <summary>
        /// Handles collision events of the fireball.
        /// </summary>
        /// <param name="other">The collision data associated with this collision.</param>
        public void OnCollisionEnter2D(Collision2D other)
        {
            HandleCollisionWithGroundOrStone(other);
            HandleCollisionWithPiranha(other);
        }

        /// <summary>
        /// Handles collisions with ground or stone, making the fireball bounce upward.
        /// </summary>
        /// <param name="other">The collision data.</param>
        private void HandleCollisionWithGroundOrStone(Collision2D other)
        {
            if (other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("Stone"))
            {
                BounceUpward();
                CheckAndDestroyIfStopped();
            }
            else
            {
                TriggerDestroyAnimation();
            }
        }

        /// <summary>
        /// Applies an upward force to the fireball, causing it to bounce.
        /// </summary>
        private void BounceUpward()
        {
            _fireBallRb.AddForce(new Vector2(0, 180));
        }

        /// <summary>
        /// Checks if the fireball has stopped moving and triggers its destruction if true.
        /// </summary>
        private void CheckAndDestroyIfStopped()
        {
            if (Mathf.RoundToInt(_fireBallRb.velocity.x) == 0)
            {
                TriggerDestroyAnimation();
            }
        }

        /// <summary>
        /// Handles collisions with Piranha, causing the Piranha to be destroyed.
        /// </summary>
        /// <param name="other">The collision data.</param>
        private void HandleCollisionWithPiranha(Collision2D other)
        {
            if (other.gameObject.CompareTag("Piranha"))
            {
                ToolController.IsEnemyDieOrCoinEat = true;
                Destroy(other.gameObject);
            }
        }

        /// <summary>
        /// Triggers the destroy animation of the fireball.
        /// </summary>
        public void TriggerDestroyAnimation()
        {
            _fireBallAnim.SetTrigger(DestroyT);
            StartCoroutine(DestroyFireball());
        }

        /// <summary>
        /// Coroutine to destroy the fireball after a short delay.
        /// </summary>
        /// <returns>IEnumerator for coroutine.</returns>
        public IEnumerator DestroyFireball()
        {
            yield return new WaitForSeconds(0.02f);
            Destroy(gameObject);
        }
    }
}
