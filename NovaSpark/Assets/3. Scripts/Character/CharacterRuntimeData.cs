using ExitGames.Client.Photon;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class CharacterRuntimeData
{
    [field:SerializeField] public int characterID { get; private set; }
    public DesignEnums.CharacterType characterType { get; private set; }
    public string name_kr { get; private set; }
    public string name_en { get; private set; }
    public string description {  get; private set; }
    public DesignEnums.BloodType bloodType { get; private set; }
    public DesignEnums.VillageType familyType { get; private set; }
    public Dictionary<DesignEnums.Ability, Ability> ability { get; private set; } = new();

    public int initLevel = 1;
    public Stat health;
    public Stat damage;
    public Stat defence;
    public Stat aimPercent;
    public Stat attackRange;
    public Stat attackSpeed;
    public Stat criticalPercent;
    public Stat missPercent;
    public Stat force; //무게 감당하는 힘
    public Stat moveSpeed;

    public CharacterRuntimeData(int key)
    {
        //정적 데이터
        var characterData = DataManager.Instance.CharacterDataByID[key];
        this.characterID = characterData.characterID;
        this.characterType = characterData.characterType;
        this.name_kr = characterData.name_kr;
        this.name_en = characterData.name_en;
        this.description = characterData.description;
        this.bloodType = characterData.bloodType;
        this.familyType = characterData.familyType;
        for(int i =0; i< characterData.abilityName.Count; i++)
        {
            //능력치 초기화
            Ability ability = new Ability
            {
                abilityInitLevel = characterData.abilityInitLevel[i],
                abilityValue = characterData.abilityValue[i]
            };
            this.ability.Add(characterData.abilityName[i], ability);
        }
        //필드 데이터
        this.health = new Stat(characterData.health);
        this.damage = new Stat(characterData.damage);
        this.defence = new Stat(characterData.defence);
        this.aimPercent = new Stat(characterData.aimPercent);
        this.attackRange = new Stat(characterData.attackRange);
        this.attackSpeed = new Stat(characterData.attackSpeed);
        this.criticalPercent = new Stat(characterData.criticalPercent);
        this.missPercent = new Stat(characterData.missPercent);
        this.force = new Stat(characterData.force);
        this.moveSpeed = new Stat(characterData.moveSpeed);

        //TODO : 나중에 반드시 분리 필요!

        foreach (var blood in DataManager.Instance.BloodDataByID)
        {
            if (blood.Key == this.bloodType)
            {
                for(int i=0; i< blood.Value.bloodAbility.Count; i++)
                {
                    // 능력치 초기화
                    if (ability.ContainsKey(blood.Value.bloodAbility[i]))
                    {
                        this.ability[blood.Value.bloodAbility[i]] = new Ability
                        {
                            abilityInitLevel = this.ability[blood.Value.bloodAbility[i]].abilityInitLevel +blood.Value.abilityInitLevel[i],
                            abilityValue = this.ability[blood.Value.bloodAbility[i]].abilityValue + blood.Value.abilityValue[i],
                        };
                    }
                    else
                    {
                        Ability ability = new Ability
                        {
                            abilityInitLevel = blood.Value.abilityInitLevel[i],
                            abilityValue = blood.Value.abilityValue[i]
                        };

                        this.ability.Add(blood.Value.bloodAbility[i], ability);
                    }
                }
            }
        }
    }

    public void DefenceSpawnStatus(int day)
    {
        var characterData = DataManager.Instance.CharacterDataByID[characterID];
        health.AddStat(characterData.health * RoomSettingData.Instance.monsterWaveStatUpWeight * DefenceManager.Instance.waveCount);
        damage.AddStat(characterData.damage * RoomSettingData.Instance.monsterWaveStatUpWeight * DefenceManager.Instance.waveCount);
        defence.AddStat(characterData.defence * RoomSettingData.Instance.monsterWaveStatUpWeight * DefenceManager.Instance.waveCount);
        aimPercent.AddStat(characterData.aimPercent * RoomSettingData.Instance.monsterWaveStatUpWeight * DefenceManager.Instance.waveCount);
        force.AddStat(characterData.force * RoomSettingData.Instance.monsterWaveStatUpWeight * DefenceManager.Instance.waveCount);
    }

    public void AddStat(DesignEnums.StatType type, float value)
    {
        string action = value >= 0 ? "증가" : "감소";
        switch (type)
        {
            case DesignEnums.StatType.Damage: damage.AddStat(value); break;
            case DesignEnums.StatType.Speed: moveSpeed.AddStat(value); break;
            case DesignEnums.StatType.Defence: defence.AddStat(value); break;
            case DesignEnums.StatType.AttackSpeed: attackSpeed.AddStat(value); break;
            case DesignEnums.StatType.AttackRange: attackRange.AddStat(value); break;
            case DesignEnums.StatType.CriticalPercent: criticalPercent.AddStat(value); break;
            case DesignEnums.StatType.Accuracy: aimPercent.AddStat(value); break;
            case DesignEnums.StatType.Missing: missPercent.AddStat(value); break;
            case DesignEnums.StatType.Hp:health.AddStat(value); break;
        }
    }

    public void RemoveStat(DesignEnums.StatType type, float value)
    {
        AddStat(type, -value);

        // Hp 감소 시 maxHp가 줄어들면 현재 체력도 줄여야 함.
        switch (type)
        {
            case DesignEnums.StatType.Hp:
                // maxHp -= value;
                // currentHp = Mathf.Min(currentHp, maxHp); // 줄어든 maxHp에 맞춤
                break;
        }
    }
}

