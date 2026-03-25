using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

public class ExcelDataManager : MonoBehaviour
{
    public static ExcelDataManager Instance { get; private set; }

    private Dictionary<string, Dictionary<string, string>> gameData = new Dictionary<string, Dictionary<string, string>>();
    private string excelFilePath;
    private string csvFilePath;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            excelFilePath = Path.Combine(Application.dataPath, "Documents", "Implementation", "game_stats_data.xlsx");
            csvFilePath = Path.Combine(Application.dataPath, "Documents", "Implementation", "game_stats_data.csv");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadGameData();
    }

    private void LoadGameData()
    {
        if (File.Exists(excelFilePath))
        {
            LoadFromExcel();
        }
        else if (File.Exists(csvFilePath))
        {
            LoadFromCsv();
        }
        else
        {
            Debug.Log("새 Excel 파일 생성...");
            CreateInitialData();
            SaveToExcel();
        }
    }

    private void LoadFromExcel()
    {
        try
        {
            using (var fs = new FileStream(excelFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var workbook = new XSSFWorkbook(fs))
            {
                int sheetCount = workbook.NumberOfSheets;
                Debug.Log($"엑셀 시트 수: {sheetCount}");

                for (int s = 0; s < sheetCount; s++)
                {
                    ISheet sheet = workbook.GetSheetAt(s);
                    string sheetName = sheet.SheetName;
                    Debug.Log($"시트 로드: {sheetName}");

                    if (sheet == null) continue;

                    for (int i = 1; i <= sheet.LastRowNum; i++)
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null || row.LastCellNum < 2) continue;

                        string id = GetCellValue(row, 0);
                        string key = GetCellValue(row, 1);
                        string value = GetCellValue(row, 3);

                        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(key)) continue;

                        // 따옴표 제거
                        id = id.Replace("\"", "");

                        // ID가 숫자이면 Player 카테고리로 처리
                        string category;
                        string fullKey;

                        if (int.TryParse(id, out int playerNum))
                        {
                            category = "Player";
                            fullKey = $"{playerNum}_{key}";
                        }
                        else
                        {
                            // 숫자가 아니면 해당 값이 카테고리
                            category = id;
                            fullKey = key;
                        }

                        if (!gameData.ContainsKey(category))
                        {
                            gameData[category] = new Dictionary<string, string>();
                        }

                        gameData[category][fullKey] = value;
                        Debug.Log($"[Excel] {category}.{fullKey} = {value}");
                    }
                }
            }

            Debug.Log("Excel 데이터 로드 완료!");
            PrintAllData();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Excel 로드 실패: {e.Message}");
            LoadFromCsv();
        }
    }

    private string GetCellValue(IRow row, int cellIndex)
    {
        ICell cell = row.GetCell(cellIndex);
        if (cell == null) return string.Empty;

        return cell.CellType switch
        {
            CellType.Numeric => cell.NumericCellValue.ToString(),
            CellType.Boolean => cell.BooleanCellValue.ToString(),
            CellType.String => cell.StringCellValue,
            _ => cell.ToString()
        };
    }

    private void LoadFromCsv()
    {
        if (File.Exists(csvFilePath))
        {
            Debug.Log("CSV 파일 로드...");
            
            string[] lines = File.ReadAllLines(csvFilePath);
            
            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i];
                if (string.IsNullOrWhiteSpace(line)) continue;

                string[] data = ParseCsvLine(line);
                if (data.Length < 4) continue;

                string category = data[0];
                string key = data[1];
                string value = data[3];

                if (string.IsNullOrEmpty(category) || string.IsNullOrEmpty(key)) continue;

                if (!gameData.ContainsKey(category))
                {
                    gameData[category] = new Dictionary<string, string>();
                }

                gameData[category][key] = value;
            }
            
            Debug.Log("CSV 데이터 로드 완료!");
            PrintAllData();
        }
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

    private void CreateInitialData()
    {
        gameData["Player"] = new Dictionary<string, string>
        {
            { "HP", "100" },
            { "Speed", "5" },
            { "Damage", "10" }
        };
        gameData["Enemy"] = new Dictionary<string, string>
        {
            { "HP", "50" },
            { "Speed", "3" }
        };
        gameData["Game"] = new Dictionary<string, string>
        {
            { "StageCount", "10" },
            { "MaxScore", "1000" }
        };
    }

    public string GetValue(string category, string key, string defaultValue = "")
    {
        if (gameData.ContainsKey(category) && gameData[category].ContainsKey(key))
        {
            Debug.Log($"[GetValue] {category}.{key} = {gameData[category][key]}");
            return gameData[category][key];
        }
        Debug.LogWarning($"[GetValue] {category}.{key} 없음, 기본값 사용: {defaultValue}");
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

    public void SaveData(string category, string key, string value)
    {
        if (!gameData.ContainsKey(category))
        {
            gameData[category] = new Dictionary<string, string>();
        }
        gameData[category][key] = value;
        SaveToExcel();
    }

    public void DeleteData(string category, string key)
    {
        if (gameData.ContainsKey(category) && gameData[category].ContainsKey(key))
        {
            gameData[category].Remove(key);
            if (gameData[category].Count == 0)
            {
                gameData.Remove(category);
            }
            SaveToExcel();
        }
    }

    private void SaveToExcel()
    {
        try
        {
            var workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("Data");

            IRow headerRow = sheet.CreateRow(0);
            headerRow.CreateCell(0).SetCellValue("Category");
            headerRow.CreateCell(1).SetCellValue("Key");
            headerRow.CreateCell(2).SetCellValue("Description");
            headerRow.CreateCell(3).SetCellValue("Value");
            headerRow.CreateCell(4).SetCellValue("Notes");

            int rowIndex = 1;
            foreach (var category in gameData)
            {
                foreach (var item in category.Value)
                {
                    IRow row = sheet.CreateRow(rowIndex++);
                    row.CreateCell(0).SetCellValue(category.Key);
                    row.CreateCell(1).SetCellValue(item.Key);
                    row.CreateCell(2).SetCellValue("");
                    row.CreateCell(3).SetCellValue(item.Value);
                    row.CreateCell(4).SetCellValue("");
                }
            }

            using (var fs = new FileStream(excelFilePath, FileMode.Create, FileAccess.Write))
            {
                workbook.Write(fs);
            }

            SaveToCsv();
            Debug.Log($"Excel 저장 완료: {excelFilePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Excel 저장 실패: {e.Message}");
            SaveToCsv();
        }
    }

    private void SaveToCsv()
    {
        try
        {
            List<string> lines = new List<string>();
            lines.Add("Category,Key,Description,Value,Notes");

            foreach (var category in gameData)
            {
                foreach (var item in category.Value)
                {
                    string line = $"\"{category.Key}\",\"{item.Key}\",\"\",\"{EscapeCsvValue(item.Value)}\",\"\"";
                    lines.Add(line);
                }
            }

            File.WriteAllLines(csvFilePath, lines);
            Debug.Log($"CSV 저장 완료: {csvFilePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"CSV 저장 실패: {e.Message}");
        }
    }

    private string EscapeCsvValue(string value)
    {
        if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
        {
            return value.Replace("\"", "\"\"");
        }
        return value;
    }

    public Dictionary<string, Dictionary<string, string>> GetAllData()
    {
        return gameData;
    }

    public bool HasCategory(string category)
    {
        return gameData.ContainsKey(category);
    }

    public bool HasKey(string category, string key)
    {
        return gameData.ContainsKey(category) && gameData[category].ContainsKey(key);
    }

    public void ClearCategory(string category)
    {
        if (gameData.ContainsKey(category))
        {
            gameData.Remove(category);
            SaveToExcel();
        }
    }

    public void ClearAllData()
    {
        gameData.Clear();
        SaveToExcel();
    }

    // 모든 데이터를 엑셀 파일로 내보내기 (시트별 분리)
    public void ExportAllToExcel()
    {
        try
        {
            var workbook = new XSSFWorkbook();

            // 각 카테고리를 별도 시트로 저장
            foreach (var category in gameData)
            {
                ISheet sheet = workbook.CreateSheet(category.Key);

                // 헤더
                IRow headerRow = sheet.CreateRow(0);
                headerRow.CreateCell(0).SetCellValue("Key");
                headerRow.CreateCell(1).SetCellValue("Value");

                // 데이터
                int rowIndex = 1;
                foreach (var item in category.Value)
                {
                    IRow row = sheet.CreateRow(rowIndex++);
                    row.CreateCell(0).SetCellValue(item.Key);
                    row.CreateCell(1).SetCellValue(item.Value);
                }
            }

            using (var fs = new FileStream(excelFilePath, FileMode.Create, FileAccess.Write))
            {
                workbook.Write(fs);
            }

            Debug.Log($"엑셀 내보내기 완료: {excelFilePath}");
            Debug.Log($"총 {gameData.Count}개 시트 저장됨");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"엑셀 내보내기 실패: {e.Message}");
        }
    }

    // 엑셀 파일에서 모든 데이터 가져오기
    public void ImportAllFromExcel()
    {
        gameData.Clear();
        LoadGameData();
        Debug.Log($"엑셀 가져오기 완료: {gameData.Count}개 카테고리 로드됨");
    }

    // 특정 카테고리 데이터 엑셀로 저장
    public void ExportCategoryToExcel(string category)
    {
        if (!gameData.ContainsKey(category))
        {
            Debug.LogWarning($"카테고리 '{category}'가 없습니다.");
            return;
        }

        try
        {
            var workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet(category);

            IRow headerRow = sheet.CreateRow(0);
            headerRow.CreateCell(0).SetCellValue("Key");
            headerRow.CreateCell(1).SetCellValue("Value");

            int rowIndex = 1;
            foreach (var item in gameData[category])
            {
                IRow row = sheet.CreateRow(rowIndex++);
                row.CreateCell(0).SetCellValue(item.Key);
                row.CreateCell(1).SetCellValue(item.Value);
            }

            string path = Path.Combine(Application.dataPath, "Documents", "Implementation", $"{category}_data.xlsx");
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                workbook.Write(fs);
            }

            Debug.Log($"카테고리 '{category}' 엑셀 저장 완료: {path}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"카테고리 엑셀 저장 실패: {e.Message}");
        }
    }

    // 모든 카테고리 목록 반환
    public List<string> GetAllCategories()
    {
        return new List<string>(gameData.Keys);
    }
}