# UIManager(2025.06.27.)

## UIManager_1. UIType 열거형 정의
```
public enum UIType
{
    MapUI,
    OptionsUI,
    InventoryUI,
    EquipUI,
    StatusUI,
    SkillUI,
    DefaultUI,
}
```
- **역할**
  - 각 팝업 UI를 구분하는 이름표 역할.
- **사용처**
  - popupUIByType 딕셔너리의 키로, 그리고 PopupUI.UIType 에 할당되어 “어떤 UI인지” 알려줌.
  
---
## UIManager_2. UIManager 클래스 필드 선언부
```
public class UIManager : Singleton<UIManager>
{
    [HideInInspector]
    public GameObject canvasTrm;

    public Dictionary<UIType, PopupUI> popupUIByType = new();
    public Stack<PopupUI> currentPopupUI = new();
}
```
- **Singleton<UIManager>** 
  - 싱글톤 패턴으로, 어디서든 UIManager.Instance 로 접근 가능
- **[HideInInspector] public GameObject canvasTrm;** 
  - 씬에 있는 "Canvas" 게임오브젝트를 찾기 위한 변수. [HideInInspector] 로 인스펙터에는 숨김.
- **public Dictionary<UIType, PopupUI> popupUIByType = new();**
  - UIType → PopupUI 매핑을 저장하는 Dictionary. Awake 시 모든 팝업을 등록해 두면, 나중에 키만으로 꺼내서 사용 가능.
- **public Stack<PopupUI> currentPopupUI = new();:**
  - Stack을 사용하여 열려 있는 팝업들을 스택에 저장(LIFO). 가장 마지막에 연 팝업부터 닫기 편리.
---

## UIManager_3. Awake() 메서드: 팝업 초기 등록
```
protected override void Awake()
{
    base.Awake();  // Singleton 초기화

    canvasTrm = GameObject.Find("Canvas");
    PopupUI[] popupUIs = canvasTrm.GetComponentsInChildren<PopupUI>(true);

    foreach (var popupUI in popupUIs)
    {
        if (!popupUIByType.ContainsKey(popupUI.UIType))
            popupUIByType.Add(popupUI.UIType, popupUI);
        else
            Debug.LogWarning($"중복된 UIType: {popupUI.UIType}");
    }
}
```
- **base.Awake()** 
  - 싱글톤 베이스 클래스의 Awake를 호출해 인스턴스를 세팅
- **GameObject.Find("Canvas")** 
  - "Canvas" 이름으로 직접 찾는다.
- **PopupUI[] popupUIs = canvasTrm.GetComponentsInChildren<PopupUI>(true);** 
  - 비활성화된 오브젝트까지 포함해서, 모든 PopupUI 컴포넌트 가져오기.
- **사전 등록** 
  - popupUIByType 사전에 UIType을 키로, popupUI 인스턴스를 값으로 추가하고, 중복 키가 있으면 경고 로그 출력.
---

## UIManager_4. ShowPanel(): UI 열기
```
public void ShowPanel(UIType uiType)
{
    // 옵션 메뉴 전용 로직
    if (uiType == UIType.OptionsUI)
    {
        foreach (var ui in new Stack<PopupUI>(currentPopupUI))
            if (ui.UIType != UIType.DefaultUI)
                ui.HidePanel();

        PauseManager.Instance.PauseGame();
    }
    else if (IsTopPanel(UIType.OptionsUI))
    {
        return; // 옵션이 열려 있으면 다른 UI 못 엶
    }

    // 실제로 원하는 팝업을 꺼내서 보여줌
    if (popupUIByType.TryGetValue(uiType, out var popupUI))
    {
        popupUI.ShowPanel();
    }
}
```
- **옵션 UI 예외 처리** 
  - OptionsUI는 단독으로 사용하므로, 다른 팝업은 모두 닫고 게임을 일시정지(PauseGame()).
  - 옵션이 열려 있을 때 다른 UI를 못 열게 막음.
- **(popupUIByType.TryGetValue(uiType, out var popupUI))**
  - TryGetValue 로 popupUI를 꺼내 ShowPanel() 호출하여 일반 UI 열기
---

## UIManager_5. HidePanel(): UI 닫기
```
public void HidePanel(UIType uiType)
{
    if (!popupUIByType.TryGetValue(uiType, out var popupUI)) return;

    popupUI.HidePanel();

    if (uiType == UIType.OptionsUI)
        PauseManager.Instance.ResumeGame();
}
```
- **public void HidePanel(UIType uiType)** 
  - 사전 조회 후 HidePanel() 호출.
- **if (uiType == UIType.OptionsUI) PauseManager.Instance.ResumeGame();**
  - OptionsUI를 닫을 땐, 일시정지 해제(ResumeGame()).
---

## UIManager_6. HideAllPanels(): 모든 UI 닫기
```
public void HideAllPanels()
{
    foreach (var popup in new Stack<PopupUI>(currentPopupUI))
    {
        if (popup.UIType != UIType.DefaultUI)
            popup.HidePanel();
    }

    // DefaultUI만 남기고 스택 재구성
    currentPopupUI = new Stack<PopupUI>(
        new Stack<PopupUI>(currentPopupUI)
            .ToArray()
            .Where(p => p.UIType == UIType.DefaultUI)
    );
}
```
- **foreach (var popup in new Stack<PopupUI>(currentPopupUI))**
  - 스택 복사 → 안전하게 순회.
- **if (popup.UIType != UIType.DefaultUI) popup.HidePanel();**
  - DefaultUI 제외하고 전부 닫기
- **currentPopupUI = new Stack<PopupUI>(new Stack<PopupUI>(currentPopupUI).ToArray().
    Where(p => p.UIType == UIType.DefaultUI));**
  - 스택 재구성 → DefaultUI만 남김
---

## UIManager_7. IsTopPanel(): 최상위 UI 확인
```
public bool IsTopPanel(UIType uiType)
{
    return currentPopupUI.Count > 0 
        && currentPopupUI.Peek().UIType == uiType;
}
```
- **currentPopupUI.Count > 0**
  - 스택에 UI가 하나라도 쌓여 있는지 확인해서, 스택이 비어 있을 때 Peek() 호출로 인한 예외를 방지.
- **currentPopupUI.Peek().UIType == uiType**
  - 스택 맨 위(가장 최근에 연) PopupUI의 UIType을 꺼내서, 파라미터로 넘어온 uiType과 같은지 비교.
---

