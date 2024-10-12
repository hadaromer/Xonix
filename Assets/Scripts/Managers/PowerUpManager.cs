using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

public class PowerUpManager : Singleton<PowerUpManager>
{
    [SerializeField] private PowerUp[] _powerUpsPrefab; // Array of power-up prefabs
    private BestObjectPool<PowerUp>[] _powerUpPool; // Array of object pools for power-ups
    private int _powerUpCount = 0; // Current count of active power-ups
    private int _maxPowerUpCount = 5; // Maximum number of power-ups that can be active at a time
    private float _minSpawnTime = 8f; // Min time between power-up spawns
    private float _maxSpawnTime = 12f; // Max time between power-up spawns
    private List<PowerUp> _activePowerUps = new List<PowerUp>(); // List to hold active power-ups
    private string _powerUpCollectedSound = "PowerUpCollected"; // Sound to play when a power-up is collected

    private void CreatePowerUpsPool()
    {
        // Initialize the object pools for each power-up prefab
        _powerUpPool = new BestObjectPool<PowerUp>[_powerUpsPrefab.Length];
        for (var i = 0; i < _powerUpsPrefab.Length; i++)
        {
            _powerUpPool[i] = new BestObjectPool<PowerUp>(_powerUpsPrefab[i], _maxPowerUpCount);
        }
    }

    public void StartSpwanPowerUps()
    {
        // Start the coroutine to spawn power-ups
        StartCoroutine(SpawnPowerUpEnumerator());
    }

    private void SpawnPowerUp()
    {
        if(_powerUpPool.Length == 0) return;
        if(!GameManager.Instance.IsGameRunning) return;
        // Generate a random spawn position within the game area
        Vector2 spawnPosition = new Vector2(
            Random.Range(-GameManager.Instance.Width / 2, GameManager.Instance.Width / 2),
            Random.Range(-GameManager.Instance.Height / 2, GameManager.Instance.Height / 2)
        );

        // Get a random power-up from the pool and set its position
        var randomPowerUp = Random.Range(0, _powerUpsPrefab.Length);
        var powerUp = _powerUpPool[randomPowerUp].Get();
        _activePowerUps.Add(powerUp);
        powerUp.transform.position = spawnPosition;
        _powerUpCount++;
    }

    public void ReleasePowerUp(PowerUp powerUp)
    {
        // Play the power-up collected sound
        AudioManager.Instance.PlaySound(_powerUpCollectedSound);
        // Start the coroutine to vanish and release the power-up
        StartCoroutine(VanishAndRelease(powerUp));
    }

    private IEnumerator VanishAndRelease(PowerUp powerUp)
    {
        float duration = 0.5f; // Duration of the vanish animation
        Vector3 originalScale = powerUp.transform.localScale;
        float elapsedTime = 0;

        // Animate the scale of the power-up to zero over the duration
        while (elapsedTime < duration)
        {
            powerUp.transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        powerUp.transform.localScale = Vector3.one;

        // Release the power-up back to the appropriate pool
        for (int i = 0; i < _powerUpsPrefab.Length; i++)
        {
            if (_powerUpsPrefab[i].GetType() == powerUp.GetType())
            {
                _powerUpPool[i].Release(powerUp);
                _powerUpCount--;
                break;
            }
        }
        _activePowerUps.Remove(powerUp);
    }

    private IEnumerator SpawnPowerUpEnumerator()
    {
        while (true)
        {
            // Wait for a random time between min spawn time and max spawn time before spawning the next power-up
            float spawnTime = Random.Range(_minSpawnTime, _maxSpawnTime);
            yield return new WaitForSeconds(spawnTime);

            // Spawn a new power-up if the count is less than the maximum
            if (_powerUpCount < _maxPowerUpCount)
            {
                SpawnPowerUp();
            }
        }
    }
    
    public void UpdateLevelData(LevelData currentLevelData)
    {
        // dispose old powerups pool
        if(_powerUpPool != null)
        {
            DisposePowerUpsPool();
        }
        _powerUpsPrefab = currentLevelData.PowerUps;
        CreatePowerUpsPool();
    }

    // Dispose the power-ups pool
    private void DisposePowerUpsPool()
    {
        foreach (var powerUp in _activePowerUps)
        {
            if (powerUp != null)
            {
                Destroy(powerUp.gameObject);
            }
        }
        for (int i = 0; i < _powerUpsPrefab.Length; i++)
        {
            _powerUpPool[i].Dispose();
        }
    }
}