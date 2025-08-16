using System.Collections.Generic;
using UnityEngine;

public class PlayerRuntimeData
{
    [field : Header("불변")]
    public int characterID { get; private set; }
    public List<float> abilityExpWeight = new();
    public List<int> abilityMaxLevel = new();
    public Stat hungry;
    public Stat thirsty;
    public int initLevel { get; private set; }
    public int maxLevel { get; private set; }
    public float exp; //레벨업에 필요한 레벨 * exp 수치
    public int curLevel;
    public int questID;

    //레벨업 시 오르는 능력치
    public float health { get; private set; }
    public float damage { get; private set; }
    public float defence { get; private set; }
    public float attackSpeed { get; private set; }
    public float criticalPercent { get; private set; }
    public float aimPercent { get; private set; }
    public float missPercent { get; private set; }
    public int force { get; private set; }
    public float moveSpeed;

    //레벨업마다 지속 증가되는 수치 0.03f 기준 10레벨이면 health * 1.3f 만큼 성장. 계산식 1 + (0.3f * level) 과 같음.
    public float levelWeight { get; private set; }

    public int preQuestID;




    public PlayerRuntimeData(int key)
    {
        var playerData = DataManager.Instance.PlayerDataByID[key];
        characterID = playerData.characterID;
        abilityExpWeight.Clear();
        abilityMaxLevel.Clear();
        abilityExpWeight.AddRange(playerData.abilityExpWeight);
        abilityMaxLevel.AddRange(playerData.abilityMaxLevel);
        hungry = new Stat(playerData.hungry);
        thirsty = new Stat(playerData.thirsty);
        questID = playerData.questID; //초기 클리어된 퀘스트로 이후 나올 수 있는 퀘스트를 지정.
        initLevel = playerData.initLevel;
        maxLevel = playerData.maxLevel;

        exp = playerData.exp; //20 -> 40 보다는 피보나치 수열 느낌 20 -> 22 -> 26 -> 30 -> 35 -> 40 -> 46 이런식으로? % 10 을 통한 십의 자리수 기준의 증가수식

        health = playerData.health;
        damage = playerData.damage;
        defence = playerData.defence;
        attackSpeed = playerData.attackSpeed;
        criticalPercent = playerData.criticalPercent;
        aimPercent = playerData.aimPercent;
        missPercent = playerData.missPercent;
        force = playerData.force;
        moveSpeed = playerData.moveSpeed;

        levelWeight = playerData.levelWeight;
        preQuestID = playerData.preQuestID;

        curLevel = initLevel;
    }
}