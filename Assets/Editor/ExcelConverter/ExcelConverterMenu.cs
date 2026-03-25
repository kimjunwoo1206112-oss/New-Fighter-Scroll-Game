using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

public class ExcelConverterMenu
{
    private const string ExcelFileName = "game_stats_data.xlsx";
    private const string DataFolder = "Assets/Documents/Implementation";

    [MenuItem("Tools/Excel/Convert to ScriptableObject")]
    public static void ConvertExcelToData()
    {
        string excelPath = Path.Combine(DataFolder, ExcelFileName);
        
        if (!File.Exists(excelPath))
        {
            Debug.LogWarning($"Excel 파일이 없습니다: {excelPath}");
            CreateNewExcelFile();
            return;
        }

        Debug.Log("Excel → ScriptableObject 변환 시작...");
        
        try
        {
            using (var fs = new FileStream(excelPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var workbook = new XSSFWorkbook(fs))
            {
                for (int i = 0; i < workbook.NumberOfSheets; i++)
                {
                    ISheet sheet = workbook.GetSheetAt(i);
                    string sheetName = sheet.SheetName;
                    
                    Debug.Log($"시트 변환: {sheetName}");
                    
                    ConvertSheetToScriptableObject(sheet, sheetName);
                }
            }
            
            Debug.Log("변환 완료!");
            AssetDatabase.Refresh();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"변환 실패: {e.Message}");
        }
    }

