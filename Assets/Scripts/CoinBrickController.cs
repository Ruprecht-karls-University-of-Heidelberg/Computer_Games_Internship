using SystemScripts;
using UnityEngine;

/// <summary>
/// Controls the behavior of coin bricks in the game, handling player interaction and triggering animations and sounds.
/// </summary>
public class CoinBrickController : MonoBehaviour
{
    public bool isTouchByPlayer;     // Indicates if the brick has been touched by the player.
    public bool isNotSpecialBrick;   // Determines if the brick is a regular brick or a special one.
    private Animator _coinBrickAnim; // Animator component for controlling the brick's animations.
    private AudioSource _coinBrickAudio; // AudioSource component for the brick's audio effects.
    public AudioClip coinSound;      // Sound effect that is played when the player hits the brick and gets a coin.
    
    // A static readonly integer that represents the "Touch_b" parameter hash in the animator.
    private static readonly int TouchB = Animator.StringToHash("Touch_b");

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// Initializes components attached to the brick.
    /// </summary>
    private void Awake()
    {
        _coinBrickAudio = GetComponent<AudioSource>();
        _coinBrickAnim = GetComponent<Animator>();
    }

    /// <summary>
    /// Called when another collider makes contact with the brick's collider.
    /// Handles interactions with the player.
    /// </summary>
    /// <param name="other">The collision data.</param>
    private void OnCollisionEnter2D(Collision2D other)
    {
        // Check for player interaction and ensure the brick hasn't been previously activated.
        if ((other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("UltimatePlayer") ||
             other.gameObject.CompareTag("BigPlayer") || other.gameObject.CompareTag("UltimateBigPlayer")) && !isTouchByPlayer)
        {
            if (isNotSpecialBrick)
            {
                _coinBrickAudio.PlayOneShot(coinSound);
                GameStatusController.Score += 200;
                GameStatusController.IsEnemyDieOrCoinEat = true;
                GameStatusController.CollectedCoin += 1;
            }

            isTouchByPlayer = true;
            _coinBrickAnim.SetBool(TouchB, isTouchByPlayer);
        }
    }
}
