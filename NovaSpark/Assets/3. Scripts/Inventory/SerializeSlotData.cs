using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializeSlotData
{
    public int itemId;          // 아이템 ID
    public int quantity;        // 수량
    public int enhanceLevel;    // 강화 레벨, 필요에 따라 추가 상태들

    public SerializeSlotData() { }

    public SerializeSlotData(Slot slot)
    {
        if (slot == null || slot.IsEmpty || slot.item == null)
        {
            itemId = 0;
            quantity = 0;
            enhanceLevel = 0;
            return;
        }

        itemId = slot.item.id;
        quantity = slot.quantity;
        // 강화 레벨 같은 커스텀 데이터는 아이템에 따라 다르게 처리
        if (slot.item is EquipmentItem eq)
            enhanceLevel = eq.enhanceLevel;
        else
            enhanceLevel = 0;
    }
}

[Serializable]
public class SerializeSlotDataList
{
    public List<SerializeSlotData> slots;

    public SerializeSlotDataList(SerializeSlotData[] array)
    {
        slots = new List<SerializeSlotData>(array);
    }

    public SerializeSlotData[] ToArray()
    {
        return slots.ToArray();
    }
}