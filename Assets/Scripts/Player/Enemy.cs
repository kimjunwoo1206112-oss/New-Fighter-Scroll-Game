using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int enemyId = 1;
    [SerializeField] private int baseHp = 1;
    [SerializeField] private int scoreValue = 100;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private bool dropItemOnDeath = true;
    [SerializeField] private float itemDropRate = 0.3f;
    [SerializeField] private GameObject enemyBulletPrefab;
    [SerializeField] private float attackInterval = 2f;
    [SerializeField] private float bulletSpeed = 3f;
    [SerializeField] private float bulletScale = 2f;
    [SerializeField] private GameObject explosionEffectPrefab;
    [SerializeField] private float screenTopY = 5f;
    [SerializeField] private float screenBottomY = -5f;
    [SerializeField] private Transform bulletSpawnPoint;

    private int hp;
    private int currentStage = 1;
    private float difficultyMultiplier = 1.1f;
    private float attackTimer;
    private EnemyPool enemyPool;

    public int HP => hp;
    public int EnemyId => enemyId;
    public bool IsActive => gameObject.activeInHierarchy;

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        LoadStatsFromCSV();
        ApplyDifficulty();
        transform.localScale = new Vector3(2.5f, 2.5f, 1f);
    }

    public void SetPool(EnemyPool pool)
    {
        enemyPool = pool;
    }

    public void ResetEnemy()
    {
        attackTimer = attackInterval;
        Initialize();
        
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.enabled = true;
        }
    }

    public void SetPosition(Vector3 newPosition)
    {
        transform.position = newPosition;
    }

    private void LoadStatsFromCSV()
    {
        if (ExcelDataManager.Instance != null)
        {
            baseHp = ExcelDataManager.Instance.GetIntValue("Enemy", "HP", baseHp);
            scoreValue = ExcelDataManager.Instance.GetIntValue("Enemy", "점수", scoreValue);
            moveSpeed = ExcelDataManager.Instance.GetFloatValue("Enemy", "이동속도", moveSpeed);
            itemDropRate = ExcelDataManager.Instance.GetFloatValue("Enemy", "드롭률", itemDropRate);
            attackInterval = ExcelDataManager.Instance.GetFloatValue("Enemy", "공격간격", attackInterval);
            
            var bullet = enemyBulletPrefab?.GetComponent<Bullet>();
            if (bullet != null)
            {
                float bulletSpeed = ExcelDataManager.Instance.GetFloatValue("Enemy", "총알속도", 3f);
                bullet.SetSpeed(bulletSpeed);
            }
            
            Debug.Log("적 스탯 엑셀에서 로드됨");
        }
    }

    public void SetStage(int stage)
    {
        currentStage = stage;
    }

    public void SetDifficultyMultiplier(float multiplier)
    {
        difficultyMultiplier = multiplier;
    }

    private void ApplyDifficulty()
    {
        hp = Mathf.RoundToInt(baseHp * Mathf.Pow(difficultyMultiplier, currentStage - 1));
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;

        if (hp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        GameManager.Instance?.AddScore(scoreValue);
        
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateHUD();
        }
        
        if (explosionEffectPrefab != null)
        {
            GameObject effect = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            effect.transform.localScale = Vector3.one * 0.35f;
            Destroy(effect, 1f);
        }
        
        EnemySpawner.Instance?.OnEnemyDestroyed();

        if (dropItemOnDeath && Random.value < itemDropRate)
        {
            SpawnItem();
        }

        ReturnToPool();
    }

    private void ReturnToPool()
    {
        if (enemyPool != null)
        {
            enemyPool.ReturnEnemy(gameObject, enemyId);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void ForceReturnToPool()
    {
        ReturnToPool();
    }

    private void SpawnItem()
    {
        GameObject item = ItemSpawner.Instance?.SpawnRandomItem(transform.position);
        if (item == null)
        {
            Debug.LogWarning("ItemSpawner가 없습니다.");
        }
    }

    private void Update()
    {
        float speedMultiplier = 1 + (currentStage - 1) * 0.1f;
        transform.Translate(Vector2.down * moveSpeed * speedMultiplier * Time.deltaTime);

        attackTimer += Time.deltaTime;
        if (attackTimer >= attackInterval && IsInScreen())
        {
            attackTimer = 0;
            Attack();
        }

        if (transform.position.y < -7f)
        {
            EnemySpawner.Instance?.OnEnemyDestroyed();
            ReturnToPool();
        }
    }

    private bool IsInScreen()
    {
        return transform.position.y >= screenBottomY && transform.position.y <= screenTopY;
    }

    private Vector3 GetBulletSpawnPosition()
    {
        if (bulletSpawnPoint != null)
        {
            return bulletSpawnPoint.position;
        }
        
        float spawnZ = transform.position.z;
        return new Vector3(transform.position.x, transform.position.y, spawnZ);
    }

    private void Attack()
    {
        if (enemyBulletPrefab != null && BulletPool.Instance != null)
        {
            GameObject bullet = BulletPool.Instance.GetEnemyBullet();
            
            if (bullet != null)
            {
                bullet.transform.position = GetBulletSpawnPosition();
                bullet.transform.localScale = Vector3.one * bulletScale;
                bullet.SetActive(true);
                
                Bullet bulletScript = bullet.GetComponent<Bullet>();
                if (bulletScript != null)
                {
                    bulletScript.SetOwner(BulletOwner.Enemy);
                    bulletScript.SetDirection(Vector2.down);
                    bulletScript.SetSpeed(bulletSpeed);
                }
            }
        }
    }
}