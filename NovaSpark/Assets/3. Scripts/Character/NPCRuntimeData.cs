using System.Collections.Generic;
using UnityEngine;

public class NPCRuntimeData
{
    [field : Header("불변")]
    public int characterID;
    public List<int> DiaglogList = new();
    
    public DesignEnums.NPCType npcType;
    public int exp = 10;

    public int playerID;
    public List<int> StartItemIds = new();
    public List<int> StartItemCount = new();

    public NPCRuntimeData(int key)
    {
        var NPCData = DataManager.Instance.NPCDataByID[key];
        this.characterID = NPCData.characterID;
        DiaglogList.Clear();
        DiaglogList.AddRange(NPCData.DiaglogList);
        npcType = NPCData.npcType;
    }
}