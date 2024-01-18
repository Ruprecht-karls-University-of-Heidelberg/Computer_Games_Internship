using System.Collections;
// using SystemScripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using AdditionalScripts;
//after changing


namespace PlayerScripts
{
    // Manages player behaviors including movement, animations, and interactions.
    public class PlayerController : MonoBehaviour
    {
        // Movement parameters
        public float speed = 410f;
        public float slideDownSpeed = 410f;
        public float jumpForce = 795f;

        // State management
        private float _flagPos; // Position of the flag for end-of-level interactions
        private float _startInvincible; // Time when invincibility starts
        private float _invincibleTime; // Duration of invincibility
        [Range(0, 1)] public float smoothTime = 0.6f; // Smoothness of player movement
        public bool isDead; // Indicates if the player is dead
        private bool _isOnGround; // Indicates if the player is on the ground
        private bool _isEatable;  // Indicates if the player can eat (power-ups)
        private bool _isFinish; // Indicates if the level is finished
        private bool _isNotHugPole; // Indicates if the player is not hugging the pole at level end
        private bool _isFacingRight; // Indicates if the player is facing right
        private bool _isGoingDownPipeAble; // Indicates if the player can go down a pipe
        private bool _isAboveSpecialPipe; // Indicates if the player is above a special pipe
        public bool isWalkingToCastle; // Indicates if the player is walking to a castle
        public bool isInCastle;  // Indicates if the player is inside a castle
        public bool isStopTime;  // Indicates if the game should stop time
        public bool isInvulnerable; // Indicates if the player is invulnerable
        public bool isInvincible; // Indicates if the player is invincible
        private Vector3 _velocity; // Player's current velocity

        // GameObject references and settings
        [Header("GameObject Settings")] public GameObject playerSprite;
        public GameObject bigPlayer;
        public GameObject bigPlayerCollider;
        public GameObject smallPlayer;
        public GameObject smallPlayerCollider;
        public GameObject playerCol;
        public GameObject fireBallPrefab;
        public Transform fireBallParent;

        // Player animations and audio
        public Animator _playerAnim;
        public Rigidbody2D _playerRb;
        public AudioSource _playerAudio;


        // Audio settings
        [Header("AudioClip Settings")] public AudioClip jumpSound;
        public AudioClip jumpBigSound;
        public AudioClip flagPoleSound;
        public AudioClip pipeSound;
        public AudioClip dieSound;
        public AudioClip oneUpSound;
        public AudioClip turnBigSound;
        public AudioClip coinSound;
        public AudioClip kickSound;
        public AudioClip endGameSound;
        public AudioClip fireballSound;


        // Animation parameters
        private static readonly int IdleB = Animator.StringToHash("Idle_b");
        private static readonly int WalkB = Animator.StringToHash("Walk_b");
        private static readonly int RunB = Animator.StringToHash("Run_b");
        private static readonly int SpeedF = Animator.StringToHash("Speed_f");
        private static readonly int JumpTrig = Animator.StringToHash("Jump_trig");
        private static readonly int DieB = Animator.StringToHash("Die_b");
        private static readonly int BigB = Animator.StringToHash("Big_b");
        private static readonly int HugB = Animator.StringToHash("Hug_b");
        private static readonly int UltimateB = Animator.StringToHash("Ultimate_b");
        private static readonly int UltimateDurationF = Animator.StringToHash("UltimateDuration_f");
        private static readonly int CrouchB = Animator.StringToHash("Crouch_b");
        private static readonly int VulnerableB = Animator.StringToHash("Vulnerable_b");
        private static readonly int FireB = Animator.StringToHash("Fire_b");

        void Awake()
        {   
            // Initialization of player state and components
            _isFacingRight = true;
            isInvulnerable = false;
            if (ToolController.PlayerTag != null)
            {
                tag = ToolController.PlayerTag;
            }

            // Get components and set initial states
            _playerAudio = GetComponent<AudioSource>();
            _velocity = Vector3.zero;
            _playerAnim = GetComponent<Animator>();
            _playerRb = GetComponent<Rigidbody2D>();
            _isFinish = false;
            _isOnGround = true;
            isInCastle = false;
        }

