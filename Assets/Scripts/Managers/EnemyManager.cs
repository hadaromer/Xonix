using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
    [SerializeField] private Enemy _enemyPrefab; // Prefab for spawning enemies
    [SerializeField] private Vector3[] _enemyPositions; // Array of predefined enemy positions
    private List<Enemy> _enemies; // List to hold spawned enemies
    private Enemy _landEnemy; // Reference to the land enemy
    private Vector3 _landEnemyStartingVector; // Starting position for the land enemy

    // Public properties to access private fields
    public List<Enemy> Enemies => _enemies;
    public Enemy LandEnemy => _landEnemy;
    public Vector3 LandEnemyStartingVector => _landEnemyStartingVector;

    // Method to spawn enemies
    public void SpawnEnemies()
    {
        _enemies = new List<Enemy>();
        for (int i = 0; i < _enemyPositions.Length; i++)
        {
            _enemies.Add(SpawnEnemy(i));
        }
        _landEnemyStartingVector = new Vector3(0, GameManager.Instance.Height / 2);
        _landEnemy = Instantiate(_enemyPrefab, _landEnemyStartingVector, Quaternion.identity);
        _landEnemy.SetLandEnemy(true);
    }

    // Method to spawn a single enemy at a valid position
    private Enemy SpawnEnemy(int index)
    {
        var nextEnemyPosition = _enemyPositions[index];
        return Instantiate(_enemyPrefab, nextEnemyPosition, Quaternion.identity);
    }

    // Method to slow down enemies for a duration
    public void SlowEnemies(float slowDuration = 5f, float slowStepTimeout = 0.06f, float normalStepTimeout = 0.04f)
    {
        StartCoroutine(SlowEnemiesRoutine(slowDuration, slowStepTimeout, normalStepTimeout));
    }

    // Coroutine to handle slowing down enemies
    private IEnumerator SlowEnemiesRoutine(float slowDuration, float slowStepTimeout, float normalStepTimeout)
    {
        foreach (var enemy in _enemies)
        {
            enemy.SetStepTimeout(slowStepTimeout);
        }
        _landEnemy.SetStepTimeout(slowStepTimeout);
        yield return new WaitForSeconds(slowDuration);
        foreach (var enemy in _enemies)
        {
            enemy.SetStepTimeout(normalStepTimeout);
        }
        _landEnemy.SetStepTimeout(normalStepTimeout);
    }

    // Method to make an enemy blink and move to a new position
    public void BlinkPosition(Enemy enemy)
    {
        enemy.transform.position = new Vector3(enemy.transform.position.x, enemy.transform.position.y, -1);
        SpriteRenderer enemySpriteRenderer = enemy.GetComponent<SpriteRenderer>();
        StartCoroutine(GameManager.Instance.Blink(3.0f, enemySpriteRenderer));
    }
}