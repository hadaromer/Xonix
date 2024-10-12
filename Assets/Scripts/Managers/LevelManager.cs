using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField] private List<LevelData> _levels; // List of LevelData ScriptableObjects
    private int _currentLevelIndex = 1; // Index of the current level

    public LevelData CurrentLevelData { get; private set; }
    public bool IsLastLevel => _currentLevelIndex == _levels.Count - 1;
    
    // Load a specific level
    public void LoadLevel(int levelIndex)
    {
        if (IsLastLevel) return;
        _currentLevelIndex = levelIndex;
        CurrentLevelData = _levels[_currentLevelIndex];
        GameManager.Instance.UpdateLevelData(CurrentLevelData);
    }
    
    // Load the next level
    public void LoadNextLevel()
    {
        if (IsLastLevel) return;
        _currentLevelIndex++;
        CurrentLevelData = _levels[_currentLevelIndex];
        GameManager.Instance.UpdateLevelData(CurrentLevelData);
    }
}