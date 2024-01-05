using System.Collections;
using SystemScripts;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PlayerScripts
{
    /// <summary>
    /// Controls the behavior and interactions of the player character in the game.
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        [Header("Player Movement Settings")]
        public float speed = 410f; // Defines the speed of the player.
        public float slideDownSpeed = 410f; // Defines the speed when the player slides down.
        public float jumpForce = 795f; // Defines the jump force of the player.
        [Range(0, 1)] public float smoothTime = 0.6f; // Defines the smoothness of the movement.

        [Header("Player Status Flags")]
        public bool isDead; // Flag to check if the player is dead.
        public bool isWalkingToCastle; // Flag to check if the player is walking to the castle.
        public bool isInCastle; // Flag to check if the player is in the castle.
        public bool isStopTime; // Flag to check if the time should be stopped.
        public bool isInvulnerable; // Flag to check if the player is invulnerable.
        public bool isInvincible; // Flag to check if the player is invincible.

        // Private variables to manage player states and interactions.
        private float _flagPos;
        private float _startInvincible;
        private float _invincibleTime;
        private bool _isOnGround;
        private bool _isEatable;
        private bool _isFinish;
        private bool _isNotHugPole;
        private bool _isFacingRight;
        private bool _isGoingDownPipeAble;
        private bool _isAboveSpecialPipe;
        private Vector3 _velocity;

        [Header("GameObject Settings")]
        public GameObject playerSprite; // The player sprite game object.
        public GameObject bigPlayer; // Big player variant game object.
        public GameObject bigPlayerCollider; // Big player collider game object.
        public GameObject smallPlayer; // Small player variant game object.
        public GameObject smallPlayerCollider; // Small player collider game object.
        public GameObject playerCol; // Player collider game object.
        public GameObject fireBallPrefab; // Fireball prefab for firing.
        public Transform fireBallParent; // Parent transform for fireballs.

        // Player components.
        private Animator _playerAnim; // Animator component for the player.
        private Rigidbody2D _playerRb; // Rigidbody2D component for physics interactions.
        private AudioSource _playerAudio; // AudioSource component for playing sounds.

        [Header("AudioClip Settings")]
        public AudioClip jumpSound; // Jump sound effect.
        public AudioClip jumpBigSound; // Jump sound effect for big player.
        public AudioClip flagPoleSound; // Sound when hitting the flag pole.
        public AudioClip pipeSound; // Sound when going through a pipe.
        public AudioClip dieSound; // Sound when the player dies.
        public AudioClip oneUpSound; // Sound for getting an extra life.
        public AudioClip turnBigSound; // Sound for turning big.
        public AudioClip coinSound; // Sound for collecting a coin.
        public AudioClip kickSound; // Sound for kicking.
        public AudioClip endGameSound; // Sound for ending the game.
        public AudioClip fireballSound; // Sound for firing a fireball.

        // Animator parameter hashes for performance optimization.
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

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// Initializes player state and components.
        /// </summary>
        void Awake()
        {
            _isFacingRight = true;
            isInvulnerable = false;
            if (GameStatusController.PlayerTag != null)
            {
                tag = GameStatusController.PlayerTag;
            }

            _playerAudio = GetComponent<AudioSource>();
            _velocity = Vector3.zero;
            _playerAnim = GetComponent<Animator>();
            _playerRb = GetComponent<Rigidbody2D>();
            _isFinish = false;
            _isOnGround = true;
            isInCastle = false;
        }
        /// <summary>
        /// Update is called every frame and handles player inputs and interactions.
        /// </summary>
        private void Update()
        {
            // Check if the player is not dead, not finished, and the game is not finished.
            if (!isDead && !_isFinish && !GameStatusController.IsGameFinish)
            {
                // Handle fireball shooting input.
                if (Input.GetKeyDown(KeyCode.Space) && GameStatusController.IsFirePlayer)
                {
                    Instantiate(fireBallPrefab, fireBallParent.position, fireBallParent.rotation);
                    _playerAudio.PlayOneShot(fireballSound);
                }

                // Handle jumping input.
                if (Input.GetKeyDown(KeyCode.A) && _isOnGround)
                {
                    _playerAudio.PlayOneShot(GameStatusController.IsBigPlayer ? jumpSound : jumpBigSound);
                    _isOnGround = false;
                    _playerAnim.SetTrigger(JumpTrig);
                    _playerRb.AddForce(new Vector2(0f, jumpForce));
                    _playerAnim.SetBool(IdleB, false);
                    _playerAnim.SetBool(WalkB, false);
                    _playerAnim.SetBool(RunB, false);
                }

                // Additional method calls and checks.
                DenyMidAirJump();

                // Handle speed boost input.
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

                // Handle input for going down a special pipe.
                if (Input.GetKeyDown(KeyCode.DownArrow) && _isAboveSpecialPipe)
                {
                    _isAboveSpecialPipe = false;
                    _playerAudio.PlayOneShot(pipeSound);
                    _isGoingDownPipeAble = true;
                    _playerRb.velocity = Vector2.zero;
                    StartCoroutine(StopGoingDownPipe());
                }

                // Handle right and left movement inputs.
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

                // Handle the player going down a pipe.
                if (_isGoingDownPipeAble)
                {
                    if (CompareTag("Player"))
                    {
                        smallPlayerCollider.SetActive(false);
                    }
                    else if (CompareTag("BigPlayer"))
                    {
                        bigPlayerCollider.SetActive(false);
                    }

                    _playerRb.isKinematic = true;
                    transform.Translate(slideDownSpeed / 2.5f * Time.deltaTime * Vector3.down);
                }
            }
        }

        /// <summary>
        /// FixedUpdate is called at a fixed interval and is used for physics updates.
        /// It handles the player's movement and interactions based on game state.
        /// </summary>
        private void FixedUpdate()
        {
            // Handle game finish logic.
            if (GameStatusController.IsGameFinish)
            {
                transform.Translate(slideDownSpeed / 1.25f * Time.deltaTime * Vector3.right);
                if (Input.GetKeyDown(KeyCode.Backspace))
                {
                    GameStatusController.IsGameFinish = false;
                    GameStatusController.IsShowMessage = false;
                    SceneManager.LoadScene(0);
                }
            }

            // Check for boss battle trigger.
            if (Mathf.RoundToInt(transform.position.x) == 285)
            {
                GameStatusController.IsBossBattle = true;
            }

            // Update player's dead status.
            isDead = GameStatusController.IsDead;

            // Handle invincibility logic.
            if (isInvincible)
            {
                _invincibleTime = Time.time - _startInvincible;
                _playerAnim.SetFloat(UltimateDurationF, _invincibleTime);
                Physics2D.IgnoreLayerCollision(8, 9, true);
                if (Time.time - _startInvincible > 10)
                {
                    StartCoroutine(BeNormal());
                }
            }

            // Handle invulnerability logic.
            if (isInvulnerable)
            {
                Physics2D.IgnoreLayerCollision(8, 9, true);
                StartCoroutine(BeVulnerable());
            }

            // Handle player death logic.
            if (isDead)
            {
                Die();
            }
            else if (!isDead && !_isFinish && !GameStatusController.IsGameFinish)
            {
                // Update animation states and move the player.
                _playerAnim.SetBool(BigB, GameStatusController.IsBigPlayer);
                _playerAnim.SetBool(FireB, GameStatusController.IsFirePlayer);
                ChangeAnim();
                MovePlayer();
                GetPlayerSpeed();
            }

            // Handle finishing logic.
            if (_isFinish)
            {
                // Hug pole animation and movement.
                if (transform.position.y > 1.5f)
                {
                    _playerAnim.SetBool(HugB, true);
                    _playerAnim.SetFloat(SpeedF, 0);
                    transform.Translate(slideDownSpeed * Time.deltaTime * Vector3.down);
                }
                else
                {
                    // Player movement after hugging the pole.
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
            }
        }
        /// <summary>
        /// Moves the player based on player input.
        /// Handles horizontal movement and crouching.
        /// </summary>
        private void MovePlayer()
        {
            // Handle horizontal movement if not crouching or going down a pipe.
            if (!Input.GetKey(KeyCode.DownArrow) && !_isGoingDownPipeAble)
            {
                var horizontalInput = Input.GetAxisRaw("Horizontal");
                Vector2 playerVelocity = _playerRb.velocity;
                Vector3 targetVelocity = new Vector2(horizontalInput * speed * Time.fixedDeltaTime, playerVelocity.y);
                _playerRb.velocity = Vector3.SmoothDamp(playerVelocity, targetVelocity, ref _velocity, smoothTime);
            }

            // Handle crouching for big players.
            if (Input.GetKey(KeyCode.DownArrow) && (CompareTag("BigPlayer") || CompareTag("UltimateBigPlayer")) &&
                !_isAboveSpecialPipe)
            {
                _playerAnim.SetBool(CrouchB, true);
                smallPlayerCollider.SetActive(true);
                bigPlayerCollider.SetActive(false);
            }
            else if (Input.GetKeyUp(KeyCode.DownArrow) &&
                     (CompareTag("BigPlayer") || CompareTag("UltimateBigPlayer")) && !_isAboveSpecialPipe)
            {
                _playerAnim.SetBool(CrouchB, false);
                smallPlayerCollider.SetActive(false);
                bigPlayerCollider.SetActive(true);
            }
        }

        /// <summary>
        /// Handles collision interactions with various objects in the game.
        /// </summary>
        /// <param name="other">The collision data.</param>
        private void OnCollisionEnter2D(Collision2D other)
        {
            // Handle collision with ground and similar objects.
            if (other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("Pipe") ||
                other.gameObject.CompareTag("Brick") || other.gameObject.CompareTag("Stone") ||
                other.gameObject.CompareTag("SpecialPipe"))
            {
                _isOnGround = true;
                _playerAnim.SetBool(IdleB, true);
                _playerAnim.SetBool(WalkB, true);
                _playerAnim.SetBool(RunB, true);
            }

            // Handle collision with PowerBrick.
            if (other.gameObject.CompareTag("PowerBrick"))
            {
                _isEatable = false;
            }

            // Handle interaction with the Pole.
            if (other.gameObject.CompareTag("Pole"))
            {
                _playerAudio.PlayOneShot(flagPoleSound);
                _flagPos = other.gameObject.transform.position.x;
                _isFinish = true;
                _playerRb.velocity = Vector2.zero;
                _playerRb.isKinematic = true;
                isWalkingToCastle = true;
                isStopTime = true;
                StartCoroutine(PlayStageClearSound());
            }

            // Handle reaching the Castle.
            if (other.gameObject.CompareTag("Castle"))
            {
                isInCastle = true;
                isWalkingToCastle = false;
                playerSprite.SetActive(false);
            }

            // Handle falling into the Death Abyss.
            if (other.gameObject.CompareTag("DeathAbyss"))
            {
                _playerAudio.PlayOneShot(dieSound);
                GameStatusController.Live -= 1;
                GameStatusController.IsBigPlayer = false;
                GameStatusController.IsFirePlayer = false;
                GameStatusController.PlayerTag = "Player";
                GameStatusController.IsDead = true;
                _playerRb.isKinematic = true;
                _playerRb.velocity = Vector2.zero;
            }

            // Handle interaction with 1Up Mushroom.
            if (other.gameObject.CompareTag("1UpMushroom") && _isEatable)
            {
                _playerAudio.PlayOneShot(oneUpSound);
                GameStatusController.Live += 1;
                _isEatable = false;
            }

            // Handle interaction with Big Mushroom.
            if (other.gameObject.CompareTag("BigMushroom") && _isEatable)
            {
                _playerAudio.PlayOneShot(turnBigSound);
                TurnIntoBigPlayer();
                _isEatable = false;
            }

            // Handle interaction with Ultimate Star.
            if (other.gameObject.CompareTag("UltimateStar") && _isEatable)
            {
                _playerAudio.PlayOneShot(turnBigSound);
                if (CompareTag("Player"))
                {
                    tag = "UltimatePlayer";
                }
                else
                {
                    tag = "UltimateBigPlayer";
                }

                isInvincible = true;
                _playerAnim.SetBool(UltimateB, isInvincible);
                _startInvincible = Time.time;
                _isEatable = false;
            }

            // Handle interaction with Fire Flower.
            if (other.gameObject.CompareTag("FireFlower") && (CompareTag("Player") || CompareTag("UltimatePlayer")) &&
                _isEatable)
            {
                _playerAudio.PlayOneShot(turnBigSound);
                TurnIntoBigPlayer();
                _isEatable = false;
            }

            if (other.gameObject.CompareTag("FireFlower") && (CompareTag("BigPlayer") || CompareTag("UltimateBigPlayer")) &&
                _isEatable)
            {
                _playerAudio.PlayOneShot(turnBigSound);
                GameStatusController.IsFirePlayer = true;
                _isEatable = false;
            }

            // Handle interaction with Special Pipe.
            if (other.gameObject.CompareTag("SpecialPipe"))
            {
                _isAboveSpecialPipe = true;
            }

            // Handle interaction with Koopa Shell.
            if (other.gameObject.CompareTag("KoopaShell"))
            {
                _playerAudio.PlayOneShot(kickSound);
            }
        }
        /// <summary>
        /// Handles logic when the player exits a collision with certain objects.
        /// </summary>
        /// <param name="other">The collision data.</param>
        private void OnCollisionExit2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("PowerBrick"))
            {
                _isEatable = true;
            }
        }

        /// <summary>
        /// Handles logic when the player triggers certain objects.
        /// </summary>
        /// <param name="other">The collider data.</param>
        private void OnTriggerEnter2D(Collider2D other)
        {
            // Handle interaction with the Princess.
            if (other.gameObject.CompareTag("Princess"))
            {
                slideDownSpeed = 0;
                GameStatusController.IsShowMessage = true;
                _playerAnim.SetFloat(SpeedF, 0);
            }
            // Handle interaction with the Axe.
            if (other.gameObject.CompareTag("Axe"))
            {
                _playerAudio.PlayOneShot(endGameSound);
                Destroy(other.gameObject);
                GameStatusController.IsBossBattle = false;
                GameStatusController.IsGameFinish = true;
                _playerAnim.SetFloat(SpeedF, 3f);
                _playerRb.velocity = Vector2.zero;
            }
            // Handle interaction with Coins.
            if (other.gameObject.CompareTag("Coin"))
            {
                _playerAudio.PlayOneShot(coinSound);
            }
        }

        private void TurnIntoBigPlayer()
        {
            if (CompareTag("Player"))
            {
                GameStatusController.PlayerTag = "BigPlayer";
                tag = GameStatusController.PlayerTag;
            }
            else
            {
                tag = "UltimateBigPlayer";
            }

            GameStatusController.IsBigPlayer = true;
            ChangeAnim();
        }

        private void Die()
        {
            _playerAnim.SetBool(DieB, isDead);
            GameStatusController.IsDead = true;
            StartCoroutine(DieAnim());
            StartCoroutine(LoadingScene());
        }

        private void GetPlayerSpeed()
        {
            _playerAnim.SetFloat(SpeedF, Mathf.Abs(_playerRb.velocity.x));
        }

        public void ChangeAnim()
        {
            bigPlayer.SetActive(GameStatusController.IsBigPlayer);
            bigPlayerCollider.SetActive(GameStatusController.IsBigPlayer);
            smallPlayer.SetActive(!GameStatusController.IsBigPlayer);
            smallPlayerCollider.SetActive(!GameStatusController.IsBigPlayer);
        }

        private void DenyMidAirJump()
        {
            if (_playerRb.velocity.y > 0 || _playerRb.velocity.y < 0)
            {
                _isOnGround = false;
                _playerAnim.SetBool(IdleB, false);
                _playerAnim.SetBool(WalkB, false);
                _playerAnim.SetBool(RunB, false);
            }
            else
            {
                _isOnGround = true;
                _playerAnim.SetBool(IdleB, true);
                _playerAnim.SetBool(WalkB, true);
                _playerAnim.SetBool(RunB, true);
            }
        }
        /// <summary>
        /// Coroutine to delay the player's ability to eat power-ups.
        /// </summary>
        private IEnumerator SetBoolEatable()
        {
            yield return new WaitForSeconds(0.5f);
            _isEatable = true;
        }

        /// <summary>
        /// Coroutine that handles the player's interaction after reaching the flag pole.
        /// </summary>
        private IEnumerator HugPole()
        {
            yield return new WaitForSeconds(1.5f);
            _isNotHugPole = true;
        }

        /// <summary>
        /// Coroutine for playing the player's death animation.
        /// </summary>
        private IEnumerator DieAnim()
        {
            yield return new WaitForSeconds(1);
            playerCol.SetActive(false);
            _playerRb.isKinematic = false;
        }

        /// <summary>
        /// Coroutine for loading a new scene after a delay.
        /// </summary>
        private static IEnumerator LoadingScene()
        {
            yield return new WaitForSeconds(3.5f);
            SceneManager.LoadScene(1);
        }

        /// <summary>
        /// Coroutine for handling the logic after clearing a stage.
        /// </summary>
        private IEnumerator PlayStageClearSound()
        {
            yield return new WaitForSeconds(1.5f);
            GameStatusController.IsStageClear = true;
        }

        /// <summary>
        /// Coroutine for making the player vulnerable again after being invulnerable.
        /// </summary>
        private IEnumerator BeVulnerable()
        {
            yield return new WaitForSeconds(2);
            _playerAnim.SetBool(VulnerableB, true);
            Physics2D.IgnoreLayerCollision(8, 9, false);
            isInvulnerable = false;
        }

        /// <summary>
        /// Coroutine for returning the player to a normal state after invincibility.
        /// </summary>
        private IEnumerator BeNormal()
        {
            yield return new WaitForSeconds(2);
            tag = GameStatusController.PlayerTag;
            isInvincible = false;
            _playerAnim.SetBool(UltimateB, isInvincible);
            Physics2D.IgnoreLayerCollision(8, 9, false);
        }

        /// <summary>
        /// Coroutine for handling the player's interaction with special pipes.
        /// </summary>
        private IEnumerator StopGoingDownPipe()
        {
            yield return new WaitForSeconds(1.5f);
            _isGoingDownPipeAble = false;
            SceneManager.LoadScene(GameStatusController.CurrentLevel);
            GameStatusController.CurrentLevel += 1;
        }
    }
}
