using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 제작 슬롯 UI를 생성하고 조작하는 클래스
/// </summary>
public class CraftUI : PopupUI
{
    [SerializeField] private Transform slotParent;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text[] materialTexts; // 재료 텍스트 여러 개 등록
    [SerializeField] private Image[] materialIcons;
    [SerializeField] private Button craftButton;
    [Header("UI CloseButton")]
    [SerializeField] private Button _closeButton;
    private Craft craft;
    private PlayerInventory player;
    private EquipmentItem selectedItem;

    private void Start()
    {
        _closeButton.onClick.AddListener(HidePanel);
        craftButton.onClick.AddListener(OnCraftButtonClicked);
        for (int i = 0; i < materialTexts.Length; i++)
        {
            materialIcons[i].gameObject.SetActive(false);
            materialTexts[i].gameObject.SetActive(false);
        }
    }

    public void Init(Craft craft, PlayerInventory playerInventory, PlayerRecipe recipe)
    {
        foreach (Transform child in slotParent)
        {
            Destroy(child.gameObject);
        }

        this.craft = craft;
        this.player = playerInventory;

        craft.ConnectPlayer(player,recipe); // 연결 필수
        var slots = craft.GetPlayerUnlockedSlots(); // 슬롯 내부에 EquipmentItem 포함


        foreach (var slot in slots)
        {
            if (slot.item is EquipmentItem equipment)
            {
                var go = Instantiate(slotPrefab, slotParent);
                var slotUI = go.GetComponent<CraftSlotUI>();
                slotUI.Setup(equipment, OnSlotClicked);
            }
            else
            {
                Debug.LogWarning($"[CraftUI] 장비 아이템이 아닙니다: {slot.item}");
            }
        }
    }

    private void OnSlotClicked(Item item)
    {
        if (item is not EquipmentItem equipment)
        {
            Debug.LogWarning("선택된 아이템이 제작 가능한 장비가 아닙니다.");
            return;
        }

        selectedItem = equipment;
        itemNameText.text = equipment.name_kr;
        descriptionText.text = equipment.description;

        for (int i = 0; i < materialTexts.Length; i++)
        {
            if (i < equipment.producedMaterial.Count)
            {
                int id = equipment.producedMaterial[i];
                int required = equipment.requiredQuantity[i];
                int owned = player.CountItem(id);

                var materialData = DataManager.Instance.GetItemByID(id);

                // 아이콘 변경
                int version = i;
                AddressableManager.Instance.LoadIconToImage(
                    materialIcons[i],
                    materialData.icon,
                    version,
                    () => version
                    );
        
                materialTexts[i].text = $"{materialData.name_kr}: {owned} / {required}";
                materialTexts[i].color = (owned >= required) ? Color.white : Color.red;
                materialIcons[i].gameObject.SetActive(true);
                materialTexts[i].gameObject.SetActive(true);
            }
            else
            {
                materialIcons[i].gameObject.SetActive(false);
                materialTexts[i].gameObject.SetActive(false);
            }
        }

        craftButton.interactable = craft.CanCraft(equipment, player);
    }

    private void OnCraftButtonClicked()
    {
        if (selectedItem == null)
            return;

        craftButton.interactable = false; // 먼저 버튼 비활성화

        if (craft.TryCraft(selectedItem, player))
        {
            Debug.Log("제작 완료!");
        }

        OnSlotClicked(selectedItem); // UI 갱신 (재료 수량 표시)
    }
}
