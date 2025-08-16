using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static DesignEnums;

public enum ItemType
{
    Equipment,
    Consumable,
    Material,
}

[Serializable]
public abstract class Item
{
    public int id { get; protected set; }
    public ItemType ItemType { get; protected set; }
    public string name_kr { get; protected set; }
    public string name_en { get; protected set; }
    public string icon { get; protected set; }
    public bool isStackable { get; protected set; }
    public int price { get; protected set; }
    public float weight { get; protected set; } = 1f; // 기본 무게 설정, 필요시 수정 가능
    public abstract Item Clone();

    public virtual void Use(CharacterRuntimeData character, Player player)
    {
        Debug.LogError($"[Item.Use] {name_kr} 아이템은 사용 불가능한 타입입니다.");
    }
}

[Serializable]
public class EquipmentItem : Item
{
    public string description;
    public DesignEnums.Rarity rarity;
    public List<DesignEnums.StatType> statType;
    public List<float> statValue;
    public DesignEnums.EquipmentType type;
    public int enhanceLevel;
    public int maxEnhanceLevel;
    public float baseEnhanceRate;
    public float enhanceAtkRatio;
    public float enhanceCostMultiplier;
    public int maxStack;
    public string prefabName;
    public bool canProduced;
    public List<int> producedMaterial;
    public List<int> requiredQuantity;
    public string ProjectileID;

    private float baseValue;
    private bool baseStatCached = false;
    public float lastEnhanceCost = -1;


    public EquipmentItem(Equipment data)
    {
        id = data.id;
        name_kr = data.name_kr;
        name_en = data.name_en;
        description = data.description;
        rarity = data.rarity;
        icon = data.icon;
        statType = new List<DesignEnums.StatType>(data.statType);
        statValue = new List<float>(data.statValue);
        type = data.type;
        enhanceLevel = data.enhanceLevel;
        maxEnhanceLevel = data.maxEnhanceLevel;
        baseEnhanceRate = data.baseEnhanceRate;
        enhanceAtkRatio = data.enhanceAtkRatio;
        enhanceCostMultiplier = data.enhanceCostMultiplier;
        isStackable = data.isStackable;
        maxStack = data.maxStack;
        prefabName = data.prefabName;
        canProduced = data.canProduced;
        producedMaterial = data.producedMaterial;
        requiredQuantity = data.requiredQuantity;
        ProjectileID = data.ProjectileID;
        price = data.price;

        baseStatCached = false;
    }
    public EquipmentItem(EquipmentItem data) // 수정: EquipmentItem 대신 EquipmentItem을 매개변수로 받도록 변경
    {
        id = data.id;
        name_kr = data.name_kr;
        name_en = data.name_en;
        description = data.description;
        rarity = data.rarity;
        icon = data.icon;
        statType = new List<DesignEnums.StatType>(data.statType);
        statValue = new List<float>(data.statValue);
        type = data.type;
        enhanceLevel = data.enhanceLevel;
        maxEnhanceLevel = data.maxEnhanceLevel;
        baseEnhanceRate = data.baseEnhanceRate;
        enhanceAtkRatio = data.enhanceAtkRatio;
        enhanceCostMultiplier = data.enhanceCostMultiplier;
        isStackable = data.isStackable;
        maxStack = data.maxStack;
        prefabName = data.prefabName;
        canProduced = data.canProduced;
        producedMaterial = data.producedMaterial;
        requiredQuantity = data.requiredQuantity;
        ProjectileID = data.ProjectileID;
        price = data.price;
    }
    public override Item Clone()
    {
        return new EquipmentItem(this); // 복사 생성자 사용
    }
    public IEnumerable<(DesignEnums.StatType, float)> GetStatPairs()
    {
        Debug.Log($"[GetStatPairs] statType.Count: {statType.Count}, statValue.Count: {statValue.Count}");

        for (int i = 0; i < statType.Count; i++)
        {
            if (i >= statValue.Count)
            {
                Debug.LogError($"[GetStatPairs] statValue 누락! Index {i} 초과.");
                yield break;
            }

            yield return (statType[i], statValue[i]);
        }
    }

    public int GetEnhancedEffect()
    {
        if (statValue == null || statValue.Count == 0)
        {
            return 0;
        }

        return Mathf.RoundToInt(statValue[0]);
    }

