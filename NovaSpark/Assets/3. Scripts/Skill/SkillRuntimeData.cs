using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillData", menuName = "Skill System/Skill Data")]
public class SkillRuntimeData : ScriptableObject
{
    public string skillId; // 식별용 id 
    public string name_kr; // UI용 이름
    public string name_en;
    public string description;
    public string icon;
    public bool isActive;
    public List<float> activeTimes;
    public float cooldown;
    public List<float> ranges;
    public List<string> statTypes;
    public List<float> statValues;
    public List<int> prevSkills;
    public bool isTargetSkill;
    public string prefabPath;
    public int maxSkillLevel = 1; // 스킬 맥스 레벨
    public SkillRuntimeData[] prerequisiteSkills; // 필요 선행스킬
    public int requiredPlayerLevel; // 필요 레벨
    public int requiredProficiency; // 필요 숙련도
    // TODO : 플레이어 만들어진 후 숙련도 세분화 필요, 스킬 타입등을 정해 필요한 숙련도를 나눌 수 있을 것
    public float baseDamage; // 기본데미지
    public float baseCooldown; // 기본쿨타임
}
