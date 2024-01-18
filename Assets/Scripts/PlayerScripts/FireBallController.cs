using System.Collections;
// using SystemScripts;
using UnityEngine;
using AdditionalScripts;

// Namespace for player-related scripts.
namespace PlayerScripts

//after changing

{
    public class FireBallController : MonoBehaviour
    {
        public float speed;

        private Rigidbody2D _fireBallRb;
        private Animator _fireBallAnim;

        // Hash for the destroy animation trigger
        private static readonly int DestroyT = Animator.StringToHash("Destroy_t");

        void Start()
        {
            InitializeComponents();
            SetInitialVelocity();
        }

        public void InitializeComponents()
        {
            _fireBallAnim = GetComponent<Animator>();
            _fireBallRb = GetComponent<Rigidbody2D>();
        }

        public void SetInitialVelocity()
        {
            // Set the velocity of the fireball
            _fireBallRb.velocity = transform.right * speed;
        }

        public void OnCollisionEnter2D(Collision2D other)
        {
            // Handle different collision scenarios
            HandleCollisionWithGroundOrStone(other);
            HandleCollisionWithPiranha(other);
        }

        private void HandleCollisionWithGroundOrStone(Collision2D other)
        {
            // Check for collision with ground or stone
            if (other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("Stone"))
            {
                BounceUpward(); // Bounce the fireball upward
                // Check if the fireball has stopped and destroy if true
                CheckAndDestroyIfStopped();
            }
            else
            {
                // Trigger destroy animation for other collisions
                TriggerDestroyAnimation();
            }
        }

        private void BounceUpward()
        {
            // Add an upward force to the fireball
            _fireBallRb.AddForce(new Vector2(0, 180));
        }

        private void CheckAndDestroyIfStopped()
        {
            // Check if the fireball has stopped moving
            if (Mathf.RoundToInt(_fireBallRb.velocity.x) == 0)
            {
                // Trigger destroy animation if the fireball has stopped
                TriggerDestroyAnimation();
            }
        }

        private void HandleCollisionWithPiranha(Collision2D other)
        {
            // Check for collision with Piranha
            if (other.gameObject.CompareTag("Piranha"))
            {
                // Update state to indicate enemy death or coin eaten
                ToolController.IsEnemyDieOrCoinEat = true;
                // Destroy the enemy object
                Destroy(other.gameObject);
            }
        }

        public void TriggerDestroyAnimation()
        {
            // Trigger the destroy animation
            _fireBallAnim.SetTrigger(DestroyT);
            // Start coroutine to destroy the fireball
            StartCoroutine(DestroyFireball());
        }

        public IEnumerator DestroyFireball()
        {
            // Coroutine to destroy the fireball after a delay
            yield return new WaitForSeconds(0.02f);
            // Destroy the fireball object
            Destroy(gameObject);
        }
    }
}