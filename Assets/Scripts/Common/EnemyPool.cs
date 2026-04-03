using UnityEngine;
using System.Collections.Generic;

public class EnemyPool : MonoBehaviour
{
    public static EnemyPool Instance { get; private set; }

    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private int poolSizePerType = 10;

    private Dictionary<int, Queue<GameObject>> enemyPools;
    private Dictionary<int, GameObject> prefabDictionary;

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
        if (enemyPrefabs != null && enemyPrefabs.Length > 0)
        {
            InitializePools();
        }
    }

    public void SetEnemyPrefabs(GameObject[] prefabs)
    {
        if (prefabs != null && prefabs.Length > 0)
        {
            enemyPrefabs = prefabs;
            InitializePools();
        }
    }

    private void InitializePools()
    {
        enemyPools = new Dictionary<int, Queue<GameObject>>();
        prefabDictionary = new Dictionary<int, GameObject>();

        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            Debug.LogWarning("EnemyPool: enemyPrefabs가 설정되지 않았습니다.");
            return;
        }

        for (int i = 0; i < enemyPrefabs.Length; i++)
        {
            if (enemyPrefabs[i] == null) continue;

            int enemyId = i;
            Queue<GameObject> pool = new Queue<GameObject>();
            prefabDictionary[enemyId] = enemyPrefabs[i];

            for (int j = 0; j < poolSizePerType; j++)
            {
                GameObject enemy = Instantiate(enemyPrefabs[i], Vector3.zero, Quaternion.identity);
                enemy.SetActive(false);
                enemy.transform.SetParent(transform);

                Enemy enemyScript = enemy.GetComponent<Enemy>();
                if (enemyScript != null)
                {
                    enemyScript.SetPool(this);
                }

                pool.Enqueue(enemy);
            }

            enemyPools[enemyId] = pool;
        }

        Debug.Log($"EnemyPool 초기화 완료: {enemyPrefabs.Length} 종류, 각 {poolSizePerType}개");
    }

    public GameObject GetEnemy(int enemyId)
    {
        if (enemyPools.ContainsKey(enemyId) && enemyPools[enemyId].Count > 0)
        {
            GameObject enemy = enemyPools[enemyId].Dequeue();
            enemy.SetActive(true);

            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.ResetEnemy();
            }
            
            enemy.transform.position = new Vector3(0, 18, -5);

            return enemy;
        }
        else if (prefabDictionary.ContainsKey(enemyId))
        {
            GameObject enemy = Instantiate(prefabDictionary[enemyId], Vector3.zero, Quaternion.identity);
            enemy.transform.SetParent(transform);

            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.SetPool(this);
                enemyScript.ResetEnemy();
            }

            return enemy;
        }

        Debug.LogWarning($"EnemyPool: ID {enemyId}에 해당하는 적 프리팹이 없습니다.");
        return null;
    }

    public GameObject GetRandomEnemy()
    {
        Debug.Log($"[EnemyPool] GetRandomEnemy 호출됨 - enemyPools.Count: {enemyPools?.Count}");
        
        if (enemyPools == null)
        {
            Debug.LogError("[EnemyPool] enemyPools가 null입니다!");
            return null;
        }
        
        if (enemyPools.Count == 0)
        {
            Debug.LogError("[EnemyPool] enemyPools가 비어있습니다! enemyPrefabs가 설정되지 않았을 수 있습니다.");
            return null;
        }

        int randomId = Random.Range(0, enemyPools.Count);
        Debug.Log($"[EnemyPool] randomId: {randomId}, 반환된 적: {GetEnemy(randomId)?.name}");
        return GetEnemy(randomId);
    }

    public void ReturnEnemy(GameObject enemy, int enemyId)
    {
        if (enemy == null) return;

        enemy.SetActive(false);

        if (enemyPools.ContainsKey(enemyId))
        {
            enemyPools[enemyId].Enqueue(enemy);
        }
        else
        {
            Destroy(enemy);
        }
    }

    public void ReturnEnemy(GameObject enemy)
    {
        if (enemy == null) return;

        Enemy enemyScript = enemy.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            ReturnEnemy(enemy, enemyScript.EnemyId);
        }
        else
        {
            enemy.SetActive(false);
        }
    }
}
