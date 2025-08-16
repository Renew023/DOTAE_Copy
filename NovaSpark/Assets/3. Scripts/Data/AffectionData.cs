using System;
using UnityEngine;

[Serializable]
public class AffectionData
{
    public int npcId;
    public int affection;
    private const int _minAffection = -100;
    private const int _maxAffection = 100;

    public AffectionLevel Level =>
        affection < 0 ? AffectionLevel.Hostile :
        affection > 0 ? AffectionLevel.Friendly :
        AffectionLevel.Neutral;

    public void Change(int amount)
    {
        affection = Mathf.Clamp(affection + amount, _minAffection, _maxAffection);
    }
}