[System.Serializable]
public struct Ability
{
    public int abilityInitLevel;
    public float abilityValue;

    public float GetAbility()
    {
        return abilityInitLevel * abilityValue;
    }
}

[System.Serializable]
public struct Stat
{
    public float Max;
    public float Current;

    public Stat(float max)
    {
        Max = max;
        Current = max;
    }

    public void AddStat(float value)
    {
        Max += value;
        Current += value;
    }

    public void Reset()
    {
        Current = Max;
    }

    public void Recover(float value)
    {
        Current = Mathf.Min(Current + value, Max);
    }
}

public class Stats
{
    public Stat health;
    public Stat damage;
    public Stat defence;
    public Stat aimPercent;
    public Stat attackRange;
    public Stat attackSpeed;
    public Stat criticalPercent;
    public Stat missPercent;
    public Stat force; //무게 감당하는 힘
    public Stat moveSpeed;

    public void AddStat(DesignEnums.StatType type, float value)
    {
        string action = value >= 0 ? "증가" : "감소";
        switch (type)
        {
            case DesignEnums.StatType.Damage: damage.AddStat(value); break;
            case DesignEnums.StatType.Speed: moveSpeed.AddStat(value); break;
            case DesignEnums.StatType.Defence: defence.AddStat(value); break;
            case DesignEnums.StatType.AttackSpeed: attackSpeed.AddStat(value); break;
            case DesignEnums.StatType.AttackRange: attackRange.AddStat(value); break;
            case DesignEnums.StatType.CriticalPercent: criticalPercent.AddStat(value); break;
            case DesignEnums.StatType.Accuracy: aimPercent.AddStat(value); break;
            case DesignEnums.StatType.Missing: missPercent.AddStat(value); break;
            case DesignEnums.StatType.Hp:
                health.AddStat(value); // Hp 변수 생성 후 주석 해제
                //currentHp += value; // 체력 증가 시 현재 체력도 같이 회복
                //if (currentHp > maxHp)
                //    currentHp = maxHp; // 초과 방지
                break;
        }
    }

    public void RemoveStat(DesignEnums.StatType type, float value)
    {
        AddStat(type, -value);

        // Hp 감소 시 maxHp가 줄어들면 현재 체력도 줄여야 함.
        switch (type)
        {
            case DesignEnums.StatType.Hp:
                // maxHp -= value;
                // currentHp = Mathf.Min(currentHp, maxHp); // 줄어든 maxHp에 맞춤
                break;
        }
    }
}
