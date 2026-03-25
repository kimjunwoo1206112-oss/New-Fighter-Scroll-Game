using System.Collections.Generic;

/// <summary>
/// Excel 파일 읽기 인터페이스.
/// 구현체를 교체하면 EPPlus, NPOI, CSV 등 다양한 포맷으로 확장 가능합니다.
/// </summary>
public interface IExcelReader
{
    /// <summary>파일을 로드합니다.</summary>
    void Load(string path);

    /// <summary>해당 이름의 시트가 존재하는지 확인합니다.</summary>
    bool HasSheet(string sheetName);

    /// <summary>시트의 첫 번째 행(헤더)을 반환합니다.</summary>
    IList<string> GetHeaders(string sheetName);

    /// <summary>시트의 총 행 수(헤더 포함)를 반환합니다.</summary>
    int GetRowCount(string sheetName);

    /// <summary>특정 행(0-indexed)의 모든 셀 값을 문자열 리스트로 반환합니다.</summary>
    IList<string> GetRow(string sheetName, int rowIndex);
}
