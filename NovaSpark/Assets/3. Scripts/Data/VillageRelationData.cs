using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class VillageRelationData
{
    public int fromVillageId;
    public int toVillageId;
    public int relation;
    private const int _minRelation = -100;
    private const int _maxRelation = 100;

    public RelationLevel Level =>
        relation < 0 ? RelationLevel.Hostile :
        relation > 0 ? RelationLevel.Friendly :
        RelationLevel.Neutral;

    public void Change(int amount)
    {
        relation = Mathf.Clamp(relation + amount, _minRelation, _maxRelation);
    }
}