    public int GetEnhanceCost()
    {
        if (enhanceLevel == 0)
        {
            return Mathf.RoundToInt(price * 0.15f); // 초기 강화 비용 (15%)
        }

        float cost = price * 0.2f * Mathf.Pow(1 + enhanceCostMultiplier, enhanceLevel);
        return Mathf.RoundToInt(cost);
    }

    public float GetCurrentSuccessRate()
    {
        float rate = baseEnhanceRate - enhanceLevel * 0.03f;
        return Mathf.Clamp(rate, 0.1f, 1f);
    }

    public bool CanEnhance()
    {
        return enhanceLevel < maxEnhanceLevel;
    }

    public void CacheBaseStat()
    {
        if (baseStatCached) return;

        if (statValue != null && statValue.Count > 0)
        {
            baseValue = statValue[0];
            baseStatCached = true;
        }
    }

    public void UpdateEnhancedStat()
    {
        if (!baseStatCached) return;

        // 강화 레벨에 따라 공격력 또는 방어력 업데이트
        statValue[0] = baseValue * Mathf.Pow(1f + enhanceAtkRatio, enhanceLevel);
    }
}

public class ConsumableItem : Item
{
    public DesignEnums.Rarity rarity;
    public List<DesignEnums.EffectType> effectType = new();
    public List<int> effectValue = new();
    public int maxStack;
    public List<int> duration = new();

    public ConsumableItem(Consumable data)
    {
        id = data.id;
        name_kr = data.name_kr;
        name_en = data.name_en;
        rarity = data.rarity;
        icon = data.icon;

        effectType.Clear();
        effectType.AddRange(data.effectType);

        effectValue.Clear();
        effectValue.AddRange(data.effectValue);
        isStackable = data.isStackable;
        maxStack = data.maxStack;

        duration.Clear();
        duration.AddRange(data.duration);
        price = data.price;
    }
    public override Item Clone()
    {
        return new ConsumableItem(this);
    }
    public ConsumableItem(ConsumableItem data)
    {
        id = data.id;
        name_kr = data.name_kr;
        name_en = data.name_en;
        rarity = data.rarity;
        icon = data.icon;
        effectType = data.effectType;
        effectValue = data.effectValue;
        isStackable = data.isStackable;
        maxStack = data.maxStack;
        duration = data.duration;
        price = data.price;
    }

    public override void Use(CharacterRuntimeData character, Player player)
    {
        for (int i = 0; i < effectType.Count; i++)
        {
            var type = effectType[i];
            var value = effectValue.Count > i ? effectValue[i] : 0;
            var time = duration.Count > i ? duration[i] : 0;

            switch (type)
            {
                case DesignEnums.EffectType.Heal:
                    character.health.Recover(value);
                    break;
                case DesignEnums.EffectType.thirsty:
                    player.PlayerRuntimeData.thirsty.Recover(value);
                    break;
                case DesignEnums.EffectType.hunger:
                    player.PlayerRuntimeData.hungry.Recover(value);
                    break;
                case DesignEnums.EffectType.UnlockRecipe:
                    player.PlayerRecipe?.Unlock(value);
                    break;
                    // 기타 즉시 효과 처리
            }

            if (time > 0)
            {
                var buff = new Buff(type, value, time);
                player.BuffData.AddBuff(buff);
            }
        }

        Debug.Log($"{name_kr} 사용 완료");
    }
}

public class MaterialItem : Item
{
    public DesignEnums.Rarity rarity;
    public string description;
    public List<DesignEnums.MaterialUse> type = new();
    public int maxStack;
    public DesignEnums.EffectType effectType;

    public MaterialItem(Material data)
    {
        id = data.id;
        name_kr = data.name_kr;
        name_en = data.name_en;
        rarity = data.rarity;
        description = data.description;

        type.Clear();
        type.AddRange(data.type);


        icon = data.icon;
        price = data.price;
        isStackable = data.isStackable;
        maxStack = data.maxStack;
        effectType = data.Alchemy;
    }
    public MaterialItem(MaterialItem data)
    {
        id = data.id;
        name_kr = data.name_kr;
        name_en = data.name_en;
        rarity = data.rarity;
        description = data.description;
        type = data.type;
        icon = data.icon;
        price = data.price;
        isStackable = data.isStackable;
        maxStack = data.maxStack;
        effectType = data.effectType;
    }
    public override Item Clone()
    {
        return new MaterialItem(this);
    }
}
