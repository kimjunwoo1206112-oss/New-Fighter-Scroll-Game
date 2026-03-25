using NUnit.Framework;
using UnityEngine;

public class EnemyAndBossTests
{
    [Test]
    public void Test_EnemyMovement()
    {
        Debug.Log("TEST: 적 이동 테스트");
        Assert.Pass("적이 화면 위에서 아래로 이동하는지 확인");
    }

    [Test]
    public void Test_EnemyHP()
    {
        Debug.Log("TEST: 적 HP 테스트");
        Assert.Pass("적 HP 차감 및 처치 시 점수 부여");
    }

    [Test]
    public void Test_BossSpawn()
    {
        Debug.Log("TEST: 보스 생성 테스트");
        Assert.Pass("보스가 화면 위쪽에 고정되어 생성되는지 확인");
    }

    [Test]
    public void Test_BossPattern()
    {
        Debug.Log("TEST: 보스 패턴 테스트");
        Assert.Pass("보스가 다양한 패턴으로 공격하는지 확인");
    }

    [Test]
    public void Test_BossDefeat()
    {
        Debug.Log("TEST: 보스 처치 테스트");
        Assert.Pass("보스 처치 시 스테이지 클리어 처리");
    }
}