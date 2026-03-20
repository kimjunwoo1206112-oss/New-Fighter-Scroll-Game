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
    public float StageTimer => stageTimer;
    public float BossSpawnTime => bossSpawnDelay;
    public bool BossSpawned => bossSpawned;

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

    [Header("Stage Settings")]
    [SerializeField] private float bossSpawnDelay = 30f;
    [SerializeField] private GameObject bossPrefab;

    private float stageTimer;
    private bool bossSpawned;

    private void Update()
    {
        if (!isGameOver && !isPaused)
        {
            stageTimer += Time.deltaTime;
            
            if (!bossSpawned && stageTimer >= bossSpawnDelay)
            {
                SpawnBoss();
            }
        }
    }

    public void NextStage()
    {
        currentStage++;
        stageTimer = 0f;
        bossSpawned = false;
        Debug.Log($"===== Stage {currentStage} 시작 (보스까지 {bossSpawnDelay}초) =====");
        
        EnemySpawner.Instance?.SetStage(currentStage);
    }

    private void SpawnBoss()
    {
        bossSpawned = true;
        if (bossPrefab != null)
        {
            Instantiate(bossPrefab);
            Debug.Log("보스出現!");
        }
        else
        {
            Debug.LogWarning("Boss Prefab이 설정되지 않았습니다.");
        }
    }

    public void RestartStage()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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