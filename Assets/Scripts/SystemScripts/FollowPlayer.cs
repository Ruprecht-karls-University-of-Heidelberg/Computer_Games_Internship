using UnityEngine;
using AdditionalScripts;

//after changing

namespace SystemScripts
{
    /// <summary>
    /// Defines a behavior where the GameObject follows the player's horizontal position with constraints.
    /// </summary>
    public class FollowPlayer : MonoBehaviour
    {
        public GameObject player; // Reference to the player GameObject.

        private float _furthestPlayerPosition; // Farthest x-position reached by the player.
        private float _currentPlayerPosition;  // Current x-position of the player.

        /// <summary>
        /// Initialize the component.
        /// </summary>
        private void Start()
        {
            InitializePlayerPosition();
        }

        /// <summary>
        /// Follows the player's position, adjusting based on certain conditions.
        /// </summary>
        void LateUpdate()
        {
            if (player != null)
            {
                UpdatePlayerPositions();
                PositionCameraBasedOnPlayer();
            }
        }

        /// <summary>
        /// Initializes the player's position if the player is referenced.
        /// </summary>
        private void InitializePlayerPosition()
        {
            if (player != null)
            {
                _currentPlayerPosition = player.transform.position.x;
            }
        }

        /// <summary>
        /// Updates the current and furthest positions of the player.
        /// </summary>
        private void UpdatePlayerPositions()
        {
            _currentPlayerPosition = player.transform.position.x;
            if (_currentPlayerPosition > _furthestPlayerPosition)
            {
                _furthestPlayerPosition = _currentPlayerPosition;
            }
        }

        /// <summary>
        /// Adjusts the camera's position based on the player's position and game conditions.
        /// </summary>
        private void PositionCameraBasedOnPlayer()
        {
            if (_currentPlayerPosition > 3.5f && _currentPlayerPosition >= _furthestPlayerPosition)
            {
                Vector3 newCameraPosition = !ToolController.IsBossBattle 
                    ? new Vector3(player.transform.position.x, 5, -10) 
                    : new Vector3(285, 5, -10);

                transform.position = newCameraPosition;
            }
        }
    }
}
