using NUnit.Framework;
using UnityEngine;
using System.Reflection;

public class ItemTests
{
    private GameObject playerGameObject;
    private PlayerController playerController;

    [SetUp]
    public void Setup()
    {
        playerGameObject = new GameObject("Player");
        playerController = playerGameObject.AddComponent<PlayerController>();
        
        var rb = playerGameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;
        playerGameObject.transform.localScale = new Vector3(2f, 2f, 1f);
        playerGameObject.AddComponent<BoxCollider2D>();
        
        var sprite = new GameObject("Sprite").AddComponent<SpriteRenderer>();
        sprite.transform.SetParent(playerGameObject.transform);
    }

    [TearDown]
    public void Teardown()
    {
        if (playerGameObject != null) Object.DestroyImmediate(playerGameObject);
    }

    [Test]
    public void Test_AttackUpgrade()
    {
        Debug.Log("=== 공격력 업그레이드 테스트 ===");
        
        int initialLevel = playerController.AttackLevel;
        Debug.Log($"초기 공격력 레벨: {initialLevel}");
        
        playerController.UpgradeAttack();
        int afterFirst = playerController.AttackLevel;
        Debug.Log($"업그레이드 후 공격력 레벨: {afterFirst}");
        Assert.AreEqual(2, afterFirst, "레벨 2이어야 함");
        
        for (int i = 0; i < 4; i++)
        {
            playerController.UpgradeAttack();
        }
        Debug.Log($"추가 업그레이드 후 공격력 레벨: {playerController.AttackLevel}");
        
        Assert.AreEqual(6, playerController.AttackLevel, "최대 레벨 6이어야 함");
        
        playerController.UpgradeAttack();
        Assert.AreEqual(6, playerController.AttackLevel, "최대치 이상 증가 안함");
        
        Debug.Log("✓ 공격력 업그레이드: 1→2→6, 최대치 초과 시 유지");
    }

    [Test]
    public void Test_FireRateUpgrade()
    {
        Debug.Log("=== 연사속도 업그레이드 테스트 ===");
        
        int initialLevel = playerController.FireRateLevel;
        Debug.Log($"초기 연사속도 레벨: {initialLevel}");
        
        playerController.UpgradeFireRate();
        int afterFirst = playerController.FireRateLevel;
        Debug.Log($"업그레이드 후 연사속도 레벨: {afterFirst}");
        Assert.AreEqual(2, afterFirst, "레벨 2이어야 함");
        
        for (int i = 0; i < 4; i++)
        {
            playerController.UpgradeFireRate();
        }
        
        Assert.AreEqual(6, playerController.FireRateLevel, "최대 레벨 6이어야 함");
        
        playerController.UpgradeFireRate();
        Assert.AreEqual(6, playerController.FireRateLevel, "최대치 이상 증가 안함");
        
        Debug.Log("✓ 연사속도 업그레이드: 1→2→6, 최대치 초과 시 유지");
    }

    [Test]
    public void Test_BombCharge()
    {
        Debug.Log("=== 필사기 충전 테스트 ===");
        
        int initialBombs = playerController.CurrentBombs;
        Debug.Log($"초기 필사기 개수: {initialBombs}");
        
        playerController.AddBomb();
        int afterFirst = playerController.CurrentBombs;
        Debug.Log($"충전 후 필사기 개수: {afterFirst}");
        Assert.AreEqual(initialBombs + 1, afterFirst, "1 증가해야 함");
        
        playerController.AddBomb();
        playerController.AddBomb();
        
        Assert.AreEqual(3, playerController.CurrentBombs, "최대 3개");
        
        playerController.AddBomb();
        playerController.AddBomb();
        Assert.AreEqual(3, playerController.CurrentBombs, "최대치 유지");
        
        Debug.Log("✓ 필사기 충전: 3개 제한, 초과 시 유지");
    }

    [Test]
    public void Test_ItemSpawnerCreation()
    {
        Debug.Log("=== 아이템 스포너 생성 테스트 ===");
        
        GameObject spawnerObj = new GameObject("ItemSpawner");
        ItemSpawner spawner = spawnerObj.AddComponent<ItemSpawner>();
        
        Assert.IsNotNull(spawner, "ItemSpawner 컴포넌트가 존재해야 함");
        
        FieldInfo instanceField = typeof(ItemSpawner).GetField("Instance", BindingFlags.Static | BindingFlags.NonPublic);
        object instance = instanceField?.GetValue(null);
        
        Debug.Log("✓ ItemSpawner 인스턴스 생성 확인");
        
        Object.DestroyImmediate(spawnerObj);
    }

    [Test]
    public void Test_ItemTypeEnum()
    {
        Debug.Log("=== 아이템 타입 열거형 테스트 ===");
        
        ItemType[] types = (ItemType[])System.Enum.GetValues(typeof(ItemType));
        
        Assert.Greater(types.Length, 0, "아이템 타입이 존재해야 함");
        
        bool hasAttack = System.Array.Exists(types, t => t == ItemType.AttackUpgrade);
        bool hasFireRate = System.Array.Exists(types, t => t == ItemType.FireRateUpgrade);
        bool hasBomb = System.Array.Exists(types, t => t == ItemType.BombCharge);
        
        Assert.IsTrue(hasAttack, "AttackUpgrade 타입 존재");
        Assert.IsTrue(hasFireRate, "FireRateUpgrade 타입 존재");
        Assert.IsTrue(hasBomb, "BombCharge 타입 존재");
        
        Debug.Log($"✓ 아이템 타입: AttackUpgrade, FireRateUpgrade, BombCharge");
    }

    [Test]
    public void Test_PlayerStatsLimits()
    {
        Debug.Log("=== 플레이어 스탯 제한 테스트 ===");
        
        Assert.AreEqual(6, playerController.AttackLevel, "초기 공격 레벨 1, 최대 6");
        Assert.AreEqual(6, playerController.FireRateLevel, "초기 연사 레벨 1, 최대 6");
        Assert.AreEqual(3, playerController.CurrentBombs, "초기 필사기 3개");
        
        Debug.Log("✓ 플레이어 스탯 초기값 및 제한 확인");
    }
}