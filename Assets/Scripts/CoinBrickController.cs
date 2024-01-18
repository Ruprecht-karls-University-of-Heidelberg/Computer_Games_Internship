using SystemScripts;
using UnityEngine;
using AdditionalScripts;

public class CoinBrickController : MonoBehaviour
{
    public bool isTouchByPlayer;  // Indicates if the brick has been touched by the player.
    public bool isNotSpecialBrick; // Determines if the brick is a regular brick or a special one.
    private Animator _coinBrickAnim;  // Animator component for controlling the brick's animations.
    private AudioSource _coinBrickAudio; // AudioSource component for the brick's audio effects.
    public AudioClip coinSound; // Sound effect that is played when the player hits the brick and gets a coin.
    
    // A static readonly integer that represents the "Touch_b" parameter hash in the animator.
    private static readonly int TouchB = Animator.StringToHash("Touch_b");

    // Awake is called when the script instance is being loaded.
    private void Awake()
    {
        // Retrieve the AudioSource and Animator components from the attached game object.
        _coinBrickAudio = GetComponent<AudioSource>();
        _coinBrickAnim = GetComponent<Animator>();
    }

    // This method is invoked when another collider enters the brick's collider.
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
