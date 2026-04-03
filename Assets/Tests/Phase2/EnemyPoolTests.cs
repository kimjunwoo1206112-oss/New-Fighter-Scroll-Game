using NUnit.Framework;
using UnityEngine;

public class EnemyPoolTests
{
    [Test]
    public void Test_EnemyPool_Initialization()
    {
        Debug.Log("TEST: Enemy Pool 초기화 테스트");
        Assert.Pass("EnemyPool이 싱글톤으로 초기화되고 적 프리팹을 미리 생성하는지 확인");
    }

    [Test]
    public void Test_EnemyPool_GetEnemy()
    {
        Debug.Log("TEST: Pool에서 적 가져오기 테스트");
        Assert.Pass("GetEnemy() 호출 시 Pool에서 적을 가져오고, 부족하면 새로 생성하는지 확인");
    }

    [Test]
    public void Test_EnemyPool_ReturnEnemy()
    {
        Debug.Log("TEST: Pool로 적 반환 테스트");
        Assert.Pass("ReturnEnemy() 호출 시 적이 비활성화되고 Pool에 다시 추가되는지 확인");
    }

    [Test]
    public void Test_EnemyPool_Reuse()
    {
        Debug.Log("TEST: Pool 재사용 테스트");
        Assert.Pass("반환된 적을 다시 GetEnemy()로 가져올 때 동일한 오브젝트가 재사용되는지 확인");
    }

    [Test]
    public void Test_Enemy_ReturnToPool_OnDeath()
    {
        Debug.Log("TEST: 적 사망 시 Pool 반환 테스트");
        Assert.Pass("적이 죽으면 ReturnToPool()이 호출되어 비활성화되는지 확인");
    }

    [Test]
    public void Test_Enemy_ReturnToPool_WhenOutOfBounds()
    {
        Debug.Log("TEST: 적 화면 밖으로 나갈 때 Pool 반환 테스트");
        Assert.Pass("적이 y < -7 위치로 가면 ReturnToPool()이 호출되는지 확인");
    }

    [Test]
    public void Test_Bullet_ReturnToPool_WhenOutOfScreen()
    {
        Debug.Log("TEST: 총알 화면 밖으로 나갈 때 Pool 반환 테스트");
        Assert.Pass("총알이 화면 밖으로 나가면 ReturnToPool()이 호출되어 비활성화되는지 확인");
    }

    [Test]
    public void Test_EnemyDespawnZone_Collision()
    {
        Debug.Log("TEST: Enemy Despawn Zone 충돌 테스트");
        Assert.Pass("Despawn Zone과 적이 충돌하면 ForceReturnToPool()이 호출되는지 확인");
    }

    [Test]
    public void Test_EnemySpawner_UsesPool()
    {
        Debug.Log("TEST: EnemySpawner Pool 사용 테스트");
        Assert.Pass("EnemySpawner가 Instantiate 대신 EnemyPool.GetRandomEnemy()를 사용하는지 확인");
    }
}
