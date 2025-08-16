using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public static class JsonLoader
{
    public static List<T> Load<T>(TextAsset jsonFile)
    {
        if (jsonFile == null)
        {
            Debug.LogWarning("JSON 파일 없음");
            return new List<T>();
        }

        return JsonConvert.DeserializeObject<List<T>>(jsonFile.text) ?? new List<T>();
    }

    public static Task<List<T>> LoadAsync<T>(TextAsset jsonFile)
    {
        if (jsonFile == null)
        {
            Debug.LogWarning("JSON 파일 없음");
            return Task.FromResult(new List<T>());
        }

        try
        {
            string json = jsonFile.text;
            var result = JsonConvert.DeserializeObject<List<T>>(json) ?? new List<T>();
            return Task.FromResult(result);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"JSON 파싱 실패: {ex.Message}");
            return Task.FromResult(new List<T>());
        }
    }
}