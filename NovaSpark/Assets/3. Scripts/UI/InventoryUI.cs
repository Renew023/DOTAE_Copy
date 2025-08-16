using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Linq;
using System;

public enum InventoryFilter
{
    All,        // 전체
    Equipment,  // 장비
    Consumable, // 소비
    Material    // 기타(재료)
}
public enum InventoryOwnerType
{
    Player,
    NPC,
    Storage
}

[System.Serializable]
public class InventoryTabGroup
{
    public string tabName;           // UI에 표시할 이름 (예: "전체", "장비"…)
    public Toggle toggle;            // 연결된 토글
    public InventoryFilter filter;   // 이 탭이 어떤 필터를 적용할지
}

public class InventoryUI : PopupUI
{
    [Header("Inventory Tabs Settings")]
    public List<InventoryTabGroup> tabs;

    [Header("Toggle Color Settings")]
    [Tooltip("꺼졌을 때 토글 배경색")]
    [SerializeField] private Color _normalColor = Color.white;
    [Tooltip("켜졌을 때 토글 배경색")]
    [SerializeField] private Color _selectedColor = Color.green;

    [Header("Inventory ButtonSettings")]
    [SerializeField] private Button _closeButton;

    [Header("Slot Prefab and Container")]
    [Tooltip("인벤토리 슬롯으로 생성할 ItemSlot 프리팹")]
    [SerializeField] private InventoryItemSlot _itemSlotPrefab;
    [Tooltip("슬롯들을 배치할 부모 Transform")]
    [SerializeField] private Transform _slotContainer;

    [Header("Inventory Data Source")]
    [SerializeField] protected Inventory _inventory;
    [SerializeField] public Player player;


    [Header("Weight Display")]
    [Tooltip("현재 소지 중인 총 무게를 표시할 TextMeshPro 컴포넌트")]
    [SerializeField] private TMP_Text _weightText;

    [Header("Gold Display")]
    [Tooltip("현재 소지 중인 골드를 표시할 TextMeshPro 컴포넌트")]
    [SerializeField] private TMP_Text _goldText;

    [Header("Inventory Function Buttons")]
    [SerializeField] protected Button _sortButton;
    [SerializeField] protected Button _sendAllButton;
    [SerializeField] protected Button _mergeSameButton;
    [SerializeField] protected Inventory _targetInventory;

    private bool _slotsCreated = false; // 슬롯 생성 여부 

    private List<InventoryItemSlot> _uiSlots = new List<InventoryItemSlot>();
    public override void Awake()
    {
        base.Awake();

        _inventory = null;
    }

    private void Start()
    {
        // 토글 & 버튼 초기화
        SetupTabsAndButtons();
    }

    // UI가 열릴 때 실행되는 메서드
    public override void ShowPanel()
    {
        base.ShowPanel();

        // 런타임 PlayerInventory 찾아서 바인딩
        if (_inventory == null)
        {
            player = FindObjectOfType<Player>();
            _inventory = player.GetComponent<PlayerInventory>();
        }

        int slotCount = _inventory.GetSlots().Count;

        // 처음 한 번만 슬롯 생성, 이후엔 갱신
        if (!_slotsCreated)
        {
            CreateSlots(slotCount);
            _slotsCreated = true;
        }
        else
        {
            // 슬롯 갯수가 부족하면 확장
            while (_uiSlots.Count < slotCount)
            {
                var uiSlot = Instantiate(_itemSlotPrefab, _slotContainer);
                uiSlot.gameObject.SetActive(false);
                _uiSlots.Add(uiSlot);
            }
        }

        // 먼저 모든 슬롯을 비활성화
        for (int i = 0; i < _uiSlots.Count; i++)
        {
            _uiSlots[i].gameObject.SetActive(false);
        }

        // 현재 인벤토리 슬롯 개수만큼만 활성화
        for (int i = 0; i < slotCount; i++)
        {
            _uiSlots[i].gameObject.SetActive(true);
            _uiSlots[i].Initialize(_inventory.GetSlots()[i], this);
        }


        // 기본 탭이 켜져 있지 않다면 첫 탭 강제 켜기
        if (!tabs.Any(t => t.toggle.isOn))
        {
            var first = tabs[0];
            first.toggle.SetIsOnWithoutNotify(true);
            ApplyToggleColor(first, true);
        }

        // 선택된 탭에 맞춰 필터 & 무게 갱신
        var active = tabs.First(t => t.toggle.isOn);
        FilterSlots(active.filter);
        UpdateDisplay();

        bool isDead = false;

        if (_inventory is NPCInventory)
        {
            // NPC 컴포넌트 찾아서 상태머신 확인
            var npc = (_inventory as Component).GetComponent<NPC>();
            if (npc != null)
                isDead = npc.StateMachine.GetState() == npc.StateMachine.NPCDeathState;

            _sendAllButton.gameObject.SetActive(isDead);
            _mergeSameButton.gameObject.SetActive(isDead);
        }

        // 살아있으면 안전하게 타겟 해제(공짜 이동 방지)
        if (!isDead)
            _targetInventory = null;
    }

