using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

/// <summary>
/// 범용 Excel → ScriptableObject 컨버터.
/// 
/// 사용 규칙:
///   1. T 는 ScriptableObject 를 상속해야 합니다.
///   2. T 의 필드 이름이 Excel 시트 이름과 일치해야 합니다.
///      예) List<ItemData> Item;  →  Excel 시트명 "Item"
///   3. 각 시트의 첫 번째 행(Row 1)은 헤더(필드명) 행입니다.
///   4. 모델 클래스는 [Serializable] 이어야 하며,
///      필드 이름이 헤더와 일치해야 합니다.
/// 
/// 의존성: NPOI (Assets/Plugins/NPOI)
///   → Window > Package Manager 또는 직접 DLL 배치
/// </summary>
public static class ExcelConverter
{
    // ──────────────────────────────────────────────
    // Public API
    // ──────────────────────────────────────────────

    /// <summary>
    /// Excel 파일을 읽어 ScriptableObject T 에 시트별로 데이터를 채웁니다.
    /// </summary>
    /// <typeparam name="T">데이터를 받을 ScriptableObject 타입 (예: GameData)</typeparam>
    /// <param name="excelPath">Excel 파일의 절대 경로 (.xlsx)</param>
    /// <param name="target">데이터를 채울 ScriptableObject 인스턴스</param>
    /// <param name="excelPath">Excel 파일의 절대 경로 (.xlsx)</param>
    /// <param name="target">데이터를 채울 ScriptableObject 인스턴스</param>
    public static void Convert<T>(string excelPath, T target) where T : ScriptableObject
    {
        if (!File.Exists(excelPath))
        {
            Debug.LogError($"[ExcelConverter] 파일이 존재하지 않습니다: {excelPath}");
            return;
        }

        IExcelReader reader = CreateReader(excelPath);
        if (reader == null) return;

        using (reader as IDisposable)
        {
            reader.Load(excelPath);
            PopulateTarget(reader, target);
        }

        Debug.Log($"[ExcelConverter] 변환 완료: {Path.GetFileName(excelPath)} → {typeof(T).Name}");
    }

    /// <summary>
    /// 인자 순서 편의 오버로드 (target, path 순서).
    /// Editor 코드에서 ExcelConverter.Convert(target, path) 형태로 사용 가능.
    /// </summary>
    public static void Convert<T>(T target, string excelPath) where T : ScriptableObject
        => Convert(excelPath, target);

    // ──────────────────────────────────────────────
    // Core – Reflection 기반 매핑
    // ──────────────────────────────────────────────

    /// <summary>
    /// 리플렉션으로 T 의 List&lt;모델&gt; 필드를 순회하며
    /// 시트 이름이 일치하는 데이터를 채웁니다.
    /// </summary>
    private static void PopulateTarget<T>(IExcelReader reader, T target) where T : ScriptableObject
    {
        Type targetType = typeof(T);

        // ScriptableObject 의 모든 필드(public + private serializable) 탐색
        FieldInfo[] fields = GetSerializableFields(targetType);

        foreach (FieldInfo field in fields)
        {
            // List<SomeModel> 형태인지 확인
            if (!IsGenericList(field.FieldType, out Type modelType)) continue;

            // 필드 이름 == 시트 이름
            string sheetName = field.Name;

            if (!reader.HasSheet(sheetName))
            {
                Debug.LogWarning($"[ExcelConverter] 시트 없음: '{sheetName}' (필드: {field.Name})");
                continue;
            }

            // 해당 시트를 모델 리스트로 변환
            IList list = ReadSheet(reader, sheetName, modelType);

            // 리플렉션으로 필드에 할당
            field.SetValue(target, list);

            Debug.Log($"[ExcelConverter] 시트 '{sheetName}' → {list.Count}개 [{modelType.Name}] 로드 완료");
        }
    }

