using UnityEngine;
using UnityEngine.UI;

public class DefaultUI : PopupUI
{
    [Header("DefaultUI ButtonsSettings")]
    [SerializeField] private Button _changeSlotButton;

    [Header("DefaultUI Panels")]
    [SerializeField] private CanvasGroup _skillSlotpanel;
    [SerializeField] private CanvasGroup _UsedItemSlotpanel;

    private bool isPanelActive = true;

    public override void Awake()
    {
        base.Awake();
    }

    public void Start()
    {
        _changeSlotButton.onClick.AddListener(TogglePanel);

        // 초기 상태 설정 스킬 패널은 보이게(true), 아이템 패널은 숨기게(false)
        SetPanelState(_skillSlotpanel, true);
        SetPanelState(_UsedItemSlotpanel, false);
    }

    private void TogglePanel()
    {
        // isPanelActive 값을 반전(토글)시켜,
        // 다음에는 반대 패널이 보이도록 준비
        isPanelActive = !isPanelActive;

        SetPanelState(_skillSlotpanel, isPanelActive);
        SetPanelState(_UsedItemSlotpanel, !isPanelActive);
    }

    private void SetPanelState(CanvasGroup panel, bool isActive)
    {
        panel.alpha = isActive ? 1f : 0f;
        panel.interactable = isActive;
        panel.blocksRaycasts = isActive;
    }
}