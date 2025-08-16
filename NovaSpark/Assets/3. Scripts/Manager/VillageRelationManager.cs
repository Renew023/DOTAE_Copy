using System;
using System.Collections.Generic;
using UnityEngine;

public enum RelationLevel
{
    Hostile,
    Neutral,
    Friendly
}

public class VillageRelationManager : MonoBehaviour, ISaveable
{
    private Dictionary<(int from, int to), VillageRelationData> _villageRelations = new();
    public event Action<(int, int), int> OnRelationChanged; // 관계 변경되었을 때 이벤트
    

    // 관계 변경(양방향)
    public void ChangeRelation(int fromId, int toId, int amount)
    {
        ApplyRelationChange(fromId, toId, amount);
        ApplyRelationChange(toId, fromId, amount);
    }

    // 실제 관계 수치 변경
    private void ApplyRelationChange(int fromId, int toId, int amount)
    {
        var key = (fromId, toId);

        if (!_villageRelations.TryGetValue(key, out var data))
        {
            data = new VillageRelationData
            {
                fromVillageId = fromId,
                toVillageId = toId,
                relation = 0
            };
            _villageRelations[key] = data;
        }

        data.Change(amount);
        OnRelationChanged?.Invoke(key, data.relation);
        Debug.Log($"{fromId} | {toId} 관계 변경: {data.relation}");
    }

    // 두 마을 간 관계 수치 조회
    public int GetRelation(int fromId, int toId)
    {
        var key = (fromId, toId);
        return _villageRelations.TryGetValue(key, out var data) ? data.relation : 0;
    }

    // 두 마을 간의 호감 단계 조회
    public RelationLevel GetRelationLevel(int fromId, int toId)
    {
        var key = (fromId, toId);
        return _villageRelations.TryGetValue(key, out var data)
            ? data.Level
            : RelationLevel.Neutral;
    }

    // 관계 데이터를 저장용 구조로 변환
    public void SaveData(GameSaveData data)
    {
        var saveData = new VillageRelationSaveData();
        var savedPairs = new HashSet<(int, int)>(); // 중복 저장 방지

        foreach (var item in _villageRelations)
        {
            var key = item.Key;
            var reverseKey = (key.to, key.from);

            // (1,2)이 이미 저장되었으면 (2,1)는 저장하지 않음
            if (savedPairs.Contains(reverseKey)) continue;

            saveData.relationList.Add(new VillageRelationEntry
            {
                fromVillageId = item.Key.from,
                toVillageId = item.Key.to,
                relation = item.Value.relation
            });
            savedPairs.Add(key);
        }

        data.villageRelationData = saveData;
    }

    // 저장된 데이터 딕셔너리로 전환
    public void LoadData(GameSaveData data)
    {
        var loaded = data.villageRelationData;
        if (loaded == null) return;

        _villageRelations.Clear();

        foreach (var item in loaded.relationList)
        {
            var keyA = (item.fromVillageId, item.toVillageId);
            var keyB = (item.toVillageId, item.fromVillageId);

            var relation = item.relation;

            _villageRelations[keyA] = new VillageRelationData
            {
                fromVillageId = item.fromVillageId,
                toVillageId = item.toVillageId,
                relation = relation
            };

            _villageRelations[keyB] = new VillageRelationData
            {
                fromVillageId = item.toVillageId,
                toVillageId = item.fromVillageId,
                relation = relation
            };
        }
    }
}