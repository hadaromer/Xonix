using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "LevelData")]
public class LevelData : ScriptableObject
{
    [SerializeField] private int _levelNumber; // Level number
    [SerializeField] private Sprite _background; // Background image for the level
    [SerializeField] private EnemyData[] _enemiesData; // Array of predefined enemy positions
    [SerializeField] private PowerUp[] _powerUps; // List of power-ups for the level
    [SerializeField] private string _levelMessage; // Message to display at the start of the level
    [SerializeField] private int _requiredPercentage; // Required percentage of enemies to be destroyed to complete the level
    
    // Public properties to access private fields
    public int LevelNumber => _levelNumber;
    public Sprite Background => _background;
    public EnemyData[] EnemiesData => _enemiesData;
    public PowerUp[] PowerUps => _powerUps;
    
    public string LevelMessage => _levelMessage;
    public int RequiredPercentage => _requiredPercentage;
}
