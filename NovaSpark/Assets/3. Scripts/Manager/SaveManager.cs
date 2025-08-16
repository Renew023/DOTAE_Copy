using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;

public class SaveManager : Singleton<SaveManager>
{
    private readonly List<ISaveable> _saveables = new(); // 저장 대상 리스트
    private Coroutine _autoSaveCoroutine; // 자동 저장 코루틴
    private const int _maxSaveFiles = 3; // 최대 저장 파일 수
    private const float _autoSaveInterval = 5f; // 자동 저장 주기(초)

    protected override void Awake()
    {
        DontDestroyOnLoad(gameObject); // 씬 전환 시에도 유지
        base.Awake();
        // LoadLatestGame();
        // StartAutoSave();
    }

    // public void Init()
    // {
    //     LoadLatestGame();
    //     // StartAutoSave();
    // }

    [ContextMenu("자동 저장 시작")]
    public void StartAutoSave()
    {
        StopAutoSave();
        _autoSaveCoroutine = StartCoroutine(AutoSaveRoutine());
        Debug.Log("자동 저장 시작");
    }

    [ContextMenu("자동 저장 중지")]
    public void StopAutoSave()
    {
        if (_autoSaveCoroutine != null)
        {
            StopCoroutine(_autoSaveCoroutine);
            _autoSaveCoroutine = null;
            Debug.Log("자동 저장 중지");
        }
    }

    [ContextMenu("최근 데이터 불러옹기")]
    public void LoadRecentSaveFiles()
    {
        string latestPath = GetLatestSavePath();
        if (string.IsNullOrEmpty(latestPath))
        {
            Debug.LogError("최근 저장 파일이 없습니다.");
            return;
        }

        SaveSData.SelectedSavePath = latestPath;
        Debug.Log($"최근 저장 데이터 지정됨: {SaveSData.SelectedSavePath}");
    }

    [ContextMenu("저장")]
    public void TestSave()
    {
        SaveGame();
        Debug.Log("저장 완료");
    }

    // 일정 시간마다 자동 저장
    private IEnumerator AutoSaveRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(_autoSaveInterval);
            SaveGame();
            Debug.Log("자동 저장 완료");
        }
    }

    // 게임 데이터 저장
    public void SaveGame(string path = null)
    {
        string savePath = path == null ? GetSavePath() : GetSaveCustomPath(path);
        var data = GatherSaveData(); // 모든 ISaveable로 부터 데이터 수집
        SaveToFile(data, savePath); // 파일로 저장
        CleanUpOldSaves(_maxSaveFiles); // 오래된 저장파일 정리
    }

    // 가장 최근 저장 파일 불러오기
    public void LoadLatestGame()
    {
        string latestPath = GetLatestSavePath();
        if (string.IsNullOrEmpty(latestPath))
        {
            Debug.LogWarning("세이브 파일 없음");
            return;
        }

        LoadFromFile(latestPath);
    }

    // 경로를 지정해서 저장 파일 불러오기
    public void LoadGameFromPath(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogWarning("세이브 파일 없음");
            return;
        }

        LoadFromFile(path);
    }

    // 등록된 모든 저장 대상에게 데이터 저장 요청
    private GameSaveData GatherSaveData()
    {
        var data = new GameSaveData();
        foreach (var saveable in _saveables)
            saveable.SaveData(data);
        return data;
    }

    // JSON으로 저장
    private void SaveToFile(GameSaveData data, string path)
    {
        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        string encrypted = AESUtil.Encrypt(json);
        File.WriteAllText(path, encrypted);
        // File.WriteAllText(path, json); 암호화 사용안할 시
        Debug.Log($"저장 완료: {path}");
    }

    // JSON으로부터 데이터 로딩 및 적용
    private void LoadFromFile(string path)
    {
        // string json = File.ReadAllText(path); 암호화 사용안할 시
        string encryptedJson = File.ReadAllText(path);
        string json = AESUtil.Decrypt(encryptedJson);
        var data = JsonConvert.DeserializeObject<GameSaveData>(json);

        foreach (var saveable in _saveables)
            saveable.LoadData(data);

        Debug.Log($"불러오기 완료: {path}");
    }

    // 저장 경로 생성 (현재 시각 기준)
    private string GetSavePath()
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        return Path.Combine(Application.persistentDataPath, $"save_{timestamp}.json");
    }

    // 사용자 지정 저장 이름
    private string GetSaveCustomPath(string name)
    {
        string saveName = name.Replace(" ", "_");
        return Path.Combine(Application.persistentDataPath, $"save_{saveName}.json");
    }

    // 가장 최근 저장 파일의 경로 가져오기
    private string GetLatestSavePath()
    {
        var files = GetSortedSaveFiles();
        return files.Length > 0 ? files[0].FullName : null;
    }

    // 오래된 저장 파일 삭제 (maxSaveCount만 유지)
    private void CleanUpOldSaves(int maxSaveCount)
    {
        var files = GetSortedSaveFiles();

        for (int i = maxSaveCount; i < files.Length; i++)
        {
            Debug.Log($"오래된 저장 파일 삭제: {files[i].Name}");
            files[i].Delete();
        }
    }

    // 최근 저장 파일 경로 리스트 반환
    public List<string> GetRecentSaveFiles(int maxCount = 3)
    {
        var files = GetSortedSaveFiles();
        var result = new List<string>();

        for (int i = 0; i < Mathf.Min(maxCount, files.Length); i++)
            result.Add(files[i].FullName);

        return result;
    }

    // 저장 파일들 생성일 기준으로 내림차순 정렬
    private FileInfo[] GetSortedSaveFiles()
    {
        var dir = new DirectoryInfo(Application.persistentDataPath);
        FileInfo[] files = dir.GetFiles("save_*.json");
        Array.Sort(files, (a, b) => b.CreationTime.CompareTo(a.CreationTime));
        return files;
    }

    // 저장 대상 등록
    public void Register(ISaveable saveable)
    {
        if (!_saveables.Contains(saveable))
            _saveables.Add(saveable);
    }

    // 저장 대상 등록 해제
    public void Unregister(ISaveable saveable)
    {
        _saveables.Remove(saveable);
    }

    // 테스트용 저장/불러오기 단축키
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
            SaveGame();
        // Debug.Log(Application.persistentDataPath);
        if (Input.GetKeyDown(KeyCode.X))
            LoadLatestGame();
    }
}