using System.Collections.Generic; // Dictionary, Stack 등 컬렉션 사용을 위해
using System.Linq;
using UnityEngine;
public enum UIType
{
    MapUI,
    OptionsUI,
    InventoryUI,
    NPCInventoryUI,
    EquipUI,
    StatusUI,
    SkillUI,
    DefaultUI,
    ShopUI,
    QuestUI,
    SlotMachineUI,
    EnhanceUI,
    CraftUI,
    StorageUI,
    HUD,
    TooltipUI,
    QuantitySelectUI,
    EnhancePopupUI
}

public class UIManager : Singleton<UIManager> // UIManager는 Singleton을 상속받아 싱글톤 패턴을 적용
{

    [HideInInspector] // Canvas의 Transform을 저장할 변수 (에디터에서 숨김)
    public GameObject canvasTrm; // Canvas 위치

    // 모든 팝업 UI를 저장하는 딕셔너리
    public Dictionary<UIType, PopupUI> popupUIByType = new();

    // 열려 있는 팝업들을 스택 구조로 관리 (최근에 연 순서대로 닫기 위해)
    public Stack<PopupUI> currentPopupUI = new(); // 최근 열린 UI 저장

    protected override void Awake()
    {
        base.Awake(); // Singleton의 Awake를 호출하여 싱글톤 인스턴스를 설정
        canvasTrm = gameObject.GetComponentInChildren<Canvas>().gameObject; //GameObject.Find("Canvas"); // "Canvas"라는 이름의 게임 오브젝트를 찾아 canvasTrm에 저장

        // Canvas 안의 모든 PopupUI 컴포넌트를 찾음 (비활성화된 오브젝트도 포함)
        PopupUI[] popupUIs = canvasTrm.GetComponentsInChildren<PopupUI>(true);

        // 각 PopupUI를 딕셔너리에 등록
        foreach (PopupUI popupUI in popupUIs)
        {
            // Debug.Log("popup이름"+popupUI.gameObject.name);
            if (!popupUIByType.ContainsKey(popupUI.UIType))
                popupUIByType.Add(popupUI.UIType, popupUI);
        }
    }

    /// <summary>
    /// 특정 이름의 UI를 찾아서 화면에 보여줌.
    public void ShowPanel(UIType uiType)
    {
        // 열려있는 UI 중 유지해야 할 DefeatUI는 건드리지 않음
        if (uiType == UIType.OptionsUI)
        {
            // OptionsUI는 단독으로 열려야 하므로 다른 UI는 모두 닫음 (DefeatUI 제외)
            var toClose = new Stack<PopupUI>(currentPopupUI);
            foreach (var ui in toClose)
            {
                if (ui.UIType != UIType.DefaultUI)
                    ui.HidePanel();
            }
        }
        else
        {
            // 다른 UI는 OptionsUI가 열려 있으면 열리지 않음
            if (IsTopPanel(UIType.OptionsUI))
                return;
        }

        // 원하는 UI를 보여줌
        if (popupUIByType.TryGetValue(uiType, out var popupUI))
        {
            popupUI.ShowPanel();
        }
    }

    /// <summary>
    /// 특정 이름의 UI를 찾아서 화면에서 숨김.
    public void HidePanel(UIType uiType)
    {
        if (popupUIByType.TryGetValue(uiType, out var popupUI))
        {
            if (!popupUI.gameObject.activeInHierarchy) return; // 이미 닫혔으면 무시
            popupUI.HidePanel();
        }
    }

    /// <summary>
    /// 현재 열려 있는 모든 UI를 한 번에 닫음.
    public void HideAllPanels()
    {
        // 스택을 복사해서 안전하게 반복
        var tempStack = new Stack<PopupUI>(currentPopupUI);

        foreach (var popup in tempStack)
        {
            if (popup.UIType != UIType.DefaultUI)
                popup.HidePanel();
        }

        // 다시 초기화 (DefeatUI만 남기기 위해)
        currentPopupUI = new Stack<PopupUI>(
            new Stack<PopupUI>(currentPopupUI).ToArray().Where(p => p.UIType == UIType.DefaultUI)
        );
    }
    /// <summary>
    /// 특정 UI가 현재 열려 있는지 확인하는 함수
    /// 가장 위에 있는 UI(Peek)가 해당 UIType인지 확인함
    public bool IsTopPanel(UIType uiType)
    {
        return currentPopupUI.Count > 0 && currentPopupUI.Peek().UIType == uiType;
    }
    public void OpenPlayerUI(NPCInventory npcInv,Inventory playerInventory)
    {
        var shopUI = popupUIByType[UIType.NPCInventoryUI] as InventoryUI;
        var enhanceUI = popupUIByType[UIType.EnhanceUI] as EnhanceUI;
        enhanceUI.SetTargetInventory(playerInventory);
        shopUI.SetTargetInventory(playerInventory);
        shopUI.SetNPCInventory(npcInv);   // 이번 NPC 인벤토리 주입
    }
    public void OpenStorageUI(Storage storage,Inventory playerInventory) 
    {
        var storageUI = popupUIByType[UIType.StorageUI] as StorageUI;
        storageUI.SetInventory(playerInventory, storage);
        // 플레이어 인벤 UI도 같이 열고 타겟 지정
        var playerUI = popupUIByType[UIType.InventoryUI] as InventoryUI;
        playerUI.SetTargetInventory(storage);  // 플레이어가 참조할 타겟은 현재 창고
        ShowPanel(UIType.StorageUI);
        ShowPanel(UIType.InventoryUI);
    }
}



