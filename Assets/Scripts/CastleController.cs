using UnityEngine;
using AdditionalScripts;

/// <summary>
/// Controls the castle behavior and animations.
/// </summary>
public class CastleController : MonoBehaviour
{
    private Animator _castleAnim;  // Animator component for controlling castle animations.
    
    // A static readonly integer that represents the "Finish_b" parameter hash in the animator.
    private static readonly int FinishB = Animator.StringToHash("Finish_b");

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        // Retrieve the Animator component from the attached game object.
        _castleAnim = GetComponent<Animator>();
    }

    /// <summary>
    /// This method is invoked when another collider enters the castle's collider.
    /// </summary>
    /// <param name="other">The collision data of the other collider.</param>
    private void OnCollisionEnter2D(Collision2D other)
    {
        // Check if the collided object has a "Player" or "BigPlayer" tag.
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("BigPlayer"))
        {
            // Set the "Finish_b" parameter in the animator to true, triggering an animation.
            _castleAnim.SetBool(FinishB, true);
        }
    }
}
