using Photon.Realtime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItemSlot : MonoBehaviour,
        IPointerClickHandler,
        IPointerEnterHandler,
        IPointerExitHandler,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler,
        IDropHandler,
    IPointerDownHandler,
    IPointerUpHandler,
        IItemSlot

{
    [Header("UI Components")]
    [SerializeField]
    private Image _icon;

    [Tooltip("아이템 이름을 표시할 TextMeshPro 컴포넌트")]
    [SerializeField]
    private TMP_Text _nameText;

    [Tooltip("아이템 수량을 표시할 TextMeshPro 컴포넌트")]
    [SerializeField]
    private TMP_Text _quantityText;

    [Header("Drag Boundary")]
    [Tooltip("드래그 후 슬롯 영역으로 인식할 RectTransform")]
    [SerializeField]
    private RectTransform _baseRect;

    [SerializeField]
    private Slot _slotData;

    [SerializeField]
    private Item _prevItem;

    [SerializeField]
    private int _prevQuantity;
    private Vector3 _originPos;

    [SerializeField]
    private InventoryUI _parentInventoryUI;
    private Player _player;

    private int _iconVersion;
    private string _currentIconKey;
    public Item ItemData => _slotData?.item;
    public int Quantity => _slotData?.quantity ?? 0;

    private void Start()
    {
        if (_slotData != null)
        {
            _prevItem = _slotData.item;
            _prevQuantity = _slotData.quantity;
        }
        Refresh();
        // 초기 위치 저장
        _originPos = transform.position;
    }

    private void Update()
    {
        if (_slotData == null)
            return;

        // _slotData가 바뀌면 Refresh() 호출
        if (_slotData.item != _prevItem || _slotData.quantity != _prevQuantity)
        {
            _prevItem = _slotData.item;
            _prevQuantity = _slotData.quantity;
            Refresh();
        }
    }
    public void HideContent()
    {
        _icon.enabled = false;
        _nameText.text = string.Empty;
        _quantityText.text = string.Empty;
        _iconVersion++; // 로딩 무효화
    }

    /// <summary>
    /// 슬롯 데이터를 기반으로 콘텐츠를 다시 보여줍니다.
    /// </summary>
    public void ShowContent()
    {
        Refresh();
    }
    /// <summary>
    /// 슬롯 데이터를 할당하고 UI를 갱신
    public void Initialize(Slot slot, InventoryUI parent)
    {
        _slotData = slot;
        _parentInventoryUI = parent;
        Refresh();
    }

    /// <summary>
    /// UI 요소들을 데이터에 맞게 업데이트
    public async void Refresh()
    {
        if (_slotData == null || _slotData.IsEmpty || _slotData.item == null)
        {
            _iconVersion++;
            _icon.enabled = false;
            _icon.sprite = null;
            _nameText.text = string.Empty;
            _quantityText.text = string.Empty;
            return;
        }

        var item = _slotData.item;
        _nameText.text = item.name_kr;
        _quantityText.text = (_slotData.quantity > 1) ? _slotData.quantity.ToString() : string.Empty;

        int localVersion = ++_iconVersion;

        AddressableManager.Instance.LoadIconToImage(
            _icon,
            item.icon,
            localVersion,
            () => _iconVersion,
            setNativeSize: true
        );
    }


    /// 슬롯 클릭
    public void OnPointerClick(PointerEventData eventData)
    {
        if (
            eventData.button == PointerEventData.InputButton.Right
            && _slotData != null
            && !_slotData.IsEmpty && _slotData.item is ConsumableItem
        )
        {
            _player = _parentInventoryUI.player;
            _slotData.item.Use(_player.characterRuntimeData, _player);

            if (_slotData.item.isStackable)
            {
                _slotData.quantity--;

                if (_slotData.quantity <= 0)
                {
                    _slotData.Clear();
                }
            }
            else
            {
                _slotData.Clear();
            }

            Refresh();

            // 무게 표시 갱신
            _parentInventoryUI.UpdateDisplay();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_slotData != null && !_slotData.IsEmpty && TooltipUI.Instance != null)
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
        else if (item is ConsumableItem c)
        {
            // description이 필요하면 추가
            string effects = "";
            for (int i = 0; i < Mathf.Min(c.effectType.Count, c.effectValue.Count); i++)
                effects += $"{c.effectType[i]}: {c.effectValue[i]}\n";

            return
                $"효과:\n{effects}" +
                $"가격: {c.price}";
        }
        else if (item is MaterialItem m)
        {
            string uses = (m.type != null && m.type.Count > 0)
                ? string.Join(", ", m.type)
                : "없음";

            return
                $"등급: {m.rarity}\n" +
                $"{m.description}\n" +
                $"용도: {uses}";
        }

        // 그 외 (예비)
        return "";
    }


    /// <summary>
    /// 드래그 시작: DragSlot에 이 슬롯을 등록하고 아이콘 표시
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_slotData != null && !_slotData.IsEmpty)
        {
            DragSlot.Instance.dragSlot = this;
            DragSlot.Instance.DragSetImage(_icon);
            DragSlot.Instance.transform.position = eventData.position;
            // 1) Canvas 최상단(마지막 자식)으로 이동
            var dragTransform = DragSlot.Instance.transform;
            dragTransform.SetParent(UIManager.Instance.canvasTrm.transform);  // 루트 Canvas 자식으로
            dragTransform.SetAsLastSibling();                                 // 순서를 맨 뒤로

            // 2) 위치 갱신
            DragSlot.Instance.transform.position = eventData.position;
        }
    }

    /// <summary>
    /// 드래그 중: 마우스 위치에 DragSlot 이미지 이동
    public void OnDrag(PointerEventData eventData)
    {
        if (DragSlot.Instance.dragSlot == this)
        {
            DragSlot.Instance.transform.position = eventData.position;
        }
    }

    /// <summary>
    /// 드래그 종료: DragSlot 이미지를 숨기고 인스턴스 해제
    public void OnEndDrag(PointerEventData eventData)
    {
        if (DragSlot.Instance.dragSlot == this)
        {
            DragSlot.Instance.SetColor(0);
            DragSlot.Instance.dragSlot = null;
        }
    }

    /// <summary>
    /// 드롭 처리: 다른 슬롯에서 드래그해 온 경우 슬롯 데이터 교환
    public void OnDrop(PointerEventData eventData)
    {
        var source = eventData.pointerDrag?.GetComponent<IItemSlot>();
        if (source == null || source == this) return;

        if (source is EquipmentSlot equipmentSlot)
        {
            equipmentSlot.RequestUnequip();
            Refresh();
            source.Refresh();
            source.GetParentInventoryUI()?.UpdateDisplay();
            _parentInventoryUI?.UpdateDisplay();
            return;
        }

        var fromInv = source.GetParentInventory();
        var toInv = _parentInventoryUI.GetInventory();
        if (fromInv == null || toInv == null) return;

        var item = source.ItemData;
        int quantity = source.Quantity;

        bool isSameInventory = fromInv == toInv;
        if (isSameInventory)
        {
            if (!TrySwapSlotData(source))
                Debug.LogWarning("스왑 실패");
            return;
        }

        // 플레이어 → 상점 (판매)
        if (fromInv.OwnerType == InventoryOwnerType.Player && toInv.OwnerType == InventoryOwnerType.NPC)
        {
            var player = fromInv as PlayerInventory;
            var shop = toInv as NPCInventory;
            int unitPrice = item.price/2;

            if (!item.isStackable || quantity == 1)
            {
                if (unitPrice > shop.gold)
                {
                    Debug.LogWarning("상점 돈 부족");
                    return;
                }
                TradeManager.Instance.DoSell(player, shop, item, 1);
                if (source.GetSlot().quantity <= 0) source.GetSlot().Clear();
                Refresh(); source.Refresh();
                source.GetParentInventoryUI()?.UpdateDisplay();
                _parentInventoryUI?.UpdateDisplay();
                return;
            }

            int maxAmount = TradeManager.Instance.GetMaxSellAmount(item, quantity, shop.gold, unitPrice);

            // UIManager 통해 QuantitySelectUI 띄우기
            var quantityUI = UIManager.Instance.popupUIByType[UIType.QuantitySelectUI] as QuantitySelectUI;
            quantityUI.Open(maxAmount, selectedAmount =>
            {
                TradeManager.Instance.DoSell(player, shop, item, selectedAmount);
                if (source.GetSlot().quantity <= 0) source.GetSlot().Clear();
                Refresh(); source.Refresh();
            });
            source.GetParentInventoryUI()?.UpdateDisplay();
            _parentInventoryUI?.UpdateDisplay();
            return;
        }

        // 상점 → 플레이어 (구매)
        if (fromInv.OwnerType == InventoryOwnerType.NPC && toInv.OwnerType == InventoryOwnerType.Player)
        {
            var shop = fromInv as NPCInventory;
            var player = toInv as PlayerInventory;

            int unitPrice = item.price;
            float freeWeight = player.PlayerMaxForce - player.GetTotalWeight();

            if (!item.isStackable || quantity == 1)
            {
                if (unitPrice > player.gold || item.weight > freeWeight)
                {
                    Debug.LogWarning("구매 불가 (돈/무게 부족)");
                    return;
                }
                TradeManager.Instance.DoBuy(shop, player, item, 1);
                if (source.GetSlot().quantity <= 0) source.GetSlot().Clear();
                Refresh(); source.Refresh();
                source.GetParentInventoryUI()?.UpdateDisplay();
                _parentInventoryUI?.UpdateDisplay();
                return;
            }

            int maxAmount = TradeManager.Instance.GetMaxBuyAmount(item, quantity, player.gold, freeWeight, unitPrice);

            var quantityUI = UIManager.Instance.popupUIByType[UIType.QuantitySelectUI] as QuantitySelectUI;
            quantityUI.Open(maxAmount, selectedAmount =>
            {
                TradeManager.Instance.DoBuy(shop, player, item, selectedAmount);
                if (source.GetSlot().quantity <= 0) source.GetSlot().Clear();
                Refresh(); source.Refresh();
            });
            source.GetParentInventoryUI()?.UpdateDisplay();
            _parentInventoryUI?.UpdateDisplay();
            return;
        }

        // 창고 → 플레이어 이동
        if (fromInv.OwnerType == InventoryOwnerType.Storage && toInv.OwnerType == InventoryOwnerType.Player)
        {
            var storage = fromInv as Storage;
            var player = toInv as PlayerInventory;
            float freeWeight = player.PlayerMaxForce - player.GetTotalWeight();

            if (!item.isStackable || quantity == 1)
            {
                if (item.weight > freeWeight)
                {
                    Debug.LogWarning("무게 초과");
                    return;
                }
                TradeManager.Instance.DoStorageTransfer(storage, player, item, 1);
                if (source.GetSlot().quantity <= 0) source.GetSlot().Clear();
                Refresh(); source.Refresh();
                source.GetParentInventoryUI()?.UpdateDisplay();
                _parentInventoryUI?.UpdateDisplay();
                return;
            }

            int maxAmount = TradeManager.Instance.GetMaxStorageTransferAmount(item, quantity, freeWeight);

            var quantityUI = UIManager.Instance.popupUIByType[UIType.QuantitySelectUI] as QuantitySelectUI;
            quantityUI.Open(maxAmount, selectedAmount =>
            {
                TradeManager.Instance.DoStorageTransfer(storage, player, item, selectedAmount);
                if (source.GetSlot().quantity <= 0) source.GetSlot().Clear();
                Refresh(); source.Refresh();
            });
            source.GetParentInventoryUI()?.UpdateDisplay();
            _parentInventoryUI?.UpdateDisplay();
            return;
        }

        // 빈 슬롯이면 AddItem 시도
        if (_slotData.IsEmpty)
        {
            if (toInv.AddItem(item, quantity))
            {
                source.GetSlot().Clear();
                Refresh(); source.Refresh();
            }
        }
        else
        {
            Debug.LogWarning("빈 슬롯이 아니므로 AddItem 사용 불가. 스왑도 불허.");
        }

        source.GetParentInventoryUI()?.UpdateDisplay();
        _parentInventoryUI?.UpdateDisplay();
        //fromInv.SyncInventoryNetwork();
        //toInv.SyncInventoryNetwork();
    }

    /// <summary>
    /// 두 슬롯의 데이터를 교환
    private bool TrySwapSlotData(IItemSlot other)
    {
        // 현재 슬롯과 다른 슬롯 아이템과 수량
        var myItem = _slotData.item;
        var myQty = _slotData.quantity;
        var otherItem = other.ItemData;
        var otherQty = other.Quantity;

        // 스왑 후 플레이어 인벤토리 무게 변화 계산
        var toInv = _parentInventoryUI.GetInventory();

        if (toInv is PlayerInventory playerInv)
        {
            float currentWeight = playerInv.GetTotalWeight();

            // 현재 슬롯 아이템 무게 빼고, 다른 슬롯 아이템 무게 더한 무게 계산
            float weightAfterSwap = currentWeight
                                    - (myItem != null ? myItem.weight * myQty : 0f)
                                    + (otherItem != null ? otherItem.weight * otherQty : 0f);

            if (weightAfterSwap > playerInv.PlayerMaxForce)
            {
                Debug.LogWarning("무게 초과로 인해 스왑 불가");
                return false;
            }
        }

        // 만약 상대 슬롯이 플레이어 인벤토리라면 그쪽도 무게 검사 필요
        var otherInv = other.GetParentInventory();
        if (otherInv is PlayerInventory otherPlayerInv)
        {
            float currentWeight = otherPlayerInv.GetTotalWeight();

            float weightAfterSwap = currentWeight
                                    - (otherItem != null ? otherItem.weight * otherQty : 0f)
                                    + (myItem != null ? myItem.weight * myQty : 0f);

            if (weightAfterSwap > otherPlayerInv.PlayerMaxForce)
            {
                Debug.LogWarning("상대 플레이어 인벤토리 무게 초과로 스왑 불가");
                return false;
            }
        }

        // 무게 문제 없으면 스왑 진행
        var tempItem = otherItem;
        var tempQty = otherQty;

        other.SetSlotData(myItem, myQty);

        _slotData.item = tempItem;
        _slotData.quantity = tempQty;

        return true;
    }

    public Slot GetSlot()
    {
        return _slotData;
    }

    public void SetSlotData(Item item, int quantity)
    {
        _slotData.item = item;
        _slotData.quantity = quantity;
        Refresh();
    }

    public void RequestUnequip()
    {

    }

    private int CalculateBuyPrice(Item item)
    {
        return item.price; // 필요시 상인에 따라 조정
    }

    private int CalculateSellPrice(Item item)
    {
        return Mathf.FloorToInt(item.price * 0.5f); // 50% 가격
    }

    public Inventory GetParentInventory()
    {
        return _parentInventoryUI.GetInventory();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }
    public InventoryUI GetParentInventoryUI()
    {
        return _parentInventoryUI;
    }
}
