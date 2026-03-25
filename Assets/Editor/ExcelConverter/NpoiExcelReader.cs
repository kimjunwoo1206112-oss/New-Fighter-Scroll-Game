using System;
using System.Collections.Generic;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using UnityEngine;

/// <summary>
/// NPOI 2.7.x (NuGet) 기반 IExcelReader 구현체.
/// 
/// NuGet 패키지 "NPOI" 가 Assets/Packages/NPOI.x.x.x/ 에 설치되어 있어야 합니다.
/// 네임스페이스는 기존 NPOI 와 동일합니다.
/// </summary>
public class NpoiExcelReader : IExcelReader, IDisposable
{
    private IWorkbook _workbook;
    private bool      _disposed;

    // ──────────────────────────────────────────────
    // IExcelReader 구현
    // ──────────────────────────────────────────────

    public void Load(string path)
    {
        string ext = Path.GetExtension(path).ToLowerInvariant();

        // FileShare.ReadWrite: Excel 등 다른 프로세스가 파일을 열고 있어도 읽기 가능
        byte[] bytes;
        using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        using (var ms = new MemoryStream())
        {
            fs.CopyTo(ms);
            bytes = ms.ToArray();
        }

        using var workbookMs = new MemoryStream(bytes);
        _workbook = ext switch
        {
            ".xlsx" => (IWorkbook)new XSSFWorkbook(workbookMs),
            ".xls"  => (IWorkbook)new HSSFWorkbook(workbookMs),
            _       => throw new NotSupportedException($"지원하지 않는 확장자: {ext}")
        };
    }

    public bool HasSheet(string sheetName)
    {
        return _workbook?.GetSheet(sheetName) != null;
    }

    public IList<string> GetHeaders(string sheetName)
    {
        ISheet sheet     = _workbook?.GetSheet(sheetName);
        IRow   headerRow = sheet?.GetRow(0);
        if (headerRow == null) return new List<string>();

        var headers = new List<string>();
        for (int col = 0; col < headerRow.LastCellNum; col++)
            headers.Add(GetCellString(headerRow.GetCell(col)));
        return headers;
    }

    public int GetRowCount(string sheetName)
    {
        ISheet sheet = _workbook?.GetSheet(sheetName);
        if (sheet == null) return 0;
        return sheet.LastRowNum + 1;   // LastRowNum 은 0-indexed
    }

    public IList<string> GetRow(string sheetName, int rowIndex)
    {
        ISheet sheet = _workbook?.GetSheet(sheetName);
        if (sheet == null) return new List<string>();

        IRow row       = sheet.GetRow(rowIndex);
        IRow headerRow = sheet.GetRow(0);
        int  colCount  = headerRow?.LastCellNum ?? (row?.LastCellNum ?? 0);

        var cells = new List<string>();
        for (int col = 0; col < colCount; col++)
            cells.Add(GetCellString(row?.GetCell(col)));
        return cells;
    }

    // ──────────────────────────────────────────────
    // 셀 → 문자열 변환
    // ──────────────────────────────────────────────

    private static string GetCellString(ICell cell)
    {
        if (cell == null) return string.Empty;

        switch (cell.CellType)
        {
            case CellType.Numeric:
                if (DateUtil.IsCellDateFormatted(cell))
                {
                    // NPOI 2.7.x: DateCellValue 는 DateTime? (Nullable)
                    DateTime? dt = cell.DateCellValue;
                    return dt.HasValue ? dt.Value.ToString("yyyy-MM-dd") : string.Empty;
                }

                double num = cell.NumericCellValue;
                // 소수점이 없으면 정수로 출력 (예: 1.0 → "1")
                return (num == Math.Truncate(num))
                    ? ((long)num).ToString()
                    : num.ToString(System.Globalization.CultureInfo.InvariantCulture);

            case CellType.Boolean:
                return cell.BooleanCellValue ? "true" : "false";

            case CellType.Formula:
                // 수식의 캐시된 결과값을 같은 메서드에 재귀 호출로 처리
                // (CellType 을 캐시 결과 타입으로 바꿔서 재진입)
                return GetFormulaResult(cell);

            case CellType.String:
                return cell.StringCellValue?.Trim() ?? string.Empty;

            case CellType.Blank:
            case CellType.Error:
            default:
                return string.Empty;
        }
    }

    /// <summary>수식 셀의 캐시된 결과값을 문자열로 반환합니다.</summary>
    private static string GetFormulaResult(ICell cell)
    {
        switch (cell.CachedFormulaResultType)
        {
            case CellType.Numeric:
                if (DateUtil.IsCellDateFormatted(cell))
                {
                    DateTime? dt = cell.DateCellValue;
                    return dt.HasValue ? dt.Value.ToString("yyyy-MM-dd") : string.Empty;
                }
                double num = cell.NumericCellValue;
                return (num == Math.Truncate(num))
                    ? ((long)num).ToString()
                    : num.ToString(System.Globalization.CultureInfo.InvariantCulture);

            case CellType.Boolean:
                return cell.BooleanCellValue ? "true" : "false";

            default:
                return cell.StringCellValue?.Trim() ?? string.Empty;
        }
    }

    // ──────────────────────────────────────────────
    // IDisposable
    // ──────────────────────────────────────────────


    public void Dispose()
    {
        if (_disposed) return;
        _workbook?.Close();
        _disposed = true;
    }
}