        // The main update loop where player input and state are checked each frame
        public void Update()
        {
            if (IsUpdateConditionsMet())
            {
                // Handling various player actions
                HandleFireballShooting();
                HandleJumping();
                DenyMidAirJump();
                HandleSpeedAndJumpForceAdjustment();
                HandlePipeEntry();
                HandleDirectionalMovement();
                HandlePipeSliding();
            }
        }

// Checks if the player can update their state (not dead, game not finished, etc.)
        private bool IsUpdateConditionsMet()
        {
            return !isDead && !_isFinish && !ToolController.IsGameFinish;
        }

// Handles the action of shooting fireballs when the space key is pressed
        private void HandleFireballShooting()
        {
            if (Input.GetKeyDown(KeyCode.Space) && ToolController.IsFirePlayer)
            {
                Instantiate(fireBallPrefab, fireBallParent.position, fireBallParent.rotation);
                _playerAudio.PlayOneShot(fireballSound);
            }
        }

// Manages the player's jumping action when the 'A' key is pressed
        private void HandleJumping()
        {
            if (Input.GetKeyDown(KeyCode.A) && _isOnGround)
            {
                _playerAudio.PlayOneShot(ToolController.IsBigPlayer ? jumpSound : jumpBigSound);
                _isOnGround = false;
                _playerAnim.SetTrigger(JumpTrig);
                _playerRb.AddForce(new Vector2(0f, jumpForce));
                SetAnimationBools(false);
            }
        }

// Sets animation states for the player
        private void SetAnimationBools(bool state)
        {
            _playerAnim.SetBool(IdleB, state);
            _playerAnim.SetBool(WalkB, state);
            _playerAnim.SetBool(RunB, state);
        }

// Adjusts player's speed and jump force on key press
        private void HandleSpeedAndJumpForceAdjustment()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                speed = 600;
                jumpForce = 1160;
            }
            else if (Input.GetKeyUp(KeyCode.S))
            {
                speed = 410;
                jumpForce = 1030;
            }
        }

// Handles the player's interaction with pipes, triggering when the down arrow key is pressed
        private void HandlePipeEntry()
        {
            if (Input.GetKeyDown(KeyCode.DownArrow) && _isAboveSpecialPipe)
            {
                EnterPipe();
            }
        }

// Manages the player's entry into a pipe, playing sound and starting the pipe descent animation
        private void EnterPipe()
        {
            _isAboveSpecialPipe = false;
            _playerAudio.PlayOneShot(pipeSound);
            _isGoingDownPipeAble = true;
            _playerRb.velocity = Vector2.zero;
            StartCoroutine(StopGoingDownPipe());
        }

// Controls player's directional movement (left and right), flipping the player sprite as needed
        private void HandleDirectionalMovement()
        {
            if (Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.DownArrow))
            {
                if (!_isFacingRight)
                {
                    transform.Rotate(0, 180, 0);
                    _isFacingRight = !_isFacingRight;
                }
            }

            if (Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.DownArrow))
            {
                if (_isFacingRight)
                {
                    transform.Rotate(0, 180, 0);
                    _isFacingRight = !_isFacingRight;
                }
            }
        }

// Manages the player's animation and physics while sliding down a pipe
        private void HandlePipeSliding()
        {
            if (_isGoingDownPipeAble)
            {
                SetPlayerCollider();
                _playerRb.isKinematic = true;
                transform.Translate(slideDownSpeed / 2.5f * Time.deltaTime * Vector3.down);
            }
        }

// Adjusts the player's collider depending on the player's current size
        private void SetPlayerCollider()
        {
            if (CompareTag("Player"))
            {
                smallPlayerCollider.SetActive(false);
            }
            else if (CompareTag("BigPlayer"))
            {
                bigPlayerCollider.SetActive(false);
            }
        }

// Regularly called to update the game state, manage player animations and physics
        private void FixedUpdate()
        {
            HandleGameFinishState();
            CheckForBossBattle();
            UpdateDeathState();

            if (isInvincible)
                HandleInvincibility();

            if (isInvulnerable)
                HandleInvulnerability();

            if (isDead)
            {
                Die();
            }
            else if (!isDead && !_isFinish && !ToolController.IsGameFinish)
            {
                UpdatePlayerState();
                MovePlayer();
                GetPlayerSpeed();
            }

            HandleFinishState();
        }

// Manages game state when the game is finished, including player movement
        private void HandleGameFinishState()
        {
            if (ToolController.IsGameFinish)
            {
                transform.Translate(slideDownSpeed / 1.25f * Time.deltaTime * Vector3.right);
                CheckForGameRestart();
            }
        }

// Checks for player input to restart the game
        private void CheckForGameRestart()
        {
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                ToolController.IsGameFinish = false;
                ToolController.IsShowMessage = false;
                SceneManager.LoadScene(0);
            }
        }
