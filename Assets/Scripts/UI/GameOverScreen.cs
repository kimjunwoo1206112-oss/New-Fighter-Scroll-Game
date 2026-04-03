using UnityEngine;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
    [Header("Texts")]
    [SerializeField] private Text scoreText;

    [Header("Buttons")]
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;

    private void OnEnable()
    {
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(OnRestart);
        }
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(OnQuit);
        }

        UpdateScore();
    }

    private void OnDisable()
    {
        if (restartButton != null)
        {
            restartButton.onClick.RemoveListener(OnRestart);
        }
        if (quitButton != null)
        {
            quitButton.onClick.RemoveListener(OnQuit);
        }
    }

    private void UpdateScore()
    {
        if (scoreText != null && GameManager.Instance != null)
        {
            scoreText.text = $"최종 점수: {GameManager.Instance.Score}";
        }
    }

    public void OnRestart()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame();
        }
        gameObject.SetActive(false);
    }

    public void OnQuit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
