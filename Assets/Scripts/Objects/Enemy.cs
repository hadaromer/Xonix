using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    private GameObject _enemy; // Reference to the enemy game object
    private Vector3 _direction; // Current movement direction
    private float _stepTimer = 0; // Timer to control movement steps
    private float _stepTimeout = 0.025f; // Time between movement steps
    private float _slowStepTimeout = 0.06f; // Time between movement steps when the enemy is slowed down
    private bool _isLandEnemy; // Flag to indicate if the enemy is a land enemy
    private bool _isFrozen; // Flag to indicate if the enemy is frozen
    private bool _isDestroyer; // Flag to indicate if the enemy is a destroyer
    private int _damage; // Flag to check if the enemy can damage the land
    private int _maxRetries = 5;

    private Vector3 _startingPosition; // Starting position of the enemy
    
    public Vector3 StartingPosition => _startingPosition;

    public bool IsLandEnemy => _isLandEnemy;
    
    // Set the frozen state of the enemy
    public void SetIsFrozen(bool isFrozen)
    {
        _isFrozen = isFrozen;
    }
    
    // Slow down the enemy
    public void SlowDown()
    {
        SetStepTimeout(_slowStepTimeout);
    }
    
    // Normalize the enemy speed
    public void NormalizeSpeed()
    {
        SetStepTimeout(_stepTimeout);
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
        if (_isFrozen || !GameManager.Instance.IsGameRunning || !GameManager.Instance.IsGameStarted) return;
        _stepTimer += Time.deltaTime;

        if (_stepTimer >= _stepTimeout)
        {
            if (_maxRetries == 0)
            {
                _enemy.transform.position = _startingPosition;
                _maxRetries = 5;
            }
            var oldEnemyPosition = _enemy.transform.position;
            var newEnemyPosition = oldEnemyPosition + _direction;
            bool validPosition = GameManager.Instance.IsThePositionIsValid(newEnemyPosition);
            // Check if the new position is a valid move (i.e., water)
            if (validPosition &&
                (_isLandEnemy ^ GameManager.Instance.IsThereWaterInPosition(newEnemyPosition)))
            {
                // Move the enemy to the new position
                _enemy.transform.position = newEnemyPosition;
                _maxRetries = 5;
            }
            else
            {
                _maxRetries--;
                // If the enemy hits a wall, try to find a new direction
                ChooseNewDirection(oldEnemyPosition);
                // If the enemy is a destroyer and not a land enemy, create water in the new position
                if (validPosition && _isDestroyer && !_isLandEnemy)
                {
                    GameManager.Instance.ChangePosition(newEnemyPosition,false,_damage);
                }
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
        if (!GameManager.Instance.IsGameRunning || _isFrozen || PlayerController.Instance.unTouchable) return;
        if (other.CompareTag("Xonix") && _isLandEnemy ^ !PlayerController.Instance.isOnLand ||
            !_isLandEnemy && other.CompareTag("Line"))
        {
            EnemyManager.Instance.BlinkPosition(this);
            GameManager.Instance.GameOver(); 
        } 
    }

    public void SetEnemyData(EnemyData enemyData)
    {
        _stepTimeout = enemyData.normalStepTime;
        _slowStepTimeout = enemyData.slowStepTime;
        _isLandEnemy = enemyData.isLandEnemy;
        _isDestroyer = enemyData.isDestroyer;
        _damage = enemyData.damage;
        _startingPosition = enemyData.startingPosition;
    }
}