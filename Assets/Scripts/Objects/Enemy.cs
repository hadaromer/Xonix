using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    private GameObject _enemy; // Reference to the enemy game object
    private Vector3 _direction; // Current movement direction
    private float _stepTimer = 0; // Timer to control movement steps
    [SerializeField] private float _stepTimeout = 0.025f; // Time between movement steps
    private bool _isLandEnemy; // Flag to indicate if the enemy is a land enemy

    // Set whether the enemy is a land enemy
    public void SetLandEnemy(bool isLandEnemy)
    {
        _isLandEnemy = isLandEnemy;
    }
    
    // Set the timeout between movement steps
    public void SetStepTimeout(float timeout)
    {
        _stepTimeout = timeout;
    }

    // Start is called before the first frame update
    void Start()
    {
        _enemy = gameObject;

        // Initialize a random direction
        int x = Random.Range(0, 2) == 0 ? -1 : 1;
        int y = Random.Range(0, 2) == 0 ? -1 : 1;
        _direction = new Vector3(x, y);
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.IsGameRunning || !GameManager.Instance.IsGameStarted) return;
        _stepTimer += Time.deltaTime;

        if (_stepTimer >= _stepTimeout)
        {
            var oldEnemyPosition = _enemy.transform.position;
            var newEnemyPosition = oldEnemyPosition + _direction;

            // Check if the new position is a valid move (i.e., water)
            if (GameManager.Instance.IsThePositionIsValid(newEnemyPosition) &&
                (_isLandEnemy ^ GameManager.Instance.IsThereWaterInPosition(newEnemyPosition)))
            {
                // Move the enemy to the new position
                _enemy.transform.position = newEnemyPosition;
            }
            else
            {
                // If the enemy hits a wall, try to find a new direction
                ChooseNewDirection(oldEnemyPosition);
            }

            _stepTimer = 0;
        }
    }

    // Choose a new direction for the enemy when it hits a wall
    void ChooseNewDirection(Vector3 currentPosition)
    {
        bool xChanged = false, yChanged = false;
        bool landInYDirection = !GameManager.Instance.IsThereWaterInPosition(currentPosition + new Vector3(0, _direction.y));
        bool landInXDirection = !GameManager.Instance.IsThereWaterInPosition(currentPosition + new Vector3(_direction.x, 0));
        bool landInDirection = !GameManager.Instance.IsThereWaterInPosition(currentPosition + _direction);
        bool invalidYDirection = !GameManager.Instance.IsThePositionIsValid(currentPosition + new Vector3(0, _direction.y));
        bool invalidXDirection = !GameManager.Instance.IsThePositionIsValid(currentPosition + new Vector3(_direction.x, 0));

        if (invalidXDirection || _isLandEnemy ^ landInXDirection)
        {
            _direction.x = -_direction.x;
            xChanged = true;
        }

        if (invalidYDirection || _isLandEnemy ^ landInYDirection)
        {
            _direction.y = -_direction.y;
            yChanged = true;
        }

        if (!xChanged && !yChanged && (_isLandEnemy ^ landInDirection))
        {
            _direction.x = -_direction.x;
            _direction.y = -_direction.y;
        }
    }
    
    // Handle collision with other objects
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!GameManager.Instance.IsGameRunning) return;
        if (other.CompareTag("Xonix") && _isLandEnemy ^ !PlayerController.Instance._isOnLand ||
            !_isLandEnemy && other.CompareTag("Line"))
        {
            EnemyManager.Instance.BlinkPosition(this);
            GameManager.Instance.GameOver(); 
        } 
    }
}