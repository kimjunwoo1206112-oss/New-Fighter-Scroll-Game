using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private ItemType itemType;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float screenBoundary = 6f;
    [SerializeField] private float lifetime = 10f;
    [SerializeField] private float collectionDistance = 1.5f;

    private Vector2 moveDirection = Vector2.down;
    private float lifetimeTimer;

    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }

    public void SetMoveDirection(Vector2 direction)
    {
        moveDirection = direction;
    }

    public void SetItemType(ItemType type)
    {
        itemType = type;
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            sr.transform.localScale = new Vector3(0.1f, 0.1f, 1f);
            sr.sortingOrder = 10;
        }
    }

    private void Awake()
    {
        transform.localScale = new Vector3(0.7f, 0.7f, 1f);
        
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col != null)
        {
            col.size = new Vector2(0.5f, 0.5f);
            col.isTrigger = true;
        }
        
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.drawMode = SpriteDrawMode.Sliced;
            sr.size = new Vector2(1f, 1f);
        }
    }

    private void Update()
    {
        lifetimeTimer += Time.deltaTime;
        if (lifetimeTimer >= lifetime)
        {
            Destroy(gameObject);
            return;
        }

        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);

        float newX = Mathf.Clamp(transform.position.x, -screenBoundary, screenBoundary);
        float newY = Mathf.Clamp(transform.position.y, -screenBoundary, screenBoundary);
        transform.position = new Vector3(newX, newY, transform.position.z);

        if (transform.position.x <= -screenBoundary || transform.position.x >= screenBoundary ||
            transform.position.y <= -screenBoundary || transform.position.y >= screenBoundary)
        {
            ChangeDirection();
        }

        CheckPlayerProximity();
    }

    private void ChangeDirection()
    {
        int dir = Random.Range(0, 4);
        switch (dir)
        {
            case 0: moveDirection = Vector2.up; break;
            case 1: moveDirection = Vector2.down; break;
            case 2: moveDirection = Vector2.left; break;
            case 3: moveDirection = Vector2.right; break;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null)
        {
            player = other.GetComponentInParent<PlayerController>();
        }
        
        if (player != null)
        {
            ApplyEffect(player);
            Destroy(gameObject);
        }
    }

    private void CheckPlayerProximity()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player == null) return;
        
        float distance = Vector2.Distance(transform.position, player.transform.position);
        if (distance < collectionDistance)
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
