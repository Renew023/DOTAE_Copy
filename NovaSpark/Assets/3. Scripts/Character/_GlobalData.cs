using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[System.Serializable]
public class LevelInfo //레벨별 정보
{
    public int LevelInfoId;
    public int level;
    public int expToNext;     // 해당 레벨에서 다음 레벨까지 필요한 경험치
    public float value;       // 해당 레벨에서 적용되는 수치 (ex: 데미지, 속도 등)
}

[System.Serializable]
public class TecLevelInfo  //기술 레벨별 정보
{
    public int TecLevelInfoId;
    public int level;
    public int expToNext;     // 해당 레벨에서 다음 레벨까지 필요한 경험치
    public float value;       // 해당 레벨에서 적용되는 수치 (ex: 데미지, 속도 등)
}

[System.Serializable]
public class StatInfo
{
    //부상 입은 수치
    public float curDamage;
    public float curSpeed;
    public float curDefence;
    public float curAttackSpeed;
    public float curAttackRange;
    public float curCriticalPercent;
    public float curAccuracy;
    public float curMissing;

    //정상 수치
    public float damage;
    public float speed;
    public float defence;
    public float attackSpeed;
    public float attackRange;
    public float criticalPercent;
    public float accuracy;
    public float missing;

    public int level;
    public int exp;

    //[NonSerialized]
    //public Dictionary<PartType, Part> partDictionary = new();
    //[NonSerialized]
    //public Dictionary<PartType, Part> maxPartDictionary = new();

    //public void BuildDictionary(PartData partHp)
    //{
    //    foreach (var part in partHp.parts)
    //    {

    //        PartType partType = PartType.Head;
    //        switch (part.partName)
    //        {
    //            case "Head":
    //                partType = PartType.Head;
    //                break;
    //            case "Body":
    //                partType = PartType.Body;
    //                break;
    //            case "Arm":
    //                partType = PartType.Arm;
    //                break;
    //            case "Leg":
    //                partType = PartType.Leg;
    //                break;
    //            case "Blood":
    //                partType = PartType.Blood;
    //                break;
    //        }

    //        maxPartDictionary.Add(partType, part); // 최대 HP를 가진 부위 정보
    //        partDictionary.Add(partType, part); // 예시로 Head만 추가, 나머지 부위도 추가해야 함.
    //    }
    //}

    public void Initialize()
    {
        //부상 입은 수치 초기화
        curDamage = damage;
        curSpeed = speed;
        curDefence = defence;
        curAttackSpeed = attackSpeed;
        curAttackRange = attackRange;
        curCriticalPercent = criticalPercent;
        curAccuracy = accuracy;
        curMissing = missing;
    }

    //초기 능력치 캐릭터 생성
    //public StatInfo(CharacterData charData, UnitStatWeight statWeight)
    //{
    //    BuildDictionary(charData.unitStatBase.initPart);

    //    this.damage = charData.unitStatBase.initDamage * statWeight.damage;
    //    this.speed = charData.unitStatBase.initSpeed * statWeight.speed;
    //    this.defence = charData.unitStatBase.initDefence * statWeight.defence;
    //    this.attackSpeed = charData.unitStatBase.initAttackSpeed * statWeight.attackSpeed;
    //    this.attackRange = charData.unitStatBase.initAttackRange * statWeight.attackRange;
    //    this.criticalPercent = charData.unitStatBase.initCriticalPercent * statWeight.criticalPercent;
    //    this.accuracy = charData.unitStatBase.initAccuracy * statWeight.accuracy;
    //    this.missing = charData.unitStatBase.initMissing * statWeight.missing;
    //    Initialize();
    //}

    public void AddStat(DesignEnums.StatType type, float value)
    {
        string action = value >= 0 ? "증가" : "감소";
        Debug.Log($"[AddStat] {action} 시도: {type} {(value >= 0 ? "+" : "")}{value}");
        switch (type)
        {
            case DesignEnums.StatType.Damage: damage += value; break;
            case DesignEnums.StatType.Speed: speed += value; break;
            case DesignEnums.StatType.Defence: defence += value; break;
            case DesignEnums.StatType.AttackSpeed: attackSpeed += value; break;
            case DesignEnums.StatType.AttackRange: attackRange += value; break;
            case DesignEnums.StatType.CriticalPercent: criticalPercent += value; break;
            case DesignEnums.StatType.Accuracy: accuracy += value; break;
            case DesignEnums.StatType.Missing: missing += value; break;
            case DesignEnums.StatType.Hp:
                // maxHp += value; / Hp 변수 생성 후 주석 해제
                // currentHp += value; // 체력 증가 시 현재 체력도 같이 회복
                // if (currentHp > maxHp)
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

    [System.Serializable]
    public class TecInfo
    {
        public List<float> tecValue; //기술 효능 값.
    }

    public enum PartType
    {
        Head,   //머리
        Body,   //몸통
        Arm,    //팔
        Leg,    //다리
        Blood   //피
    }

    //TEST : 검, 창, 해머, 활
    public enum TecType //TODO :  기술 숙련도 타입으로 수정 예정
    {
        Sword,
        Spear,
        Hammer,
        Bow,
        UnLock,
        Run,
        Attack
    }

    public enum StatType
    {
        Hp,
        Damage,
        Speed,
        Defence,
        AttackSpeed,
        AttackRange,
        CriticalPercent,
        Accuracy,
        Missing,
    }
