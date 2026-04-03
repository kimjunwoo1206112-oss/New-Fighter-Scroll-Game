using UnityEngine;

public class EnemyDespawnZone : MonoBehaviour
{
    [SerializeField] private bool showDebugGizmos = true;
    [SerializeField] private Color gizmoColor = new Color(1f, 0f, 0f, 0.3f);

    private void OnTriggerEnter2D(Collider2D other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            EnemySpawner.Instance?.OnEnemyDestroyed();
            enemy.ForceReturnToPool();
        }

        Bullet bullet = other.GetComponent<Bullet>();
        if (bullet != null)
        {
            BulletPool.Instance?.ReturnEnemyBullet(bullet.gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        if (!showDebugGizmos) return;

        Gizmos.color = gizmoColor;
        
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            if (collider is BoxCollider2D boxCollider)
            {
                Gizmos.DrawCube(transform.position + (Vector3)boxCollider.offset, boxCollider.size);
            }
            else if (collider is CircleCollider2D circleCollider)
            {
                Gizmos.DrawSphere(transform.position + (Vector3)circleCollider.offset, circleCollider.radius);
            }
        }
    }
}