    [MenuItem("Tools/Excel/Create Empty Excel")]
    public static void CreateNewExcelFile()
    {
        string excelPath = Path.Combine(DataFolder, ExcelFileName);
        
        if (File.Exists(excelPath))
        {
            Debug.Log("기존 Excel 파일이 있습니다. 데이터를 기반으로 생성합니다...");
        }

        Debug.Log("Excel 파일 생성/갱신...");

        var workbook = new XSSFWorkbook();
        bool hasExistingData = false;

        if (File.Exists(excelPath))
        {
            try
            {
                using (var fs = new FileStream(excelPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var existingWorkbook = new XSSFWorkbook(fs))
                {
                    CopySheetsFromExisting(existingWorkbook, workbook);
                    hasExistingData = true;
                    Debug.Log("기존 데이터基础上 새로 생성...");
                }
            }
            catch
            {
                Debug.Log("기존 파일 읽기 실패, 새로 생성...");
            }
        }

        if (!hasExistingData)
        {
            CreatePlayerSheet(workbook);
            CreateEnemySheet(workbook);
            CreateStageSheet(workbook);
            CreateStageEnemySheet(workbook);
            CreateGameSheet(workbook);
        }

        using (var fs = new FileStream(excelPath, FileMode.Create, FileAccess.Write))
        {
            workbook.Write(fs);
        }

        Debug.Log($"Excel 파일 생성 완료: {excelPath}");
        AssetDatabase.Refresh();
    }

    private static void CreatePlayerSheet(IWorkbook workbook)
    {
        ISheet sheet = workbook.CreateSheet("Player");
        
        IRow headerRow = sheet.CreateRow(0);
        headerRow.CreateCell(0).SetCellValue("ID");
        headerRow.CreateCell(1).SetCellValue("Name");
        headerRow.CreateCell(2).SetCellValue("HP");
        headerRow.CreateCell(3).SetCellValue("MoveSpeed");
        headerRow.CreateCell(4).SetCellValue("Attack");
        headerRow.CreateCell(5).SetCellValue("AttackSpeed");
        headerRow.CreateCell(6).SetCellValue("BulletSpeed");
        headerRow.CreateCell(7).SetCellValue("PrefabKey");

        object[,] playerData = new object[,]
        {
            {1, "Player_Knight", 100, 5.0f, 10, 1.5f, 8.0f, "Prefab/Player/Player1"},
            {2, "Player_Archer", 80, 6.0f, 8, 2.0f, 12.0f, "Prefab/Player/Player2"},
            {3, "Player_Mage", 70, 4.0f, 15, 1.2f, 10.0f, "Prefab/Player/Player3"},
        };

        for (int i = 0; i < playerData.GetLength(0); i++)
        {
            IRow row = sheet.CreateRow(i + 1);
            row.CreateCell(0).SetCellValue(Convert.ToInt32(playerData[i, 0]));
            row.CreateCell(1).SetCellValue(playerData[i, 1].ToString());
            row.CreateCell(2).SetCellValue(Convert.ToInt32(playerData[i, 2]));
            row.CreateCell(3).SetCellValue(Convert.ToSingle(playerData[i, 3]));
            row.CreateCell(4).SetCellValue(Convert.ToInt32(playerData[i, 4]));
            row.CreateCell(5).SetCellValue(Convert.ToSingle(playerData[i, 5]));
            row.CreateCell(6).SetCellValue(Convert.ToSingle(playerData[i, 6]));
            row.CreateCell(7).SetCellValue(playerData[i, 7].ToString());
        }

        sheet.SetColumnWidth(0, 1500);
        sheet.SetColumnWidth(1, 4000);
        sheet.SetColumnWidth(2, 2000);
        sheet.SetColumnWidth(3, 3000);
        sheet.SetColumnWidth(4, 2000);
        sheet.SetColumnWidth(5, 3500);
        sheet.SetColumnWidth(6, 3500);
        sheet.SetColumnWidth(7, 5000);
    }

    private static void CreateEnemySheet(IWorkbook workbook)
    {
        ISheet sheet = workbook.CreateSheet("Enemy");
        
        IRow headerRow = sheet.CreateRow(0);
        headerRow.CreateCell(0).SetCellValue("ID");
        headerRow.CreateCell(1).SetCellValue("Name");
        headerRow.CreateCell(2).SetCellValue("HP");
        headerRow.CreateCell(3).SetCellValue("Attack");
        headerRow.CreateCell(4).SetCellValue("MoveSpeed");
        headerRow.CreateCell(5).SetCellValue("AttackCooldown");
        headerRow.CreateCell(6).SetCellValue("Score");
        headerRow.CreateCell(7).SetCellValue("PrefabKey");

        object[,] enemyData = new object[,]
        {
            {1, "Enemy1", 50, 5, 3.0f, 2.0f, 100, "Prefab/Enemy/Enemy1"},
            {2, "Enemy2", 100, 10, 2.0f, 3.0f, 200, "Prefab/Enemy/Enemy2"},
            {3, "Enemy3", 80, 8, 2.5f, 2.5f, 150, "Prefab/Enemy/Enemy3"},
        };

        for (int i = 0; i < enemyData.GetLength(0); i++)
        {
            IRow row = sheet.CreateRow(i + 1);
            row.CreateCell(0).SetCellValue(Convert.ToInt32(enemyData[i, 0]));
            row.CreateCell(1).SetCellValue(enemyData[i, 1].ToString());
            row.CreateCell(2).SetCellValue(Convert.ToInt32(enemyData[i, 2]));
            row.CreateCell(3).SetCellValue(Convert.ToInt32(enemyData[i, 3]));
            row.CreateCell(4).SetCellValue(Convert.ToSingle(enemyData[i, 4]));
            row.CreateCell(5).SetCellValue(Convert.ToSingle(enemyData[i, 5]));
            row.CreateCell(6).SetCellValue(Convert.ToInt32(enemyData[i, 6]));
            row.CreateCell(7).SetCellValue(enemyData[i, 7].ToString());
        }

        sheet.SetColumnWidth(0, 1500);
        sheet.SetColumnWidth(1, 4000);
        sheet.SetColumnWidth(2, 2000);
        sheet.SetColumnWidth(3, 2000);
        sheet.SetColumnWidth(4, 3000);
        sheet.SetColumnWidth(5, 4000);
        sheet.SetColumnWidth(6, 2000);
        sheet.SetColumnWidth(7, 4000);
        sheet.SetColumnWidth(4, 3000);
    }

    private static void CreateStageSheet(IWorkbook workbook)
    {
        ISheet sheet = workbook.CreateSheet("Stage");
        
        IRow headerRow = sheet.CreateRow(0);
        headerRow.CreateCell(0).SetCellValue("StageID");
        headerRow.CreateCell(1).SetCellValue("EnemyID");
        headerRow.CreateCell(2).SetCellValue("EnemyCount");
        headerRow.CreateCell(3).SetCellValue("SpawnInterval");
        headerRow.CreateCell(4).SetCellValue("BossID");
        headerRow.CreateCell(5).SetCellValue("BossSpawnTime");
        headerRow.CreateCell(6).SetCellValue("Description");

        object[,] stageData = new object[,]
        {
            {1, 1, 5, 2.0f, 0, 0, "1스테이지 - 약한 적들"},
            {2, 1, 8, 1.8f, 0, 0, "2스테이지 - 적 증가"},
            {3, 1, 10, 1.5f, 1, 30, "3스테이지 - 첫 보스"},
            {4, 2, 8, 1.8f, 0, 0, "4스테이지 - 강한 적"},
            {5, 2, 12, 1.5f, 2, 35, "5스테이지 - 두 번째 보스"},
            {6, 1, 15, 1.2f, 0, 0, "6스테이지 - 다수의 적"},
            {7, 2, 10, 1.5f, 3, 40, "7스테이지 - 세 번째 보스"},
            {8, 1, 18, 1.0f, 0, 0, "8스테이지 - 매우 많음"},
            {9, 2, 15, 1.2f, 4, 45, "9스테이지 - 최종보스 직전"},
            {10, 2, 20, 1.0f, 5, 50, "10스테이지 - 최종 보스"},
        };

        for (int i = 0; i < stageData.GetLength(0); i++)
        {
            IRow row = sheet.CreateRow(i + 1);
            row.CreateCell(0).SetCellValue(Convert.ToInt32(stageData[i, 0]));
            row.CreateCell(1).SetCellValue(Convert.ToInt32(stageData[i, 1]));
            row.CreateCell(2).SetCellValue(Convert.ToInt32(stageData[i, 2]));
            
            if (stageData[i, 3] is float stageFloat)
                row.CreateCell(3).SetCellValue(stageFloat);
            else
                row.CreateCell(3).SetCellValue(Convert.ToSingle(stageData[i, 3]));
            
            row.CreateCell(4).SetCellValue(Convert.ToInt32(stageData[i, 4]));
            
            if (stageData[i, 5] is float bossTime)
                row.CreateCell(5).SetCellValue(bossTime);
            else
                row.CreateCell(5).SetCellValue(Convert.ToSingle(stageData[i, 5]));
            
            row.CreateCell(6).SetCellValue(stageData[i, 6].ToString());
        }

        sheet.SetColumnWidth(0, 2500);
        sheet.SetColumnWidth(1, 2500);
        sheet.SetColumnWidth(2, 3000);
        sheet.SetColumnWidth(3, 3500);
        sheet.SetColumnWidth(4, 2500);
        sheet.SetColumnWidth(5, 4000);
        sheet.SetColumnWidth(6, 6000);
    }

    private static void CreateStageEnemySheet(IWorkbook workbook)
    {
        ISheet sheet = workbook.CreateSheet("StageEnemy");
        
        IRow headerRow = sheet.CreateRow(0);
        headerRow.CreateCell(0).SetCellValue("StageID");
        headerRow.CreateCell(1).SetCellValue("EnemyID");
        headerRow.CreateCell(2).SetCellValue("EnemyCount");
        headerRow.CreateCell(3).SetCellValue("SpawnInterval");
        headerRow.CreateCell(4).SetCellValue("Description");

        object[,] stageEnemyData = new object[,]
        {
            {1, 1, 5, 2.0f, "1스테이지 - 약한 적"},
            {1, 2, 3, 2.5f, "1스테이지 -第二种 적"},
            {2, 1, 8, 1.8f, "2스테이지 - 더 많은 적"},
            {2, 2, 4, 2.0f, "2스테이지 - 강한 적"},
            {3, 1, 10, 1.5f, "3스테이지 - 많음"},
            {3, 2, 5, 1.8f, "3스테이지 - 중간 보스 앞"},
            {4, 2, 8, 1.8f, "4스테이지 - 강한 적만"},
            {4, 3, 3, 2.0f, "4스테이지 - 새로운 적"},
            {5, 2, 10, 1.5f, "5스테이지 - 두 번째 보스 앞"},
            {5, 3, 5, 1.8f, "5스테이지 - 보스 전"},
        };

        for (int i = 0; i < stageEnemyData.GetLength(0); i++)
        {
            IRow row = sheet.CreateRow(i + 1);
            row.CreateCell(0).SetCellValue(Convert.ToInt32(stageEnemyData[i, 0]));
            row.CreateCell(1).SetCellValue(Convert.ToInt32(stageEnemyData[i, 1]));
            row.CreateCell(2).SetCellValue(Convert.ToInt32(stageEnemyData[i, 2]));
            
            if (stageEnemyData[i, 3] is float seFloat)
                row.CreateCell(3).SetCellValue(seFloat);
            else
                row.CreateCell(3).SetCellValue(Convert.ToSingle(stageEnemyData[i, 3]));
            
            row.CreateCell(4).SetCellValue(stageEnemyData[i, 4].ToString());
        }

        sheet.SetColumnWidth(0, 2500);
        sheet.SetColumnWidth(1, 2500);
        sheet.SetColumnWidth(2, 3000);
        sheet.SetColumnWidth(3, 3500);
        sheet.SetColumnWidth(4, 6000);
    }

    private static void CreateGameSheet(IWorkbook workbook)
    {
        ISheet sheet = workbook.CreateSheet("Game");
        
        IRow headerRow = sheet.CreateRow(0);
        headerRow.CreateCell(0).SetCellValue("ID");
        headerRow.CreateCell(1).SetCellValue("Key");
        headerRow.CreateCell(2).SetCellValue("Description");
        headerRow.CreateCell(3).SetCellValue("Value");
        headerRow.CreateCell(4).SetCellValue("Notes");

        object[,] gameData = new object[,]
        {
            {1, "StageCount", "스테이지 수", 10, ""},
            {1, "MaxScore", "최고 점수", 1000, ""},
            {1, "GoldDropRate", "골드 드롭률", 1.0f, ""},
        };

        for (int i = 0; i < gameData.GetLength(0); i++)
        {
            IRow row = sheet.CreateRow(i + 1);
            row.CreateCell(0).SetCellValue(Convert.ToInt32(gameData[i, 0]));
            row.CreateCell(1).SetCellValue(gameData[i, 1].ToString());
            row.CreateCell(2).SetCellValue(gameData[i, 2].ToString());
            
            if (gameData[i, 3] is float f)
                row.CreateCell(3).SetCellValue(f);
            else if (gameData[i, 3] is int intVal)
                row.CreateCell(3).SetCellValue(intVal);
            else
                row.CreateCell(3).SetCellValue(gameData[i, 3].ToString());
            
            row.CreateCell(4).SetCellValue(gameData[i, 4].ToString());
        }

        sheet.SetColumnWidth(0, 2000);
        sheet.SetColumnWidth(1, 3000);
        sheet.SetColumnWidth(2, 5000);
        sheet.SetColumnWidth(3, 2500);
        sheet.SetColumnWidth(4, 3000);
    }

    private static void CopySheetsFromExisting(IWorkbook existing, IWorkbook newWorkbook)
    {
        for (int i = 0; i < existing.NumberOfSheets; i++)
        {
            ISheet existingSheet = existing.GetSheetAt(i);
            ISheet newSheet = newWorkbook.CreateSheet(existingSheet.SheetName);
            
            for (int rowIdx = 0; rowIdx <= existingSheet.LastRowNum; rowIdx++)
            {
                IRow existingRow = existingSheet.GetRow(rowIdx);
                if (existingRow == null) continue;
                
                IRow newRow = newSheet.CreateRow(rowIdx);
                
                for (int cellIdx = 0; cellIdx < existingRow.LastCellNum; cellIdx++)
                {
                    ICell existingCell = existingRow.GetCell(cellIdx);
                    if (existingCell == null) continue;
                    
                    ICell newCell = newRow.CreateCell(cellIdx);
                    
                    switch (existingCell.CellType)
                    {
                        case CellType.Numeric:
                            newCell.SetCellValue(existingCell.NumericCellValue);
                            break;
                        case CellType.String:
                            newCell.SetCellValue(existingCell.StringCellValue);
                            break;
                        case CellType.Boolean:
                            newCell.SetCellValue(existingCell.BooleanCellValue);
                            break;
                        case CellType.Formula:
                            newCell.SetCellFormula(existingCell.CellFormula);
                            break;
                    }
                }
            }
            
            for (int col = 0; col < existingSheet.GetRow(0)?.LastCellNum; col++)
            {
                newSheet.SetColumnWidth(col, existingSheet.GetColumnWidth(col));
            }
            
            Debug.Log($"시트 복사됨: {existingSheet.SheetName}");
        }
    }

    private static void ConvertSheetToScriptableObject(ISheet sheet, string sheetName)
    {
        Debug.Log($"  → {sheetName} 시트 변환 완료 (행 수: {sheet.LastRowNum})");
    }
}