using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{
    [SerializeField] private GameObject _playerPrefab; // Prefab for the player
    [SerializeField] private Line _movementLinePrefab; // Prefab for the movement line
    [SerializeField] private float _stepTimeout = 0.025f; // Time between movement steps

    private float _stepTimer = 0; // Timer to control movement steps
    private Vector3 _recentPlayerMovement = Vector3.zero; // Recent movement direction
    private GameObject _xonix; // Reference to the player game object
    private BestObjectPool<Line> _linePool; // Object pool for movement lines
    private List<Line> _movementLine = new(); // List to hold movement lines
    private Dictionary<Vector3, bool> _xonixPositions = new(); // Dictionary to track player positions
    public bool _isRunning { get; private set; } // Flag to indicate if the player is running
    public bool _isOnLand { get; private set; } // Flag to indicate if the player is on land
    private Vector3 startPosition; // Starting position of the player

    private void Awake()
    {
        startPosition = new Vector3(0, -1 * (GameManager.Instance.Height / 2) + 1); // Initialize starting position
        _linePool = new BestObjectPool<Line>(_movementLinePrefab, 100); // Initialize the line pool
        _xonix = Instantiate(_playerPrefab, startPosition, Quaternion.identity); // Instantiate the player
    }

    // Method to start a new game session
    public void StartGameSession()
    {
        _xonix.transform.position = startPosition;
        _xonixPositions.Add(_xonix.transform.position, true);
        _recentPlayerMovement = Vector3.zero;
        _isRunning = false;
        _isOnLand = true;
        foreach (var line in _movementLine)
        {
            _linePool.Release(line);
        }

        _movementLine.Clear();
        _xonixPositions.Clear();
    }

    void Update()
    {
        if (!IsGameActive()) return;

        HandlePlayerInput();

        _stepTimer += Time.deltaTime;
        if (_stepTimer >= _stepTimeout)
        {
            if (_recentPlayerMovement == Vector3.zero) return;

            var currentPosition = _xonix.transform.position;
            var newPosition = _xonix.transform.position + _recentPlayerMovement;

            if (GameManager.Instance.IsThereWaterInPosition(currentPosition))
            {
                HandleWaterPosition(currentPosition);
            }
            else
            {
                HandleLandPosition(currentPosition);
            }

            if (!GameManager.Instance.IsThePositionIsValid(newPosition))
            {
                _isRunning = false;
                return;
            }

            UpdatePlayerPosition(newPosition);
            _stepTimer = 0;
        }
    }

    // Check if the game is active
    private bool IsGameActive()
    {
        return GameManager.Instance != null && GameManager.Instance.IsGameRunning && GameManager.Instance.IsGameStarted;
    }

    // Handle player input
    private void HandlePlayerInput()
    {
        var playerInput = GetInputOnUpdate();
        if (playerInput != Vector3.zero)
        {
            _recentPlayerMovement = playerInput;
        }
    }

    // Handle player position when on water
    private void HandleWaterPosition(Vector3 currentPosition)
    {
        bool added = _xonixPositions.TryAdd(currentPosition, true);
        if (added)
        {
            var newMovementLine = _linePool.Get();
            newMovementLine.transform.position = currentPosition;
            _movementLine.Add(newMovementLine);
        }
        else
        {
            GameManager.Instance.GameOver();
        }
    }

    // Handle player position when on land
    private void HandleLandPosition(Vector3 currentPosition)
    {
        foreach (var line in _movementLine)
        {
            GameManager.Instance.MakeLandInPosition(line.transform.position);
            _linePool.Release(line);
        }

        if (_movementLine.Count > 0)
        {
            GameManager.Instance.PaintArea(_movementLine.First().transform.position);
            GameManager.Instance.PaintArea(_movementLine.Last().transform.position);
        }

        _movementLine.Clear();
        _xonixPositions.Clear();
        _xonixPositions.Add(currentPosition, true);
    }

    // Update player position and handle sprite flipping
    private void UpdatePlayerPosition(Vector3 newPosition)
    {
        // Check walking direction and flip sprite
        if (_recentPlayerMovement.x > 0) // Moving right
        {
            _xonix.transform.localScale = new Vector3(1, 1, 1); // Flip to face right
        }
        else if (_recentPlayerMovement.x < 0) // Moving left
        {
            _xonix.transform.localScale = new Vector3(-1, 1, 1); // Flip to face left
        }

        _isRunning = true;
        _isOnLand = !GameManager.Instance.IsThereWaterInPosition(_xonix.transform.position);
        _xonix.transform.position = newPosition;
    }

    // Method to get player input for movement
    private Vector3 GetInputOnUpdate()
    {
        var movementDirection = new Vector3();
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            movementDirection = Vector3.right;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            movementDirection = Vector3.left;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            movementDirection = Vector3.up;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            movementDirection = Vector3.down;
        }

        return movementDirection;
    }

    public void SpeedUp()
    {
        StartCoroutine(SpeedUpCoroutine());
    }

    private IEnumerator SpeedUpCoroutine()
    {
        _stepTimeout = 0.01f;
        yield return new WaitForSeconds(5);
        _stepTimeout = 0.025f;
    }
}