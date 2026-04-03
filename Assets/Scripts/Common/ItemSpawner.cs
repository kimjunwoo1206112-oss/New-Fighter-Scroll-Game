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
                itemScript.SetItemType(type);
            }
            
            SpriteRenderer sr = item.GetComponentInChildren<SpriteRenderer>();
            if (sr != null)
            {
                sr.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
                sr.sortingOrder = 10;
            }
            
            return item;
        }

        return CreateItemForTest(type, position);
    }

    private GameObject CreateItemForTest(ItemType type, Vector2 position)
    {
        GameObject item = new GameObject($"TestItem_{type}");
        item.transform.position = position;
        
        Item itemScript = item.AddComponent<Item>();
        
        var typeField = typeof(Item).GetField("itemType", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (typeField != null)
        {
            typeField.SetValue(itemScript, type);
        }

        var sprite = new GameObject("Sprite").AddComponent<SpriteRenderer>();
        sprite.transform.SetParent(item.transform);
        sprite.transform.localPosition = Vector3.zero;
        sprite.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
        
        switch (type)
        {
            case ItemType.AttackUpgrade:
                sprite.color = Color.red;
                break;
            case ItemType.FireRateUpgrade:
                sprite.color = Color.green;
                break;
            case ItemType.BombCharge:
                sprite.color = Color.yellow;
                break;
        }

        itemScript.SetMoveSpeed(moveSpeed);
        itemScript.SetMoveDirection(moveDirection);

        Debug.Log($"테스트 아이템 생성: {type}");
        return item;
    }

    [ContextMenu("테스트: 공격력 아이템")]
    public void TestSpawnAttackItem()
    {
        SpawnItem(ItemType.AttackUpgrade, new Vector2(Random.Range(-3f, 3f), Random.Range(-2f, 2f)));
    }

    [ContextMenu("테스트: 연사속도 아이템")]
    public void TestSpawnFireRateItem()
    {
        SpawnItem(ItemType.FireRateUpgrade, new Vector2(Random.Range(-3f, 3f), Random.Range(-2f, 2f)));
    }

    [ContextMenu("테스트: 필사기 아이템")]
    public void TestSpawnBombItem()
    {
        SpawnItem(ItemType.BombCharge, new Vector2(Random.Range(-3f, 3f), Random.Range(-2f, 2f)));
    }

    [ContextMenu("테스트: 랜덤 아이템")]
    public void TestSpawnRandomItem()
    {
        SpawnRandomItem(new Vector2(Random.Range(-3f, 3f), Random.Range(-2f, 2f)));
    }
}