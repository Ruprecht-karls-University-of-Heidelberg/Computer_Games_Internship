using System;
using System.Collections;
using SystemScripts;
using UnityEngine;

/// <summary>
/// Controls the behavior and movement of power-ups in the game.
/// </summary>
public class PowerUpsController : MonoBehaviour
{
    public int speedRight;           // Speed at which the power-up moves to the right.
    public int speedUp;              // Speed at which the power-up moves upwards.
    public bool isMoving;            // Indicates if the power-up is currently in motion.
    public bool isTouchByPlayer;     // Determines if the power-up has been touched by the player.
    private bool _isEatable;         // Determines if the power-up is currently eatable by the player.
    private float _firstYPos;        // Stores the original Y position of the power-up.

    private AudioSource _powerAudio; // AudioSource component for the power-up's audio effects.
    public AudioClip appearSound;    // Sound effect that is played when the power-up appears.

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// Initializes the power-up's properties.
    /// </summary>
    void Awake()
    {
        _powerAudio = GetComponent<AudioSource>();
        Physics2D.IgnoreLayerCollision(9, 10, true);
        _firstYPos = transform.position.y;
    }

    /// <summary>
    /// Update is called once per frame to handle power-up movement.
    /// </summary>
    void Update()
    {
        if (isTouchByPlayer && !CompareTag("Coin"))
        {
            if (transform.position.y < _firstYPos + 1)
            {
                transform.Translate(speedUp * Time.deltaTime * Vector2.up);
            }
            else if (CompareTag("BigMushroom") || CompareTag("1UpMushroom"))
            {
                isMoving = true;
                GetComponent<Rigidbody2D>().isKinematic = false;
            }
        }

        if (isMoving && (CompareTag("BigMushroom") || CompareTag("1UpMushroom")))
        {
            isTouchByPlayer = false;
            transform.Translate(speedRight * Time.deltaTime * Vector2.right);
        }
    }

    /// <summary>
    /// Invoked when another collider makes contact with the power-up's collider.
    /// Handles interactions with the player and other objects.
    /// </summary>
    /// <param name="other">The collision data.</param>
    private void OnCollisionEnter2D(Collision2D other)
    {
        InteractionWithPlayer(other.gameObject);

        // Reverse direction upon collision with specific objects.
        if (other.gameObject.CompareTag("Stone") || other.gameObject.CompareTag("Pipe") ||
            other.gameObject.CompareTag("Untagged"))
        {
            speedRight = -speedRight;
        }
    }


    /// <summary>
    /// This method is invoked when another collider overlaps the power-up's trigger collider.
    /// Handles interactions such as coin collection.
    /// </summary>
    /// <param name="other">The collider data.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Handle coin collection by the player.
        if (CompareTag("Coin") && (other.CompareTag("Player") || other.CompareTag("BigPlayer") ||
                                   other.CompareTag("UltimatePlayer") || other.CompareTag("UltimateBigPlayer")))
        {
            GameStatusController.CollectedCoin += 1;
            GameStatusController.Score += 200;
            GameStatusController.IsEnemyDieOrCoinEat = true;
            Destroy(gameObject);
        }

        // Check for other interactions with the player.
        InteractionWithPlayer(other.gameObject);
    }

    /// <summary>
    /// Coroutine to set the power-up as eatable after a certain delay.
    /// </summary>
    private IEnumerator SetBoolEatable()
    {
        yield return new WaitForSeconds(1);
        _isEatable = true;
    }

    /// <summary>
    /// Handles the interaction between the power-up and player characters.
    /// </summary>
    /// <param name="other">The GameObject involved in the interaction.</param>
    void InteractionWithPlayer(GameObject other)
    {
        // Activate power-up when touched by the player.
        if ((other.CompareTag("Player") || other.CompareTag("UltimatePlayer")) && !CompareTag("Coin"))
        {
            _powerAudio.PlayOneShot(appearSound);
            isTouchByPlayer = true;
            StartCoroutine(SetBoolEatable());
        }
        else if ((other.CompareTag("BigPlayer") || other.CompareTag("UltimateBigPlayer")) && !CompareTag("Coin"))
        {
            _powerAudio.PlayOneShot(appearSound);
            isTouchByPlayer = true;
            StartCoroutine(SetBoolEatable());
        }

        // Handle the player consuming the power-up.
        if ((other.CompareTag("Player") || other.CompareTag("UltimatePlayer")) && _isEatable)
        {
            GameStatusController.Score += 1000;
            GameStatusController.IsPowerUpEat = true;
            _isEatable = false;
            Destroy(gameObject);
        }
        else if ((other.CompareTag("BigPlayer") || other.CompareTag("UltimateBigPlayer")) && _isEatable)
        {
            GameStatusController.Score += 1000;
            GameStatusController.IsPowerUpEat = true;
            _isEatable = false;
            Destroy(gameObject);
        }
    }
}