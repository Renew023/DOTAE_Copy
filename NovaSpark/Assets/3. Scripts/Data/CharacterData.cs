using System;
using System.Collections.Generic;
[Serializable]
public class CharacterData {
    public int characterID;
    public DesignEnums.CharacterType characterType;
    public string name_kr;
    public string name_en;
    public string description;
    public DesignEnums.BloodType bloodType;
    public DesignEnums.VillageType familyType;
    public int Level;
    public float health;
    public float damage;
    public float defence;
    public float attackSpeed;
    public float attackRange;
    public float criticalPercent;
    public float aimPercent;
    public float missPercent;
    public int force;
    public float moveSpeed;
    public List<DesignEnums.Ability> abilityName;
    public List<int> abilityInitLevel;
    public List<float> abilityValue;
    public List<int> SkillListID;
    public List<int> SkillListLevel;
    public DesignEnums.RiggingType RiggingType;
    public List<int> StartItem;
    public List<int> StartItemCount;
}
