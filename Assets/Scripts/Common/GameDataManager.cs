using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance { get; private set; }

    private Dictionary<string, Dictionary<int, Dictionary<string, string>>> idBasedData = new Dictionary<string, Dictionary<int, Dictionary<string, string>>>();
    private Dictionary<string, Dictionary<string, string>> gameData = new Dictionary<string, Dictionary<string, string>>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadGameData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadGameData()
    {
        string filePath = Path.Combine(Application.dataPath, "Documents", "Implementation", "game_stats_data.csv");
        
        if (!File.Exists(filePath))
        {
            Debug.LogWarning($"CSV 파일을 찾을 수 없습니다: {filePath}");
            return;
        }

        string[] lines = File.ReadAllLines(filePath);
        
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] data = ParseCsvLine(line);
            if (data.Length < 4) continue;

            string category = data[0];
            string key = data[1];
            string value = data[3];

            if (string.IsNullOrEmpty(category)) continue;

            if (!gameData.ContainsKey(category))
            {
                gameData[category] = new Dictionary<string, string>();
            }

            gameData[category][key] = value;
        }

        Debug.Log("게임 데이터 로드 완료!");
        PrintAllData();
    }

    private string[] ParseCsvLine(string line)
    {
        List<string> result = new List<string>();
        bool inQuotes = false;
        string current = "";

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ',' && !inQuotes)
            {
                result.Add(current.Trim());
                current = "";
            }
            else
            {
                current += c;
            }
        }

        result.Add(current.Trim());
        return result.ToArray();
    }

    public string GetValue(string category, string key, string defaultValue = "")
    {
        if (gameData.ContainsKey(category) && gameData[category].ContainsKey(key))
        {
            return gameData[category][key];
        }
        return defaultValue;
    }

    public int GetIntValue(string category, string key, int defaultValue = 0)
    {
        string value = GetValue(category, key);
        if (int.TryParse(value, out int result))
        {
            return result;
        }
        return defaultValue;
    }

    public float GetFloatValue(string category, string key, float defaultValue = 0f)
    {
        string value = GetValue(category, key);
        if (float.TryParse(value, out float result))
        {
            return result;
        }
        return defaultValue;
    }

    public int GetPlayerStatInt(string statKey, int defaultValue = 0)
    {
        string value = GetPlayerStat(statKey);
        if (int.TryParse(value, out int result))
        {
            return result;
        }
        return defaultValue;
    }

    public float GetPlayerStatFloat(string statKey, float defaultValue = 0f)
    {
        string value = GetPlayerStat(statKey);
        if (float.TryParse(value, out float result))
        {
            return result;
        }
        return defaultValue;
    }

    public string GetPlayerStat(string statKey, string defaultValue = "")
    {
        int id = PlayerDataManager.Instance != null ? PlayerDataManager.Instance.SelectedPlayerId : 1;
        
        if (gameData.ContainsKey("Player"))
        {
            var playerRow = gameData["Player"].FirstOrDefault(x => x.Key == $"ID_{id}_{statKey}");
            if (!playerRow.Equals(default(KeyValuePair<string, string>)))
            {
                return playerRow.Value;
            }
            
            var nameRow = gameData["Player"].FirstOrDefault(x => x.Key == $"ID_{id}_Name");
            if (!nameRow.Equals(default(KeyValuePair<string, string>)))
            {
                Debug.Log($"플레이어 {nameRow.Value} 선택됨, {statKey}查找...");
            }
        }
        return defaultValue;
    }

    private void PrintAllData()
    {
        foreach (var category in gameData)
        {
            Debug.Log($"=== {category.Key} ===");
            foreach (var item in category.Value)
            {
                Debug.Log($"  {item.Key}: {item.Value}");
            }
        }
    }

    public void ReloadData()
    {
        gameData.Clear();
        LoadGameData();
    }
}