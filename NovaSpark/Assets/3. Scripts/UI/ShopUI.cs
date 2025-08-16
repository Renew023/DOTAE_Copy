using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 상점 UI를 관리하는 스크립트입니다.
/// PopupUI를 상속받아 UIManager를 통해 제어됩니다.
/// </summary>
public class ShopUI : PopupUI
{
    [Header("Inventory ButtonSettings")]
    [SerializeField] private Button _closeButton;

    public override void Awake()
    {
        base.Awake();

    }
    private void Start()
    {
        _closeButton.onClick.AddListener(HidePanel);
    }
}
