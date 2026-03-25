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
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SpawnSelectedPlayer();
    }

    private void SpawnSelectedPlayer()
    {
        // 이미 플레이어가 있으면 생성하지 않음
        if (FindObjectOfType<PlayerController>() != null)
        {
            Debug.Log("플레이어가 이미 존재합니다.");
            return;
        }

        // PlayerPrefs에서 선택한 플레이어 ID 가져오기
        int selectedId = PlayerPrefs.GetInt("SelectedPlayerId", 1);
        Debug.Log($"PlayerPrefs에서 선택한 ID: {selectedId}");
        
        int index = selectedId - 1;

        if (playerPrefabs.Length > index && playerPrefabs[index] != null)
        {
            currentPlayer = Instantiate(playerPrefabs[index], Vector3.zero, Quaternion.identity);
            Debug.Log($"플레이어 생성: ID {selectedId} (프리팹 {index})");
        }
        else
        {
            Debug.LogWarning($"플레이어 프리팹 {selectedId}이 설정되지 않았습니다.");
        }
    }

    public void AddScore(int points)
    {
        score += points;
    }

    [Header("Stage Settings")]
    [SerializeField] private float bossSpawnDelay = 30f;
    [SerializeField] private GameObject bossPrefab;

    [Header("Player Prefabs")]
    [SerializeField] private GameObject[] playerPrefabs = new GameObject[3];

    private float stageTimer;
    private bool bossSpawned;
    private GameObject currentPlayer;

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