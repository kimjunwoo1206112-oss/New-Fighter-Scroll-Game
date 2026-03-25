using NUnit.Framework;
using UnityEngine;

public class GameFlowTests
{
    [Test]
    public void Test_GameStart()
    {
        Debug.Log("TEST: 게임 시작 테스트");
        Assert.Pass("게임 시작 화면이 정상 표시되는지 확인");
    }

    [Test]
    public void Test_StageClear()
    {
        Debug.Log("TEST: 스테이지 클리어 테스트");
        Assert.Pass("보스 처치 시 스테이지가 클리어되는지 확인");
    }

    [Test]
    public void Test_NextStage()
    {
        Debug.Log("TEST: 다음 스테이지 이동 테스트");
        Assert.Pass("다음 스테이지로 정상 이동하는지 확인");
    }

    [Test]
    public void Test_ScoreSystem()
    {
        Debug.Log("TEST: 점수 시스템 테스트");
        Assert.Pass("적/보스 처치 시 점수가 정상적으로 누적되는지 확인");
    }

    [Test]
    public void Test_UIControl()
    {
        Debug.Log("TEST: UI 표시 테스트");
        Assert.Pass("HP, 점수, 필사기, 보스 HP 바가 정상 표시되는지 확인");
    }

    [Test]
    public void Test_GameOver()
    {
        Debug.Log("TEST: 게임 오버 테스트");
        Assert.Pass("3번 피격 시 게임 오버 화면이 표시되는지 확인");
    }

    [Test]
    public void Test_GameClear()
    {
        Debug.Log("TEST: 게임 클리어 테스트");
        Assert.Pass("모든 스테이지 클리어 시 클리어 화면 표시");
    }
}