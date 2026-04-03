using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance { get; private set; }

    [Header("Enemy Prefabs (Pool에 전달)")]
    [SerializeField] private GameObject[] enemyPrefabs;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 1.5f;
    [SerializeField] private float spawnXMin = -4f;
    [SerializeField] private float spawnXMax = 4f;
    [SerializeField][Range(1f, 30f)] private float spawnY = 18f;
    [SerializeField] private int maxEnemies = 20;

    [Header("Difficulty")]
    [SerializeField] private int stage = 1;
    [SerializeField] private float difficultyMultiplier = 1.1f;

    private float spawnTimer;
    private int currentEnemyCount;
    private EnemyPool enemyPool;

    public int Stage => stage;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        enemyPool = EnemyPool.Instance;
        
        if (enemyPool == null)
        {
            Debug.LogError("EnemySpawner: EnemyPool.Instance가 null입니다! EnemyPool 오브젝트가 씬에 있는지 확인하세요.");
        }
        else if (enemyPrefabs != null && enemyPrefabs.Length > 0)
        {
            enemyPool.SetEnemyPrefabs(enemyPrefabs);
        }
    }

    private void Update()
    {
        spawnTimer += Time.deltaTime;

        float adjustedInterval = spawnInterval / (1 + (stage - 1) * 0.1f);

        if (spawnTimer >= adjustedInterval && currentEnemyCount < maxEnemies)
        {
            spawnTimer = 0;
            Debug.Log($"[EnemySpawner] Update에서 SpawnEnemy 호출 - spawnTimer: {spawnTimer}");
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        Debug.Log($"[EnemySpawner] SpawnEnemy 호출됨 - currentEnemyCount: {currentEnemyCount}, maxEnemies: {maxEnemies}");
        
        if (enemyPool == null)
        {
            Debug.LogError("[EnemySpawner] EnemyPool이 null입니다!");
            return;
        }

        GameObject enemy = enemyPool.GetRandomEnemy();
        if (enemy == null)
        {
            Debug.LogError("[EnemySpawner] enemyPool.GetRandomEnemy()가 null을 반환했습니다!");
            return;
        }
        
        Debug.Log($"[EnemySpawner] 적 생성됨: {enemy.name}");

        float xPos = Random.Range(spawnXMin, spawnXMax);
        float spawnYPos = 18f;
        Vector3 spawnPos = new Vector3(xPos, spawnYPos, -5);
        enemy.transform.position = spawnPos;
        
        Debug.Log($"[EnemySpawner] 적 생성 위치: {spawnPos}");
        
        Enemy enemyScript = enemy.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            enemyScript.SetStage(stage);
            enemyScript.SetDifficultyMultiplier(difficultyMultiplier);
        }

        currentEnemyCount++;
    }

    public void OnEnemyDestroyed()
    {
        currentEnemyCount--;
        if (currentEnemyCount < 0) currentEnemyCount = 0;
    }

    public void SetStage(int newStage)
    {
        stage = newStage;
    }
}