using System;
using System.Collections.Generic;
[Serializable]
public class Material {
    public int id;
    public string name_kr;
    public string name_en;
    public DesignEnums.Rarity rarity;
    public string description;
    public List<DesignEnums.MaterialUse> type;
    public string icon;
    public int price;
    public bool isStackable;
    public int maxStack;
    public DesignEnums.EffectType Alchemy;
    public float weight;
}