# PopupUI
## PopupUI_1. 필드 & 컴포넌트 초기화
```
public class PopupUI : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    public UIType UIType;

    protected CanvasGroup _panel;
    protected RectTransform _rectTransform;
    protected Canvas _canvas;

    [SerializeField] private bool _disableDrag = false;
    [SerializeField] private RectTransform _dragArea = null;

    public virtual void Awake()
    {
        _panel = GetComponent<CanvasGroup>();
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();

        if (UIType == UIType.DefaultUI)
        {
            _panel.alpha = 1f;
            _panel.blocksRaycasts = true;
        }
        else
        {
            _panel.alpha = 0f;
            _panel.blocksRaycasts = false;
        }
    }
    ...
}
```
- **protected CanvasGroup _panel;**
  - CanvasGroup으로 투명도·클릭 차단 제어.
- **if (UIType == UIType.DefaultUI)
        {
            _panel.alpha = 1f;
            _panel.blocksRaycasts = true;
        }**
  - 기본(DefaultUI) 만 처음부터 보이도록 설정.
---

## PopupUI_2. ShowPanel(): UI 숨김
```
public virtual void ShowPanel()
{
    UIManager.Instance.currentPopupUI.Push(this);
    _rectTransform.SetAsLastSibling();

    _panel.blocksRaycasts = true;
    _panel.alpha = 1f;
}
```
- **currentPopupUI.Push(this)**
  - this를 스택에 넣어서, 나중에 어떤 순서로 닫아야 할지 기억.
- **_rectTransform.SetAsLastSibling()**
  - 계층상(Hierarchy) 맨 위로 올려서 다른 UI 위에 표시.
- **_panel.blocksRaycasts = true; _panel.alpha = 1f;**
  - 클릭 허용·완전 보이게 설정.
---

## PopupUI_3. HidePanel(): UI 안 보이게
```
public virtual void HidePanel()
{
    UIManager.Instance.currentPopupUI.TryPop(out _);
    _rectTransform.SetAsFirstSibling();

    _panel.alpha = 0f;
    _panel.blocksRaycasts = false;
}
```
- **currentPopupUI.TryPop(out _)**
  - 스택에서 가장 마지막에 넣은 UI를 꺼내고(LIFO), 닫힌 순서를 관리.
- **_rectTransform.SetAsFirstSibling()**
  - 계층상 맨 아래로 보내서 보이지 않게 하고 다른 UI 뒤로 숨김.
- **_panel.blocksRaycasts = false; _panel.alpha = 0f;**
  - 클릭 차단·완전 투명 처리.
---

## PopupUI_4. 드래그 처리 (IPointerDownHandler, IDragHandler)
```
public void OnPointerDown(PointerEventData eventData)
{
    if (_disableDrag) return;
    if (_dragArea != null &&
        !RectTransformUtility.RectangleContainsScreenPoint(_dragArea, eventData.position, eventData.pressEventCamera))
        return;

    RectTransformUtility.ScreenPointToLocalPointInRectangle(
        _rectTransform, eventData.position, eventData.pressEventCamera, out Vector2 offset);
    _dragOffset = offset;
}

public void OnDrag(PointerEventData eventData)
{
    if (_disableDrag) return;
    if (_dragArea != null &&
        !RectTransformUtility.RectangleContainsScreenPoint(_dragArea, eventData.position, eventData.pressEventCamera))
        return;

    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
        _canvas.GetComponent<RectTransform>(),
        eventData.position, eventData.pressEventCamera,
        out Vector2 pointerPos))
    {
        Vector2 newPos = pointerPos - _dragOffset;
        _rectTransform.localPosition = ClampToCanvas(newPos);
    }
}
```
- **public void OnPointerDown(PointerEventData eventData)**
  - 드래그 시작 위치 계산 (_dragOffset)
  - _disableDrag 또는 영역 밖이면 무시
- **public void OnDrag(PointerEventData eventData)**
  - 포인터 위치를 캔버스 좌표로 변환
  - _dragOffset 보정 후 localPosition 업데이트
  - ClampToCanvas로 경계 안에서만 움직이도록 제한
---

## PopupUI_5. ClampToCanvas(): 캔버스 밖으로 못 나가게 설정.
```
private Vector2 ClampToCanvas(Vector2 targetPosition)
{
    RectTransform canvasRect = _canvas.GetComponent<RectTransform>();

    Vector2 halfSize = _rectTransform.sizeDelta * _rectTransform.lossyScale / 2f;
    Vector2 canvasHalfSize = canvasRect.sizeDelta / 2f;

    float clampedX = Mathf.Clamp(
        targetPosition.x,
        -canvasHalfSize.x + halfSize.x,
        canvasHalfSize.x - halfSize.x
    );

    float clampedY = Mathf.Clamp(
        targetPosition.y,
        -canvasHalfSize.y + halfSize.y,
        canvasHalfSize.y - halfSize.y
    );

    return new Vector2(clampedX, clampedY);
}
```
- **목적**
  - UI가 캔버스 영역을 벗어나지 않도록 X/Y 좌표를 제한
- **계산 순서**
  - UI 절반 크기(halfSize)와 캔버스 절반 크기(canvasHalfSize) 계산
  - Mathf.Clamp로 최소/최대값 사이에 고정
---

# Dialogue System

## Table of Contents

