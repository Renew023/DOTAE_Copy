using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 제작 가능한 아이템 정보를 담는 UI 슬롯
/// </summary>
public class CraftSlotUI : MonoBehaviour, 
    IPointerClickHandler, 
    IPointerDownHandler, 
    IPointerUpHandler,
    IPointerEnterHandler,
    IPointerExitHandler
{
    [SerializeField] private Item item;
    [SerializeField] private Slot _slotData;
    private System.Action<EquipmentItem> onClick;
    private int _iconVersion;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private Image _icon;
    private void Awake()
    {
        // Raycast Target 설정 확인 및 수정
        if (_icon != null)
        {
            _icon.raycastTarget = true;
        }
        else
        {
            Debug.LogWarning($"CraftSlotUI: _icon이 할당되지 않음 - {gameObject.name}");
        }
    }

    public void Setup(Item item, System.Action<EquipmentItem> onClick)
    {
        this.item = item;
        this.onClick = onClick;

        _nameText.text = item.name_kr;
        int version = ++_iconVersion;

        AddressableManager.Instance.LoadIconToImage(_icon, item.icon, version, () => _iconVersion);
    }

    public void OnPointerClick(PointerEventData eventData)
    {

        if (eventData.button == PointerEventData.InputButton.Left)
        {

            if (onClick != null)
            {
                onClick.Invoke(item as EquipmentItem);
            }
            else
            {
                Debug.LogWarning("onClick 델리게이트가 연결되지 않음");
            }
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        // 마우스 클릭 시 추가 동작이 필요하면 여기에 구현
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // 마우스 클릭 해제 시 추가 동작이 필요하면 여기에 구현
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null && TooltipUI.Instance != null)
        {
            string title = item.name_kr;
            string body = BuildTooltipBody(item);
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