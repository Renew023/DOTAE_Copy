using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DesignEnums;

public class Buff : MonoBehaviour
{
    public DesignEnums.EffectType effectType;
    public float amount;
    public float duration;

    public Buff(DesignEnums.EffectType effectType, float amount, float duration)
    {
        this.effectType = effectType;
        this.amount = amount;
        this.duration = duration;
    }
    public DesignEnums.StatType? GetTargetStat()
    {
        return BuffEffectTypeConverter.ToStatType(effectType);
    }
}
public static class BuffEffectTypeConverter
{
    public static DesignEnums.StatType? ToStatType(DesignEnums.EffectType effectType)
    {
        return effectType switch
        {
            DesignEnums.EffectType.AttackBuff => DesignEnums.StatType.Damage,
            DesignEnums.EffectType.DefenceBuff => DesignEnums.StatType.Defence,
            DesignEnums.EffectType.MoveSpeedBuff => DesignEnums.StatType.Speed,
            DesignEnums.EffectType.AttackSpeedBuff => DesignEnums.StatType.AttackSpeed,
            _ => null
        };
    }
}