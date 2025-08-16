using System;
using System.Collections.Generic;
[Serializable]
public class EnemyData {
    public int characterID;
    public bool isFirst;
    public float targetingRange;
    public float wanderMoveRange;
    public float AttackRange;
    public List<DesignEnums.PlaceType> spawnPlace;
    public List<int> DropItemIDs;
    public List<float> DropRates;
    public List<int> DropCounts;
    public int DropGoldMin;
    public int DropGoldMax;
}
