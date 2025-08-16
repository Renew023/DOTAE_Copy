using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

[Serializable]
public class VillageRelationEntry
{
    public int fromVillageId;
    public int toVillageId;
    public int relation;
}

[Serializable]
public class VillageRelationSaveData
{
    public List<VillageRelationEntry> relationList = new();
}