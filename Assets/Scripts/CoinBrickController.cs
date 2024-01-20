using SystemScripts;
using UnityEngine;
using AdditionalScripts;

/// <summary>
/// Controls the behavior of a coin brick in the game.
/// </summary>
public class CoinBrickController : MonoBehaviour
{
    /// <summary>
    /// Indicates if the brick has been touched by the player.
    /// </summary>
    public bool isTouchByPlayer;

    /// <summary>
    /// Determines if the brick is a regular brick or a special one.
    /// </summary>
    public bool isNotSpecialBrick;

    /// <summary>
    /// Animator component for controlling the brick's animations.
    /// </summary>
    private Animator _coinBrickAnim;

    /// <summary>
    /// AudioSource component for the brick's audio effects.
    /// </summary>
    private AudioSource _coinBrickAudio;

    /// <summary>
    /// Sound effect that is played when the player hits the brick and gets a coin.
    /// </summary>
    public AudioClip coinSound;

    /// <summary>
    /// A static readonly integer that represents the "Touch_b" parameter hash in the animator.
    /// </summary>
    private static readonly int TouchB = Animator.StringToHash("Touch_b");

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        // Retrieve the AudioSource and Animator components from the attached game object.
        _coinBrickAudio = GetComponent<AudioSource>();
        _coinBrickAnim = GetComponent<Animator>();
    }

    /// <summary>
    /// This method is invoked when another collider enters the brick's collider.
    /// </summary>
    /// <param name="other">The collider that entered the brick's collider.</param>
    private void OnCollisionEnter2D(Collision2D other)
    {
        // Check if the collided object has one of the player-related tags and if the brick hasn't been touched before.
        if ((other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("UltimatePlayer") ||
             other.gameObject.CompareTag("BigPlayer") || other.gameObject.CompareTag("UltimateBigPlayer")) && !isTouchByPlayer)
        {
            // Check if it's a regular brick and not a special one.
            if (isNotSpecialBrick)
            {
                // Play the coin sound, increase the score and update relevant game status values.
                _coinBrickAudio.PlayOneShot(coinSound);
                ToolController.Score += 200;
                ToolController.IsEnemyDieOrCoinEat = true;
                ToolController.CollectedCoin += 1;
            }

            // Set the brick as touched by the player and trigger the relevant animation.
            isTouchByPlayer = true;
            _coinBrickAnim.SetBool(TouchB, isTouchByPlayer);
        }
    }
}
