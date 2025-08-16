using UnityEngine;
using UnityEngine.UI;

public class EquipUI : PopupUI
{
    [Header("EquipUI ButtonSettings")]
    [SerializeField] private Button _closeButton;
    [SerializeField] private EquipmentSlot[] slotUIs;
    [SerializeField] private EquipmentInventory equipmentInventory;


    public override void Awake()
    {
        base.Awake();
    }

    public void Start()
    { // 런타임 EquipmentInventory 찾아서 바인딩
        if (equipmentInventory == null)
            equipmentInventory = FindObjectOfType<EquipmentInventory>();
        _closeButton.onClick.AddListener(HidePanel);    
        Init(equipmentInventory);
    }

    public override void ShowPanel()
    {
        base.ShowPanel();

        if (equipmentInventory == null)
            equipmentInventory = FindObjectOfType<EquipmentInventory>();
        Init(equipmentInventory);
    }

    public void Init(EquipmentInventory inventory)
    {
        equipmentInventory = inventory;

        foreach (var slotUI in slotUIs)
        {
            slotUI.Init(inventory, slotUI.slotType);
        }
    }

    public void RefreshAll()
    {
        foreach (var slotUI in slotUIs)
        {
            slotUI.Refresh();
        }
    }
}