1. [Dialogue 클래스 정의](#1-dialogue-클래스-정의)
2. [DialogueEvent 클래스 정의](#2-dialogueevent-클래스-정의)
3. [DialogueManager 컴포넌트](#3-dialoguemanager-컴포넌트)

   * [3.1 필드 & UI 참조](#31-필드--ui-참조)
   * [3.2 대사 시작: ShowDialogueEvent()](#32-대사-시작-showdialogueevent)
   * [3.3 대사 진행: NextDialogue()](#33-대사-진행-nextdialogue)
   * [3.4 조건 분기 예시](#34-조건-분기-예시)
   * [3.5 UI 숨김: HideDialogue()](#35-ui-숨김-hidedialogue)
4. [Key Binding System](#4-key-binding-system)

   * [4.1 InputRebindUI 클래스](#41-inputrebindui-클래스)
   * [4.2 RebindEntry 클래스](#42-rebindentry-클래스)

---

## 1. Dialogue 클래스 정의

```
[System.Serializable]
public class Dialogue
{
    public string characterName;   // 대사 실행 캐릭터 이름
    public string[] contexts;      // 대사 문장 배열
    public string[] conditions;    // 조건 키 이름 배열 (예: {"hasKey", "questCompleted"})
    public int[] conditionTargets; // 조건 목표 값 배열 (예: {5, 20})
}
```

* **역할**

  * 하나의 대사 블록을 정의
  * 분기 조건(`conditions`, `conditionTargets`)을 포함하여 대사 활성화 제어
* **필드 설명**

  * `characterName`: 대사를 말하는 캐릭터 이름
  * `contexts`: 순차적으로 표시할 문장 목록
  * `conditions`: 각 대사 분기 조건 키 이름 배열
  * `conditionTargets`: 해당 조건이 만족해야 할 목표 값 배열

---

## 2. DialogueEvent 클래스 정의

```
[System.Serializable]
public class DialogueEvent
{
    public string eventName;      // 이벤트 식별 이름
    public Vector2 line;          // 대사 UI 시작 위치 (화면 좌표)
    public Dialogue[] dialogues;  // 이벤트에 포함된 대사 배열
}
```

* **역할**

  * 대화 이벤트 단위로 대사 묶음 관리
* **필드 설명**

  * `eventName`: 대화 이벤트 구분용 문자열
  * `line`: 대사창을 표시할 화면상의 위치 좌표
  * `dialogues`: 순차 실행 또는 분기될 `Dialogue` 배열

---

## 3. DialogueManager 컴포넌트

`DialogueManager`는 `DialogueEvent` 또는 `Dialogue[]`를 받아 UI에 표시하고, 호출 또는 입력으로 대사를 진행합니다.

### 3.1 필드 & UI 참조

```
public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject dialogueBar;   // 대사 텍스트 바
    [SerializeField] private GameObject nameBar;       // 캐릭터 이름 바
    [SerializeField] private TMP_Text txtDialogue;     // 대사 텍스트 컴포넌트
    [SerializeField] private TMP_Text txtName;         // 이름 텍스트 컴포넌트

    private Dialogue[] dialogues;                      // 진행 중인 대사 배열
    private int dialogueIndex;                         // dialogues 배열 인덱스
    private int contextIndex;                          // 현재 Dialogue 내 문장 인덱스
    private bool isActive = false;                     // 대사 활성화 여부

    public bool IsActive => isActive;
}
```

* **UI 할당**: `dialogueBar`, `nameBar`, `txtDialogue`, `txtName`를 인스펙터에서 연결
* **내부 상태**: `dialogues`, `dialogueIndex`, `contextIndex`, `isActive`

### 3.2 대사 시작: ShowDialogueEvent()

```
public void ShowDialogueEvent(DialogueEvent evt)
{
    // UI 위치 설정
    Vector2 screenPos = evt.line;
    dialogueBar.GetComponent<RectTransform>().anchoredPosition = screenPos;
    nameBar.GetComponent<RectTransform>().anchoredPosition = screenPos;

    // 대사 배열 설정 및 표시
    ShowDialogue(evt.dialogues);
}

public void ShowDialogue(Dialogue[] p_dialogues)
{
    if (p_dialogues == null || p_dialogues.Length == 0) return;

    dialogues = p_dialogues;
    dialogueIndex = 0;
    contextIndex = 0;
    isActive = true;

    dialogueBar.SetActive(true);
    nameBar.SetActive(true);
    DisplayCurrent();
}
```

---

### 3.3 대사 진행: NextDialogue()

```
public void NextDialogue()
{
    if (!isActive) return;
    var current = dialogues[dialogueIndex];

    // 분기 조건 검사
    if (ShouldSkip(current))
    {
        SkipToNextDialogue();
        return;
    }

    // 같은 Dialogue 내 다음 문장
    if (contextIndex < current.contexts.Length - 1)
    {
        contextIndex++;
        DisplayCurrent();
        return;
    }

    // 다음 Dialogue 객체
    if (dialogueIndex < dialogues.Length - 1)
    {
        dialogueIndex++;
        contextIndex = 0;
        DisplayCurrent();
        return;
    }

    // 대사 종료
    HideDialogue();
}
```

---

### 3.4 조건 분기 예시

```
private bool ShouldSkip(Dialogue dlg)
{
    for (int i = 0; i < dlg.conditions.Length; i++)
    {
        int value = GameState.Instance.GetValue(dlg.conditions[i]);
        if (value < dlg.conditionTargets[i])
            return true; // 조건 미달 시 해당 대사 건너뜀
    }
    return false;
}

private void SkipToNextDialogue()
{
    if (dialogueIndex < dialogues.Length - 1)
    {
        dialogueIndex++;
        contextIndex = 0;
        DisplayCurrent();
    }
    else
        HideDialogue();
}
```

---

### 3.5 UI 숨김: HideDialogue()

```
public void HideDialogue()
{
    isActive = false;
    dialogueBar.SetActive(false);
    nameBar.SetActive(false);
}
```

---

## 4. Key Binding System

키 바인딩 기능을 제공하는 `InputRebindUI`와 `RebindEntry` 클래스를 설명합니다.

### 4.1 InputRebindUI 클래스

```
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
        // Inspector 참조 Null 체크
        if (_sectionHeaderPrefab == null) Debug.LogError("SectionHeaderPrefab 이 할당되지 않았습니다!");
        if (_content == null) Debug.LogError("Content Transform 이 할당되지 않았습니다!");

        // 0) 이전 저장된 오버라이드 로드
        if (PlayerPrefs.HasKey("rebinds"))
        {
            var json = PlayerPrefs.GetString("rebinds");
            _actions.LoadBindingOverridesFromJson(json);
        }

        // 1) 액션맵별 섹션 및 Entry 생성
        foreach (var mapName in _actionMapNames)
        {
            var map = _actions.FindActionMap(mapName);
            if (map == null) continue;

            // 섹션 헤더
            var headerGO = Instantiate(_sectionHeaderPrefab, _content);
            var headerText = headerGO.GetComponentInChildren<TMP_Text>();
            if (headerText == null)
            {
                Debug.LogError("SectionHeaderPrefab 안에 TMP_Text 컴포넌트가 없습니다!");
                continue;
            }
            headerText.text = map.name;

            // RebindEntry 생성
            foreach (var action in map.actions)
            {
                if (_excludeActionNames.Contains(action.name)) continue;
                var entryGO = Instantiate(_entryPrefab, _content);
                var entry = entryGO.GetComponent<RebindEntry>();
                entry.Setup(action, StartRebind);
                _entries.Add(entry);
            }
        }

        // 2) Save 버튼: 오버라이드 JSON 저장
        _saveButton.onClick.AddListener(() =>
        {
            var json = _actions.SaveBindingOverridesAsJson();
            PlayerPrefs.SetString("rebinds", json);
            Debug.Log("Bindings saved.");
        });

        // 3) Reset 버튼: 오버라이드 제거
        _resetButton.onClick.AddListener(() =>
        {
            _actions.RemoveAllBindingOverrides();
            PlayerPrefs.DeleteKey("rebinds");
            RefreshAllLabels();
            Debug.Log("Bindings reset to default.");
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
                  var newPath = action.bindings[bindingIndex].effectivePath;
                  foreach (var map in _actions.actionMaps)
                      foreach (var otherAction in map.actions)
                      {
                          if (otherAction == action) continue;
                          for (int i = 0; i < otherAction.bindings.Count; i++)
                          {
                              if (otherAction.bindings[i].effectivePath == newPath)
                              {
                                  otherAction.ApplyBindingOverride(i, "");
                                  Debug.Log($"Disabled duplicate key on [{otherAction.name}] binding #{i}");
                              }
                          }
                      }
                  RefreshAllLabels();
              })
              .Start();
    }
}
```

* **역할**

  * 입력 액션맵 단위로 섹션을 자동 생성하고, 각 액션마다 `RebindEntry`를 인스턴스화하여 리바인딩 UI 구성
  * `Save`/`Reset` 버튼으로 PlayerPrefs에 키 바인딩을 저장 및 초기화

---

### 4.2 RebindEntry 클래스

```
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class RebindEntry : MonoBehaviour
{
    [SerializeField] private TMP_Text _actionName;
    [SerializeField] private Button _rebindButton;

    private InputAction _action;
    private int _bindingIndex;
    private TMP_Text _buttonLabel;
    private System.Action<InputAction, int, TMP_Text> _onRebindRequested;

    public void Setup(InputAction action, System.Action<InputAction, int, TMP_Text> onRebind)
    {
        _action = action;
        _onRebindRequested = onRebind;
        _actionName.text = action.name;

        _buttonLabel = _rebindButton.GetComponentInChildren<TMP_Text>();
        _bindingIndex = 0;
        RefreshBindingDisplay();
        _rebindButton.onClick.AddListener(() =>
            _onRebindRequested?.Invoke(_action, _bindingIndex, _buttonLabel)
        );
    }

    public void RefreshBindingDisplay()
    {
        var binding = _action.bindings[_bindingIndex];
        var human = InputControlPath.ToHumanReadableString(
            binding.effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);
        _buttonLabel.text = human;
    }
}
```

* **역할**

  * 하나의 액션 바인딩 항목 UI를 관리: 버튼 클릭 시 리바인딩 시작, 현재 바인딩 표시
  * `RefreshBindingDisplay()`로 사람이 읽기 쉬운 경로 문자열 생성 및 텍스트 업데이트

---
# 대화 및 시간 UI 관리 스크립트 (2025.07.11.)

## 1. InteractionEvent (InteractionEvent.cs)

**역할**
씬 내 상호작용 지점에 붙여서, 지정된 `DialogueEvent` 범위에 따라 대사를 미리보기 및 실행합니다.

**사용처**

* `DialogueManager`가 있는 씬의 빈 `GameObject` 등에 컴포넌트로 추가

### 1.1 Inspector 설정

```
[Header("인스펙터 설정: 대화 이벤트 범위")]
public DialogueEvent dialogue;

[Space, SerializeField, Tooltip("범위에 해당하는 대화 배열 (미리보기용)")]
private NpcDialogue[] previewDialogues;
```

* `dialogue`: 시작\~종료 라인 정보(`Vector2`)를 갖는 `DialogueEvent`
* `previewDialogues`: 에디터 모드에서 `dialogue.line` 범위의 `NpcDialogue`를 동기 로드하여 배열에 할당

### 1.2 Awake / Reset / OnValidate

```
private void Awake()
{
    dialogueManager = FindObjectOfType<DialogueManager>();
    if (dialogueManager == null)
        Debug.LogError("씬에 DialogueManager가 없습니다!");
}

private void Reset()    => UpdatePreview();
private void OnValidate() => UpdatePreview();
```

* **Awake()**: 씬에서 `DialogueManager`를 찾아 캐싱, 없으면 에러 로그 출력
* **Reset() / OnValidate()**: 에디터에서 값이 변경될 때마다 `previewDialogues` 갱신

### 1.3 UpdatePreview()

```
private void UpdatePreview()
{
    int start = (int)dialogue.line.x;
    int end   = (int)dialogue.line.y;

    if (Application.isPlaying)
    {
        previewDialogues = DataManager.Instance.GetDialogues(start, end);
    }
#if UNITY_EDITOR
    else
    {
        var handle = Addressables.LoadAssetAsync<TextAsset>("NpcDialogue");
        handle.WaitForCompletion();
        TextAsset jsonAsset = handle.Result;

        if (jsonAsset != null)
        {
            var all = JsonConvert.DeserializeObject<List<NpcDialogue>>(jsonAsset.text);
            previewDialogues = all
                .Skip(start - 1)
                .Take(end - start + 1)
                .ToArray();
        }
        else previewDialogues = new NpcDialogue[0];

        EditorUtility.SetDirty(this);
    }
#else
    else previewDialogues = new NpcDialogue[0];
#endif
}
```

* **런타임**: `DataManager`에서 대사 로드
* **에디터**: Addressables + JSON 역직렬화로 미리보기 제공

### 1.4 Interact()

```
public void Interact()
{
    var dialogs = DataManager.Instance.GetDialogues(
        (int)dialogue.line.x,
        (int)dialogue.line.y
    );
    dialogueManager.ShowDialogue(dialogs);

    if (dialogs == null || dialogs.Length == 0)
        Debug.LogWarning($"대화 데이터가 없습니다: {dialogue.line.x}~{dialogue.line.y}");
}
```

* 플레이어 상호작용 시 대사를 가져와 `DialogueManager`에 전달

---

## 3. ClockUI (ClockUI.cs)

**역할**
* 시간의 흐름을 시각화하는 UI로, 아날로그 시침 회전 및 낮⇆밤 전환 시 와이프 이펙트를 처리합니다.

**사용처**

* UI 경로 하위에 시침(`RectTransform`)과 `Image` 컴포넌트를 가진 오브젝트에 추가

### 3.1 Inspector 설정

```c
[Header("Clock UI Elements")]
[SerializeField] private Image wipeImage;
[SerializeField] private float wipeDuration = 0.5f;
[SerializeField] private RectTransform clockHand;

[Header("Wipe Sprites")]
[SerializeField] private Sprite nightWipeSprite;
[SerializeField] private Sprite dayWipeSprite;

[Header("Animator")]
[SerializeField] private Animator clockAnimator;
```

* `wipeImage`: Radial Filled 타입으로 페이드 이펙트 처리
* `clockHand`: 시침 회전 대상
* `nightWipeSprite` / `dayWipeSprite`: 낮→밤, 밤→낮 전환 시 사용
* `clockAnimator`: Blend Tree를 이용한 진행도 표시

### 3.2 Awake()

```
private void Awake()
{
    wipeImage.type       = Image.Type.Filled;
    wipeImage.fillMethod = Image.FillMethod.Radial360;
    wipeImage.fillAmount = 0f;
}
```

* 와이프 이미지를 Radial360 Filled 타입으로 초기 설정

### 3.3 이벤트 구독

```
private void OnEnable()
{
    TimeManager.Instance.OnDayStateChanged += OnDayStateChanged;
}

private void OnDisable()
{
    if (TimeManager.Instance != null)
        TimeManager.Instance.OnDayStateChanged -= OnDayStateChanged;
}
```

* 시간 상태 변경 이벤트 구독 및 해제

### 3.4 Update()

```
void Update()
{
    float progress = TimeManager.Instance.DayProgress;
    float angle    = progress * 360f;
    clockHand.localRotation = Quaternion.Euler(0f, 0f, -angle);
}
```

* `DayProgress(0~1)`에 따라 시침 회전

### 3.5 OnDayStateChanged()

```
private void OnDayStateChanged(DayState newState)
{
    wipeImage.sprite = (newState == DayState.Night)
        ? nightWipeSprite
        : dayWipeSprite;

    StartCoroutine(WipeThenChangeProgress());
}
```

* 전환 시 와이프 스프라이트 교체 후 코루틴 실행

### 3.6 WipeThenChangeProgress()

```
private IEnumerator WipeThenChangeProgress()
{
    yield return StartCoroutine(WipeEffect());

    float progress = TimeManager.Instance.DayProgress;
    clockAnimator.SetFloat("Progress", progress);
}
```

* ① 와이프 이펙트 → ② 애니메이터에 진행도 전달

### 3.7 WipeEffect()

```
private IEnumerator WipeEffect()
{
    wipeImage.enabled     = true;
    wipeImage.fillAmount  = 0f;

    float t = 0f;
    while (t < wipeDuration)
    {
        t += Time.deltaTime;
        wipeImage.fillAmount = Mathf.Clamp01(t / wipeDuration);
        yield return null;
    }

    yield return null;
    wipeImage.fillAmount = 0f;
    wipeImage.enabled    = false;
}
```
---
* `fillAmount`를 0→1로 변경하여 페이드 효과 구현, 완료 후 리셋
# InventoryUI, InventoryItemSlot (2025.07.18.)
## 1. Inventory

### 역할
* 인벤토리 데이터를 시각적으로 표시하고, 장비/소비/재료 등 다양한 필터 탭과 연동하여 아이템 슬롯을 갱신, 플레이어/상점/NPC 인벤토리를 유동적으로 보여주는 팝업 UI입니다.

### 사용처
* UIManager를 통해 UIType.InventoryUI로 호출되며, 기본적으로 PlayerInventory, 또는 SetInventory()를 통해 NPCInventory에도 적용됩니다.
---
### 1.1 필드 구조 및 초기화
```
[Header("Inventory Tabs Settings")]
public List<InventoryTabGroup> tabs;

[Header("Slot Prefab and Container")]
[SerializeField] private InventoryItemSlot _itemSlotPrefab;
[SerializeField] private Transform _slotContainer;

[Header("Inventory Data Source")]
[SerializeField] private Inventory _inventory;

[Header("Toggle Color Settings")]
[SerializeField] private Color _normalColor;
[SerializeField] private Color _selectedColor;

[Header("Inventory ButtonSettings")]
[SerializeField] private Button _closeButton;
[SerializeField] private Button _openButton;

[Header("Weight Display")]
[SerializeField] private TMP_Text _weightText;
```
* tabs: 인벤토리 필터 탭(전체, 장비, 소비 등)

* _itemSlotPrefab: 슬롯 UI 프리팹

* _inventory: 현재 연결된 인벤토리 객체

* _weightText: 총 무게 출력 (현재 주석 처리 중)
---
### 1.2 ShowPanel(): UI 열기
```
public override void ShowPanel()
{
    base.ShowPanel();

    if (!_slotsCreated)
    {
        CreateSlots();
        _slotsCreated = true;
    }
    else
    {
        RefreshSlots();
    }

    if (!tabs.Any(t => t.toggle.isOn))
    {
        var first = tabs[0];
        first.toggle.SetIsOnWithoutNotify(true);
        ApplyToggleColor(first, true);
    }

    var active = tabs.First(t => t.toggle.isOn);
    FilterSlots(active.filter);
    UpdateWeightDisplay();
}
```
* 슬롯은 처음 1회만 생성하고 이후엔 갱신

* 필터 탭이 켜져 있지 않으면 첫 탭 자동 선택
---
### 1.3 슬롯 생성 및 갱신
```
private void CreateSlots()
{
    _uiSlots.Clear();
    var dataSlots = _inventory.GetSlots();

    foreach (var dataSlot in dataSlots)
    {
        var uiSlot = Instantiate(_itemSlotPrefab, _slotContainer);
        uiSlot.Initialize(dataSlot, this);
        _uiSlots.Add(uiSlot);
    }
}

private void RefreshSlots()
{
    var dataSlots = _inventory.GetSlots();

    for (int i = 0; i < Mathf.Min(_uiSlots.Count, dataSlots.Count); i++)
    {
        _uiSlots[i].Initialize(dataSlots[i], this);
    }
}
```
* CreateSlots(): 슬롯 생성 및 초기화

* RefreshSlots(): 이미 생성된 슬롯에 데이터만 갱신
---
### 1.4 필터 탭 기능
```
private void FilterSlots(InventoryFilter filter)
{
    foreach (var uiSlot in _uiSlots)
    {
        var slot = uiSlot.GetSlot();
        bool matches = filter == InventoryFilter.All ||
                       (!slot.IsEmpty && MatchesFilter(slot.item.ItemType, filter));

        if (matches) uiSlot.ShowContent();
        else uiSlot.HideContent();
    }
}
```
* 각 슬롯의 아이템 타입과 필터 조건(장비/소비/재료 등)을 비교해 슬롯 표시 여부 결정
---
### 1.5 탭 및 버튼 초기화
```
private void SetupTabsAndButtons()
{
    _closeButton.onClick.AddListener(HidePanel);

    if (_inventory is PlayerInventory)
        _openButton.onClick.AddListener(() => UIManager.Instance.ShowPanel(UIType.InventoryUI));

    foreach (var tab in tabs)
    {
        ApplyToggleColor(tab, false);
        tab.toggle.SetIsOnWithoutNotify(false);

        tab.toggle.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
            {
                DeselectOtherToggles(tab);
                ApplyToggleColor(tab, true);
                FilterSlots(tab.filter);
            }
            else ApplyToggleColor(tab, false);
        });
    }
}
```
* 탭 전환 시 필터링 및 색상 전환 처리

* Close/Open 버튼 연결
---
### 1.6 무게 표시 & 인벤토리 전환
```
private void UpdateWeightDisplay()
{
    if (_inventory is PlayerInventory playerInv && _weightText != null)
    {
        // float totalWeight = playerInv.GetTotalWeight();
        // _weightText.text = totalWeight.ToString("F1");
    }
}

public void SetInventory(Inventory inv)
{
    if (_inventory is NPCInventory oldNpc)
        oldNpc.OnInventoryChanged -= RefreshSlots;

    _inventory = inv;

    if (_inventory is NPCInventory newNpc)
        newNpc.OnInventoryChanged += RefreshSlots;

    foreach (var uiSlot in _uiSlots)
        Destroy(uiSlot.gameObject);
    _uiSlots.Clear();

    CreateSlots();
    FilterSlots(tabs[0].filter);
    UpdateWeightDisplay();
}
```
* SetInventory(): NPC 인벤토리 등으로 바인딩 교체 가능

* UpdateWeightDisplay(): 총 무게 출력 (주석 처리됨)
---
### 1.7 기타 지원 메서드
```
private void DeselectOtherToggles(InventoryTabGroup selectedTab) { ... }
private void ApplyToggleColor(InventoryTabGroup tabGroup, bool isSelected) { ... }
탭 색상 처리 및 단일 선택을 위한 유틸 메서드
```
---
## 2. InventoryItemSlot
### 역할
* 인벤토리 슬롯 하나를 표현하는 UI 컴포넌트로, 아이템 표시, 드래그 앤 드롭, 우클릭 사용, 툴팁 등 슬롯과 관련된 모든 상호작용을 처리합니다.

### 사용처
* InventoryUI에서 슬롯 프리팹으로 인스턴스화되어 각 아이템 데이터를 표현하고, Slot 클래스와 연결됩니다.
---
### 2.1 필드 구성
```
[Header("UI Components")]
[SerializeField] private Image _icon;
[SerializeField] private TMP_Text _nameText;
[SerializeField] private TMP_Text _quantityText;

[Header("Drag Boundary")]
[SerializeField] private RectTransform _baseRect;

[SerializeField] private Slot _slotData;
[SerializeField] private Item _prevItem;
[SerializeField] private int _prevQuantity;

private Vector3 _originPos;
private InventoryUI _parentInventoryUI;
```
* _slotData: 연결된 슬롯 데이터(Slot 클래스)

* _prevItem, _prevQuantity: 변경 감지용 이전 상태

* _icon, _nameText, _quantityText: 슬롯 내 아이템 정보 표시

* _parentInventoryUI: 부모 인벤토리 참조
---
### 2.2 초기화 및 갱신
```
public void Initialize(Slot slot, InventoryUI parent)
{
    _slotData = slot;
    _parentInventoryUI = parent;
    Refresh();
}

public void Refresh()
{
    if (_slotData == null || _slotData.IsEmpty)
    {
        _icon.enabled = false;
        _nameText.text = string.Empty;
        _quantityText.text = string.Empty;
    }
    else
    {
        _icon.enabled = true;
        // _icon.sprite = _slotData.item.icon;
        _nameText.text = _slotData.item.name_kr;
        _quantityText.text = _slotData.quantity > 1 ? _slotData.quantity.ToString() : string.Empty;
    }
}
```
* Initialize(): 슬롯과 부모 인벤토리를 연결

* Refresh(): 슬롯 UI를 현재 데이터로 갱신
---
### 2.3 콘텐츠 표시 제어
 ```
public void ShowContent() => Refresh();

public void HideContent()
{
    _icon.enabled = false;
    _nameText.text = string.Empty;
    _quantityText.text = string.Empty;
}
 ```
* ShowContent(): 현재 슬롯 데이터로 다시 표시

* HideContent(): 빈 칸처럼 숨김 처리
---
### 2.4 우클릭 처리
 ```
public void OnPointerClick(PointerEventData eventData)
{
    if (eventData.button == PointerEventData.InputButton.Right && _slotData != null && !_slotData.IsEmpty)
    {
        if (_slotData.item.isStackable)
            _slotData.quantity--;
        Refresh();
    }
}
 ```
* 우클릭 시 아이템 사용 또는 수량 감소
---
### 2.5 드래그 앤 드롭
```
public void OnBeginDrag(PointerEventData eventData)
{
    if (_slotData != null && !_slotData.IsEmpty)
    {
        DragSlot.Instance.dragSlot = this;
        DragSlot.Instance.DragSetImage(_icon);
        DragSlot.Instance.transform.position = eventData.position;
    }
}

public void OnDrag(PointerEventData eventData)
{
    if (DragSlot.Instance.dragSlot == this)
        DragSlot.Instance.transform.position = eventData.position;
}

public void OnEndDrag(PointerEventData eventData)
{
    if (DragSlot.Instance.dragSlot == this)
    {
        DragSlot.Instance.SetColor(0);
        DragSlot.Instance.dragSlot = null;
    }
}
```
* DragSlot을 활용한 이미지 표시 및 드래그 이동 처리
---
### 2.6 드롭 처리 및 거래 시스템
```
public void OnDrop(PointerEventData eventData)
{
    var source = eventData.pointerDrag?.GetComponent<IItemSlot>();
    if (source == null || source == this) return;

    var fromInv = source.GetParentInventory();
    var toInv = _parentInventoryUI.GetInventory();
    if (fromInv == null || toInv == null) return;

    // 1. 판매
    if (fromInv.OwnerType == Player && toInv.OwnerType == NPC)
    {
        int price = CalculateSellPrice(source.ItemData);
        ((PlayerInventory)fromInv).money += price;
    }
    // 2. 구매 or 도둑질
    else if (fromInv.OwnerType == NPC && toInv.OwnerType == Player)
    {
        var npc = (NPCInventory)fromInv;
        var player = (PlayerInventory)toInv;

        if (npc.Npc.StateMachine.GetState() == npc.Npc.StateMachine.NPCDeathState)
        {
            Debug.Log("훔치기 성공! 돈 차감 없음");
        }
        else
        {
            int price = CalculateBuyPrice(source.ItemData);
            if (player.money >= price)
                player.money -= price;
            else
            {
                Debug.LogWarning("돈이 부족합니다.");
                return;
            }
        }
    }

    // 아이템 이동 or 스왑
    if (_slotData.IsEmpty)
    {
        _slotData.item = source.ItemData;
        _slotData.quantity = source.Quantity;
        source.GetSlot().Clear();
    }
    else
    {
        SwapSlotData(source);
    }

    Refresh();
    source.Refresh();
}
```
* 판매/구매/도둑질 처리 포함

* 빈 슬롯 이동 or 아이템 교환 지원
---
### 2.7 슬롯 데이터 유틸
 ``` 
private void SwapSlotData(IItemSlot other)
{
    var tempItem = other.ItemData;
    var tempQty = other.Quantity;

    other.SetSlotData(_slotData.item, _slotData.quantity);

    _slotData.item = tempItem;
    _slotData.quantity = tempQty;
}

public Slot GetSlot() => _slotData;

public void SetSlotData(Item item, int quantity)
{
    _slotData.item = item;
    _slotData.quantity = quantity;
}

public Inventory GetParentInventory()
{
    return _parentInventoryUI.GetInventory();
}
 ``` 
* SwapSlotData(): 슬롯 간 교환 처리

* SetSlotData(): 수동 할당

* GetParentInventory(): 드래그 대상 구분을 위한 참조 반환
---

# TooltipUI (2025.08.01.)

## 개요

`TooltipUI`는 `PopupUI`를 상속해 마우스 커서를 따라다니며 정보(제목/본문)를 즉시 보여주는 툴팁 시스템이다. 즉시 표시/숨김, 화면 경계 내 위치 보정, 싱글톤 접근, 하위 루트 제어를 포함한다.

---

## 목차

1. [주요 기능](#1-주요-기능)
2. [필드 설명](#2-필드-설명)
3. [초기화](#3-초기화)
4. [표시 / 숨김](#4-표시--숨김)

   * [4.1 Show(string title, string body)](#41-showstring-title-string-body)
   * [4.2 HideTooltip()](#42-hidetooltip)
5. [위치 업데이트](#5-위치-업데이트)
6. [클램핑 (화면 경계 제한)](#6-클램핑-화면-경계-제한)
7. [싱글톤 패턴 & 보호](#7-싱글톤-패턴--보호)
8. [확장 포인트 / 주의사항](#8-확장-포인트--주의사항)

---

## 1. 주요 기능

* 커서를 따라다니는 툴팁 표시.
* 제목과 본문을 각각 설정하여 즉시 렌더링.
* 화면 내부로 위치 보정(클램핑).
* `PopupUI` 기반으로 스택 관리 및 계층 조정.
* 싱글톤 접근 방식으로 전역 사용 편의 제공.

---

## 2. 필드 설명

```
[Header("Tooltip Parts")]
[SerializeField] private GameObject _root;       // 전체 툴팁 루트 (배경 + 텍스트)
[SerializeField] private TMP_Text _titleText;    // 제목 텍스트
[SerializeField] private TMP_Text _bodyText;     // 본문 텍스트

[Tooltip("마우스 기준 오프셋")]
[SerializeField] private Vector2 _cursorOffset = new(10, -10); // 커서 기준 위치 보정

private RectTransform _canvasRect;              // 부모 캔버스 RectTransform (좌표 변환용)
private Canvas _parentCanvas;                   // 상위 Canvas 참조
private Camera _canvasCamera;                   // 캔버스가 카메라 기반일 경우 사용할 카메라
private RectTransform _rectTransform;           // 이 툴팁 자체 RectTransform
private CanvasGroup _canvasGroup;               // 투명도/클릭 제어

private bool _followCursor = false;             // 커서를 따라다닐지 여부

public static TooltipUI Instance { get; private set; } // 싱글톤 인스턴스
```

---

## 3. 초기화

```
private void Awake()
```

* `base.Awake()`로 `PopupUI` 기본 초기화 수행.
* 싱글톤 중복 검사: 기존 인스턴스가 있으면 파괴.
* RectTransform, CanvasGroup 등 컴포넌트 캐시.
* 부모 Canvas 정보 추출하여 렌더 모드에 따라 사용할 카메라 결정.
* 툴팁 루트 비활성화 및 초기 상태(alpha 0, 클릭 불가)로 설정.
* `_isOpen` 플래그 false로 초기화.

---

## 4. 표시 / 숨김

### 4.1 Show(string title, string body)

```
public void Show(string title, string body)
```

* 제목/본문을 전달받아 텍스트를 업데이트.
* 계층 최상위로 올리고 루트를 활성화.
* 커서를 추적하도록 `_followCursor`를 true로 설정.
* CanvasGroup을 통해 즉시 보이게 하고 인터랙션 허용.
* `PopupUI` 스택에 자신을 등록해서 계층/우선순위 유지.
* 커서 위치로 즉시 위치 갱신.

### 4.2 HideTooltip()

```
public void HideTooltip()
```

* 커서 추적(false) 중지.
* CanvasGroup을 통해 투명 처리하고 클릭 차단.
* 루트를 비활성화.
* `_isOpen`을 false로 설정.
* `PopupUI` 스택에서 제거.

---

## 5. 위치 업데이트

```
private void Update()
```

* `_followCursor`가 true인 동안 매 프레임 `UpdatePositionToCursor()` 호출.

```
private void UpdatePositionToCursor()
```

* 현재 마우스 스크린 좌표에 오프셋을 더해 위치 계산.
* 부모 캔버스 존재 시 `RectTransformUtility.ScreenPointToLocalPointInRectangle`로 로컬 좌표로 변환.
* 변환 성공 시 `ClampToCanvas()`로 제한한 후 `localPosition`에 반영.
* 부모 캔버스가 없으면 월드 좌표 기준으로 직접 위치 설정.

---

## 6. 클램핑 (화면 경계 제한)

```
private Vector2 ClampToCanvas(Vector2 pos)
```

* 캔버스 Rect가 없으면 입력값 그대로 반환.
* 툴팁 절반 크기를 기준으로 최소/최대 값을 계산해 X/Y를 `Mathf.Clamp`로 제한하여 화면 밖으로 나가지 않도록 함.

---

## 7. 싱글톤 패턴 & 보호

* `Instance`가 이미 존재하면 새로 생성된 객체를 파괴하여 중복 방지.
* 전역 접근을 통해 다른 시스템(예: 아이템 툴팁 호출 코드)에서 편리하게 사용 가능.

---

## 8. 확장 포인트 / 주의사항

* `PopupUI` 기반이므로 `Show()`/`HideTooltip()` 호출 시 내부 스택 관리가 자동으로 이루어짐.
* 커서 따라다니는 위치 계산에서 캔버스 렌더 모드에 따라 카메라 할당이 정확한지 확인해야 한다.
* 오프셋 값(`_cursorOffset`)을 조정해 툴팁이 마우스와 겹치지 않도록 튜닝할 것.
* 다른 팝업과 겹칠 때 Z순서 문제는 `SetAsLastSibling()`로 제어 가능하다.
---
# PopupUI (2025.07.25.)

## 개요

`PopupUI`는 화면에 띄우는 팝업 창의 공통 기반 클래스이며, 드래그, 열기/닫기 애니메이션, 서브 패널 관리 등을 포함한 확장된 UI 동작을 제공한다.

---

## 목차

1. [주요 변경/추가된 기능](#1-주요-변경추가된-기능)
2. [필드 설명](#2-필드-설명)
3. [생명주기 및 초기화](#3-생명주기-및-초기화)
4. [패널 표시 / 숨김](#4-패널-표시--숨김)

   * [4.1 ShowPanel()](#41-showpanel)
   * [4.2 HidePanel()](#42-hidepanel)
5. [애니메이션 흐름](#5-애니메이션-흐름)

   * [5.1 PlayOpenAnimation()](#51-playopenanimation)
   * [5.2 PlayCloseAnimation()](#52-playcloseanimation)
   * [5.3 후속 훅: AfterOpen / AfterClose](#53-후속-훅-afteropen--afterclose)
6. [드래그 처리](#6-드래그-처리)
7. [도움말 / 확장 포인트](#7-도움말--확장-포인트)

---

## 1. 주요 변경/추가된 기능

* **애니메이터 기반 열기/닫기 애니메이션**을 도입하여 트랜지션과 타이밍을 명확하게 제어.
* **서브 패널(\_subPanels)** 개념 추가: 메인 팝업이 열린 뒤 활성화되는 하위 UI 그룹을 별도 관리.
* **AfterOpen / AfterClose 훅**을 제공하여 파생 클래스가 애니메이션 완료 시점에 추가 작업 삽입 가능.
* \*\*드래그 제한 영역(\_dragArea)\*\*과 드래그 비활성화 옵션을 통한 정교한 드래그 제어.
* **기존 기본 UI 상태 초기화 구조 개선** (DefaultUI 외 초기 상태 설정 유지).

---

## 2. 필드 설명

```
public UIType UIType; // 인스펙터에서 지정: 이 팝업의 타입 식별자

protected CanvasGroup _panel;          // 메인 패널의 투명도/인터랙션 제어
protected RectTransform _rectTransform; // 위치/계층 조작
protected Canvas _canvas;              // 상위 Canvas 참조
private Vector2 _dragOffset;           // 드래그 중 오프셋 저장

[Header("드래그 설정")]
[SerializeField] private bool _disableDrag = false;      // 드래그 전체 비활성화 플래그
[SerializeField] private RectTransform _dragArea = null; // 드래그 허용 영역 제한

[Header("자식 패널")]
[SerializeField] private List<CanvasGroup> _subPanels = new List<CanvasGroup>(); // 열기 후 활성화될 하위 패널들

private Animator _animator;             // 열기/닫기 애니메이션 제어기
[SerializeField] private string openStateName = "Popup_Open";  // 열기 상태 이름 (애니메이터)
[SerializeField] private string closeStateName = "Popup_Close"; // 닫기 상태 이름

protected bool _isOpen = false;         // 현재 열려 있는 상태 플래그
```

---

## 3. 생명주기 및 초기화

```
public virtual void Awake()
```

* 애니메이터, CanvasGroup, RectTransform, 부모 Canvas를 캐시.
* `UIType`이 `DefaultUI`인 경우 기본적으로 보이게 설정하고, 그 외엔 숨김.
* 하위 패널들(`_subPanels`)은 처음에 모두 투명 및 클릭 불가로 초기화.

---

## 4. 패널 표시 / 숨김

### 4.1 ShowPanel()

```
public virtual void ShowPanel()
```

* `UIManager`의 스택에 자신을 푸시하여 열린 순서 관리.
* 계층에서 최상위로 올리고 `_isOpen`을 true로 설정.
* 메인 패널 클릭/표시 가능하게 설정.
* `PlayOpenAnimation()` 코루틴을 시작하여 애니메이션을 재생하고, 끝난 뒤 서브 패널을 활성화.

### 4.2 HidePanel()

```
public virtual void HidePanel()
```

* `UIManager` 스택에서 팝업을 팝 처리.
* 계층상 순서를 맨 아래로 보내고 `_isOpen` false 설정.
* 하위 패널들 비활성화 (투명 & 클릭 불가).
* 닫기 애니메이션 재생을 위한 `PlayCloseAnimation()` 시작.

---

## 5. 애니메이션 흐름

### 5.1 PlayOpenAnimation()

```
protected virtual IEnumerator PlayOpenAnimation()
```

* 애니메이터가 있을 경우 트리거 `"isOpen"`을 세팅.
* 지정된 `openStateName` 상태로 진입할 때까지 기다린 다음, 그 상태 길이만큼 대기.
* 이후 모든 서브 패널을 표시 (`alpha=1`, `blocksRaycasts=true`).
* 끝나면 `AfterOpen()` 호출 (파생 클래스 확장 포인트).

### 5.2 PlayCloseAnimation()

```
protected virtual IEnumerator PlayCloseAnimation()
```

* `AfterClose()`를 먼저 호출하여 닫히기 직전 동작 수행 가능.
* 애니메이터가 있을 경우 트리거 `"isClose"`을 세팅하고 `closeStateName` 진입/재생 대기.
* 애니메이터 후, 메인 패널을 투명 & 클릭 불가로 만든다.

### 5.3 후속 훅: AfterOpen / AfterClose

```
protected virtual void AfterOpen() { /* 파생 클래스 오버라이드 지점 */ }
protected virtual void AfterClose() { /* 파생 클래스 오버라이드 지점 */ }
```

* subclass가 애니메이션 완료 시 추가 로직(예: 포커스, 입력 활성화 등)을 구현할 수 있게 분리됨.

---

## 6. 드래그 처리

```
public void OnPointerDown(PointerEventData eventData)
public void OnDrag(PointerEventData eventData)
```

* `_disableDrag`가 켜져 있으면 무시.
* `_dragArea`가 지정된 경우에는 해당 영역 안에서만 드래그 시작/계속 가능.
* 화면 좌표를 캔버스 로컬로 변환하여 `_dragOffset` 보정 후 이동.
* `ClampToCanvas()`를 통해 팝업이 캔버스를 벗어나지 않도록 제한.

```
private Vector2 ClampToCanvas(Vector2 targetPosition)
```

* 팝업의 절반 크기와 캔버스 절반 크기를 고려하여 X/Y를 `Mathf.Clamp`로 제한.

---

## 7. 도움말 / 확장 포인트

* **Animator 파라미터 이름** (`"isOpen"`, `"isClose"`)이나 상태 이름(`openStateName`, `closeStateName`)은 애니메이터 컨트롤러와 반드시 일치시켜야 한다.
* **서브 패널 리스트**를 이용해 메인 열기 애니메이션이 끝난 뒤 별도 정보(예: 탭 콘텐츠, 툴팁)를 순차적으로 활성화할 수 있다.
* \*\*파생 클래스가 \*\*`**/**`**를 override**하여 입력 잠금 해제, 포커스 강제 지정, 사운드 재생 등을 쉽게 삽입.
* **드래그 영역**을 둬서 사용자가 의도하지 않은 곳에서 창을 이동하지 못하게 제어 가능.