// Checks if the player has reached the position for a boss battle
        private void CheckForBossBattle()
        {
            if (Mathf.RoundToInt(transform.position.x) == 285)
            {
                ToolController.IsBossBattle = true;
            }
        }

// Updates the player's death state based on the game controller
        private void UpdateDeathState()
        {
            isDead = ToolController.IsDead;
        }

// Manages the player's invincibility effects, including ignoring collisions
        private void HandleInvincibility()
        {
            _invincibleTime = Time.time - _startInvincible;
            _playerAnim.SetFloat(UltimateDurationF, _invincibleTime);
            Physics2D.IgnoreLayerCollision(8, 9, true);
            if (Time.time - _startInvincible > 10)
            {
                StartCoroutine(BeNormal());
            }
        }
// Manages the player's invulnerability state, ignoring collisions temporarily
        private void HandleInvulnerability()
        {
            Physics2D.IgnoreLayerCollision(8, 9, true);
            StartCoroutine(BeVulnerable());
        }

// Updates the player's animation states based on the current game state
        private void UpdatePlayerState()
        {
            _playerAnim.SetBool(BigB, ToolController.IsBigPlayer);
            _playerAnim.SetBool(FireB, ToolController.IsFirePlayer);
            ChangeAnim();
        }

// Manages the player's finish state, including animations and movement
        private void HandleFinishState()
        {
            if (_isFinish)
            {
                if (transform.position.y > 1.5f)
                {
                    PerformSlideDown();
                }
                else
                {
                    HandleHugPole();
                }
            }
        }

// Executes the slide down animation when finishing the level
        private void PerformSlideDown()
        {
            _playerAnim.SetBool(HugB, true);
            _playerAnim.SetFloat(SpeedF, 0);
            transform.Translate(slideDownSpeed * Time.deltaTime * Vector3.down);
        }

// Manages the player's interaction with the flag pole at the end of a level
        private void HandleHugPole()
        {
            if (transform.position.x < _flagPos + 0.8f)
            {
                _playerAnim.SetBool(HugB, false);
                transform.localScale = new Vector3(-1, 1, 1);
                transform.position = new Vector3(_flagPos + 0.8f, transform.position.y);
            }

            _playerRb.isKinematic = false;
            StartCoroutine(HugPole());
            if (_isNotHugPole)
            {
                transform.localScale = Vector3.one;
                _playerAnim.SetFloat(SpeedF, 3f);
                transform.Translate(slideDownSpeed * Time.deltaTime * Vector3.right);
            }
        }

// Manages the player's movement, including horizontal movement and crouching
        private void MovePlayer()
        {
            if (!_isGoingDownPipeAble)
            {
                HandleHorizontalMovement();
                HandleCrouching();
            }
        }

// Handles the player's horizontal movement based on player input
        private void HandleHorizontalMovement()
        {
            if (!Input.GetKey(KeyCode.DownArrow))
            {
                var horizontalInput = Input.GetAxisRaw("Horizontal");
                Vector2 playerVelocity = _playerRb.velocity;
                Vector3 targetVelocity = new Vector2(horizontalInput * speed * Time.fixedDeltaTime, playerVelocity.y);
                _playerRb.velocity = Vector3.SmoothDamp(playerVelocity, targetVelocity, ref _velocity, smoothTime);
            }
        }

// Manages the player's crouching action, changing the collider and animation
        private void HandleCrouching()
        {
            bool isBigOrUltimatePlayer = CompareTag("BigPlayer") || CompareTag("UltimateBigPlayer");
            if (Input.GetKey(KeyCode.DownArrow) && isBigOrUltimatePlayer && !_isAboveSpecialPipe)
            {
                CrouchPlayer();
            }
            else if (Input.GetKeyUp(KeyCode.DownArrow) && isBigOrUltimatePlayer && !_isAboveSpecialPipe)
            {
                StandUpPlayer();
            }
        }
// Activates the crouching animation and adjusts the collider for a small player
        private void CrouchPlayer()
        {
            _playerAnim.SetBool(CrouchB, true);
            smallPlayerCollider.SetActive(true);
            bigPlayerCollider.SetActive(false);
        }
// Deactivates the crouching animation and adjusts the collider for a big player
        private void StandUpPlayer()
        {
            _playerAnim.SetBool(CrouchB, false);
            smallPlayerCollider.SetActive(false);
            bigPlayerCollider.SetActive(true);
        }

