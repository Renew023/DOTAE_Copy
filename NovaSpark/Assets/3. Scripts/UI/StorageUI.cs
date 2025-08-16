using UnityEngine;
using UnityEngine.UI;

public class StorageUI : InventoryUI
{
    //[Tooltip("저장소 UI의 닫기 버튼")]
    //[SerializeField] private Button _closedButton;
    private void Start()
    {
        SetupTabsAndButtons();
        //_closedButton.onClick.AddListener(() => UIManager.Instance.HidePanel(UIType.StorageUI));
    }
    public void SetInventory(Inventory player,Storage storage)
    {
        _targetInventory = player;
        _inventory = storage;
    }
}
