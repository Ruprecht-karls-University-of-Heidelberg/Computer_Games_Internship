using System;
using System.Collections;
using SystemScripts;
using UnityEngine;
using AdditionalScripts;

/// <summary>
/// Controls the behavior of a brick in the game.
/// </summary>
public class BrickController : MonoBehaviour
{
    /// <summary>
    /// Indicates whether the brick has been touched by the player.
    /// </summary>
    public bool isTouchByPlayer;

    /// <summary>
    /// Indicates whether the brick is a special brick.
    /// </summary>
    public bool isSpecialBrick;

    /// <summary>
    /// The health of the special brick.
    /// </summary>
    public int specialBrickHealth;

    /// <summary>
    /// The box collider component to disable when the brick is broken.
    /// </summary>
    public BoxCollider2D disableCollider;

    /// <summary>
    /// The game object representing the broken brick pieces.
    /// </summary>
    public GameObject breakBrickPieces;

    /// <summary>
    /// The game object representing the animation sprite.
    /// </summary>
    public GameObject animationSprite;

    private Animator _brickAnim;
    private AudioSource _brickAudio;

    /// <summary>
    /// The sound played when the brick is bumped.
    /// </summary>
    public AudioClip bumpSound;

    /// <summary>
    /// The sound played when the brick is broken.
    /// </summary>
    public AudioClip breakSound;

    /// <summary>
    /// The sound played when a coin is collected from the brick.
    /// </summary>
    public AudioClip coinSound;

    // Hash codes for the animation states
    private static readonly int TouchB = Animator.StringToHash("Touch_b");
    private static readonly int TouchT = Animator.StringToHash("Touch_t");
    private static readonly int SpecialB = Animator.StringToHash("Special_b");
    private static readonly int FinalHitB = Animator.StringToHash("FinalHit_b");

    private void Awake()
    {
        // Initializing the audio and animator components
        _brickAudio = GetComponent<AudioSource>();
        _brickAnim = GetComponent<Animator>();
    }

    private void Update()
    {
        // Set the special brick animation bool
        _brickAnim.SetBool(SpecialB, isSpecialBrick);

        if (specialBrickHealth == 0)
        {
            _brickAnim.SetBool(FinalHitB, true);
        }
    }

    /// <summary>
    /// Handles collision events with other objects.
    /// </summary>
    /// <param name="other">The collision data.</param>
    private void OnCollisionEnter2D(Collision2D other)
    {
        bool isPlayer = other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("UltimatePlayer");
        bool isBigPlayer = other.gameObject.CompareTag("BigPlayer") || other.gameObject.CompareTag("UltimateBigPlayer");

        if ((isPlayer || isBigPlayer) && !isSpecialBrick)
        {
            HandleRegularBrickCollision(isPlayer, isBigPlayer);
        }

        // Handle special brick collision
        if (isSpecialBrick && (isPlayer || isBigPlayer))
        {
            HandleSpecialBrickCollision();
        }
    }

    /// <summary>
    /// Handles collision with regular bricks.
    /// </summary>
    /// <param name="isPlayer">True if the colliding object is a player.</param>
    /// <param name="isBigPlayer">True if the colliding object is a big player.</param>
    private void HandleRegularBrickCollision(bool isPlayer, bool isBigPlayer)
    {
        if (isPlayer)
        {
            _brickAudio.PlayOneShot(bumpSound);
            isTouchByPlayer = true;
            _brickAnim.SetBool(TouchB, isTouchByPlayer);
        }
        else if (isBigPlayer)
        {
            _brickAudio.PlayOneShot(breakSound);
            disableCollider.enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;
            breakBrickPieces.SetActive(true);
            animationSprite.SetActive(false);
            _brickAnim.SetTrigger(TouchT);
            StartCoroutine(Destroy());
        }
    }

    /// <summary>
    /// Handles collision with special bricks.
    /// </summary>
    private void HandleSpecialBrickCollision()
    {
        if (specialBrickHealth > 0)
        {
            _brickAudio.PlayOneShot(coinSound);
            specialBrickHealth -= 1;
            ToolController.CollectedCoin += 1;
            ToolController.Score += 200;
            ToolController.IsEnemyDieOrCoinEat = true;
            isTouchByPlayer = true;
            _brickAnim.SetBool(TouchB, isTouchByPlayer);
        }
    }

    /// <summary>
    /// Handles collision exit events.
    /// </summary>
    /// <param name="other">The collision data.</param>
    private void OnCollisionExit2D(Collision2D other)
    {
        // Reset the touch by player state when the player is no longer in contact
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("UltimatePlayer") ||
            other.gameObject.CompareTag("BigPlayer") || other.gameObject.CompareTag("UltimateBigPlayer"))
        {
            isTouchByPlayer = false;
            _brickAnim.SetBool(TouchB, isTouchByPlayer);
        }
    }

    /// <summary>
    /// Coroutine to destroy the brick object after a short delay.
    /// </summary>
    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}
