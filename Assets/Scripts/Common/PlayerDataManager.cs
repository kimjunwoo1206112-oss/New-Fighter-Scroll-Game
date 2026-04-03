using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance { get; private set; }

    private int selectedPlayerId = 1;
    private int currentStage = 1;
    
    private readonly int[] shipHP =     { 100, 80, 70 };
    private readonly float[] shipSpeed = { 5.0f, 6.0f, 4.0f };
    private readonly int[] shipAtk =     { 3, 2, 5 };
    private readonly int[] shipFireRate = { 3, 5, 2 };

    public int SelectedPlayerId => selectedPlayerId;
    public int CurrentStage => currentStage;
    public int CurrentHP => shipHP[selectedPlayerId - 1];
    public float CurrentSpeed => shipSpeed[selectedPlayerId - 1];
    public int CurrentAttack => shipAtk[selectedPlayerId - 1];
    public int CurrentFireRate => shipFireRate[selectedPlayerId - 1];

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadPlayerData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SavePlayerData()
    {
        PlayerPrefs.SetInt("SelectedPlayerId", selectedPlayerId);
        PlayerPrefs.SetInt("CurrentStage", currentStage);
        PlayerPrefs.Save();
        Debug.Log("플레이어 데이터 저장됨");
    }

    public void LoadPlayerData()
    {
        selectedPlayerId = PlayerPrefs.GetInt("SelectedPlayerId", 1);
        currentStage = PlayerPrefs.GetInt("CurrentStage", 1);
        Debug.Log($"플레이어 데이터 로드됨: ID {selectedPlayerId}, Stage {currentStage}");
    }

    public void ClearPlayerData()
    {
        PlayerPrefs.DeleteKey("SelectedPlayerId");
        PlayerPrefs.DeleteKey("CurrentStage");
        PlayerPrefs.Save();
        ResetGame();
    }

    public void SelectPlayer(int playerId)
    {
        selectedPlayerId = playerId;
        SavePlayerData();
        Debug.Log($"플레이어 선택: ID {playerId}");
    }

    public void SetStage(int stage)
    {
        currentStage = stage;
        SavePlayerData();
    }

    public void NextStage()
    {
        currentStage++;
    }

    public void ResetGame()
    {
        selectedPlayerId = 1;
        currentStage = 1;
    }
}