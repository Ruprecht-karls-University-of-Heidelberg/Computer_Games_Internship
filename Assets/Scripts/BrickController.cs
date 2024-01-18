using System;
using System.Collections;
using SystemScripts;
using UnityEngine;
using AdditionalScripts;

public class BrickController : MonoBehaviour
{
    public bool isTouchByPlayer;
    public bool isSpecialBrick;
    public int specialBrickHealth;
    public BoxCollider2D disableCollider;
    public GameObject breakBrickPieces;
    public GameObject animationSprite;
    private Animator _brickAnim;
    private AudioSource _brickAudio;
    public AudioClip bumpSound;
    public AudioClip breakSound;
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

    // Coroutine to destroy the brick object after a short delay
    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}