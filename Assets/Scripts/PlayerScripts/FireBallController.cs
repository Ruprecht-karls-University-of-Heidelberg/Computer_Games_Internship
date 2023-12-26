using System.Collections;
using SystemScripts;
using UnityEngine;

// Namespace for player-related scripts.
namespace PlayerScripts
{
    // Controls the behavior of the fireball in the game.
    public class FireBallController : MonoBehaviour
    {
        public float speed; // Defines the speed of the fireball.

        // Private references to components.
        private Rigidbody2D _fireBallRb; // Rigidbody2D component of the fireball for physics interactions.
        private Animator _fireBallAnim; // Animator component of the fireball for animations.
        private static readonly int DestroyT = Animator.StringToHash("Destroy_t"); // Converts the string "Destroy_t" to a hash for better performance with the animator.

        // Start is called before the first frame update.
        void Start()
        {
            // Initialize the animator and rigidbody components.
            _fireBallAnim = GetComponent<Animator>();
            _fireBallRb = GetComponent<Rigidbody2D>();

            // Set the initial velocity of the fireball based on its direction and speed.
            _fireBallRb.velocity = transform.right * speed;
        }

        // This method is called when the fireball collides with another object.
        private void OnCollisionEnter2D(Collision2D other)
        {
            // If the fireball hits the ground or stone...
            if (other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("Stone"))
            {
                // ...apply an upward force to the fireball.
                _fireBallRb.AddForce(new Vector2(0, 180));

                // If the fireball's horizontal velocity is near zero...
                if (Mathf.RoundToInt(_fireBallRb.velocity.x) == 0)
                {
                    // ...trigger the "Destroy_t" animation and initiate the destroy coroutine.
                    _fireBallAnim.SetTrigger(DestroyT);
                    StartCoroutine(Destroy());
                }
            }
            else // If the fireball collides with something other than ground or stone...
            {
                // ...trigger the "Destroy_t" animation and initiate the destroy coroutine.
                _fireBallAnim.SetTrigger(DestroyT);
                StartCoroutine(Destroy());
            }

            // If the fireball hits a Piranha...
            if (other.gameObject.CompareTag("Piranha"))
            {
                // ...set the enemy death or coin eaten status to true and destroy the Piranha.
                GameStatusController.IsEnemyDieOrCoinEat = true;
                Destroy(other.gameObject);
            }
        }

        // Coroutine to destroy the fireball after a short delay.
        private IEnumerator Destroy()
        {
            yield return new WaitForSeconds(0.02f); // Wait for 0.02 seconds.
            Destroy(gameObject); // Destroy the fireball.
        }
    }
}
