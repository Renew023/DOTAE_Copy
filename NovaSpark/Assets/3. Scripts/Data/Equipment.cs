using System;
using System.Collections.Generic;
[Serializable]
public class Equipment {
    public int id;
    public string name_kr;
    public string name_en;
    public string description;
    public DesignEnums.Rarity rarity;
    public string icon;
    public List<DesignEnums.StatType> statType;
    public List<float> statValue;
    public DesignEnums.EquipmentType type;
    public int enhanceLevel;
    public int maxEnhanceLevel;
    public float baseEnhanceRate;
    public float enhanceAtkRatio;
    public float enhanceCostMultiplier;
    public bool isStackable;
    public int maxStack;
    public string prefabName;
    public bool canProduced;
    public List<int> producedMaterial;
    public List<int> requiredQuantity;
    public string ProjectileID;
    public int price;
}
