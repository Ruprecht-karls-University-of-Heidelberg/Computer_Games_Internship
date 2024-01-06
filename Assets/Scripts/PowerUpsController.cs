using System;
using System.Collections;
// using SystemScripts;
using UnityEngine;
using AdditionalScripts;

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

    // Awake is called when the script instance is being loaded.
    void Awake()
    {
        _powerAudio = GetComponent<AudioSource>();
        // Prevent collision between certain layers.
        Physics2D.IgnoreLayerCollision(9, 10, true);
        _firstYPos = transform.position.y; // Store the initial Y position of the power-up.
    }

    // Update is called once per frame.
    void Update()
    {
        // Handle the movement of the power-up when it's touched by the player and isn't a coin.
        if (isTouchByPlayer && !CompareTag("Coin"))
        {
            // Move the power-up upwards.
            if (transform.position.y < _firstYPos + 1)
            {
                transform.Translate(speedUp * Time.deltaTime * Vector2.up);
            }
            // Enable motion for certain power-up types after rising.
            else if (CompareTag("BigMushroom") || CompareTag("1UpMushroom"))
            {
                isMoving = true;
                GetComponent<Rigidbody2D>().isKinematic = false;
            }
        }

        // Control horizontal movement for certain power-up types.
        if (isMoving && (CompareTag("BigMushroom") || CompareTag("1UpMushroom")))
        {
            isTouchByPlayer = false;
            transform.Translate(speedRight * Time.deltaTime * Vector2.right);
        }
    }

    // This method is invoked when another collider enters the power-up's collider.
    private void OnCollisionEnter2D(Collision2D other)
    {
        InteractionWithPlayer(other.gameObject);

        // Reverse horizontal direction when hitting certain objects.
        if (other.gameObject.CompareTag("Stone") || other.gameObject.CompareTag("Pipe") ||
            other.gameObject.CompareTag("Untagged"))
        {
            speedRight = -speedRight;
        }
    }

    // This method is invoked when another collider overlaps the power-up's trigger collider.
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Handle coin collection.
        if (CompareTag("Coin") && (other.CompareTag("Player") || other.CompareTag("BigPlayer") ||
                                   other.CompareTag("UltimatePlayer") || other.CompareTag("UltimateBigPlayer")))
        {
            ToolController.CollectedCoin += 1;
            ToolController.Score += 200;
            ToolController.IsEnemyDieOrCoinEat = true;
            Destroy(gameObject);
        }

        InteractionWithPlayer(other.gameObject);
    }

    // Set the power-up as eatable after a certain delay.
    private IEnumerator SetBoolEatable()
    {
        yield return new WaitForSeconds(1);
        _isEatable = true;
    }

    // Handle the interaction between the power-up and player characters.
    void InteractionWithPlayer(GameObject other)
    {
        if ((other.CompareTag("Player") || other.CompareTag("UltimatePlayer")) && !CompareTag("Coin"))
        {
            _powerAudio.PlayOneShot(appearSound);
            isTouchByPlayer = true;
            StartCoroutine(SetBoolEatable());
        }
        else if (other.CompareTag("BigPlayer") || other.CompareTag("UltimateBigPlayer") && !CompareTag("Coin"))
        {
            _powerAudio.PlayOneShot(appearSound);
            isTouchByPlayer = true;
            StartCoroutine(SetBoolEatable());
        }

        // Handle the player eating the power-up.
        if ((other.CompareTag("Player") || other.CompareTag("UltimatePlayer")) && _isEatable)
        {
            ToolController.Score += 1000;
            ToolController.IsPowerUpEat = true;
            _isEatable = false;
            Destroy(gameObject);
        }
        else if ((other.CompareTag("BigPlayer") || other.CompareTag("UltimateBigPlayer")) && _isEatable)
        {
            ToolController.Score += 1000;
            ToolController.IsPowerUpEat = true;
            _isEatable = false;
            Destroy(gameObject);
        }
    }
}
