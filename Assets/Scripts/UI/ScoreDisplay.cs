using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplay : MonoBehaviour
{
    [Header("Score Texts")]
    [SerializeField] private Text scoreText;
    [SerializeField] private Text highScoreText;

    private int highScore = 0;

    private void Start()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        UpdateHighScoreDisplay();
    }

    public void UpdateScore(int currentScore)
    {
        if (scoreText != null)
        {
            scoreText.text = $"점수: {currentScore}";
        }

        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt("HighScore", highScore);
            UpdateHighScoreDisplay();
        }
    }

    private void UpdateHighScoreDisplay()
    {
        if (highScoreText != null)
        {
            highScoreText.text = $"최고: {highScore}";
        }
    }
}
