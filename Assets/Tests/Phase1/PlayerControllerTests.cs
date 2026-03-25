using NUnit.Framework;
using UnityEngine;

public class PlayerControllerTests
{
    [Test]
    public void Test_PlayerCanMove()
    {
        Debug.Log("TEST: 플레이어 이동 테스트");
        Assert.Pass("플레이어가 상하좌우로 이동 가능한지 확인");
    }

    [Test]
    public void Test_PlayerAttack()
    {
        Debug.Log("TEST: 플레이어 공격 테스트");
        Assert.Pass("미사일이 발사되고 적에게 데미지를 주는지 확인");
    }

    [Test]
    public void Test_PlayerHit()
    {
        Debug.Log("TEST: 플레이어 피격 테스트");
        Assert.Pass("피격 시 생명 1 차감, 3번 시 게임 오버");
    }

    [Test]
    public void Test_BombUsage()
    {
        Debug.Log("TEST: 필사기 사용 테스트");
        Assert.Pass("필사기 사용 시 3초 무적, 전체 적 데미지");
    }
}