using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private TextMeshProUGUI _percentageText; // UI element to display the percentage
    [SerializeField] private TextMeshProUGUI _scoreText; // UI element to display the score
    [SerializeField] private TextMeshProUGUI _timeText; // UI element to display the time
    [SerializeField] private Image _winImage; // UI element to display the win image
    [SerializeField] private Image _loseImage; // UI element to display the lose image
    [SerializeField] private TMP_InputField _playerName; // Input field for the player's name
    [SerializeField] private TextMeshProUGUI _levelMessageText; // UI element to display level message
    [SerializeField] private TextMeshProUGUI _levelNumberText; // UI element to display level number
    [SerializeField] private Image _levelMessageBackground; // UI element to display level message background
    
    // Public properties to access private fields
    public TextMeshProUGUI PercentageText => _percentageText;
    public TextMeshProUGUI ScoreText => _scoreText;
    public TMP_InputField PlayerName => _playerName;

    // Update the time text UI element
    public void UpdateTimeText(int currentTime)
    {
        _timeText.text = currentTime.ToString();
    }

    // Show the win image and set focus to the player name input field
    public void ShowWinImage()
    {
        StartCoroutine(FadeInImage(_winImage, 2f));
        _playerName.gameObject.SetActive(true);
        _playerName.Select(); // Set focus to the input field
    }

    // Show the lose image and set focus to the player name input field
    public void ShowLoseImage()
    {
        StartCoroutine(FadeInImage(_loseImage, 2f));
        _playerName.gameObject.SetActive(true);
        _playerName.Select(); // Set focus to the input field
    }

    // Coroutine to fade in an image over a specified duration
    private IEnumerator FadeInImage(Image image, float duration)
    {
        float elapsedTime = 0f;
        Color color = image.color;
        color.a = 0f;
        image.color = color;
        image.gameObject.SetActive(true);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsedTime / duration);
            image.color = color;
            yield return null;
        }
    }

    // Hide the score board by deactivating the name and score text UI elements
    public void HideScoreBoard(TextMeshProUGUI[] namesText, TextMeshProUGUI[] scoresText)
    {
        foreach (var nameText in namesText)
        {
            nameText.gameObject.SetActive(false);
        }

        foreach (var scoreText in scoresText)
        {
            scoreText.gameObject.SetActive(false);
        }
    }

    // Display the level message using a typewriter effect
    public IEnumerator TypeWriterEffect(string message, int _levelNumber)
    {
        _levelNumberText.text = "Level " + _levelNumber;
        _levelNumberText.gameObject.SetActive(true);
        _levelMessageText.text = "";
        _levelMessageBackground.gameObject.SetActive(true);
        _levelMessageText.gameObject.SetActive(true);
        AudioManager.Instance.PlaySound("Keyboard");
        foreach (char letter in message.ToCharArray())
        {
            _levelMessageText.text += letter;
            // wait for random time between 0.01 and 0.1 seconds
            yield return new WaitForSeconds(Random.Range(0.01f, 0.1f));
            //yield return new WaitForSeconds(0.05f); // Adjust the speed of the typewriter effect
        }
        yield return new WaitForSeconds(2f); // Display the message for 5 seconds
        _levelMessageBackground.gameObject.SetActive(false);
        _levelMessageText.gameObject.SetActive(false);
        _levelNumberText.gameObject.SetActive(false);
    }

    public void UpdateScoreText(int percentage, int score)
    {
        _percentageText.text = percentage + "%";
        _scoreText.text = score.ToString();
    }
}