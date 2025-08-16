using UnityEngine;

public class SkillTreeManager
{
    private PlayerSkillState playerSkillState;

    // Player player

    //public SkillTreeManager(Player player)
    //{
    //    this.player = player;
    //}

    TestPlayer testPlayer;

    public SkillTreeManager(PlayerSkillState playerSkillState, TestPlayer testPlayer, int playerLevel, int playerProficiency, int availableSkillPoints)
    {
        this.playerSkillState = playerSkillState;
        this.testPlayer = testPlayer;
        this.playerLevel = playerLevel;
        this.playerProficiency = playerProficiency;
        this.availableSkillPoints = availableSkillPoints;
    }
    private int playerLevel; // 플레이어 레벨
    private int playerProficiency; // 숙련도
    private int availableSkillPoints; // 사용가능 스킬포인트


    // 스킬 해금 가능 여부 체그
    public bool CanUnlockSkill(SkillRuntimeData skill)
    {
        // 선행 스킬 해금 여부 체크
        foreach (var preSkill in skill.prerequisiteSkills)
        {
            if (!playerSkillState.IsSkillUnlocked(preSkill.skillId))
                return false;
        }

        // 레벨 체크
        if(playerLevel < skill.requiredPlayerLevel)
            return false;

        // 숙련도 체크 TODO : 플레이어 만든 후 숙련도 세분화 필요
        if(playerProficiency < skill.requiredProficiency)
            return false;

        return true;
    }

    // 스킬 포인트 투자 시도
    public bool TryInvestPoints(SkillRuntimeData skill)
    {
        
        if (availableSkillPoints <= 0)
        {
            Debug.Log("[TryInvestPoints] 포인트 부족");
            return false; // 포인트 없음
        }

        if (!CanUnlockSkill(skill))
        {
            Debug.Log($"[TryInvestPoints] 해금 조건 미충족: {skill.name_kr}");
            return false; // 해금조건
        }

        int currentPoint = playerSkillState.GetSkillLevel(skill.skillId);
       
        if (currentPoint >= skill.maxSkillLevel)
        {
            if (currentPoint >= skill.maxSkillLevel)
            {
                Debug.Log("[TryInvestPoints] 최대 레벨 도달");
                return false; // 최대레벨
            }
        }

        bool invested = playerSkillState.InvestPoints(skill);
        if(!invested)
        {
            Debug.Log("[TryInvestPoints] 투자 실패");
            return false;
        }

        availableSkillPoints--;
        Debug.Log($"[TryInvestPoints] 투자 성공! 남은 스킬 포인트: {availableSkillPoints}");
        return invested;
    }

    // 남은 스킬 포인트 조회
    public int GetAvailableSkillPoints() => availableSkillPoints;
}
