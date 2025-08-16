using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MapUI : PopupUI
{
    [Header("UI CloseButton")]
    [SerializeField] private Button _closeButton;

    [Header("자식 패널 (GameObject 활성화/비활성화용)")]
    [SerializeField] private List<GameObject> _childPanels = new();

    public override void Awake()
    {
        base.Awake();
        // 여기선 버튼 리스너 등록은 OnEnable/OnDisable로
    }

    private void OnEnable()
    {
        if (_closeButton != null)
            _closeButton.onClick.AddListener(HidePanel);
    }

    private void OnDisable()
    {
        if (_closeButton != null)
            _closeButton.onClick.RemoveListener(HidePanel);
    }

    protected override void AfterOpen()
    {
        // 기본 서브패널(CanvasGroup) 대신 GameObject 활성화
        foreach (var go in _childPanels)
        {
            if (go != null)
                go.SetActive(true);
        }
    }

    protected override void AfterClose()
    {
        foreach (var go in _childPanels)
        {
            if (go != null)
                go.SetActive(false);
        }
    }
}
