using UnityEngine;

/// <summary>
/// Controls the behavior and animations of the castle in the game.
/// </summary>
public class CastleController : MonoBehaviour
{
    private Animator _castleAnim; // Animator component for controlling castle animations.
    
    // A static readonly integer that represents the "Finish_b" parameter hash in the animator.
    private static readonly int FinishB = Animator.StringToHash("Finish_b");

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// Initializes the castle's Animator component.
    /// </summary>
    private void Awake()
    {
        _castleAnim = GetComponent<Animator>();
    }

    /// <summary>
    /// Called when another collider makes contact with the castle's collider.
    /// Triggers the castle's finish animation on player contact.
    /// </summary>
    /// <param name="other">The collision data.</param>
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("BigPlayer"))
        {
            _castleAnim.SetBool(FinishB, true);
        }
    }
}
