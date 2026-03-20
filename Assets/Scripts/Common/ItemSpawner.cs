using UnityEngine;

public enum ItemType
{
    AttackUpgrade,
    FireRateUpgrade,
    BombCharge
}

public class ItemSpawner : MonoBehaviour
{
    public static ItemSpawner Instance { get; private set; }

    [Header("Item Prefabs")]
    [SerializeField] private GameObject attackUpgradePrefab;
    [SerializeField] private GameObject fireRateUpgradePrefab;
    [SerializeField] private GameObject bombChargePrefab;

    [Header("Spawn Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float changeDirectionInterval = 1f;

    private float directionTimer;
    private Vector2 moveDirection;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        directionTimer += Time.deltaTime;
        if (directionTimer >= changeDirectionInterval)
        {
            directionTimer = 0;
            ChangeDirection();
        }
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

    public GameObject SpawnRandomItem(Vector2 position)
    {
        float rand = Random.value;
        ItemType type;

        if (rand < 0.5f)
        {
            type = ItemType.AttackUpgrade;
        }
        else if (rand < 0.8f)
        {
            type = ItemType.FireRateUpgrade;
        }
        else
        {
            type = ItemType.BombCharge;
        }

        return SpawnItem(type, position);
    }

    public GameObject SpawnItem(ItemType type, Vector2 position)
    {
        GameObject prefab = null;

        switch (type)
        {
            case ItemType.AttackUpgrade:
                prefab = attackUpgradePrefab;
                break;
            case ItemType.FireRateUpgrade:
                prefab = fireRateUpgradePrefab;
                break;
            case ItemType.BombCharge:
                prefab = bombChargePrefab;
                break;
        }

        if (prefab != null)
        {
            GameObject item = Instantiate(prefab, position, Quaternion.identity);
            Item itemScript = item.GetComponent<Item>();
            if (itemScript != null)
            {
                itemScript.SetMoveSpeed(moveSpeed);
                itemScript.SetMoveDirection(moveDirection);
            }
            return item;
        }

        return null;
    }
}