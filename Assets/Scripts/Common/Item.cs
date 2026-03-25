using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private ItemType itemType;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float screenBoundary = 6f;

    private Vector2 moveDirection = Vector2.down;

    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }

    public void SetMoveDirection(Vector2 direction)
    {
        moveDirection = direction;
    }

    private void Update()
    {
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);

        if (Mathf.Abs(transform.position.x) > screenBoundary || 
            Mathf.Abs(transform.position.y) > screenBoundary)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            ApplyEffect(player);
            Destroy(gameObject);
        }
    }

    private void ApplyEffect(PlayerController player)
    {
        switch (itemType)
        {
            case ItemType.AttackUpgrade:
                player.UpgradeAttack();
                Debug.Log("공격력 업그레이드!");
                break;
            case ItemType.FireRateUpgrade:
                player.UpgradeFireRate();
                Debug.Log("연사속도 업그레이드!");
                break;
            case ItemType.BombCharge:
                player.AddBomb();
                Debug.Log("필사기 충전!");
                break;
        }
    }
}