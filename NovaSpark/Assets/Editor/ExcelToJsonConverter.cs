using ExcelDataReader;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class ExcelToJsonConverter : EditorWindow
{
    [MenuItem("Tools/Excel → Generate (Class + Enums + Loader)")]
    public static void ConvertExcel()
    {
        string excelDir = Application.dataPath + "/Excel";
        string jsonDir = Application.dataPath + "/Addressables/Data";
        string classDir = Application.dataPath + "/3. Scripts/Data";

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        Directory.CreateDirectory(excelDir);
        Directory.CreateDirectory(jsonDir);
        Directory.CreateDirectory(classDir);

        // EnumDefinitions 처리
        string enumPath = Path.Combine(excelDir, "EnumDefinition.xlsx");
        if (File.Exists(enumPath))
        {
            GenerateEnums(enumPath, classDir);
        }
        else
        {
            Debug.LogWarning("EnumDefinition.xlsx 파일이 존재하지 않습니다.");
        }

        // 각 엑셀 파일 처리
        foreach (var file in Directory.GetFiles(excelDir, "*.xlsx"))
        {
            if (file.Contains("~") || file.Contains("EnumDefinition"))
                continue;

            using var stream = File.Open(file, FileMode.Open, FileAccess.Read);
            using var reader = ExcelReaderFactory.CreateReader(stream);
            var result = reader.AsDataSet();

            foreach (DataTable table in result.Tables)
            {
                string sheetName = Sanitize(table.TableName);
                if (table.Rows.Count < 3)
                {
                    Debug.LogWarning($"[{sheetName}] 시트에 3행 이상 데이터가 없어 스킵됨");
                    continue;
                }

                Debug.Log($"{sheetName} 시작");
                var typeRow = table.Rows[1];
                var fieldRow = table.Rows[2];

                var types = new List<string>();
                var fields = new List<string>();
                for (int c = 0; c < table.Columns.Count; c++)
                {
                    types.Add(typeRow[c]?.ToString()?.Trim() ?? "string");
                    fields.Add(Sanitize(fieldRow[c]?.ToString()?.Trim() ?? $"field{c}"));
                }

                // 클래스 생성
                var cs = new StringBuilder();
                cs.AppendLine("using System;");
                cs.AppendLine("using System.Collections.Generic;");
                cs.AppendLine("[Serializable]");
                cs.AppendLine($"public class {sheetName} {{");
                for (int i = 0; i < fields.Count; i++)
                    cs.AppendLine($"    public {ConvertType(types[i])} {fields[i]};");
                cs.AppendLine("}");
                File.WriteAllText(Path.Combine(classDir, $"{sheetName}.cs"), cs.ToString());

                // JSON 생성
                var dataList = new List<Dictionary<string, object>>();
                for (int r = 3; r < table.Rows.Count; r++)
                {
                    var row = table.Rows[r];
                    var data = new Dictionary<string, object>();
                    for (int c = 0; c < fields.Count; c++)
                    {
                        string raw = row[c]?.ToString()?.Trim() ?? "";
                        data[fields[c]] = ParseValue(types[c], raw);
                    }
                    dataList.Add(data);
                }
                File.WriteAllText(
                    Path.Combine(jsonDir, $"{sheetName}.json"),
                    JsonConvert.SerializeObject(dataList, Formatting.Indented)
                );
                Debug.Log($"{sheetName} 처리 완료");
            }
        }

        AssetDatabase.Refresh();
    }

    static void GenerateEnums(string enumPath, string outputDir)
    {
        using var stream = File.Open(enumPath, FileMode.Open, FileAccess.Read);
        using var reader = ExcelReaderFactory.CreateReader(stream);
        var table = reader.AsDataSet().Tables[0];

        var sb = new StringBuilder();
        sb.AppendLine("public static class DesignEnums {");

        for (int r = 0; r < table.Rows.Count; r++)
        {
            string enumName = Sanitize(table.Rows[r][0]?.ToString()?.Trim());
            if (string.IsNullOrEmpty(enumName))
                continue;

            sb.AppendLine($"    public enum {enumName} {{");
            for (int c = 1; c < table.Columns.Count; c++)
            {
                string raw = table.Rows[r][c]?.ToString()?.Trim();
                if (string.IsNullOrEmpty(raw))
                    continue;

                string val = Sanitize(raw);
                if (val == "_null") // '_null' 필터링
                    continue;

                sb.AppendLine($"        {val} = {c - 1},");
            }
            sb.AppendLine("    }");
        }

        sb.AppendLine("}");
        File.WriteAllText(Path.Combine(outputDir, "DesignEnums.cs"), sb.ToString());
    }

    static string ConvertType(string type)
    {
        if (type.StartsWith("Enum<") && type.EndsWith(">"))
            return $"DesignEnums.{type[5..^1]}";

        if (type.StartsWith("List<Enum<") && type.EndsWith(">>"))
            return $"List<DesignEnums.{type[10..^2]}>";

        if (type == "int" || type == "float" || type == "bool" || type == "string")
            return type;

        if (type == "List<int>")
            return "List<int>";
        if (type == "List<float>")
            return "List<float>";
        if (type == "List<string>")
            return "List<string>";

        throw new Exception($"[ConvertType] 지원되지 않는 타입: '{type}'");
    }

    static object ParseValue(string type, string value)
    {
        //if (string.IsNullOrWhiteSpace(value))
        //    return null;

        string expStr = "";

        if (type.StartsWith("List<Enum<") && type.EndsWith(">>"))
        {
            if (value == expStr) value = "None";
            return value
                .Split('|')
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();
        }

        if (type.StartsWith("Enum<") && type.EndsWith(">"))
        {
            if (value == expStr)
            {
                value = "None";
            }
            return value;
        }

        if (type == "List<int>")
        {
            if (value == expStr) value = "0";
            return value
            .Split('|')
            .Select(s => s.Trim())
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s =>
                int.TryParse(s, out var i)
                    ? i
                    : throw new FormatException($"List<int> 파싱 실패: '{s}'")
            )
            .ToList();
        }

        if (type == "List<float>")
        {
            if (value == expStr) value = "0";
            return value
                .Split('|')
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s =>
                    float.TryParse(s, out var f)
                        ? f
                        : throw new FormatException($"List<float> 파싱 실패: '{s}'")
                )
                .ToList();
        }

        if (type == "List<string>")
        {
            if (value == expStr) return null;
            return value
                .Split('|')
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();
        }

        if (type == "int")
        {
            if (value == expStr) value = "0";
            return int.TryParse(value, out var i)
                ? i
                : throw new FormatException($"int 파싱 실패: '{value}'");
        }

        if (type == "float")
        {
            if (value == expStr) value = "0";
            return float.TryParse(value, out var f)
                ? f
                : throw new FormatException($"float 파싱 실패: '{value}'");
        }

        if (type == "bool")
        {
            if (value == expStr) value = "false";
            return bool.TryParse(value, out var b)
                ? b
                : throw new FormatException($"bool 파싱 실패: '{value}'");
        }

        return value;
    }

    static string Sanitize(string name)
    {
        if (string.IsNullOrEmpty(name))
            return "_null";

        // 예약어 간단 처리
        string[] reserved = { "class", "int", "float", "bool", "string", "public", "enum" };
        name = Regex.Replace(name, "[^a-zA-Z0-9_]", "_");
        if (char.IsDigit(name[0]))
            name = "_" + name;
        if (reserved.Contains(name))
            name = "_" + name;

        return name;
    }
}
