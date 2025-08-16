using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuffData
{
    private Player _player;
    private List<ActiveBuff> _activeBuffs = new List<ActiveBuff>();

    public PlayerBuffData(Player player)
    {
        _player = player;
    }

    public void AddBuff(Buff buff)
    {
        var statType = buff.GetTargetStat();
        if (statType == null)
        {
            Debug.LogWarning($"Unknown stat type for buff: {buff.effectType}");
            return;
        }

        // 스탯 적용
        _player.AddBuffStat(statType.Value, buff.amount);

        // 버프 리스트에 등록
        _activeBuffs.Add(new ActiveBuff
        {
            buff = buff,
            remainingTime = buff.duration
        });
        Debug.Log($"[Buff] {buff.effectType} +{buff.amount} ({buff.duration}s)");
    }

    public void UpdateBuff(float deltaTime)
    {
        for (int i = _activeBuffs.Count - 1; i >= 0; i--)
        {
            _activeBuffs[i].remainingTime -= deltaTime;
            if (_activeBuffs[i].remainingTime <= 0f)
            {
                var statType = _activeBuffs[i].buff.GetTargetStat();
                if (statType != null)
                    _player.AddBuffStat(statType.Value, -_activeBuffs[i].buff.amount);
                Debug.Log($"[Buff End] {_activeBuffs[i].buff.effectType} 사라짐");
                _activeBuffs.RemoveAt(i);
            }
        }
    }

    private class ActiveBuff
    {
        public Buff buff;
        public float remainingTime;
    }
}
