using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyRuntimeData
{
    [field : Header("불변")]
    public int characterID;
    public bool isFirst = true;
    public float targetingRange;
    public float wanderMoveRange;
    public float AttackRange;
    public List<DesignEnums.PlaceType> spawnPlace = new();

    [field: Header("가변")]
    public int exp;

    public EnemyRuntimeData(int key)
    {
        var enemyData = DataManager.Instance.EnemyDataByID[key];
        characterID = enemyData.characterID;
        //isFirst = enemyData.isFirst;
        targetingRange = enemyData.targetingRange;
        wanderMoveRange = enemyData.wanderMoveRange;
        AttackRange = enemyData.AttackRange;
        spawnPlace.Clear();
        spawnPlace.AddRange(enemyData.spawnPlace);
    }
}