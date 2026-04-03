using UnityEngine;

public enum BulletOwner
{
    Player,
    Enemy
}

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private int damage = 1;
    [SerializeField] private BulletOwner owner;

    private Vector2 direction = Vector2.up;
    private bool hasHit;

    private void Awake()
    {
        transform.localScale = new Vector3(2f, 2f, 1f);
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    public void SetDamage(int newDamage)
    {
        damage = newDamage;
    }

    public void SetOwner(BulletOwner newOwner)
    {
        owner = newOwner;
        if (owner == BulletOwner.Enemy)
        {
            transform.localScale = new Vector3(2f, 2f, 1f);
        }
        else
        {
            transform.localScale = new Vector3(2f, 2f, 1f);
        }
    }

    public void SetDirection(Vector2 newDirection)
    {
        direction = newDirection;
        
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void Update()
    {
        Vector3 oldPos = transform.position;
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
        Vector3 newPos = transform.position;
        
        Debug.Log($"[Bullet Update] Old: {oldPos}, New: {newPos}, Direction: {direction}");

        if (Mathf.Abs(transform.position.y) > 30f || 
            Mathf.Abs(transform.position.x) > 30f)
        {
            ReturnToPool();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;

        if (owner == BulletOwner.Player)
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                hasHit = true;
                enemy.TakeDamage(damage);
                ReturnToPool();
                return;
            }

            Boss boss = other.GetComponent<Boss>();
            if (boss != null)
            {
                hasHit = true;
                boss.TakeDamage(damage);
                ReturnToPool();
                return;
            }
        }
        else if (owner == BulletOwner.Enemy)
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                Debug.Log("Player hit by enemy bullet!");
                hasHit = true;
                player.TakeDamage(1);
                ReturnToPool();
                return;
            }
        }
    }

    public void ResetBullet()
    {
        direction = Vector2.up;
        transform.rotation = Quaternion.identity;
        hasHit = false;
        
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.enabled = true;
        }
    }

    private void ReturnToPool()
    {
        if (owner == BulletOwner.Player)
        {
            BulletPool.Instance?.ReturnPlayerBullet(gameObject);
        }
        else
        {
            BulletPool.Instance?.ReturnEnemyBullet(gameObject);
        }
    }
}
