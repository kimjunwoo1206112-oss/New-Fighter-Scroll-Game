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
    }

    public void SetDirection(Vector2 newDirection)
    {
        direction = newDirection;
        if (direction == Vector2.down)
        {
            transform.rotation = Quaternion.Euler(0, 0, 180);
        }
    }

    private void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        if (Mathf.Abs(transform.position.y) > 10f || 
            Mathf.Abs(transform.position.x) > 10f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (owner == BulletOwner.Player)
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Destroy(gameObject);
                return;
            }

            Boss boss = other.GetComponent<Boss>();
            if (boss != null)
            {
                boss.TakeDamage(damage);
                Destroy(gameObject);
                return;
            }
        }
        else if (owner == BulletOwner.Enemy)
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(1);
                Destroy(gameObject);
            }
        }
    }
}