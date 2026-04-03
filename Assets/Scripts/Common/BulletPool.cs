using UnityEngine;
using System.Collections.Generic;

public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance { get; private set; }

    [SerializeField] private GameObject playerBulletPrefab;
    [SerializeField] private GameObject enemyBulletPrefab;
    [SerializeField] private int poolSize = 50;

    private Queue<GameObject> playerBulletPool;
    private Queue<GameObject> enemyBulletPool;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializePools();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializePools()
    {
        playerBulletPool = new Queue<GameObject>();
        enemyBulletPool = new Queue<GameObject>();

        if (playerBulletPrefab != null)
        {
            for (int i = 0; i < poolSize; i++)
            {
                GameObject bullet = Instantiate(playerBulletPrefab, Vector3.zero, Quaternion.identity);
                bullet.SetActive(false);
                bullet.transform.SetParent(transform);
                playerBulletPool.Enqueue(bullet);
            }
        }

        if (enemyBulletPrefab != null)
        {
            for (int i = 0; i < poolSize; i++)
            {
                GameObject bullet = Instantiate(enemyBulletPrefab, Vector3.zero, Quaternion.identity);
                bullet.SetActive(false);
                bullet.transform.SetParent(transform);
                enemyBulletPool.Enqueue(bullet);
            }
        }
    }

    public GameObject GetPlayerBullet()
    {
        return GetBulletFromPool(playerBulletPool, playerBulletPrefab);
    }

    public GameObject GetEnemyBullet()
    {
        return GetBulletFromPool(enemyBulletPool, enemyBulletPrefab);
    }

    private GameObject GetBulletFromPool(Queue<GameObject> pool, GameObject prefab)
    {
        GameObject bullet;

        if (pool.Count > 0)
        {
            bullet = pool.Dequeue();
            bullet.transform.SetParent(null);  // 부모 제거 (월드 좌표계)
            bullet.SetActive(true);
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.ResetBullet();
            }
        }
        else
        {
            bullet = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            bullet.transform.SetParent(null);  // 부모 제거 (월드 좌표계)
            bullet.SetActive(true);
        }

        return bullet;
    }

    public void ReturnPlayerBullet(GameObject bullet)
    {
        ReturnBulletToPool(bullet, playerBulletPool);
    }

    public void ReturnEnemyBullet(GameObject bullet)
    {
        ReturnBulletToPool(bullet, enemyBulletPool);
    }

    private void ReturnBulletToPool(GameObject bullet, Queue<GameObject> pool)
    {
        bullet.transform.SetParent(transform);  // 먼저 부모 설정
        bullet.SetActive(false);  // 그 다음 비활성화
        pool.Enqueue(bullet);
    }
}