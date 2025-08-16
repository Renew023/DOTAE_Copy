using System;
using System.Collections.Generic;
[Serializable]
public class Consumable {
    public int id;
    public string name_kr;
    public string name_en;
    public DesignEnums.Rarity rarity;
    public string description;
    public string icon;
    public List<DesignEnums.EffectType> effectType;
    public List<int> effectValue;
    public bool isStackable;
    public int maxStack;
    public List<int> duration;
    public int price;
    public float weight;
}
