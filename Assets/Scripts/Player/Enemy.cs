using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int enemyId = 1;
    [SerializeField] private int baseHp = 1;
    [SerializeField] private int scoreValue = 100;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private bool dropItemOnDeath = true;
    [SerializeField] private float itemDropRate = 0.3f;
    [SerializeField] private GameObject enemyBulletPrefab;
    [SerializeField] private float attackInterval = 3f;
    [SerializeField] private float bulletSpeed = 3f;

    private int hp;
    private int currentStage = 1;
    private float difficultyMultiplier = 1.1f;
    private float attackTimer;

    public int HP => hp;

    private void Start()
    {
        LoadStatsFromCSV();
        ApplyDifficulty();
        transform.localScale = new Vector3(2.5f, 2.5f, 1f);
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
        EnemySpawner.Instance?.OnEnemyDestroyed();

        if (dropItemOnDeath && Random.value < itemDropRate)
        {
            SpawnItem();
        }

        Destroy(gameObject);
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
        if (attackTimer >= attackInterval)
        {
            attackTimer = 0;
            Attack();
        }

        if (transform.position.y < -7f)
        {
            EnemySpawner.Instance?.OnEnemyDestroyed();
            Destroy(gameObject);
        }
    }

    private void Attack()
    {
        if (enemyBulletPrefab != null)
        {
            GameObject bullet = Instantiate(enemyBulletPrefab, transform.position, Quaternion.identity);
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