    private void FilterSlots(InventoryFilter filter)
    {
        foreach (var uiSlot in _uiSlots)
        {
            var slot = uiSlot.GetSlot();
            bool matches = filter == InventoryFilter.All
                           || (!slot.IsEmpty && GetFilterFromItem(slot.item) == filter);

            if (matches)
                uiSlot.ShowContent();
            else
                uiSlot.HideContent();
        }
    }
    private InventoryFilter GetFilterFromItem(Item item)
    {
        switch (item)
        {
            case EquipmentItem _: return InventoryFilter.Equipment;
            case ConsumableItem _: return InventoryFilter.Consumable;
            case MaterialItem _: return InventoryFilter.Material;
            default: return InventoryFilter.All;
        }
    }
    private bool MatchesFilter(ItemType itemType, InventoryFilter filter)
    {
        // 필터 타입별 매칭 로직
        switch (filter)
        {
            case InventoryFilter.Equipment:
                return itemType == ItemType.Equipment;
            case InventoryFilter.Consumable:
                return itemType == ItemType.Consumable;
            case InventoryFilter.Material:
                return itemType == ItemType.Material;
            default:
                return true;
        }
    }

    private void DeselectOtherToggles(InventoryTabGroup selectedTab)
    {
        // 다른 탭의 토글을 모두 off 상태로 변경
        foreach (var tab in tabs)
        {
            if (tab != selectedTab)
            {
                // 색 되돌리고, 이벤트 콜백은 호출되지 않도록
                tab.toggle.SetIsOnWithoutNotify(false);
                ApplyToggleColor(tab, false);
            }
        }
    }

    private void ApplyToggleColor(InventoryTabGroup tabGroup, bool isSelected)
    {
        // 토글 배경 이미지 색상 적용
        var img = tabGroup.toggle.targetGraphic as Image;
        if (img != null)
            img.color = isSelected ? _selectedColor : _normalColor;
    }


    private void CreateSlots(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var uiSlot = Instantiate(_itemSlotPrefab, _slotContainer);
            uiSlot.gameObject.SetActive(false); // 처음엔 꺼두기
            _uiSlots.Add(uiSlot);
        }
    }

    /// <summary>
    /// 이미 생성된 UI 슬롯에 대해 최신 데이터로 갱신
    protected void RefreshSlots()
    {
        // 이미 생성된 슬롯에 최신 데이터 갱신
        var dataSlots = _inventory.GetSlots();
        int count = Mathf.Min(_uiSlots.Count, dataSlots.Count);
        for (int i = 0; i < count; i++)
        {
            _uiSlots[i].Initialize(dataSlots[i], this);
        }
    }

    protected void SetupTabsAndButtons()
    {
        // Close/Open 버튼
        _closeButton.onClick.AddListener(HidePanel);


        _sortButton.onClick.AddListener(() =>
    {
        _inventory.SortItem();
        RefreshSlots();
    });

        _sendAllButton.onClick.AddListener(() =>
        {
            if (_targetInventory != null)
            {
                _inventory.SendAllItem(_targetInventory);
                RefreshSlots();
            }
        });

        _mergeSameButton.onClick.AddListener(() =>
        {
            if (_targetInventory != null)
            {
                _inventory.MergeSameItem(_targetInventory);
                RefreshSlots();
            }
        });

        // 토글 초기화
        foreach (var tab in tabs)
        {
            // 꺼진 상태로
            ApplyToggleColor(tab, false);
            tab.toggle.SetIsOnWithoutNotify(false);

            // ValueChanged 리스너
            tab.toggle.onValueChanged.AddListener(isOn =>
            {
                if (isOn)
                {
                    DeselectOtherToggles(tab);
                    ApplyToggleColor(tab, true);
                    FilterSlots(tab.filter);
                }
                else
                {
                    ApplyToggleColor(tab, false);
                }
            });
        }
    }
    /// <summary>
    /// PlayerInventory로 캐스트하여 총 무게를 계산하고 표시
    /// </summary>
    public void UpdateDisplay()
    {
        if (_inventory is PlayerInventory playerInv && _weightText != null)
        {
            float totalWeight = playerInv.GetTotalWeight();
            float maxWeight = playerInv.PlayerMaxForce;
            _weightText.text = $"{totalWeight:F1} / {maxWeight:F1}";

        }
        
            _goldText.text = $"{_inventory.gold} Gold"; // 가진 돈 표시
    }
    public Inventory GetInventory()
    {
        return _inventory;
    }

    public void SetNPCInventory(Inventory inv)
    {
        // (기존 구독 해제)
        if (_inventory is NPCInventory oldNpc)
            oldNpc.OnInventoryChanged -= RefreshSlots;

        _inventory = inv;

        // (새 구독 추가)
        if (_inventory is NPCInventory newNpc)
            newNpc.OnInventoryChanged += RefreshSlots;

        RefreshSlots();
        FilterSlots(tabs[0].filter);
        UpdateDisplay();
    }
    public void SetTargetInventory(Inventory target)
    {
        _targetInventory = target;
    }
}