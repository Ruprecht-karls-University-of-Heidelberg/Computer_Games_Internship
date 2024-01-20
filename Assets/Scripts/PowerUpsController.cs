using System;
using System.Collections;
using UnityEngine;
using AdditionalScripts;

/// <summary>
/// Controls the behavior of power-ups in the game.
/// </summary>
public class PowerUpsController : MonoBehaviour
{
    public int speedRight;           ///< The horizontal speed of the power-up.
    public int speedUp;              ///< The vertical speed of the power-up.
    public bool isMoving;            ///< Flag indicating whether the power-up is moving.
    public bool isTouchByPlayer;     ///< Flag indicating whether the power-up has been touched by the player.
    private bool _isEatable;         ///< Internal flag indicating whether the power-up is eatable.
    private float _firstYPos;        ///< The initial vertical position of the power-up.

    private AudioSource _powerAudio; ///< Audio source for playing power-up sounds.
    public AudioClip appearSound;    ///< Sound clip that plays when the power-up appears.

    /// <summary>
    /// Initializes the power-up.
    /// </summary>
    public void Awake()
    {
        InitializePowerUp();
    }

    /// <summary>
    /// Called once per frame, handles power-up movement and behavior.
    /// </summary>
    void Update()
    {
        HandlePowerUpMovement();
    }

    /// <summary>
    /// Handles collision with other game objects.
    /// </summary>
    /// <param name="other">The other game object involved in the collision.</param>
    private void OnCollisionEnter2D(Collision2D other)
    {
        HandleCollision(other.gameObject);
    }

    /// <summary>
    /// Handles the trigger enter event, typically used for interactions without physics impacts.
    /// </summary>
    /// <param name="other">The collider that triggered the event.</param>
    public void OnTriggerEnter2D(Collider2D other)
    {
        HandleTriggerEnter(other.gameObject);
    }

    /// <summary>
    /// Initializes power-up properties and sets initial conditions.
    /// </summary>
    private void InitializePowerUp()
    {
        _powerAudio = GetComponent<AudioSource>();
        Physics2D.IgnoreLayerCollision(9, 10, true);
        _firstYPos = transform.position.y;
    }

    /// <summary>
    /// Handles the movement of the power-up, including its interaction with the player.
    /// </summary>
    private void HandlePowerUpMovement()
    {
        if (isTouchByPlayer && !CompareTag("Coin"))
        {
            MovePowerUp();
            EnableMotionForCertainPowerUps();
        }

        if (isMoving && (CompareTag("BigMushroom") || CompareTag("1UpMushroom")))
        {
            isTouchByPlayer = false;
            transform.Translate(speedRight * Time.deltaTime * Vector2.right);
        }
    }

    /// <summary>
    /// Moves the power-up vertically until it reaches a certain height.
    /// </summary>
    private void MovePowerUp()
    {
        if (transform.position.y < _firstYPos + 1)
        {
            transform.Translate(speedUp * Time.deltaTime * Vector2.up);
        }
    }

    /// <summary>
    /// Enables horizontal motion for specific power-ups after they reach a certain height.
    /// </summary>
    private void EnableMotionForCertainPowerUps()
    {
        if (transform.position.y >= _firstYPos + 1 && (CompareTag("BigMushroom") || CompareTag("1UpMushroom")))
        {
            isMoving = true;
            GetComponent<Rigidbody2D>().isKinematic = false;
        }
    }

    /// <summary>
    /// Handles interactions with other game objects, such as players, stones, and pipes.
    /// </summary>
    /// <param name="other">The game object involved in the interaction.</param>
    private void HandleCollision(GameObject other)
    {
        InteractionWithPlayer(other);

        if (other.CompareTag("Stone") || other.CompareTag("Pipe") || other.CompareTag("Untagged"))
        {
            speedRight = -speedRight;
        }
    }

    /// <summary>
    /// Handles trigger interactions, specifically for coin collection.
    /// </summary>
    /// <param name="other">The game object that triggered the event.</param>
    private void HandleTriggerEnter(GameObject other)
    {
        if (IsCoin(other))
        {
            UpdateCoinCollection();
            Destroy(gameObject);
        }
        else
        {
            InteractionWithPlayer(other);
        }
    }

    /// <summary>
    /// Determines if the power-up is a coin.
    /// </summary>
    /// <param name="other">The game object to check.</param>
    /// <returns>True if the power-up is a coin, otherwise false.</returns>
    private bool IsCoin(GameObject other)
    {
        return CompareTag("Coin") && (other.CompareTag("Player") || other.CompareTag("BigPlayer") ||
                                      other.CompareTag("UltimatePlayer") || other.CompareTag("UltimateBigPlayer"));
    }

    /// <summary>
    /// Updates the coin collection count and score.
    /// </summary>
    private void UpdateCoinCollection()
    {
        ToolController.CollectedCoin += 1;
        ToolController.Score += 200;
        ToolController.IsEnemyDieOrCoinEat = true;
    }

    /// <summary>
    /// Coroutine that sets the power-up to be eatable after a delay.
    /// </summary>
    private IEnumerator SetBoolEatable()
    {
        yield return new WaitForSeconds(1);
        _isEatable = true;
    }

    /// <summary>
    /// Handles interaction between the power-up and the player.
    /// </summary>
    /// <param name="other">The player game object.</param>
    private void InteractionWithPlayer(GameObject other)
    {
        if (!CompareTag("Coin") && (other.CompareTag("Player") || other.CompareTag("UltimatePlayer") ||
                                    other.CompareTag("BigPlayer") || other.CompareTag("UltimateBigPlayer")))
        {
            HandlePowerUpInteraction();
        }

        if (_isEatable && (other.CompareTag("Player") || other.CompareTag("BigPlayer") ||
                           other.CompareTag("UltimatePlayer") || other.CompareTag("UltimateBigPlayer")))
        {
            ConsumePowerUp();
        }
    }

    /// <summary>
    /// Handles the initial interaction when a player touches the power-up.
    /// </summary>
    private void HandlePowerUpInteraction()
    {
        _powerAudio.PlayOneShot(appearSound);
        isTouchByPlayer = true;
        StartCoroutine(SetBoolEatable());
    }

    /// <summary>
    /// Consumes the power-up, updates the score, and destroys the power-up game object.
    /// </summary>
    private void ConsumePowerUp()
    {
        ToolController.Score += 1000;
        ToolController.IsPowerUpEat = true;
        _isEatable = false;
        Destroy(gameObject);
    }
}
