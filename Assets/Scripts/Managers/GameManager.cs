using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : Singleton<GameManager>
{
    [SerializeField] private int _width = 100; // Width of the game area
    public int Width => _width;
    [SerializeField] private int _height = 60; // Height of the game area
    public int Height => _height;
    [SerializeField] private int _landWidth = 4; // Width of the land area
    [SerializeField] private GameObject _wallPrefab; // Prefab for the wall
    [SerializeField] private GameObject _waterPrefab; // Prefab for the water
    [SerializeField] private GameObject _livePrefab; // Prefab for the lives
    [SerializeField] private GameObject _bombPrefab; // Prefab for the bomb
    [SerializeField] private Sprite _background; // Background sprite
    [SerializeField] private GameObject _explodeGameObject; // Explosion effect
    
    private int _lives = 3; // Number of lives
    private int _bombsCount = 0; // Number of bombs
    private GameObject _bomb;
    private bool _isGameRunning = true; // Is the game currently running
    public bool IsGameRunning => _isGameRunning;
    private bool _isGameStarted; // Has the game started
    public bool IsGameStarted => _isGameStarted;
    private GameObject[,] _waters; // Array to hold water objects
    private bool[,] _watersState; // Array to hold the state of water objects
    private GameObject _food; // Food object
    private GameObject[] _livesPrefabs; // Array to hold life objects
    private int _currentTime = 90; // Current time
    private int _requiredPercentage = 100; // Required percentage to win
    private GameObject _backgroundObject; // Background object
    private string _failSound = "Fail"; // Sound for failure
    private string _winSound = "Win"; // Sound for winning
    private string _loseSound = "Lose"; // Sound for losing
    private string _gameMusic = "GameMusic"; // Background game music
    private string _timeWarning = "TimeWarning"; // Sound for time warning
    private string _lastSecond = "LastSecond"; // Sound for the last second
    private string _destroy = "Destroy"; // Sound for destroying land
    private string _explode = "Explode"; // Sound for explosion
    private string _emptyBomb = "EmptyBomb"; // Sound for empty bomb
    private Color _waterOriginalColor; // Original color of water
    private Color _landColor = new Color(1f, 1f, 1f, 0f); // Color of the land

    private int RightBorder => (_width / 2) + 1; // Right border of the game area
    private int LeftBorder => -1 * (_width / 2 + 1); // Left border of the game area
    private int TopBorder => (_height / 2) + 1; // Top border of the game area
    private int BottomBorder => -1 * (_height / 2 + 1); // Bottom border of the game area
    public int RequiredPercentage => _requiredPercentage;

    private void Awake()
    {
        ScoreManager.Instance.UpdateHighScores();
        AudioManager.Instance.PlaySoundLoop(_gameMusic);
    }

    public void ResetHighScores()
    {
        PlayerPrefs.DeleteAll();
        ScoreManager.Instance.UpdateHighScores();
    }

    public void StartGameSession()
    {
        CreateWaters();
        UIManager.Instance.HideScoreBoard(ScoreManager.Instance.NamesText, ScoreManager.Instance.ScoresText);
        StartCoroutine(DecreaseTime());
        SpawnLives();
        SpawnBomb();
        LevelManager.Instance.LoadLevel(0);
    }

    private void CreateBackground()
    {
        if (_backgroundObject == null)
        {
            _backgroundObject = new GameObject("NewSprite");
            _backgroundObject.AddComponent<SpriteRenderer>();
        }
        var spriteRenderer = _backgroundObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = _background;
        _backgroundObject.transform.position = new Vector3(0, 0, -1);
        _backgroundObject.transform.localScale = new Vector3(5, 4.5f);
        spriteRenderer.sortingLayerName = "Default";
        spriteRenderer.sortingOrder = -1;
    }

    private void SpawnLives()
    {
        _livesPrefabs = new GameObject[3];
        for (int i = 0; i < _livesPrefabs.Length; i++)
        {
            _livesPrefabs[i] = Instantiate(_livePrefab);
            _livesPrefabs[i].transform.position = new Vector3(RightBorder + 8, BottomBorder + 2 + i * 5);
        }
    }

    private void SpawnBomb()
    {
        _bomb = Instantiate(_bombPrefab);
        _bomb.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.3f);
        _bomb.transform.position = new Vector3(LeftBorder - 8, BottomBorder + 2);
        
    }

    private IEnumerator DecreaseTime()
    {
        UIManager.Instance.UpdateTimeText(_currentTime);
        while (_currentTime > 0)
        {
            if (_currentTime < 5 && _currentTime > 1 && _isGameRunning)
            {
                AudioManager.Instance.PlaySound(_timeWarning);
            }

            if (_currentTime == 1 && _isGameRunning)
            {
                AudioManager.Instance.PlaySound(_lastSecond);
            }

            yield return new WaitForSeconds(1);
            if (_isGameRunning)
            {
                _currentTime--;
                UIManager.Instance.UpdateTimeText(_currentTime);
            }
        }

        _lives = 0;
        GameOverByTime();
    }

    private void Update()
    {
        if (_isGameRunning && ScoreManager.Instance.CurrentPercentage >= _requiredPercentage)
        {
            _isGameRunning = false;
            ScoreManager.Instance.CalculateFinalScore(_currentTime, _lives);
            ScoreManager.Instance.ResetPercentage();
            AudioManager.Instance.PlaySound(_winSound);
            if (LevelManager.Instance.IsLastLevel)
            {
                UIManager.Instance.ShowWinImage();
            }
            else
            {
                LevelManager.Instance.LoadNextLevel();
            }
        }

        if (Input.GetKeyDown(KeyCode.Return) && !string.IsNullOrEmpty(UIManager.Instance.PlayerName.text))
        {
            ScoreManager.Instance.SetHighScore();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
        }
    }

    public void GameOver()
    {
        AudioManager.Instance.PlaySound(_failSound);
        _isGameRunning = false;
        _lives--;
        SpriteRenderer heartSpriteRenderer = _livesPrefabs[_lives].GetComponent<SpriteRenderer>();
        StartCoroutine(Blink(3.0f, heartSpriteRenderer, false));
    }

    
    private void GameOverByTime()
    {
        _isGameRunning = false;
        AudioManager.Instance.PlaySound(_loseSound);
        UIManager.Instance.ShowLoseImage();
    }
    
    public void ContinueGame()
    {
        if (!_isGameRunning && _lives == 0)
        {
            AudioManager.Instance.PlaySound(_loseSound);
            UIManager.Instance.ShowLoseImage();
            return;
        }

        if (!_isGameRunning)
        {
            _livesPrefabs[_lives].GetComponent<SpriteRenderer>().enabled = false;
            PlayerController.Instance.StartGameSession();
            EnemyManager.Instance.ResetLandEnemies();
            _isGameRunning = true;
        }
    }

    // Add a life to the player
    public void AddLife()
    {
        if (_lives < 3)
        {
            _livesPrefabs[_lives].GetComponent<SpriteRenderer>().enabled = true;
            _lives++;
        }
    }

    // Create the water objects
    private void CreateWaters()
    {
        // save original color of water
        _waterOriginalColor = _waterPrefab.GetComponent<SpriteRenderer>().color;
        _waters = new GameObject[_height + 1, _width + 1];
        _watersState = new bool[_height + 1, _width + 1];

        for (int i = -_height / 2 + _landWidth; i <= _height / 2 - _landWidth; i++)
        for (int j = -_width / 2 + _landWidth; j <= _width / 2 - _landWidth; j++)
        {
            _waters[i + _height / 2, j + _width / 2] = CreateWater(i, j);
            _watersState[i + _height / 2, j + _width / 2] = true;
        }
    }

    // Create water in the given position
    private GameObject CreateWater(int x, int y)
    {
        var water = Instantiate(_waterPrefab);
        water.transform.position = new Vector3(y, x, 1);
        return water;
    }

    // Check if there is water in the given position
    public bool IsThereWaterInPosition(Vector3 position)
    {
        int i = (int)position.y + _height / 2;
        int j = (int)position.x + _width / 2;
        if (i > _height || j > _width || i < 0 || j < 0) return false;
        return _watersState[i, j];
    }

    // Check if the given position is valid
    public bool IsThePositionIsValid(Vector3 position)
    {
        return Mathf.Abs(position.x) < _width / 2 && Mathf.Abs(position.y) < _height / 2;
    }

    // Change the position of the water and land based on the given position
    public void ChangePosition(Vector3 position, bool isWater, int damage = 0)
    {
        int i = (int)position.y + _height / 2;
        int j = (int)position.x + _width / 2;
        if(isWater)
        {
            MakeLandInPosition(i, j);
        }
        else
        {
            // make water in the postion and in all positions that are connected to it with radius of 2
            bool landDestroyed = false;
            for (int a = i - damage; a <= i + damage; a++)
            for (int b = j - damage; b <= j + damage; b++)
            {
                if (a >= _landWidth && a < _height + 1 - _landWidth 
                                    && b >= _landWidth && b < _width + 1 - _landWidth)
                {
                    if (MakeWaterInPosition(a, b))
                        landDestroyed = true;
                }
            }

            if (landDestroyed)
            {
                AudioManager.Instance.PlaySound(_destroy);
                UpdateScore();
            }
        }
    }

    // Make water in the given position
    private bool MakeWaterInPosition(int i, int j)
    {
        if (_watersState[i, j]) return false;
        _watersState[i, j] = true;
        StartCoroutine(ChangeColorWithWaveEffect(_waters[i, j].GetComponent<SpriteRenderer>(), _waterOriginalColor));
        return true;
    }

    // Make land in the given position
    public void MakeLandInPosition(int i, int j)
    {
        if(!_watersState[i, j]) return;
        _watersState[i, j] = false;
        _waters[i, j].GetComponent<SpriteRenderer>().color = _landColor;
    }
    
    // Coroutine to change the color of the water with a wave effect
    private IEnumerator ChangeColorWithWaveEffect(SpriteRenderer spriteRenderer, Color targetColor)
    {
        Color originalColor = spriteRenderer.color;
        float duration = 0.5f; // Duration of the effect
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            spriteRenderer.color = new Color(Random.value, Random.value, Random.value);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        spriteRenderer.color = targetColor;
    }

    // Set the enemies' positions in the status array
    private int[,] SetEnemiesPosition(int[,] status)
    {
        foreach (var enemy in EnemyManager.Instance.Enemies)
        {
            if(enemy.IsLandEnemy) continue;
            var position = enemy.transform.position;
            int i = (int)position.y + _height / 2;
            int j = (int)position.x + _width / 2;
            if (i > _height || j > _width || i < 0 || j < 0) continue;
            status[i, j] = 2;
            status = FloodFill(position, status, 2);
        }

        return status;
    }

    // Flood fill algorithm to mark areas in the grid
    public int[,] FloodFill(Vector3 position, int[,] status, int mode)
    {
        Stack<Vector3> points = new Stack<Vector3>();
        int i, j;
        bool first = true;
        points.Push(position);

        while (points.Count > 0)
        {
            var currentPoint = points.Pop();
            i = (int)currentPoint.y + _height / 2;
            j = (int)currentPoint.x + _width / 2;

            // Check if the current point is out of bounds
            if (i > _height || j > _width || i < 0 || j < 0) continue;

            // Skip already processed points
            if (!first && status[i, j] != 0) continue;

            // Process the current point if it's the first point or if there's water
            if (first || IsThereWaterInPosition(currentPoint))
            {
                status[i, j] = mode;
                points.Push(new Vector3(currentPoint.x - 1, currentPoint.y));
                points.Push(new Vector3(currentPoint.x + 1, currentPoint.y));
                points.Push(new Vector3(currentPoint.x, currentPoint.y - 1));
                points.Push(new Vector3(currentPoint.x, currentPoint.y + 1));
            }

            first = false;
        }

        return status;
    }

    // Paint the area starting from the given position
    public void PaintArea(Vector3 position)
    {
        int i = (int)position.y + _height / 2;
        int j = (int)position.x + _width / 2;

        // Check if the position is out of bounds
        if (i > _height || j > _width || i < 0 || j < 0) return;

        int[,] status = new int[_height + 1, _width + 1];
        status[i, j] = 1;

        // Set enemy positions in the status array
        status = SetEnemiesPosition(status);

        // Perform flood fill to mark the area
        FloodFill(position, status, 1);

        // Convert marked areas to land
        for (int a = 0; a < _height + 1; a++)
        {
            for (int b = 0; b < _width + 1; b++)
            {
                if (status[a, b] == 1)
                {
                    MakeLandInPosition(a, b);
                }
            }
        }

        // Update the score and UI
        UpdateScore();
    }

    // Update the score based on the painted area
    private void UpdateScore()
    {
        int total = 0, painted = 0;

        for (int a = _landWidth; a < _height + 1 - _landWidth; a++)
        {
            for (int b = _landWidth; b < _width + 1 - _landWidth; b++)
            {
                if (!_watersState[a, b])
                {
                    painted++;
                }

                total++;
            }
        }

        int currentPercentage = (painted * 100 * 4 * 100) / (total * 3 *_requiredPercentage);
        ScoreManager.Instance.UpdateScore(currentPercentage);
    }

    // Coroutine to make an object blink
    public IEnumerator Blink(float duration, SpriteRenderer spriteRenderer, bool visibleAtEnd = true)
    {
        float endTime = Time.time + duration;

        while (Time.time < endTime)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(0.2f);
        }

        spriteRenderer.enabled = visibleAtEnd;
        ContinueGame();
    }
    
    public void AddTime()
    {
        _currentTime += 10;
        UIManager.Instance.UpdateTimeText(_currentTime);
    }

    public void UpdateLevelData(LevelData currentLevelData)
    {
        // change the background
        _background = currentLevelData.Background;
        CreateBackground();
        
        // reset the game
        _currentTime = 90;
        _requiredPercentage = currentLevelData.RequiredPercentage;
        UIManager.Instance.UpdateTimeText(_currentTime);
        ScoreManager.Instance.ResetPercentage();
        
        // reset the waters
        if (_waters != null)
        {
            for (int i = -_height / 2 + _landWidth; i <= _height / 2 - _landWidth; i++)
            for (int j = -_width / 2 + _landWidth; j <= _width / 2 - _landWidth; j++)
            {
                    _watersState[i + _height / 2, j + _width / 2] = true;
                    _waters[i + _height / 2, j + _width / 2].GetComponent<SpriteRenderer>().color = _waterOriginalColor; 
            }
        }

        // reset the enemies
        EnemyManager.Instance.UpdateLevelData(currentLevelData);
        
        // reset the power-ups
        PowerUpManager.Instance.UpdateLevelData(currentLevelData);
        PowerUpManager.Instance.StartSpwanPowerUps();
        
        // reset the player
        PlayerController.Instance.StartGameSession();
        
        // reset the lives
        if (_livesPrefabs != null)
        {
            for (int i = 0; i < _livesPrefabs.Length; i++)
            {
                _livesPrefabs[i].GetComponent<SpriteRenderer>().enabled = true;
            }
        }
        _lives = 3;
        
        // reset the bomb
        _bombsCount = 0;
        _bomb.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.3f);
        
        // show the level message
        StartCoroutine(ShowLevelMessage(currentLevelData.LevelMessage,currentLevelData.LevelNumber));
    }
    
    private IEnumerator ShowLevelMessage(string message, int _levelNumber)    
    {
        yield return UIManager.Instance.TypeWriterEffect(message, _levelNumber);
        _isGameStarted = true;
        _isGameRunning = true;
    }

    // Collect the bomb
    public void Bomb()
    {
        if (_bombsCount > 0) return;
        _bombsCount++;
        _bomb.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
    }

    // Explode the bomb
    public void Explode(Vector3 position)
    {
        if (_bombsCount > 0)
        {
            _explodeGameObject.transform.position = position;
            StartCoroutine(ExplodeCoroutine(position));
            _bombsCount--;
            _bomb.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.3f);
        }
        else
        {
            AudioManager.Instance.PlaySound(_emptyBomb);
        }
    }

    // Coroutine to show the explosion effect
    private IEnumerator ExplodeCoroutine(Vector3 position)
    {
        _explodeGameObject.SetActive(true);
        AudioManager.Instance.PlaySound(_explode);
        foreach (var enemy in EnemyManager.Instance.Enemies)
        {
            var enemyPosition = enemy.transform.position;
            if(enemyPosition.x < position.x + 10 && enemyPosition.x > position.x - 10
               && enemyPosition.y < position.y + 10 && enemyPosition.y > position.y - 10)
            {
                EnemyManager.Instance.FreezeEnemy(enemy);
            }
        }
        
        yield return new WaitForSeconds(0.5f);
        
        _explodeGameObject.SetActive(false);
    }
}