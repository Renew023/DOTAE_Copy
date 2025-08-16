using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using UnityEngine;

/// <summary>
/// 실제 제작 로직을 담당하는 클래스. 작업대 오브젝트에 붙는다.
/// </summary>
public class Craft : MonoBehaviour, IInteractable
{
    public PlayerInventory connectedPlayer;
    private PlayerRecipe recipe;

    public void ConnectPlayer(PlayerInventory player, PlayerRecipe recipe)
    {
        connectedPlayer = player;
        this.recipe = recipe;
    }

    public List<Slot> GetPlayerUnlockedSlots()
    {
        if (connectedPlayer == null)
            return new List<Slot>();
        if (recipe == null)
            return new List<Slot>();

        var ids = recipe.GetUnlockedRecipeIds();
        var slots = new List<Slot>();

        foreach (var id in ids)
        {
            var equipment = DataManager.Instance.GetEquipmentItem(id);
            if (equipment != null && equipment.canProduced)
            {
                slots.Add(new Slot
                {
                    item = equipment
                });
                Debug.Log(equipment.name_kr);
            }
        }
        return slots;
    }
    /// <summary>
    /// 플레이어가 이 아이템을 제작할 수 있는지 확인 (재료 충분한지)
    /// </summary>
    public bool CanCraft(EquipmentItem item, PlayerInventory player)
    {
        for (int i = 0; i < item.producedMaterial.Count; i++)
        {
            int materialId = item.producedMaterial[i];
            int requiredQty = item.requiredQuantity[i];

            if (!player.HasItem(materialId, requiredQty))
                return false;
        }
        return true;
    }

    /// <summary>
    /// 제작을 시도함. 재료가 충분하면 소모하고 결과물 추가
    /// </summary>
    public bool TryCraft(EquipmentItem item, PlayerInventory player)
    {
        if (!CanCraft(item, player))
            return false;

        // 재료 소모
        for (int i = 0; i < item.producedMaterial.Count; i++)
        {
            int materialId = item.producedMaterial[i];
            int requiredQty = item.requiredQuantity[i];
            player.ConsumeItem(materialId, requiredQty);
        }

        // 결과 아이템 지급
        player.AddItem(item, 1);
        Debug.Log($"[제작 성공] {item.name_kr}");
        return true;
    }

    public void GetInteractObjectType(bool isCan)
    {
        return;
    }

    public string PromptText()
    {
        return "제작하기";
    }

    public void OnInteract(CharacterObject owner)
    {
        if (owner.TryGetComponent<PlayerInventory>(out var inventory))
        {
            UIManager.Instance.ShowPanel(UIType.CraftUI);

            var craftUI = UIManager.Instance.popupUIByType[UIType.CraftUI] as CraftUI;
            craftUI.Init(this, inventory, owner.GetComponent<PlayerRecipe>());
        }
    }

    public bool IsCan()
    {
        return true;
    }
}
