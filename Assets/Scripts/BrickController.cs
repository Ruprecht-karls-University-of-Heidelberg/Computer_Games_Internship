using System;
using System.Collections;
using SystemScripts;
using UnityEngine;

/// <summary>
/// Controls the behavior and interactions of brick objects in the game.
/// </summary>
public class BrickController : MonoBehaviour
{
    public bool isTouchByPlayer; // Indicates if the brick has been touched by the player.
    public bool isSpecialBrick;  // Determines if the brick is a special brick.
    public int specialBrickHealth; // Health of the special brick.
    public BoxCollider2D disableCollider; // Collider to be disabled when the brick breaks.
    public GameObject breakBrickPieces; // GameObject for the broken brick pieces.
    public GameObject animationSprite; // GameObject for the brick's animation sprite.
    private Animator _brickAnim;       // Animator component for the brick.
    private AudioSource _brickAudio;   // AudioSource component for brick's audio effects.
    public AudioClip bumpSound;        // Sound effect for bumping the brick.
    public AudioClip breakSound;       // Sound effect for breaking the brick.
    public AudioClip coinSound;        // Sound effect for getting a coin from the brick.

    // Hash codes for the animation states.
    private static readonly int TouchB = Animator.StringToHash("Touch_b");
    private static readonly int TouchT = Animator.StringToHash("Touch_t");
    private static readonly int SpecialB = Animator.StringToHash("Special_b");
    private static readonly int FinalHitB = Animator.StringToHash("FinalHit_b");

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// Initializes audio and animator components.
    /// </summary>
    private void Awake()
    {
        _brickAudio = GetComponent<AudioSource>();
        _brickAnim = GetComponent<Animator>();
    }

    /// <summary>
    /// Update is called once per frame to handle the special brick animation.
    /// </summary>
    private void Update()
    {
        _brickAnim.SetBool(SpecialB, isSpecialBrick);
        if (specialBrickHealth == 0)
        {
            _brickAnim.SetBool(FinalHitB, true);
        }
    }

    /// <summary>
    /// Called when another collider makes contact with the brick's collider.
    /// Handles different interactions based on the type of brick and the collider.
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
        if (isSpecialBrick && (isPlayer || isBigPlayer))
        {
            HandleSpecialBrickCollision();
        }
    }

    /// <summary>
    /// Handles collision for regular bricks.
    /// </summary>
    /// <param name="isPlayer">Indicates if a player character collided.</param>
    /// <param name="isBigPlayer">Indicates if a big player character collided.</param>
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
    /// Handles collisions for special bricks, including coin collection and health decrement.
    /// </summary>
    private void HandleSpecialBrickCollision()
    {
        if (specialBrickHealth > 0)
        {
            _brickAudio.PlayOneShot(coinSound);
            specialBrickHealth -= 1;
            GameStatusController.CollectedCoin += 1;
            GameStatusController.Score += 200;
            GameStatusController.IsEnemyDieOrCoinEat = true;
            isTouchByPlayer = true;
            _brickAnim.SetBool(TouchB, isTouchByPlayer);
        }
    }

    /// <summary>
    /// Called when another collider stops making contact with the brick's collider.
    /// Resets the 'isTouchByPlayer' state.
    /// </summary>
    /// <param name="other">The collision data.</param>
    private void OnCollisionExit2D(Collision2D other)
    {
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