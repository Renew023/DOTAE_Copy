using System;
using System.Collections.Generic;
using UnityEngine;

public enum AffectionLevel
{
    Hostile,
    Neutral,
    Friendly
}

public class AffectionManager : MonoBehaviour, ISaveable
{
    private Dictionary<int, AffectionData> _affectionByNpc = new();
    private SaveManager saveManager;
    private TimeManager timeManager;
    private NPCManager npcManager;

    public event Action<int, int> OnAffectionChanged;

    // TimeManager의 OneDayPassed 이벤트 구독
    // private void OnEnable()
    // {
    //     saveManager = SaveManager.Instance;
    //     timeManager = TimeManager.Instance;
    //     npcManager = NPCManager.Instance;
    //     if (saveManager != null)
    //         saveManager.Register(this);
    //     if (timeManager != null)
    //         timeManager.OneDayPassed += OnDayPassed;
    // }

    // TimeManager의 OneDayPassed 이벤트 구독
    public void Init(SaveManager saveManager, TimeManager timeManager, NPCManager npcManager)
    {
        this.saveManager = saveManager;
        this.timeManager = timeManager;
        this.npcManager = npcManager;

        saveManager.Register(this);
        timeManager.OneDayPassed += OnDayPassed;
    }

    // TimeManager 이벤트 구독 해제
    private void OnDisable()
    {
        if (saveManager != null)
            saveManager.Unregister(this);
        if (timeManager != null)
            timeManager.OneDayPassed -= OnDayPassed;
    }

    // NPC 호감도 등록
    public void RegisterNPC(int npcId)
    {
        if (_affectionByNpc.ContainsKey(npcId))
            return;

        _affectionByNpc[npcId] = new AffectionData { npcId = npcId, affection = 0 };
        Debug.Log($"신규 NPC 호감도 등록됨. (id: {npcId}, affection: 0)");
    }

    // NPC 호감도 수치 변경
    public void ChangeAffection(int npcId, int amount)
    {
        // 혹시 등록 안되어 있을 시 등록
        if (!_affectionByNpc.TryGetValue(npcId, out var data))
        {
            RegisterNPC(npcId);
            data = _affectionByNpc[npcId];
        }

        data.Change(amount);
        Debug.Log($"{npcId}의 호감도 변화: {data.affection}");

        OnAffectionChanged?.Invoke(npcId, data.affection);

        var level = GetAffectionLevel(npcId);
        if (level == AffectionLevel.Hostile)
        {
            var npc = npcManager.GetNPCById(npcId);
            // TODO: NPC.cs에서 적대 상태로 변경하는 메서드 호출
        }
    }

      /* 외부에서 이벤트 구독해서 호감도 변경될 떄마다 메서드 호출
      private void OnEnable()
      {
          GameManager.Instance.AffectionManager.OnAffectionChanged += ABCD;
      }

      private void OnDisable()
      {
          GameManager.Instance.AffectionManager.OnAffectionChanged -= ABCD;
      }

      private void ABCD(int npcId, int affection)
      {
          Debug.Log($"{npcId} 호감도 변경됨: {affection}");
      }
      */

    // 하루가 지나면 일정 수치 이하인 호감도 회복
    public void OnDayPassed()
    {
        foreach (var data in _affectionByNpc.Values)
        {
            if (data.affection < 0)
            {
                data.Change(10);
            }
        }

        Debug.Log("하루경과: 호감도 +10 상승");
    }

    // NPC 호감도 가져옴
    public int GetAffection(int npcId)
    {
        return _affectionByNpc.TryGetValue(npcId, out var data) ? data.affection : 0;
    }

    // 호감도 등급 반환
    public AffectionLevel GetAffectionLevel(int npcId)
    {
        return _affectionByNpc.TryGetValue(npcId, out var data)
            ? data.Level
            : AffectionLevel.Neutral;
    }

    // 호감도 저장 딕셔너리 데이터를 리스트롤 변환해 저장
    public void SaveData(GameSaveData data)
    {
        var saveData = new AffectionSaveData();

        foreach (var item in _affectionByNpc)
        {
            saveData.affectionList.Add(new AffectionEntry
            {
                npcId = item.Key,
                affection = item.Value.affection,
            });
        }

        data.affectionData = saveData;
    }

    // 호감도 불러오기, 리스트 데이터 딕셔너리로 재구성
    public void LoadData(GameSaveData data)
    {
        var loaded = data.affectionData;
        if (loaded == null) return;
        _affectionByNpc.Clear();

        foreach (var entry in loaded.affectionList)
        {
            _affectionByNpc[entry.npcId] = new AffectionData { npcId = entry.npcId, affection = entry.affection };
        }
    }
}