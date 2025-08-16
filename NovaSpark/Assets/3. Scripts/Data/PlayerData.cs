using System;
using System.Collections.Generic;
[Serializable]
public class PlayerData {
    public int characterID;
    public List<float> abilityExpWeight;
    public List<int> abilityMaxLevel;
    public float hungry;
    public float thirsty;
    public int initLevel;
    public int maxLevel;
    public float levelWeight;
    public float exp;
    public int questID;
    public float health;
    public float damage;
    public float defence;
    public float attackSpeed;
    public float criticalPercent;
    public float aimPercent;
    public float missPercent;
    public float moveSpeed;
    public int force;
    public int preQuestID;
}
