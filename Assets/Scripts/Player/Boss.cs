using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int maxHp = 100;
    [SerializeField] private int scoreValue = 5000;

    [Header("Attack Pattern")]
    [SerializeField] private GameObject enemyBulletPrefab;
    [SerializeField] private float attackInterval = 2f;
    [SerializeField] private int attackPatternCount = 3;

    private int currentHp;
    private float attackTimer;
    private bool isActive = false;

    public int MaxHp => maxHp;
    public int CurrentHp => currentHp;

    private void Start()
    {
        currentHp = maxHp;
        isActive = true;
    }

    private void Update()
    {
        if (!isActive) return;

        attackTimer += Time.deltaTime;
        if (attackTimer >= attackInterval)
        {
            attackTimer = 0;
            Attack();
        }
    }

    private void Attack()
    {
        int pattern = Random.Range(0, attackPatternCount);
        switch (pattern)
        {
            case 0:
                AttackPattern1();
                break;
            case 1:
                AttackPattern2();
                break;
            case 2:
                AttackPattern3();
                break;
        }
    }

    private void AttackPattern1()
    {
        if (enemyBulletPrefab != null)
        {
            GameObject bullet = Instantiate(enemyBulletPrefab, transform.position, Quaternion.identity);
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.SetOwner(BulletOwner.Enemy);
                bulletScript.SetDirection(Vector2.down);
            }
        }
    }

    private void AttackPattern2()
    {
        if (enemyBulletPrefab != null)
        {
            for (int i = -1; i <= 1; i++)
            {
                GameObject bullet = Instantiate(enemyBulletPrefab, transform.position, Quaternion.identity);
                Bullet bulletScript = bullet.GetComponent<Bullet>();
                if (bulletScript != null)
                {
                    bulletScript.SetOwner(BulletOwner.Enemy);
                    bulletScript.SetDirection(new Vector2(i * 0.3f, -1f).normalized);
                }
            }
        }
    }

    private void AttackPattern3()
    {
        if (enemyBulletPrefab != null)
        {
            for (int i = -2; i <= 2; i++)
            {
                GameObject bullet = Instantiate(enemyBulletPrefab, transform.position, Quaternion.identity);
                Bullet bulletScript = bullet.GetComponent<Bullet>();
                if (bulletScript != null)
                {
                    bulletScript.SetOwner(BulletOwner.Enemy);
                    bulletScript.SetDirection(new Vector2(i * 0.2f, -1f).normalized);
                }
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHp -= damage;

        if (currentHp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        GameManager.Instance?.AddScore(scoreValue);
        GameManager.Instance?.NextStage();
        Destroy(gameObject);
    }
}