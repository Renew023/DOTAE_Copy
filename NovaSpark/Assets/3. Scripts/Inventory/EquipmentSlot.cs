using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler,
    IDropHandler,
    IItemSlot,
    IPointerEnterHandler,
    IPointerExitHandler
{
    public DesignEnums.EquipmentType slotType; // 이 슬롯이 어떤 부위인지 지정

    [SerializeField] private Image iconImage;

    private int _iconVersion;

    private EquipmentInventory inventory;
    private EquipmentItem currentItem;

    public Item ItemData => currentItem;
    public int Quantity => 1;
    public Slot GetSlot() => inventory.GetSlot(slotType);

    public void Init(EquipmentInventory inventory, DesignEnums.EquipmentType type)
    {
        this.inventory = inventory;
        this.slotType = type;
        Refresh();
    }

    public async void Refresh()
    {
        if (inventory == null)
        {
            return;
        }

        var slot = inventory.GetSlot(slotType);
        if (!slot.IsEmpty && slot.item is EquipmentItem equip)
        {
            currentItem = equip;

            int version = ++_iconVersion;

            AddressableManager.Instance.LoadIconToImage(
                iconImage,
                equip.icon,
                version,
                () => _iconVersion
            );
        }
        else
        {
            currentItem = null;
            iconImage.enabled = false;
            iconImage.sprite = null;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        var source = DragSlot.Instance.dragSlot;

        if (source == null)
        {
            Debug.Log("[OnDrop] dragSlot이 null입니다.");
            return;
        }

        var item = source.GetSlot().item as EquipmentItem;

        if (item == null)
        {
            Debug.Log("드롭된 아이템이 장비가 아닙니다.");
            return;
        }

        // 타입이 일치하지 않으면 거부
        if (item.type != slotType)
        {
            Debug.Log($"잘못된 부위에 드롭됨. {item.type}은 {slotType} 슬롯에 장착할 수 없습니다.");
            return;
        }


        bool result = inventory.Equip(item);


        // 장착 시도
        if (result)
        {
            source.GetSlot().Clear(); // 인벤토리에서 제거
            source.Refresh();         // 장비 슬롯 갱신
            Refresh();
        }
        else
        {
            Debug.Log("장비 장착 실패");
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        DragSlot.Instance.SetColor(0);
        DragSlot.Instance.dragSlot = null;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (DragSlot.Instance.dragSlot == this)
        {
            DragSlot.Instance.transform.position = eventData.position;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (currentItem == null) return;

        DragSlot.Instance.dragSlot = this;
        DragSlot.Instance.DragSetImage(iconImage);
        DragSlot.Instance.transform.position = eventData.position;
    }

    public void SetSlotData(Item item, int quantity)
    {
        var slot = inventory.GetSlot(slotType);
        slot.item = item;
        slot.quantity = quantity;

        Refresh();
    }

    public void RequestUnequip()
    {
        inventory.Unequip(slotType);
    }

    public Inventory GetParentInventory()
    {
        return null;
    }

    public InventoryUI GetParentInventoryUI()
    {
        return null;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentItem != null && TooltipUI.Instance != null)
        {
            string title = currentItem.name_kr;
            string body = BuildTooltipBody(currentItem);
            TooltipUI.Instance.Show(title, body);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipUI.Instance?.HideTooltip();
    }

    private string BuildTooltipBody(Item item)
    {
        if (item is EquipmentItem e)
        {
            string stats = "";
            foreach (var (type, val) in e.GetStatPairs())
                stats += $"{type}: {val}\n";

            return
                $"{e.description}\n" +
                $"장비 타입: {e.type}\n" +
                $"강화: {e.enhanceLevel}/{e.maxEnhanceLevel}\n" +
                $"스탯:\n{stats}" +
                $"가격: {e.price}";
        }
        return "";
    }
}
