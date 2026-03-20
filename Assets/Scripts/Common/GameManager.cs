using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    [SerializeField] private int currentStage = 1;
    [SerializeField] private int score = 0;
    [SerializeField] private bool isGameOver = false;
    [SerializeField] private bool isPaused = false;

    public int CurrentStage => currentStage;
    public int Score => score;
    public bool IsGameOver => isGameOver;
    public bool IsPaused => isPaused;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddScore(int points)
    {
        score += points;
    }

    public void NextStage()
    {
        currentStage++;
        Debug.Log($"Stage {currentStage} 시작");
    }

    public void GameOver()
    {
        isGameOver = true;
        Debug.Log($"게임 오버! 최종 점수: {score}");
    }

    public void RestartGame()
    {
        isGameOver = false;
        score = 0;
        currentStage = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;
    }
}