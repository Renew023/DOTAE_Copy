using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public interface IInventory
{
    bool AddItem(Item item, int amount);
    bool RemoveItem(Item item, int index);
    List<Slot> GetSlots();
}

public class Inventory : MonoBehaviourPunCallbacks, IInventory
{
    [SerializeField] protected List<Slot> _slots = new();
    public virtual InventoryOwnerType OwnerType { get; }

    public int gold = 0; // 가진 돈

    public void Init(int InvenSize) // 인벤토리 사이즈 설정
    {
        if (_slots == null)
            _slots = new List<Slot>();

        _slots.Clear();

        for (int i = 0; i < InvenSize; i++)
            _slots.Add(new Slot());
    }

    public List<Slot> GetSlots() // 자신의 슬롯 현황을 반환
    {
        if (_slots == null)
        {
            Debug.LogError("slots가 초기화되지 않음!");
        }
        return _slots;
    }

    public virtual bool AddItem(Item item, int amount)
    {

        if (item == null)
        {
            Debug.LogWarning("null item 추가 시도");
            return false;
        }

        // 중첩 가능한 아이템이면 기존 슬롯에 더하기 시도
        if (item.isStackable)
        {
            foreach (var slot in _slots)
            {
                if (!slot.IsEmpty && slot.item.id == item.id)
                {
                    slot.quantity += amount;
                    return true;
                }
            }
        }

        // 빈 슬롯에 클론해서 추가
        foreach (var slot in _slots)
        {
            if (slot.IsEmpty)
            {
                slot.item = item.Clone();  // ← 여기서 클론
                slot.quantity = amount;
                slot.itemName = slot.item.name_kr; // 아이템 이름 동기화
                return true;
            }
        }
        return false; // 인벤토리 꽉참
    }

    //public virtual List<Slot> GetItemsByType(ItemType type) 아이템 타입에 따른 창 분리 준비
    //{
    //    return slots.Where(s => !s.IsEmpty && s.item.type == type).ToList();
    //}

    public virtual void SortItem() // 정렬기능
    {
        // 스택이 가능한 아이템 중 같은 아이템이 있는지 확인 후 병합
        for (int i = 0; i < _slots.Count; i++)
        {
            var slotA = _slots[i];
            if (slotA.IsEmpty || !slotA.item.isStackable) continue;

            for (int j = i + 1; j < _slots.Count; j++)
            {
                var slotB = _slots[j];
                if (slotB.IsEmpty) continue;

                if (slotA.item.id == slotB.item.id)
                {
                    slotA.quantity += slotB.quantity;
                    slotB.Clear();
                }
            }
        }
        // 아이템의 타입과 이름을 기준으로 정렬
        _slots = _slots
            .Where(s => !s.IsEmpty)
            .OrderBy(s => (int)s.item.ItemType)      // 1순위: ItemType (enum의 순서)
            .ThenBy(s => s.item.name_kr)            // 2순위: 이름
            .Concat(_slots.Where(s => s.IsEmpty))    // 빈 슬롯은 뒤로 보내기
            .ToList();
    }

    // 모든 아이템을 보내는 기능
    public virtual void SendAllItem(IInventory targetInventory)
    {
        if (targetInventory == null)
        {
            Debug.LogError("targetInventory is NULL!!");
            return;
        }
        for (int i = 0; i < _slots.Count; i++) // 자신의 인벤토리를 돌며
        {
            var fromSlot = _slots[i];
            // 비어있으면 스킵
            if (fromSlot.IsEmpty)
            {
                continue;
            }
            var targetSlots = targetInventory.GetSlots(); // 대상 슬롯의 현황을 반환 받아와서

            bool moved = false;


            foreach (var targetSlot in targetSlots) // 대상 인벤토리를 돌고
            {
                if (!targetSlot.IsEmpty && targetSlot.item.id == fromSlot.item.id && fromSlot.item.isStackable) // 창고의 아이템과 같은 아이템이 있으면
                {
                    targetSlot.quantity += fromSlot.quantity; // 스택을 쌓아줌
                    fromSlot.Clear();
                    moved = true;
                    break; // 병합 완료
                }
            }

            if (!moved)
            {
                foreach (var targetSlot in targetSlots)
                {
                    if (targetSlot.IsEmpty)
                    {
                        targetSlot.item = fromSlot.item; // 같은 아이템이 없으면 슬롯에 넣기
                        targetSlot.quantity = fromSlot.quantity; // 스택을 동일하게 변경
                        targetSlot.itemName = targetSlot.item.name_kr; // 아이템 이름 동기화
                        fromSlot.Clear(); // 프롬의 슬롯은 비우기
                        moved = true;
                        break;
                    }
                }
            }
           
            if (!moved)
            {
                Debug.LogWarning("타겟 인벤토리에 공간 부족");
                break;
            }
        }
    }
    // 같은 아이템을 보내는 기능
    public virtual void MergeSameItem(IInventory targetInventory)
    {
        for (int i = 0; i < _slots.Count; i++) // 자신의 인벤토리를 돌며
        {
            var fromSlot = _slots[i];
            // 비어있으면 스킵
            if (fromSlot.IsEmpty)
                continue;

            var targetSlots = targetInventory.GetSlots(); // 대상 슬롯의 현황을 반환 받아와서

            foreach (var targetSlot in targetSlots) // 대상 인벤토리를 돌고
            {
                if (!targetSlot.IsEmpty && targetSlot.item == fromSlot.item && fromSlot.item.isStackable) // 창고의 아이템과 같은 아이템이 있으면
                {
                    targetSlot.quantity += fromSlot.quantity; // 스택을 쌓아줌
                    fromSlot.Clear();
                    break; // 병합 완료
                }
            }
        }
    }
    protected void ClearAllSlots()
    {
        foreach (var slot in _slots)
        {
            slot.Clear(); // 슬롯 비우기 (슬롯 자체는 유지)
        }
    }

    public virtual void SyncInventoryNetwork() { }

    public virtual float GetTotalWeight()
    {
        float totalWeight = 0f;
        foreach (var slot in GetSlots())
        {
            if (!slot.IsEmpty && slot.item != null)
            {
                totalWeight += slot.item.weight * slot.quantity;
            }
        }
        return totalWeight;
    }
    public bool AddItemToSlot(int index, Item item, int amount)
    {
        var slot = _slots[index];

        if (slot.IsEmpty)
        {
            slot.item = item;
            slot.quantity = amount;
            return true;
        }

        // 스택 가능한 같은 아이템이면 수량 추가
        if (slot.item.id == item.id && item.isStackable)
        {
            slot.quantity += amount;
            return true;
        }

        return false; // 해당 슬롯에 넣을 수 없음
    }

    public virtual bool RemoveItem(Item item, int amount)
    {
        int remaining = amount;

        for (int i = 0; i < _slots.Count; i++)
        {
            var slot = _slots[i];
            if (!slot.IsEmpty && slot.item.id == item.id)
            {
                if (slot.quantity > remaining)
                {
                    slot.quantity -= remaining;
                    return true;
                }
                else
                {
                    remaining -= slot.quantity;
                    slot.Clear();
                    if (remaining <= 0)
                        return true;
                }
            }
        }

        return false; // 충분한 수량을 찾지 못했음
    }

    public void AddGold(int amount)
    {
        gold += amount;
        gold = Mathf.Max(0, gold); // 음수 방지 등 검증
    }
    public bool HasEnoughGold(int cost)
    {
        return gold >= cost;
    }
}
