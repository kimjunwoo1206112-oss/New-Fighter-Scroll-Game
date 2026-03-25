using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance { get; private set; }

    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject[] enemyPrefabs;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 1.5f;
    [SerializeField] private float spawnXMin = -4f;
    [SerializeField] private float spawnXMax = 4f;
    [SerializeField] private float spawnY = 6f;
    [SerializeField] private int maxEnemies = 20;

    [Header("Difficulty")]
    [SerializeField] private int stage = 1;
    [SerializeField] private float difficultyMultiplier = 1.1f;

    private float spawnTimer;
    private int currentEnemyCount;

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

    private void Update()
    {
        spawnTimer += Time.deltaTime;

        float adjustedInterval = spawnInterval / (1 + (stage - 1) * 0.1f);

        if (spawnTimer >= adjustedInterval && currentEnemyCount < maxEnemies)
        {
            spawnTimer = 0;
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0) return;

        int enemyIndex = Random.Range(0, enemyPrefabs.Length);
        float xPos = Random.Range(spawnXMin, spawnXMax);
        Vector2 spawnPos = new Vector2(xPos, spawnY);

        GameObject enemy = Instantiate(enemyPrefabs[enemyIndex], spawnPos, Quaternion.identity);
        
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