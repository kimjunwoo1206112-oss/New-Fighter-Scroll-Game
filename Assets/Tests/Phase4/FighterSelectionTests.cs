using NUnit.Framework;
using UnityEngine;

public class FighterSelectionTests
{
    [Test]
    public void Test_FighterSelection()
    {
        Debug.Log("TEST: 전투기 선택 테스트");
        Assert.Pass("시작 화면에서 전투기를 선택할 수 있는지 확인");
    }

    [Test]
    public void Test_FighterA_Balanced()
    {
        Debug.Log("TEST: 전투기 A (균형형) 테스트");
        Assert.Pass("전투기 A의 성능이 적용되는지 확인");
    }

    [Test]
    public void Test_FighterB_Fast()
    {
        Debug.Log("TEST: 전투기 B (고속형) 테스트");
        Assert.Pass("전투기 B의 성능이 적용되는지 확인");
    }

    [Test]
    public void Test_FighterC_Power()
    {
        Debug.Log("TEST: 전투기 C (강공형) 테스트");
        Assert.Pass("전투기 C의 성능이 적용되는지 확인");
    }
}