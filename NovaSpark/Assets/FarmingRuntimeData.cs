using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmingRuntimeData
{
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

    public FarmingRuntimeData(int key)
    {
        FarmingBlockID = DataManager.Instance.farmingData[key].FarmingBlockID;
        name_kr = DataManager.Instance.farmingData[key].name_kr;
        name_en = DataManager.Instance.farmingData[key].name_en;
        description = DataManager.Instance.farmingData[key].description;
        blockHp = DataManager.Instance.farmingData[key].blockHp;
        dropCount = DataManager.Instance.farmingData[key].dropCount;
        defence = DataManager.Instance.farmingData[key].defence;
        getItemIdList = DataManager.Instance.farmingData[key].getItemIdList;
        getItemMaxDropValue = DataManager.Instance.farmingData[key].getItemMaxDropValue;
        itemDropValue = DataManager.Instance.farmingData[key].itemDropValue;
        respawnCooltime = DataManager.Instance.farmingData[key].respawnCooltime;
        toolId = DataManager.Instance.farmingData[key].toolId;
        toolDamageUpPercent = DataManager.Instance.farmingData[key].toolDamageUpPercent;
        //Debug.Log($"FarmingRuntimeData created for FarmingBlockID: {FarmingBlockID}, Name: {name_kr}");
    }
}
