using UnityEngine;
using UnityEngine.InputSystem;

public class UIPanelKey : MonoBehaviour
{
    public void OnMapPanel(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        if (!CanProceed() && !UIManager.Instance.IsTopPanel(UIType.MapUI)) return;

        TogglePanel(UIType.MapUI);
    }

    public void OnInventoryPanel(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        if (!CanProceed() && !UIManager.Instance.IsTopPanel(UIType.InventoryUI)) return;

        TogglePanel(UIType.InventoryUI);
    }

    public void OnEquipPanel(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        if (!CanProceed() && !UIManager.Instance.IsTopPanel(UIType.EquipUI)) return;

        TogglePanel(UIType.EquipUI);
    }

    public void OnStatusPanel(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        if (!CanProceed() && !UIManager.Instance.IsTopPanel(UIType.StatusUI)) return;

        TogglePanel(UIType.StatusUI);
    }

    public void OnSkillPanel(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        if (!CanProceed() && !UIManager.Instance.IsTopPanel(UIType.SkillUI)) return;

        TogglePanel(UIType.SkillUI);
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        // 옵션창이 최상단에 있으면 → 닫기
        if (UIManager.Instance.IsTopPanel(UIType.OptionsUI))
        {
            UIManager.Instance.HidePanel(UIType.OptionsUI);
            return;
        }

        // 다른 UI가 열려 있으면 → 최상단 UI 닫기
        if (UIManager.Instance.currentPopupUI.Count > 0)
        {
            UIManager.Instance.HideAllPanels();
            return;
        }

        // 아무것도 없으면 → 옵션창 열기
        UIManager.Instance.ShowPanel(UIType.OptionsUI);
    }

    /// <summary>
    /// UI 토글: 켜져 있으면 닫고, 없으면 열기
    /// </summary>
    private void TogglePanel(UIType type)
    {
        if (UIManager.Instance.IsTopPanel(type))
        {
            UIManager.Instance.HidePanel(type);
        }
        else
        {
            UIManager.Instance.ShowPanel(type);
        }
    }

    private bool CanProceed()
    {
        if (ChatManager.Instance != null && ChatManager.Instance.IsChatActive)
            return false;

        // 옵션창이 떠 있으면 다른 UI는 열지 않음
        return !UIManager.Instance.IsTopPanel(UIType.OptionsUI);
    }
}
