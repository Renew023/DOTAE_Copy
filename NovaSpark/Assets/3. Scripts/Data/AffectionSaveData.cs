using System;
using System.Collections.Generic;

[Serializable]
public class AffectionEntry
{
    public int npcId;
    public int affection;
}

[Serializable]
public class AffectionSaveData
{
    public List<AffectionEntry> affectionList = new();
}