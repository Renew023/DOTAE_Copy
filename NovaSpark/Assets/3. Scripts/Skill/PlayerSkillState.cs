using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerSkillState
{
    // 플레이어 식별코드 (임시)
    public string ownerId;

    // 스킬 ID 별 투자한 포인트 수 저장 - 스킬 레벨
    // TODO : 저장기능 연결할때 직렬화 가능하게 변경해주어야함
    public Dictionary<string, int> investedPoints = new Dictionary<string, int>();

    // 해금된 스킬 ID 집합 - 0레벨에서 투자 시 해금
    // TODO : 저장기능 연결할때 직렬화 가능하게 변경해주어야함
    public HashSet<string> unlockedSkills = new HashSet<string>();  

    // 포인트 투자
    public bool InvestPoints(SkillRuntimeData data)
    {
        string id = data.skillId;

        if (!investedPoints.ContainsKey(id))
            investedPoints[id] = 0;

        int currentLevel = investedPoints[id];
       // if (currentLevel >= data.maxSkillLevel)
       //   return false; // 최대 레벨 도달

        investedPoints[id] = currentLevel + 1;

        if (currentLevel == 0) // 첫 투자면 해금 처리
            UnlockSkill(id);
        Debug.Log($"[TryInvestPoints] 현재 스킬 레벨({data.name_kr}): {currentLevel+1}");
        return true;
    }

    // 스킬 해금 - 해금된 스킬 집합에 선택된 스킬 추가
    public void UnlockSkill(string skillId)
    {
        if(!unlockedSkills.Contains(skillId))
            unlockedSkills.Add(skillId);
    }

    // 해금 여부 체그
    public bool IsSkillUnlocked(string skillId)
    {
        return unlockedSkills.Contains(skillId);
    }

    // 현재 투자 포인트 조회 함수
    public int GetSkillLevel(string skillId)
    {
        return investedPoints.ContainsKey(skillId) ? investedPoints[skillId] : 0;
    }
}
