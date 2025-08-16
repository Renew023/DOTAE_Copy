using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EnhanceItemSlot : MonoBehaviour, 
    IDropHandler,
    IPointerEnterHandler,
    IPointerExitHandler
{
    [SerializeField]
    private Image _icon;

    private Slot _slotData;

    [SerializeField]
    private EnhanceUI _enhanceUI;

    public void OnDrop(PointerEventData eventData)
    {
        var source = eventData.pointerDrag?.GetComponent<InventoryItemSlot>();
        if (source == null || source.ItemData == null)
            return;

        var slot = source.GetSlot();
        if (slot.item is not EquipmentItem equipmentItem)
        {
            Debug.Log("강화 가능한 장비가 아님");
            return;
        }

        _slotData = slot;
        _enhanceUI.SetSelectedEquipment(equipmentItem);

        Refresh();
    }

    private void Refresh()
    {
        if (_slotData == null || _slotData.item == null || _slotData.quantity <= 0)
        {
            _icon.enabled = false;
        }
        else
        {
            _icon.enabled = true;
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_slotData != null && TooltipUI.Instance != null)
        {
            string title = _slotData.item.name_kr;
            string body = BuildTooltipBody(_slotData.item);
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
