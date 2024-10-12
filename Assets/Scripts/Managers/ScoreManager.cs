using UnityEngine;
using TMPro;

public class ScoreManager : Singleton<ScoreManager>
{
    [SerializeField] private TextMeshProUGUI[] _namesText; // UI elements to display high score names
    [SerializeField] private TextMeshProUGUI[] _scoresText; // UI elements to display high scores
    [SerializeField] private TMP_InputField _playerName; // Input field for the player's name

    private int _currentScore = 0; // Current score of the player
    private int _currentPercentage = 0; // Current percentage of the game completed

    // Public properties to access private fields
    public int CurrentScore => _currentScore;
    public int CurrentPercentage => _currentPercentage;
    public TextMeshProUGUI[] NamesText => _namesText;
    public TextMeshProUGUI[] ScoresText => _scoresText;
    
    // Update the high scores displayed in the UI
    public void UpdateHighScores()
    {
        int[] highScores = PlayerPrefsX.GetIntArray("HighScores", 0, 5);
        string[] highScoreNames = PlayerPrefsX.GetStringArray("HighScoreNames", "-----", 5); 
        for (int i = 0; i < highScores.Length; i++)
        {
            _namesText[i].text = highScoreNames[i];
            _scoresText[i].text = highScores[i].ToString();
        }
    }

    // Set a new high score if the current score is higher than any of the existing high scores
    public void SetHighScore()
    {
        int[] highScores = PlayerPrefsX.GetIntArray("HighScores", 0, 5); 
        string[] highScoreNames = PlayerPrefsX.GetStringArray("HighScoreNames", "-----", 5); 
        int tempCurrentScore = _currentScore; 
        string tempCurrentName = _playerName.text;

        for (int i = 0; i < highScores.Length; i++)
        {
            if (tempCurrentScore > highScores[i])
            {
                int tempScore = highScores[i]; 
                string tempName = highScoreNames[i];
                highScores[i] = tempCurrentScore;
                highScoreNames[i] = tempCurrentName;
                tempCurrentScore = tempScore;
                tempCurrentName = tempName;
            }
        }

        PlayerPrefsX.SetIntArray("HighScores", highScores); 
        PlayerPrefsX.SetStringArray("HighScoreNames", highScoreNames);
        PlayerPrefs.Save();
    }

    // Calculate the final score based on the remaining time and lives
    public void CalculateFinalScore(int currentTime, int lives)
    {
        _currentScore += currentTime * 10 + lives * 100;
    }

    // Update the score based on the new percentage of the game completed
    public void UpdateScore(int newPercentage)
    {
        if(_currentPercentage == newPercentage) return;
        int oldPercentage = _currentPercentage; 
        _currentPercentage = newPercentage; 
        if(_currentPercentage > oldPercentage)
            _currentScore += (int)Mathf.Pow((_currentPercentage - oldPercentage + 1), 2);
        UIManager.Instance.UpdateScoreText(_currentPercentage, _currentScore);
    }
    
    // Reset the current percentage
    public void ResetPercentage()
    {
        _currentPercentage = 0;
        UIManager.Instance.UpdateScoreText(_currentPercentage, _currentScore);
    }
}