// Collision handling for various game objects like ground, power-ups, and hazards
        private void OnCollisionEnter2D(Collision2D other)
        {
            string tag = other.gameObject.tag;

            if (IsGroundRelatedTag(tag))
            {
                HandleGroundRelatedCollision();
            }

            if (tag == "PowerBrick")
            {
                _isEatable = false;
            }

            if (tag == "Pole")
            {
                HandlePoleCollision(other);
            }

            if (tag == "Castle")
            {
                HandleCastleCollision();
            }

            if (tag == "DeathAbyss")
            {
                HandleDeathAbyssCollision();
            }

            if (tag == "1UpMushroom" && _isEatable)
            {
                Handle1UpMushroomCollision();
            }

            if (tag == "BigMushroom" && _isEatable)
            {
                HandleBigMushroomCollision();
            }

            if (tag == "UltimateStar" && _isEatable)
            {
                HandleUltimateStarCollision();
            }

            if (tag == "FireFlower" && _isEatable)
            {
                HandleFireFlowerCollision();
            }

            if (tag == "SpecialPipe")
            {
                _isAboveSpecialPipe = true;
            }

            if (tag == "KoopaShell")
            {
                _playerAudio.PlayOneShot(kickSound);
            }
        }

// Determines if a tag is related to ground elements
        private bool IsGroundRelatedTag(string tag)
        {
            return tag == "Ground" || tag == "Pipe" || tag == "Brick" || tag == "Stone" || tag == "SpecialPipe";
        }
// Handles collisions with ground-related objects, setting the player on the ground
        private void HandleGroundRelatedCollision()
        {
            _isOnGround = true;
            SetAnimationBools(true);
        }

// Manages the interaction with the flag pole, setting the finish state
        private void HandlePoleCollision(Collision2D other)
        {
            _playerAudio.PlayOneShot(flagPoleSound);
            _flagPos = other.gameObject.transform.position.x;
            SetFinishState();
        }

// Sets the state to indicate the game level is finished
        private void SetFinishState()
        {
            _isFinish = true;
            _playerRb.velocity = Vector2.zero;
            _playerRb.isKinematic = true;
            isWalkingToCastle = true;
            isStopTime = true;
            StartCoroutine(PlayStageClearSound());
        }

// Handles the event when the player collides with the castle
        private void HandleCastleCollision()
        {
            isInCastle = true;
            isWalkingToCastle = false;
            playerSprite.SetActive(false);
        }
// Manages the event when the player falls into a death abyss
        private void HandleDeathAbyssCollision()
        {
            _playerAudio.PlayOneShot(dieSound);
            UpdateToolControllerOnDeath();
            _playerRb.isKinematic = true;
            _playerRb.velocity = Vector2.zero;
        }
// Updates the game state when the player dies
        private void UpdateToolControllerOnDeath()
        {
            ToolController.Live -= 1;
            ToolController.IsBigPlayer = false;
            ToolController.IsFirePlayer = false;
            ToolController.PlayerTag = "Player";
            ToolController.IsDead = true;
        }
// Handles the event of colliding with a 1-Up Mushroom
        private void Handle1UpMushroomCollision()
        {
            _playerAudio.PlayOneShot(oneUpSound);
            ToolController.Live += 1;
            _isEatable = false;
        }
// Manages the event of colliding with a Big Mushroom
        private void HandleBigMushroomCollision()
        {
            _playerAudio.PlayOneShot(turnBigSound);
            TurnIntoBigPlayer();
            _isEatable = false;
        }
// Handles the event of colliding with an Ultimate Star
        private void HandleUltimateStarCollision()
        {
            _playerAudio.PlayOneShot(turnBigSound);
            TurnIntoUltimatePlayer();
            _isEatable = false;
        }
// Transforms the player into an ultimate player with invincibility
        private void TurnIntoUltimatePlayer()
        {
            if (CompareTag("Player"))
            {
                tag = "UltimatePlayer";
            }
            else
            {
                tag = "UltimateBigPlayer";
            }
            isInvincible = true;
            _playerAnim.SetBool(UltimateB, isInvincible);  //update the animation
            _startInvincible = Time.time;   // Starts the invincibility timer
        }

// Handles the event of colliding with a Fire Flower
        private void HandleFireFlowerCollision()
        {
            _playerAudio.PlayOneShot(turnBigSound); // Plays the power-up sound
            
            // Transforms the player based on the current state
            if (CompareTag("Player") || CompareTag("UltimatePlayer"))
            {
                TurnIntoBigPlayer();
            }
            else if (CompareTag("BigPlayer") || CompareTag("UltimateBigPlayer"))
            {
                ToolController.IsFirePlayer = true;
            }
            // Disables further collision with the item
            _isEatable = false; 
        }

