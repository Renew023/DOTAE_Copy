using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;


public class InputRebindUI : MonoBehaviour
{
    [Header("Input Asset")]
    [SerializeField] private InputActionAsset _actions;

    [Header("액션맵 섹션 설정")]
    [Tooltip("한 화면에 나열할 액션맵 이름들")]
    [SerializeField] private List<string> _actionMapNames;

    [Tooltip("섹션 헤더 프리팹: 단순 TMP_Text")]
    [SerializeField] private GameObject _sectionHeaderPrefab;

    [Tooltip("액션마다 생성할 RebindEntry 프리팹")]
    [SerializeField] private GameObject _entryPrefab;

    [Tooltip("ScrollView Content Transform")]
    [SerializeField] private Transform _content;

    [Header("버튼")]
    [SerializeField] private Button _saveButton;
    [SerializeField] private Button _resetButton;

    private List<RebindEntry> _entries = new List<RebindEntry>();

    [SerializeField]
    [Tooltip("리바인드 UI 생성 시 제외할 액션 이름들")]
    private List<string> _excludeActionNames = new List<string>
{
    "Attack",
    "Run",
    "QuickSlot1",
    // 필요에 따라 더 추가…
};
    private void Start()
    {

        // 0) 이전에 저장된 오버라이드가 있으면 불러오기
        if (PlayerPrefs.HasKey("rebinds"))
        {
            var json = PlayerPrefs.GetString("rebinds");
            _actions.LoadBindingOverridesFromJson(json);
        }

        // 1) 액션맵별로 섹션 헤더 + 액션 리스트 생성
        foreach (var mapName in _actionMapNames)
        {
            var map = _actions.FindActionMap(mapName);
            if (map == null) continue;

            // 1-1) 섹션 헤더
            var headerGO = Instantiate(_sectionHeaderPrefab, _content);
            var headerText = headerGO.GetComponentInChildren<TMP_Text>();
            if (headerText == null)
            {
                continue;
            }

            headerText.text = map.name;

            // 1-2) 해당 액션맵의 각 액션에 대해 RebindEntry 생성
            foreach (var action in map.actions)
            {
                // 1) 제외 목록에 들어있으면 건너뛴다
                if (_excludeActionNames.Contains(action.name))
                    continue;

                // 2) 정상적인 RebindEntry 생성
                var entryGO = Instantiate(_entryPrefab, _content);
                var entry = entryGO.GetComponent<RebindEntry>();
                entry.Setup(action, StartRebind);
                _entries.Add(entry);
            }
        }

        // 2) Save 버튼: 현재 오버라이드 JSON을 PlayerPrefs에 저장
        _saveButton.onClick.AddListener(() =>
        {
            var json = _actions.SaveBindingOverridesAsJson();
            PlayerPrefs.SetString("rebinds", json);
        });

        // 3) Reset 버튼: 오버라이드 전부 제거 + 저장된 Prefs 삭제
        _resetButton.onClick.AddListener(() =>
        {
            _actions.RemoveAllBindingOverrides();
            PlayerPrefs.DeleteKey("rebinds");
            RefreshAllLabels();
        });
    }

    private void RefreshAllLabels()
    {
        foreach (var e in _entries)
            e.RefreshBindingDisplay();
    }

    private void StartRebind(InputAction action, int bindingIndex, TMP_Text display)
    {
        display.text = "Press any key…";
        action.Disable();

        action.PerformInteractiveRebinding(bindingIndex)
              .WithControlsExcluding("Mouse")
              .OnComplete(op =>
              {
                  op.Dispose();
                  action.Enable();

                  // 1) 새로 할당된 키 경로 가져오기
                  var newPath = action.bindings[bindingIndex].effectivePath;

                  // 2) 모든 액션맵·액션·바인딩 순회하며
                  foreach (var map in _actions.actionMaps)
                      foreach (var otherAction in map.actions)
                      {
                          // 자기 자신 건너뛰기
                          if (otherAction == action)
                              continue;

                          // 3) 같은 키를 쓰는 바인딩을 만나면
                          for (int i = 0; i < otherAction.bindings.Count; i++)
                          {
                              if (otherAction.bindings[i].effectivePath == newPath)
                              {
                                  // 빈 문자열 override 를 걸어 비활성화
                                  otherAction.ApplyBindingOverride(i, "");
                              }
                          }
                      }

                  // 4) UI 갱신
                  RefreshAllLabels();
              })
              .Start();
    }

}
