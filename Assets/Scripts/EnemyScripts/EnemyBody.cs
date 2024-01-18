using System.Collections;
//using SystemScripts;
using PlayerScripts;
using UnityEngine;
using AdditionalScripts;

namespace EnemyScripts
{
    public class EnemyBody : MonoBehaviour
    {
        private EnemyController _enemyController;
        public GameObject enemy;
        private AudioSource _enemyAudio;
        public AudioClip hitPlayerSound;
        public AudioClip turnSmallPlayerSound;

        private BoxCollider2D _boxCollider2D;
        private Rigidbody2D _playerRigidbody2D;

        private void Awake()
        {
            _enemyAudio = GetComponent<AudioSource>();
            _boxCollider2D = GetComponent<BoxCollider2D>();

            if (enemy != null)
            {
                _enemyController = enemy.GetComponent<EnemyController>();
            }
        }

        private void Update()
        {
            if (_enemyController != null && _enemyController.isTouchByPlayer)
            {
                _boxCollider2D.offset = Vector2.zero;
                _boxCollider2D.size = new Vector2(1, 0.01f);
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                HandlePlayerCollision(other.gameObject);
            }
        }

        private void HandlePlayerCollision(GameObject playerGameObject)
        {
            PlayerController playerController = playerGameObject.GetComponent<PlayerController>();

            if (playerController == null) return;

            _playerRigidbody2D = playerController.GetComponent<Rigidbody2D>();

            if (!playerController.isInvulnerable)
            {
                HandleNonInvulnerablePlayer(playerController);
            }
            else
            {
                HandleInvulnerablePlayer(playerController);
            }
        }

        private void HandleNonInvulnerablePlayer(PlayerController playerController)
        {
            _enemyAudio.PlayOneShot(hitPlayerSound);
            ToolController.IsDead = true;
            ToolController.Live -= 1;
            _playerRigidbody2D.isKinematic = true;
            _playerRigidbody2D.velocity = Vector2.zero;
        }

        private void HandleInvulnerablePlayer(PlayerController playerController)
        {
            if (ToolController.IsBigPlayer)
            {
                _enemyAudio.PlayOneShot(turnSmallPlayerSound);
                ToolController.IsBigPlayer = false;
                ToolController.IsFirePlayer = false;
                ToolController.PlayerTag = "Player";
                playerController.gameObject.tag = ToolController.PlayerTag;
                playerController.ChangeAnim();
                playerController.isInvulnerable = true;
                // Uncomment the line below if you wish to use the Die coroutine
                // StartCoroutine(Die(playerController.gameObject));
            }
            else
            {
                Physics2D.IgnoreCollision(_boxCollider2D, playerController.GetComponent<Collider2D>());
            }
        }

        IEnumerator Die(GameObject playerGameObject)
        {
            yield return new WaitForSeconds(1);
            playerGameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 4000));
            playerGameObject.GetComponent<Rigidbody2D>().gravityScale = 25;
        }
    }
}