// Resets the eatable state when exiting collision with a Power Brick
       private void OnCollisionExit2D(Collision2D other)
       {
            if (other.gameObject.CompareTag("PowerBrick"))
            {
                _isEatable = true;
            }
       }
// Handles collision triggers with various objects
       private void OnTriggerEnter2D(Collider2D other)
       {
            switch (other.gameObject.tag)
            {
                case "Princess":
                    HandlePrincessCollision();
                    break;
                case "Axe":
                    HandleAxeCollision(other);
                    break;
                case "Coin":
                    _playerAudio.PlayOneShot(coinSound);
                    break;
            }
       }
// Handles collision with the Princess
       private void HandlePrincessCollision()
       {
            slideDownSpeed = 0;
            ToolController.IsShowMessage = true;
            _playerAnim.SetFloat(SpeedF, 0);
       }
// Handles collision with an Axe
       private void HandleAxeCollision(Collider2D other)
       {
            _playerAudio.PlayOneShot(endGameSound);
            Destroy(other.gameObject);
            ToolController.IsBossBattle = false;
            ToolController.IsGameFinish = true;
            _playerAnim.SetFloat(SpeedF, 3f);
            _playerRb.velocity = Vector2.zero;
       }
// Transforms the player into a Big Player
       private void TurnIntoBigPlayer()
       {
            tag = CompareTag("Player") ? "BigPlayer" : "UltimateBigPlayer"; // Updates the player's tag
            ToolController.PlayerTag = tag; // Updates the player tag in the ToolController
            ToolController.IsBigPlayer = true; // Indicates the player is now big
            ChangeAnim(); // Changes the player's animation accordingly
       }

       public void Die()
       {
            _playerAnim.SetBool(DieB, isDead);  // Triggers the death animation
            ToolController.IsDead = true;  // Updates the death state in the ToolController
            StartCoroutine(DieAnim());  // Starts the death animation coroutine
            StartCoroutine(LoadingScene()); // Starts the loading scene coroutine
       }
// Updates the player's speed in the animator
       private void GetPlayerSpeed()
       {
        // Sets the speed based on player's velocity
            _playerAnim.SetFloat(SpeedF, Mathf.Abs(_playerRb.velocity.x));
       }
// Changes the player's animation based on the size state
       public void ChangeAnim()
       {
            bool isBigPlayer = ToolController.IsBigPlayer;
            bigPlayer.SetActive(isBigPlayer);
            bigPlayerCollider.SetActive(isBigPlayer);
            smallPlayer.SetActive(!isBigPlayer);
            smallPlayerCollider.SetActive(!isBigPlayer);
       }
// Denies the player from jumping in mid-air
       private void DenyMidAirJump()
       {
            _isOnGround = _playerRb.velocity.y == 0;
            SetAnimationBools(_isOnGround);
       }

// Coroutine to set the eatable state after a delay
       private IEnumerator SetBoolEatable()
       {
            yield return new WaitForSeconds(0.5f);
            _isEatable = true;
       }
// Coroutine for handling the hug pole animation
       private IEnumerator HugPole()
       {
            yield return new WaitForSeconds(1.5f);
            _isNotHugPole = true;
       }
// Coroutine for the death animation sequence
       private IEnumerator DieAnim()
       {
            yield return new WaitForSeconds(1);
            playerCol.SetActive(false);
            _playerRb.isKinematic = false;
       }

       private static IEnumerator LoadingScene()
       {
            yield return new WaitForSeconds(3.5f);
            SceneManager.LoadScene(1);
       }

       private IEnumerator PlayStageClearSound()
       {
            yield return new WaitForSeconds(1.5f);
            ToolController.IsStageClear = true;
       }

       private IEnumerator BeVulnerable()
       {
            yield return new WaitForSeconds(2);
            _playerAnim.SetBool(VulnerableB, true);
            Physics2D.IgnoreLayerCollision(8, 9, false);
            isInvulnerable = false;
       }

       private IEnumerator BeNormal()
       {
            yield return new WaitForSeconds(2);
            tag = ToolController.PlayerTag;
            isInvincible = false;
            _playerAnim.SetBool(UltimateB, isInvincible);
            Physics2D.IgnoreLayerCollision(8, 9, false);
       }
// Coroutine for stopping the sliding down the pipe sequence
       private IEnumerator StopGoingDownPipe()
       {
            yield return new WaitForSeconds(1.5f);
            _isGoingDownPipeAble = false;
            SceneManager.LoadScene(ToolController.CurrentLevel);
            ToolController.CurrentLevel += 1;
       }
    }
}
