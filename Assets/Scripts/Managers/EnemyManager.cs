using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
    private EnemyData[] _enemiesData; // Array of predefined enemy data
    private List<Enemy> _enemies; // List to hold spawned enemies
    private Enemy _landEnemy; // Reference to the land enemy
    private Vector3 _landEnemyStartingVector; // Starting position for the land enemy

    // Public properties to access private fields
    public List<Enemy> Enemies => _enemies;

    // Method to spawn enemies
    public void SpawnEnemies()
    {
        _enemies = new List<Enemy>();
        foreach (var enemyData in _enemiesData)
        {
            var enemy = SpawnEnemy(enemyData.startingPosition, enemyData.enemyPrefab);
            enemy.SetEnemyData(enemyData);
            _enemies.Add(enemy);
        }
    }

    // Method to spawn a single enemy at a valid position
    private Enemy SpawnEnemy(Vector3 startingPosition,Enemy _enemyPrefab)
    {
        return Instantiate(_enemyPrefab, startingPosition, Quaternion.identity);
    }
    
    // Method to freeze an enemy for a durtaion
    public void FreezeEnemy(Enemy enemy, float freezeDuration = 5f)
    {
        StartCoroutine(FreezeEnemyRoutine(enemy, freezeDuration));
    }
    
    // Coroutine to handle freezing an enemy
    private IEnumerator FreezeEnemyRoutine(Enemy enemy, float freezeDuration)
    {
        enemy.SetIsFrozen(true);
        enemy.GetComponent<SpriteRenderer>().color = Color.blue;
        yield return new WaitForSeconds(freezeDuration);
        if (enemy != null) // Check if the enemy is not destroyed
        {
            enemy.GetComponent<SpriteRenderer>().color = Color.white;
            enemy.SetIsFrozen(false);
        }
    }
    

    // Method to slow down enemies for a duration
    public void SlowEnemies(float slowDuration = 5f)
    {
        StartCoroutine(SlowEnemiesRoutine(slowDuration));
    }

    // Coroutine to handle slowing down enemies
    private IEnumerator SlowEnemiesRoutine(float slowDuration)
    {
        foreach (var enemy in _enemies)
        {
            enemy.SlowDown();
        }
        yield return new WaitForSeconds(slowDuration);
        foreach (var enemy in _enemies)
        {
            enemy.NormalizeSpeed();
        }
    }

    // Method to make an enemy blink and move to a new position
    public void BlinkPosition(Enemy enemy)
    {
        enemy.transform.position = new Vector3(enemy.transform.position.x, enemy.transform.position.y, -1);
        SpriteRenderer enemySpriteRenderer = enemy.GetComponent<SpriteRenderer>();
        StartCoroutine(GameManager.Instance.Blink(3.0f, enemySpriteRenderer));
    }
    
    // Method to update the level data
    public void UpdateLevelData(LevelData currentLevelData)
    {
        _enemiesData = currentLevelData.EnemiesData;
        if(_enemies != null)
        {
            DestroyEnemies();
        }
        SpawnEnemies();
    }
    
    // Method to destroy all enemies
    private void DestroyEnemies()
    {
        for (int i = 0; i < _enemies.Count; i++)
        {
            Destroy(_enemies[i].gameObject);
        }
    }
    
    // Method to reset land enemies to their starting position
    public void ResetLandEnemies()
    {
        foreach (var enemy in _enemies)
        {
            if (enemy.IsLandEnemy)
            {
                enemy.transform.position = enemy.StartingPosition;
            }
        }
    }
}