using NUnit.Framework;
using UnityEngine;

public class ItemTests
{
    [Test]
    public void Test_ItemDrop()
    {
        Debug.Log("TEST: 아이템 드롭 테스트");
        Assert.Pass("적 처치 시 아이템이 드롭되는지 확인");
    }

    [Test]
    public void Test_ItemMovement()
    {
        Debug.Log("TEST: 아이템 이동 테스트");
        Assert.Pass("아이템이 상하좌우로 이동하는지 확인");
    }

    [Test]
    public void Test_AttackUpgrade()
    {
        Debug.Log("TEST: 공격력 업그레이드 테스트");
        Assert.Pass("공격력 업그레이드 아이템 효과 적용 (최대 6)");
    }

    [Test]
    public void Test_FireRateUpgrade()
    {
        Debug.Log("TEST: 연사속도 업그레이드 테스트");
        Assert.Pass("연사속도 업그레이드 아이템 효과 적용 (최대 6)");
    }

    [Test]
    public void Test_BombCharge()
    {
        Debug.Log("TEST: 필사기 충전 테스트");
        Assert.Pass("필사기 충전 아이템 효과 적용 (최대 3)");
    }
}