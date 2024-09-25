using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Animator _animator; // Reference to the Animator component
    
    private static readonly int Run = Animator.StringToHash("Run"); // Hash for the "Run" parameter in the Animator

    // Update is called once per frame
    void Update()
    {
        // Set the "Run" parameter in the Animator based on the player's running state
        _animator.SetBool(Run, PlayerController.Instance._isRunning);
    }
    
    // Handle collision with other objects
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!GameManager.Instance.IsGameRunning) return;

        bool powerUpCollected = false;

        // Check if the player collided with a "SlowEnemies" power-up
        if (other.CompareTag("SlowEnemies"))
        {
            powerUpCollected = true;
            EnemyManager.Instance.SlowEnemies(); 
        }

        // Check if the player collided with a "Life" power-up
        if (other.CompareTag("Life"))
        {
            powerUpCollected = true;
            GameManager.Instance.AddLife(); 
        }
        // Check if the player collided with a "SpeedUp" power-up
        if(other.CompareTag("SpeedUp"))
        {
            powerUpCollected = true;
            PlayerController.Instance.SpeedUp();
        }
        // Check if the player collided with a "AddTime" power-up
        if(other.CompareTag("AddTime"))
        {
            powerUpCollected = true;
            GameManager.Instance.AddTime();
        }

        // If a power-up was collected, release it
        if (powerUpCollected)
        {
            PowerUpManager.Instance.ReleasePowerUp(other.GetComponent<PowerUp>());
        }
    }
}