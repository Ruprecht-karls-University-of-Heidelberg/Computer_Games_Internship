using UnityEngine;
using AdditionalScripts;

namespace SystemScripts
{
    /// <summary>
    /// Represents a behavior where the GameObject follows the player's horizontal position with some constraints.
    /// </summary>
    public class FollowPlayer : MonoBehaviour
    {
        public GameObject player; // Reference to the player object.

        // Variables to keep track of player's position
        private float _furthestPlayerPosition; // The furthest x-position the player has reached.
        private float _currentPlayerPosition;  // The current x-position of the player.

        /// <summary>
        /// Initialization logic.
        /// </summary>
        private void Start()
        {
            // If the player reference is set, initialize the current player position.
            if (player != null)
            {
                _currentPlayerPosition = player.transform.position.x;
            }
        }

        /// <summary>
        /// In the LateUpdate, after all standard Update calls, the camera follows the player's position.
        /// </summary>
        void LateUpdate()
        {
            // Check if the player object reference is set.
            if (player != null)
            {
                // Update the current player's position.
                _currentPlayerPosition = player.transform.position.x;
                
                // If the current player position is greater than the furthest position, update the furthest position.
                if (_currentPlayerPosition >= _furthestPlayerPosition)
                {
                    _furthestPlayerPosition = _currentPlayerPosition;
                }

                // Position the camera to follow the player if certain conditions are met.
                // It only follows if the player's x-position is greater than 3.5
                // and if the player's current position is the furthest they've gone.
                if (_currentPlayerPosition > 3.5f && _currentPlayerPosition >= _furthestPlayerPosition)
                {
                    // If there's no ongoing boss battle, position the camera behind the player.
                    // Else, fix the camera's position for the boss battle.
                    transform.position = !ToolController.IsBossBattle
                        ? new Vector3(player.transform.position.x, 5, -10)
                        : new Vector3(285, 5, -10);
                }
            }
        }
    }
}
