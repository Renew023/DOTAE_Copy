using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
[System.Serializable]
public class OptionsTabGroup
{
    public string tabName;           // "Weapon", "Use", "Etc"
    public Toggle toggle;            // 연결된 토글
    public CanvasGroup panel;        // 대응하는 패널
}
public class OptionsUI : PopupUI
{
    [Header("Options SettingsButton")]
    [SerializeField] private Button _closeButton;
    [SerializeField] private Button _quitStartSceneButton;
    [SerializeField] private Button _openButton;
    [SerializeField] private Button _quitgameButton;

    [Header("Options Tabs Settings")]
    [SerializeField] private List<OptionsTabGroup> tabs;

    [Header("Toggle Color Settings")]
    [Tooltip("꺼졌을 때 토글 배경색")]
    [SerializeField] private Color _normalColor = Color.white;
    [Tooltip("켜졌을 때 토글 배경색")]
    [SerializeField] private Color _selectedColor = Color.green;
    public override void Awake()
    {
        base.Awake();
    }
    private void Start()
    {
        _closeButton.onClick.AddListener(HidePanel);
        _openButton.onClick.AddListener(() => UIManager.Instance.ShowPanel(UIType.OptionsUI));
        _quitgameButton.onClick.AddListener(QuitGame);
        // Quit 버튼 리스너 세팅
        var exitHandler = FindObjectOfType<RoomExitHandler>();
        _quitStartSceneButton.onClick.RemoveAllListeners();
        _quitStartSceneButton.onClick.AddListener(QuitStartScene);

        // 1) 모든 탭에 콜백 등록 & 초기 색·패널 상태 세팅
        foreach (var tab in tabs)
        {
            // 콜백 등록
            tab.toggle.onValueChanged.AddListener(isOn =>
            {
                if (isOn)
                {
                    DeselectOtherToggles(tab);
                    ApplyToggleColor(tab, true);
                    SetActivePanel(tab.tabName);
                }
                else
                {
                    ApplyToggleColor(tab, false);
                }
            });

            // 처음엔 모두 노멀 컬러 & 비활성 패널
            ApplyToggleColor(tab, false);
            tab.panel.alpha = 0f;
            tab.panel.interactable = false;
            tab.panel.blocksRaycasts = false;
        }

        // 2) 기본 탭 한 개만 선택 상태로 만들기
        if (tabs.Count > 0)
        {
            var defaultTab = tabs[0];
            defaultTab.toggle.SetIsOnWithoutNotify(true);   // 토글만 켜고 콜백은 호출하지 않음
            ApplyToggleColor(defaultTab, true);             // 색 적용
            SetActivePanel(defaultTab.tabName);             // 패널 활성화
        }
    }

    private void DeselectOtherToggles(OptionsTabGroup selectedTab)
    {
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

    private void ApplyToggleColor(OptionsTabGroup tabGroup, bool isSelected)
    {
        // 토글의 배경 이미지(targetGraphic)에 색을 입혀줍니다.
        var img = tabGroup.toggle.targetGraphic as Image;
        if (img != null)
            img.color = isSelected ? _selectedColor : _normalColor;
    }

    private void SetActivePanel(string selectedTabName)
    {
        foreach (var tab in tabs)
        {
            bool isActive = tab.tabName == selectedTabName;
            tab.panel.alpha = isActive ? 1 : 0;
            tab.panel.interactable = isActive;
            tab.panel.blocksRaycasts = isActive;

            if (isActive)
                tab.toggle.Select();
        }
    }
    public  void QuitStartScene()
    {
        // 룸 나가기 완료 직후 실행
        SceneManager.LoadScene("StartScene");
    }
    private void QuitGame()
    {
        Application.Quit();// 게임 종료
    }
}


