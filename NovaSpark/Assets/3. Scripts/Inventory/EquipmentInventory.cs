using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class EquipmentInventory : MonoBehaviour
{
    private Dictionary<DesignEnums.EquipmentType, Slot> equippedSlots = new();
    public PlayerInventory playerInventory;
    public Player player;

    private void Awake()
    {
        playerInventory = GetComponent<PlayerInventory>();
        player = GetComponent<Player>();
        foreach (DesignEnums.EquipmentType type in Enum.GetValues(typeof(DesignEnums.EquipmentType)))
        {
            equippedSlots[type] = new Slot();
        }
    }

    public void EquipDefaultWeapon()
    {
        int defaultWeaponId = 1001;
        Item item = DataManager.Instance.GetItemByID(defaultWeaponId);
        EquipmentItem weapon = item?.Clone() as EquipmentItem;

        if (weapon != null)
        {
            Equip(weapon);
        }
        else
        {
            Debug.LogWarning("기본 무기 아이템을 찾을 수 없습니다.");
        }
    }

    public bool Equip(EquipmentItem item)
    {
        var type = item.type;
        var slot = equippedSlots[type];
        if (!equippedSlots.ContainsKey(type))
        {
            Debug.LogError($"[Equip] equippedSlots에 {type} 없음!");
            return false;
        }

        // 요구레벨보다 자신의 레벨이 낮으면 false
        //if (item.요구레벨 > player.level)
        //{
        //    return false; 
        //}

        // 해당 부위에 이미 착용 중이면 교체

        if (!slot.IsEmpty)
        {
            Unequip(type);
        }

        slot.item = item;
        slot.quantity = 1;


        player.Equip(item);
        return true;
    }

    public void Unequip(DesignEnums.EquipmentType type)
    {
        if (equippedSlots.TryGetValue(type, out var slot) && !slot.IsEmpty)
        {
            player.UnEquip(slot.item as EquipmentItem); 
            playerInventory.AddItem(slot.item, slot.quantity); // 인벤토리로 복귀
            slot.Clear();
        }
        else
        {
            Debug.Log("[Unequip] 장비 없음 혹은 슬롯 못 찾음");
        }
    }

    public Slot GetSlot(DesignEnums.EquipmentType type) => equippedSlots[type];
}