    /// <summary>
    /// 시트 한 장을 읽어 List&lt;modelType&gt; 형태의 IList 로 반환합니다.
    /// </summary>
    private static IList ReadSheet(IExcelReader reader, string sheetName, Type modelType)
    {
        // List<T> 동적 생성
        Type listType = typeof(List<>).MakeGenericType(modelType);
        IList list = (IList)Activator.CreateInstance(listType);

        // 헤더 행(Row 0) 읽기
        IList<string> headers = reader.GetHeaders(sheetName);
        if (headers == null || headers.Count == 0)
        {
            Debug.LogWarning($"[ExcelConverter] 헤더가 없습니다: 시트 '{sheetName}'");
            return list;
        }

        // 모델 타입의 필드 캐시 (헤더 이름 → FieldInfo)
        Dictionary<string, FieldInfo> fieldMap = BuildFieldMap(modelType, headers);

        // 데이터 행(Row 1~) 읽기
        int rowCount = reader.GetRowCount(sheetName);
        for (int row = 1; row < rowCount; row++)
        {
            IList<string> cells = reader.GetRow(sheetName, row);
            if (IsEmptyRow(cells)) continue;

            object instance = Activator.CreateInstance(modelType);

            for (int col = 0; col < headers.Count && col < cells.Count; col++)
            {
                string header = headers[col];
                if (string.IsNullOrEmpty(header)) continue;
                if (!fieldMap.TryGetValue(header, out FieldInfo fi)) continue;

                string rawValue = cells[col];
                object converted = ConvertValue(rawValue, fi.FieldType);
                fi.SetValue(instance, converted);
            }

            list.Add(instance);
        }

        return list;
    }

    // ──────────────────────────────────────────────
    // Type Helpers
    // ──────────────────────────────────────────────

    private static FieldInfo[] GetSerializableFields(Type type)
    {
        // public 필드 + [SerializeField] private 필드
        var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        return type.GetFields(flags)
            .Where(f => f.IsPublic ||
                        f.GetCustomAttribute<SerializeField>() != null)
            .ToArray();
    }

    private static bool IsGenericList(Type type, out Type elementType)
    {
        elementType = null;
        if (!type.IsGenericType) return false;
        if (type.GetGenericTypeDefinition() != typeof(List<>)) return false;

        elementType = type.GetGenericArguments()[0];
        return true;
    }

    private static Dictionary<string, FieldInfo> BuildFieldMap(Type modelType, IList<string> headers)
    {
        var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        var allFields = modelType.GetFields(flags)
            .Where(f => f.IsPublic || f.GetCustomAttribute<SerializeField>() != null)
            .ToDictionary(f => f.Name, f => f);

        var map = new Dictionary<string, FieldInfo>(StringComparer.OrdinalIgnoreCase);
        foreach (string header in headers)
        {
            if (allFields.TryGetValue(header, out FieldInfo fi))
                map[header] = fi;
        }
        return map;
    }

    // ──────────────────────────────────────────────
    // Value Conversion
    // ──────────────────────────────────────────────

    /// <summary>
    /// 문자열 값을 대상 필드 타입으로 변환합니다.
    /// 지원 타입: int, float, double, bool, string, long, Enum
    /// </summary>
    public static object ConvertValue(string raw, Type targetType)
    {
        if (string.IsNullOrEmpty(raw))
            return GetDefault(targetType);

        // Nullable<T> 언랩
        Type underlying = Nullable.GetUnderlyingType(targetType);
        if (underlying != null)
            targetType = underlying;

        try
        {
            if (targetType == typeof(string))  return raw;
            if (targetType == typeof(int))     return int.Parse(raw);
            if (targetType == typeof(float))   return float.Parse(raw, System.Globalization.CultureInfo.InvariantCulture);
            if (targetType == typeof(double))  return double.Parse(raw, System.Globalization.CultureInfo.InvariantCulture);
            if (targetType == typeof(long))    return long.Parse(raw);
            if (targetType == typeof(bool))
            {
                if (raw == "1" || raw.ToLower() == "true") return true;
                if (raw == "0" || raw.ToLower() == "false") return false;
                return bool.Parse(raw);
            }
            if (targetType.IsEnum)
                return Enum.Parse(targetType, raw, ignoreCase: true);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[ExcelConverter] 값 변환 실패: '{raw}' → {targetType.Name} / {e.Message}");
            return GetDefault(targetType);
        }

        return GetDefault(targetType);
    }

    private static object GetDefault(Type type)
    {
        return type.IsValueType ? Activator.CreateInstance(type) : null;
    }

    private static bool IsEmptyRow(IList<string> cells)
    {
        return cells == null || cells.All(c => string.IsNullOrWhiteSpace(c));
    }

    // ──────────────────────────────────────────────
    // Reader 팩토리
    // ──────────────────────────────────────────────

    private static IExcelReader CreateReader(string path)
    {
        string ext = Path.GetExtension(path).ToLower();
        switch (ext)
        {
            case ".xlsx":
            case ".xls":
                return new NpoiExcelReader();
            default:
                Debug.LogError($"[ExcelConverter] 지원하지 않는 파일 형식: {ext}");
                return null;
        }
    }
}
