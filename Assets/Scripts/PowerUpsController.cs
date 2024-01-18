using System;
using System.Collections;
using UnityEngine;
using AdditionalScripts;
//after changing

public class PowerUpsController : MonoBehaviour
{
    public int speedRight;           
    public int speedUp;              
    public bool isMoving;            
    public bool isTouchByPlayer;     
    private bool _isEatable;         
    private float _firstYPos;        

    private AudioSource _powerAudio; 
    public AudioClip appearSound;    

    void Awake()
    {
        InitializePowerUp();
    }

    void Update()
    {
        HandlePowerUpMovement();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        HandleCollision(other.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleTriggerEnter(other.gameObject);
    }

    private void InitializePowerUp()
    {
        _powerAudio = GetComponent<AudioSource>();
        Physics2D.IgnoreLayerCollision(9, 10, true);
        _firstYPos = transform.position.y;
    }

    private void HandlePowerUpMovement()
    {
        if (isTouchByPlayer && !CompareTag("Coin"))
        {
            MovePowerUp();
            EnableMotionForCertainPowerUps();
        }

        if (isMoving && (CompareTag("BigMushroom") || CompareTag("1UpMushroom")))
        {
            isTouchByPlayer = false;
            transform.Translate(speedRight * Time.deltaTime * Vector2.right);
        }
    }

    private void MovePowerUp()
    {
        if (transform.position.y < _firstYPos + 1)
        {
            transform.Translate(speedUp * Time.deltaTime * Vector2.up);
        }
    }

    private void EnableMotionForCertainPowerUps()
    {
        if (transform.position.y >= _firstYPos + 1 && (CompareTag("BigMushroom") || CompareTag("1UpMushroom")))
        {
            isMoving = true;
            GetComponent<Rigidbody2D>().isKinematic = false;
        }
    }

    private void HandleCollision(GameObject other)
    {
        InteractionWithPlayer(other);

        if (other.CompareTag("Stone") || other.CompareTag("Pipe") || other.CompareTag("Untagged"))
        {
            speedRight = -speedRight;
        }
    }

    private void HandleTriggerEnter(GameObject other)
    {
        if (IsCoin(other))
        {
            UpdateCoinCollection();
            Destroy(gameObject);
        }
        else
        {
            InteractionWithPlayer(other);
        }
    }

    private bool IsCoin(GameObject other)
    {
        return CompareTag("Coin") && (other.CompareTag("Player") || other.CompareTag("BigPlayer") ||
                                      other.CompareTag("UltimatePlayer") || other.CompareTag("UltimateBigPlayer"));
    }

    private void UpdateCoinCollection()
    {
        ToolController.CollectedCoin += 1;
        ToolController.Score += 200;
        ToolController.IsEnemyDieOrCoinEat = true;
    }

    private IEnumerator SetBoolEatable()
    {
        yield return new WaitForSeconds(1);
        _isEatable = true;
    }

    void InteractionWithPlayer(GameObject other)
    {
        if (!CompareTag("Coin") && (other.CompareTag("Player") || other.CompareTag("UltimatePlayer") ||
                                    other.CompareTag("BigPlayer") || other.CompareTag("UltimateBigPlayer")))
        {
            HandlePowerUpInteraction();
        }

        if (_isEatable && (other.CompareTag("Player") || other.CompareTag("BigPlayer") ||
                           other.CompareTag("UltimatePlayer") || other.CompareTag("UltimateBigPlayer")))
        {
            ConsumePowerUp();
        }
    }

    private void HandlePowerUpInteraction()
    {
        _powerAudio.PlayOneShot(appearSound);
        isTouchByPlayer = true;
        StartCoroutine(SetBoolEatable());
    }

    private void ConsumePowerUp()
    {
        ToolController.Score += 1000;
        ToolController.IsPowerUpEat = true;
        _isEatable = false;
        Destroy(gameObject);
    }
}
