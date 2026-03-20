using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float screenBoundaryX = 4.5f;
    [SerializeField] private float screenBoundaryY = 4f;

    [Header("Attack")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float baseFireRate = 0.2f;
    [SerializeField] private int baseDamage = 1;
    [SerializeField] private float bulletSpeed = 10f;

    [Header("Stats")]
    [SerializeField] private int maxLives = 3;
    [SerializeField] private int maxBombs = 3;
    [SerializeField] private float invincibilityDuration = 0.5f;
    [SerializeField] private float bombInvincibilityDuration = 3f;

    private int currentLives;
    private int currentBombs;
    private int attackLevel = 1;
    private int fireRateLevel = 1;

    private float fireCooldown;
    private bool isInvincible;
    private bool isBombActive;
    private float invincibilityTimer;

    private Vector2 moveDirection;
    private Rigidbody2D rb;

    public int CurrentLives => currentLives;
    public int CurrentBombs => currentBombs;
    public int AttackLevel => attackLevel;
    public int FireRateLevel => fireRateLevel;
    public bool IsInvincible => isInvincible;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        rb.gravityScale = 0;
        rb.freezeRotation = true;
        transform.localScale = new Vector3(2f, 2f, 1f);
    }

    private void Start()
    {
        currentLives = maxLives;
        currentBombs = 3;
    }

    private void Update()
    {
        HandleInput();
        HandleAttack();
        HandleInvincibility();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleInput()
    {
        float h = 0, v = 0;

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) h = -1;
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) h = 1;
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) v = 1;
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) v = -1;

        moveDirection = new Vector2(h, v).normalized;
    }

    private void HandleMovement()
    {
        Vector2 movement = moveDirection * moveSpeed * Time.fixedDeltaTime;
        Vector2 newPosition = rb.position + movement;

        newPosition.x = Mathf.Clamp(newPosition.x, -screenBoundaryX, screenBoundaryX);
        newPosition.y = Mathf.Clamp(newPosition.y, -screenBoundaryY, screenBoundaryY);

        rb.MovePosition(newPosition);
    }

    private void HandleAttack()
    {
        fireCooldown -= Time.deltaTime;

        bool fireKey = Input.GetKey(KeyCode.Z) || Input.GetMouseButton(0);
        
        if (fireKey && fireCooldown <= 0)
        {
            Fire();
        }
    }

    private void Fire()
    {
        float currentFireRate = baseFireRate / (1 + (fireRateLevel - 1) * 0.3f);
        fireCooldown = currentFireRate;

        Vector3 spawnPosition = transform.position;
        
        if (firePoint != null)
        {
            spawnPosition = firePoint.position;
        }
        else
        {
            spawnPosition.y += 0.5f;
        }

        if (bulletPrefab != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.SetDamage(baseDamage + attackLevel - 1);
                bulletScript.SetSpeed(bulletSpeed);
                bulletScript.SetOwner(BulletOwner.Player);
            }
        }
    }

    private void HandleInvincibility()
    {
        if (isInvincible || isBombActive)
        {
            invincibilityTimer -= Time.deltaTime;

            float flashInterval = isBombActive ? 0.1f : 0.15f;
            float flash = Mathf.PingPong(Time.time / flashInterval, 1);
            SetPlayerVisibility(flash > 0.5f);

            if (invincibilityTimer <= 0)
            {
                isInvincible = false;
                isBombActive = false;
                SetPlayerVisibility(true);
            }
        }
    }

    private void SetPlayerVisibility(bool visible)
    {
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
        foreach (var sprite in sprites)
        {
            sprite.enabled = visible;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible || isBombActive) return;

        Debug.Log($"Player took damage! Lives: {currentLives}");

        currentLives -= damage;

        if (currentLives <= 0)
        {
            currentLives = 0;
            GameOver();
        }
        else
        {
            StartInvincibility(invincibilityDuration);
        }
    }

    public void StartInvincibility(float duration)
    {
        isInvincible = true;
        invincibilityTimer = duration;
    }

    public void UseBomb()
    {
        if (currentBombs <= 0) return;

        currentBombs--;
        isBombActive = true;
        invincibilityTimer = bombInvincibilityDuration;

        BombEffect();
    }

    private void BombEffect()
    {
        var enemies = FindObjectsOfType<Enemy>();
        foreach (var enemy in enemies)
        {
            enemy.TakeDamage(999);
        }

        var bosses = FindObjectsOfType<Boss>();
        foreach (var boss in bosses)
        {
            boss.TakeDamage(999);
        }
    }

    public void AddBomb()
    {
        if (currentBombs < maxBombs)
        {
            currentBombs++;
        }
    }

    public void UpgradeAttack()
    {
        if (attackLevel < 6)
        {
            attackLevel++;
        }
    }

    public void UpgradeFireRate()
    {
        if (fireRateLevel < 6)
        {
            fireRateLevel++;
        }
    }

    private void GameOver()
    {
        Debug.Log("GAME OVER");
        gameObject.SetActive(false);
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
    }
}