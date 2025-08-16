using UnityEngine;

public class TestSkillUI : MonoBehaviour
{
    public SkillTreeManager skillTreeManager;
    public SkillRuntimeData fireball;
    public SkillRuntimeData fireWall;
    public SkillRuntimeData fireArrow;
    void Start()
    {
        var testPlayer = FindObjectOfType<TestPlayer>();
        skillTreeManager = testPlayer.skillTreeManager;
    }

    public void OnClickFireball()
    {
        TryUnlock(fireball);
    }

    public void OnClickFireWall()
    {
        TryUnlock(fireWall);
    }

    public void OnClickMeteor()
    {
        TryUnlock(fireArrow);
    }

    private void TryUnlock(SkillRuntimeData skill)
    {
        bool success = skillTreeManager.TryInvestPoints(skill);
        if (success)
            Debug.Log($"{skill.name_kr} 해금/레벨업 성공");
        else
            Debug.Log($"{skill.name_kr} 조건 부족 또는 포인트 없음");
    }
}

