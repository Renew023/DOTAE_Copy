using System;
using System.Collections.Generic;
[Serializable]
public class FarmingData {
    public int FarmingBlockID;
    public string name_kr;
    public string name_en;
    public string description;
    public int blockHp;
    public int dropCount;
    public int defence;
    public List<int> getItemIdList;
    public List<int> getItemMaxDropValue;
    public List<float> itemDropValue;
    public float respawnCooltime;
    public int toolId;
    public float toolDamageUpPercent;
}
