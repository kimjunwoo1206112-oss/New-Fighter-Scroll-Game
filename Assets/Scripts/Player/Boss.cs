using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int baseMaxHp = 100;
    [SerializeField] private int scoreValue = 5000;
    [SerializeField] private float difficultyMultiplier = 1.5f;

    [Header("Attack Pattern")]
    [SerializeField] private GameObject enemyBulletPrefab;
    [SerializeField] private float attackInterval = 2f;
    [SerializeField] private int attackPatternCount = 3;
    [SerializeField] private float bulletSpeed = 3f;
    [SerializeField] private float sideOffset = 0.5f;

    private int currentHp;
    private int maxHp;
    private float attackTimer;
    private float zigzagTimer;
    private bool isActive = false;
    private int currentStage = 1;
    private const float zigzagInterval = 7f;

    public int MaxHp => maxHp;
    public float ZigzagTimer => zigzagTimer;
    public Vector3 LeftAttackPos => transform.position + new Vector3(-sideOffset, -0.5f, 0);
    public Vector3 RightAttackPos => transform.position + new Vector3(sideOffset, -0.5f, 0);
    public int CurrentHp => currentHp;

    private void Start()
    {
        ApplyStageDifficulty();
        currentHp = maxHp;
        isActive = true;
        transform.localScale = new Vector3(2f, 2f, 1f);
        transform.position = new Vector3(0, 4.5f, 0);
    }

    public void SetStage(int stage)
    {
        currentStage = stage;
    }

    private void ApplyStageDifficulty()
    {
        maxHp = Mathf.RoundToInt(baseMaxHp * Mathf.Pow(difficultyMultiplier, currentStage - 1));
    }

    private void Update()
    {
        if (!isActive) return;

        float adjustedInterval = attackInterval / (1 + (currentStage - 1) * 0.1f);
        attackTimer += Time.deltaTime;
        zigzagTimer += Time.deltaTime;
        
        if (attackTimer >= adjustedInterval)
        {
            attackTimer = 0;
            Attack();
        }
        
        if (zigzagTimer >= zigzagInterval)
        {
            zigzagTimer = 0;
            AttackPatternZigzag();
        }
    }

    private float GetCurrentBulletSpeed()
    {
        if (currentHp <= maxHp / 2)
        {
            return bulletSpeed * 2f;
        }
        return bulletSpeed;
    }

    private void Attack()
    {
        int pattern = Random.Range(0, attackPatternCount + currentStage - 1);
        pattern = Mathf.Min(pattern, 7);
        
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
            case 3:
                AttackPattern4();
                break;
            case 4:
                AttackPattern5();
                break;
            case 5:
                AttackPattern6();
                break;
            case 6:
                AttackPattern7();
                break;
            default:
                AttackPattern1();
                break;
        }
    }

    private void AttackPattern6()
    {
        if (enemyBulletPrefab != null)
        {
            GameObject bulletL = Instantiate(enemyBulletPrefab, LeftAttackPos, Quaternion.identity);
            Bullet bulletScriptL = bulletL.GetComponent<Bullet>();
            if (bulletScriptL != null)
            {
                bulletScriptL.SetOwner(BulletOwner.Enemy);
                bulletScriptL.SetDirection(new Vector2(-0.3f, -1f).normalized);
                bulletScriptL.SetSpeed(GetCurrentBulletSpeed());
            }

            GameObject bulletR = Instantiate(enemyBulletPrefab, RightAttackPos, Quaternion.identity);
            Bullet bulletScriptR = bulletR.GetComponent<Bullet>();
            if (bulletScriptR != null)
            {
                bulletScriptR.SetOwner(BulletOwner.Enemy);
                bulletScriptR.SetDirection(new Vector2(0.3f, -1f).normalized);
                bulletScriptR.SetSpeed(GetCurrentBulletSpeed());
            }
        }
    }

    private void AttackPattern7()
    {
        if (enemyBulletPrefab != null)
        {
            for (int i = 0; i < 3; i++)
            {
                GameObject bulletL = Instantiate(enemyBulletPrefab, LeftAttackPos, Quaternion.identity);
                Bullet bulletScriptL = bulletL.GetComponent<Bullet>();
                if (bulletScriptL != null)
                {
                    bulletScriptL.SetOwner(BulletOwner.Enemy);
                    bulletScriptL.SetDirection(new Vector2(-0.5f - (i * 0.2f), -1f).normalized);
                    bulletScriptL.SetSpeed(GetCurrentBulletSpeed());
                }

                GameObject bulletR = Instantiate(enemyBulletPrefab, RightAttackPos, Quaternion.identity);
                Bullet bulletScriptR = bulletR.GetComponent<Bullet>();
                if (bulletScriptR != null)
                {
                    bulletScriptR.SetOwner(BulletOwner.Enemy);
                    bulletScriptR.SetDirection(new Vector2(0.5f + (i * 0.2f), -1f).normalized);
                    bulletScriptR.SetSpeed(GetCurrentBulletSpeed());
                }
            }
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
                bulletScript.SetSpeed(GetCurrentBulletSpeed());
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
                    bulletScript.SetSpeed(GetCurrentBulletSpeed());
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
                    bulletScript.SetSpeed(GetCurrentBulletSpeed());
                }
            }
        }
    }

    private void AttackPattern4()
    {
        if (enemyBulletPrefab != null)
        {
            for (int i = -3; i <= 3; i++)
            {
                GameObject bullet = Instantiate(enemyBulletPrefab, transform.position, Quaternion.identity);
                Bullet bulletScript = bullet.GetComponent<Bullet>();
                if (bulletScript != null)
                {
                    bulletScript.SetOwner(BulletOwner.Enemy);
                    bulletScript.SetDirection(new Vector2(i * 0.15f, -1f).normalized);
                    bulletScript.SetSpeed(GetCurrentBulletSpeed() * 1.2f);
                }
            }
        }
    }

    private void AttackPattern5()
    {
        if (enemyBulletPrefab != null)
        {
            for (int i = 0; i < 8; i++)
            {
                float angle = i * 45f;
                Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
                GameObject bullet = Instantiate(enemyBulletPrefab, transform.position, Quaternion.identity);
                Bullet bulletScript = bullet.GetComponent<Bullet>();
                if (bulletScript != null)
                {
                    bulletScript.SetOwner(BulletOwner.Enemy);
                    bulletScript.SetDirection(dir);
                    bulletScript.SetSpeed(GetCurrentBulletSpeed() * 0.8f);
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

    private void AttackPatternZigzag()
    {
        if (enemyBulletPrefab == null) return;

        StartCoroutine(ZigzagRoutine());
    }

    private System.Collections.IEnumerator ZigzagRoutine()
    {
        float zigzagSpeed = GetCurrentBulletSpeed();
        
        for (int i = 0; i < 5; i++)
        {
            float offsetX = (i % 2 == 0) ? 0.5f : -0.5f;
            Vector3 spawnPos = transform.position + new Vector3(offsetX, -0.5f, 0);
            
            GameObject bullet = Instantiate(enemyBulletPrefab, spawnPos, Quaternion.identity);
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.SetOwner(BulletOwner.Enemy);
                bulletScript.SetDirection(new Vector2(offsetX * 2f, -1f).normalized);
                bulletScript.SetSpeed(zigzagSpeed);
            }
            
            yield return new WaitForSeconds(0.15f);
        }
    }
}