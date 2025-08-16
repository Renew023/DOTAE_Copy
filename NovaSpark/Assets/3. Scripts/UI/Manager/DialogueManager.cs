using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject go_DialogueBar;
    [SerializeField] private GameObject go_DialogueNameBar;
    [SerializeField] private TMP_Text txt_Dialogue;
    [SerializeField] private TMP_Text txt_Name;
    [Header("Choice UI")]
    [SerializeField] private ChoiceUI choiceUI;

    private NpcDialogue[] dialogues;
    private int dialogueIndex;
    private int contextIndex;
    private bool isActive = false;

    public bool IsActive => isActive;

    private void Start()
    {
        choiceUI.OnChoiceSelected += OnChoiceSelected;
    }
    private void OnDestroy()
    {
        choiceUI.OnChoiceSelected -= OnChoiceSelected;
    }

    public void ShowDialogue(NpcDialogue[] p_dialogues)
    {
        if (p_dialogues == null || p_dialogues.Length == 0) return;

        dialogues = p_dialogues;
        dialogueIndex = 0;
        contextIndex = 0;
        isActive = true;

        go_DialogueBar.SetActive(true);
        go_DialogueNameBar.SetActive(true);

        DisplayCurrent();

        // **추가**: 첫 항목이 선택지/퀘스트 타입인 경우 바로 선택지 띄우기
        var first = dialogues[0];
        if (first.DialogueType == DesignEnums.DialogueType.PlayerChoice ||
            first.DialogueType == DesignEnums.DialogueType.Quest)
        {
            choiceUI.ShowChoices(first.choiceTexts, first.choiceActions);
        }
    }

    public void NextDialogue()
    {
        if (!isActive) return;

        var current = dialogues[dialogueIndex];

        switch (current.DialogueType)
        {
            case DesignEnums.DialogueType.Basic:
                if (contextIndex < current.contexts.Count - 1)
                {
                    contextIndex++;
                    DisplayCurrent();
                }
                else AdvanceDialogueObject();
                break;

            case DesignEnums.DialogueType.PlayerChoice:
            case DesignEnums.DialogueType.Quest:
                // 이미 첫 클릭에서 선택지 띄웠으니, 두번째 클릭부터 선택지에 진입
                // 여기선 그냥 AdvanceDialogueObject로 대화 종료/다음으로
                AdvanceDialogueObject();
                break;
        }
    }

    private void AdvanceDialogueObject()
    {
        // 다음 Dialogue 객체로 넘기는 기존 로직
        if (dialogueIndex < dialogues.Length - 1)
        {
            dialogueIndex++;
            contextIndex = 0;
            DisplayCurrent();
        }
        else
        {
            HideDialogue();
        }
    }

    private void DisplayCurrent()
    {
        var dlg = dialogues[dialogueIndex];
        txt_Name.text = dlg.characterName;
        txt_Dialogue.text = dlg.contexts.Count > 0
            ? dlg.contexts[contextIndex]
            : "<빈 대사>";
    }

    public void HideDialogue()
    {
        isActive = false;
        go_DialogueBar.SetActive(false);
        go_DialogueNameBar.SetActive(false);
    }

    // TODO: 실제 UI로 치환할 ShowChoices 예시
    private void ShowChoices(List<string> texts, List<string> actions)
    {
        // 예: ChoicePanel.SetUp(texts, actions, OnChoiceSelected);
    }

    private void HandleQuestSelection(NpcDialogue questDialogue)
    {
        // 예: if (choice == "AcceptQuest") QuestManager.Accept(questDialogue.questId);
    }

    private void OnChoiceSelected(string actionKey)
    {
        choiceUI.HideChoices();

        // 1) 액션키가 숫자(ID)일 때
        if (int.TryParse(actionKey, out int nextId))
        {
            // 해당 ID의 대화 한 건만 로드
            var nextArr = DataManager.Instance.GetDialogues(nextId, nextId);
            if (nextArr.Length == 0)
            {
                HideDialogue();
                return;
            }

            var next = nextArr[0];

            // 2) Quest 타입이고, 선행퀘가 있는데 아직 완료되지 않았다면
            /*if (next.DialogueType == DesignEnums.DialogueType.Quest
                && next.prerequisiteQuestId.HasValue
                && !QuestManager.Instance.IsCompleted(next.prerequisiteQuestId.Value))
            {
                // 항상 failContexts 에서만 대사를 읽어옴
                var failLines = next.failContexts;
                if (failLines != null && failLines.Count > 0)
                {
                    // 임시로 Basic 노드로 감싸서 보여줌
                    var temp = new NpcDialogue
                    {
                        id = -1,
                        characterName = next.characterName,
                        DialogueType = DesignEnums.DialogueType.Basic,
                        contexts = failLines
                    };
                    ShowDialogue(new[] { temp });
                    return;
                }
                else
                {
                    HideDialogue();
                    return;
                }
            }*/

            // 3) 그 외(선행퀘 필요 없거나 이미 깼거나, Basic/PlayerChoice) 정상 처리
            ShowDialogue(new[] { next });
            return;
        }

        // 4) 숫자가 아닐 때(예: OpenShop, AcceptQuest 등 기존 키)
        switch (actionKey)
        {
            case "OpenShop":
                UIManager.Instance.ShowPanel(UIType.ShopUI);
                break;
            case "AcceptQuest":
                UIManager.Instance.ShowPanel(UIType.QuestUI);
                break;
            case "RejectQuest":
                UIManager.Instance.HidePanel(UIType.QuestUI);
                break;
                // ... 그 외 동작
        }

        // 선택 후엔 대화 닫거나 다음으로
        HideDialogue();
    }
}

