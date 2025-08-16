using System;
using System.Collections.Generic;
[Serializable]
public class SkillData {
    public int id;
    public string name_kr;
    public string name_er;
    public string description;
    public string icon;
    public bool isActive;
    public List<float> time;
    public float cooltime;
    public List<float> distance;
    public List<DesignEnums.StatType> skillStatType;
    public List<float> skillStatValue;
    public List<int> prevSkill;
    public bool isTargetSkill;
    public List<string> prefab;
}
