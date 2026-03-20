using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int hp = 1;
    [SerializeField] private int scoreValue = 100;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private bool dropItemOnDeath = true;
    [SerializeField] private float itemDropRate = 0.3f;

    public int HP => hp;

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
            Debug.LogWarning("ItemSpawner가 없습니다. 아이템 스포너를 추가해주세요.");
        }
    }

    private void Update()
    {
        transform.Translate(Vector2.down * moveSpeed * Time.deltaTime);

        if (transform.position.y < -7f)
        {
            Destroy(gameObject);
        }
    }
}