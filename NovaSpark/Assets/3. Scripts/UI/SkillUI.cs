using UnityEngine;
using UnityEngine.UI;

public class SkillUI : PopupUI
{
    [Header("UI CloseButton")]
    [SerializeField] private Button _closeButton;

    public override void Awake()
    {
        base.Awake();

        _closeButton.onClick.AddListener(HidePanel);
    }
}

