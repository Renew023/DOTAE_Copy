using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;


public class PlayerInventory : Inventory
{
    [SerializeField] private int _InventorySize = 40;
    private Player _player;

    public float PlayerMaxForce => _player != null ? _player.characterRuntimeData.force.Max : 0f;

    public override InventoryOwnerType OwnerType => InventoryOwnerType.Player;
    private void Awake()
    {
        _player = GetComponent<Player>();
        Init(_InventorySize); // 슬롯 수 설정
    }

    //public float GetTotalWeight() // 아이템무게와 플레이어가 감당할 무게가 생긴다면 사용 고려
    //{
    //    return _slots.Where(s => !s.IsEmpty).Sum(s => s.item.weight * s.quantity);
    //}

    

    /// <summary>
    /// 특정 ID 아이템이 지정 수량 이상 있는지 확인
    /// </summary>
    public bool HasItem(int itemID, int amount)
    {
        int total = 0;

        foreach (var slot in GetSlots())
        {
            if (!slot.IsEmpty && slot.item.id == itemID)
            {
                total += slot.quantity;
                if (total >= amount)
                    return true;
            }
        }

        return false;
    }

    public int CountItem(int itemID)
    {
        int total = 0;
        foreach (var slot in GetSlots())
        {
            if (!slot.IsEmpty && slot.item.id == itemID)
                total += slot.quantity;
        }
        return total;
    }

    /// <summary>
    /// 지정 수량만큼 아이템을 소비 (슬롯 분산 처리)
    /// </summary>
    public bool ConsumeItem(int itemID, int amount)
    {
        if (CountItem(itemID) < amount)
        {
            Debug.LogWarning($"[ConsumeItem] 재료 부족: {itemID}, 필요량 {amount}");
            return false;
        }

        foreach (var slot in GetSlots())
        {
            if (!slot.IsEmpty && slot.item.id == itemID)
            {
                if (slot.quantity >= amount)
                {
                    slot.quantity -= amount;
                    if (slot.quantity == 0)
                        slot.Clear();
                    return true;
                }
                else
                {
                    amount -= slot.quantity;
                    slot.Clear();
                }
            }
        }
        return true;
    }

    public override bool AddItem(Item item, int amount)
    {
        if (amount == 0) return false;
        Debug.Log("item" + item.id);
        if (item == null)
            return false;

        float newWeight = GetTotalWeight() + item.weight * amount;

        if (_player != null && newWeight > _player.characterRuntimeData.force.Max)
        {
            Debug.LogWarning("더 이상 아이템을 들 수 없습니다! 무게 초과");
            return false;
        }

        UserHelpManager.Instance.CreateText("아이템 획득 :" + item.name_kr + " / " + amount + "개");

        return base.AddItem(item, amount);
    }

    public override float GetTotalWeight()
    {
        return base.GetTotalWeight();
    }
}
