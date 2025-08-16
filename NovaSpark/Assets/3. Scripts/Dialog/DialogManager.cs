using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

//NPC가 각각 Dialog를 가지고 있어야함.
//그래야 순차적인 Dialog 지급이 가능해질 듯 한데
//플레이어 -> 501번 NPC에게 Interact.
//NPC에게 Dialog가 존재하는지 확인 후 있다면 퀘스트 버튼 활성화
//퀘스트 버튼을 누르면 해당 Dialog를 DialogManager에서 실행.
//끝나면 해당 NPC의 Dialog 데이터를 갱신함.
//무엇으로? -> 단순히 다이어로그 루프.
//클리어 되었는지에 대한 여부? -> Dictionary 혹은 HashSet으로 이미 한 내용은 넘김
//Dialog가 존재하지 않다면, NPC에게 더 이상 남은 퀘스트는 없음.

public class DialogManager : Singleton<DialogManager>
{
    public Player player;

    [Header("다이어로그")]
    public HashSet<int> getDialog = new(); //퀘스트 중일 때
    public HashSet<int> clearDialog = new(); //퀘스트 혹은 다이어로그 클리어 시에
    public Button dialogPanel; //다이어로그 패널 누르면 그냥 스토리 진행됨
    public TextMeshProUGUI dialogCharNameText; //다이어로그 텍스트 이름
    public TextMeshProUGUI dialogContentText; //다이어로그 텍스트 대화
    public Dictionary<int, NPCDialog> NPCdialogDict = new(); //NPC가 가진 대화 내용
    public List<QuestBox> selectQuestButton = new(); //선택한 퀘스트 버튼 

    [Header("퀘스트 진행")]
    public GameObject questListPanel;           //퀘스트창 표시
    public ObjectPool<QuestBox> questPool;      //퀘스트풀
    public Transform questListPos;              //퀘스트 생성 위치 
    public Transform questSelectButtonPos;      //퀘스트 선택 버튼 생성 위치
    public List<QuestBox> questBoxes = new();   //퀘스트 리스트
    public QuestBox questPrefab;                //퀘스트 박스 프리팹
    public Dictionary<int, Dictionary<int, int>> questTarget = new(); //퀘스트 목표 //좌측은 퀘스트 ID, 우측에는 퀘스트에 필요한 아이템 ID + 필요한 개수.
                                                                        //여기서 생기는 문제점, 현재 NPCDialog는 NPC와 대화했을 때 기점으로 생겨버림. 하지만 퀘스트를 볼 때에는
                                                                        //NPC에 대한 정보를 알 수가 없음. NPC에 대한 정보를 모른채 Dialog를 찾는 것이 불가능.
    public Dictionary<int, NPCDialog> questProgress = new(); //퀘스트 완성도

    [Header("퀘스트 수락 시 떠야하는 패널")]
    public GameObject questPanel;
    public TextMeshProUGUI questCharNameText;
    public TextMeshProUGUI questContentText;
    public TextMeshProUGUI questMissionText;
    public Button questPanelCloseButton;

    public int npcKey;
    public int dialogKey;

    public NPCDialog dialogCash;
    private int curPage;

    protected override void Awake()
    {
        DontDestroyOnLoad(this);
        base.Awake();

        dialogPanel = gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).GetComponent<Button>();
        dialogCharNameText = dialogPanel.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
        dialogContentText = dialogPanel.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>();
        //Debug.Log("DialogManager 해시코드: " + DialogManager.Instance.GetHashCode());

        dialogPanel.onClick.AddListener(()=> NextDialog());

        questPanelCloseButton.onClick.AddListener(() =>
        {
            questPanel.SetActive(false);
            DialogOff();
        });

        DialogOff();

        questPool = new ObjectPool<QuestBox>(
                    () => Instantiate(questPrefab),
                    obj => obj.gameObject.SetActive(true),
                    obj =>
                    {
                        obj.gameObject.SetActive(false);
                        obj.questButton.onClick.RemoveAllListeners();
                    }
                );
    }

    #region QuestEvent 참고 위치
        // 장소 : Village, PlayerArea
        // 대화 : NPC
        // 킬 : Enemy, NPC
        // 아이템 미정 :
        // 아이템 강화 : 

    #endregion

    public void AddQuestSuccess(int key, DesignEnums.QuestEvent type = DesignEnums.QuestEvent.None, int value = 1) //Type 해당 타입과 맞지 않으면 증가하지 않음.
    {
        //Debug.LogError(key + " 퀘스트 성공, 타입: " + type + ", 값: " + value);
        foreach (var questEntry in questProgress)
        {
            int questId = questEntry.Key;
            var objectives = questEntry.Value;

            for (int i = 0; i < objectives.questEventList.Count; i++)
            {
                if (objectives.questEventList[i] == type) //해당 타입과 맞다면, 처치와 대화는 다르니까. 대상은 같을 수도 있음. 외에도 NPC 몰래 보기 등 있을 수도 있지.
                {
                    if (questTarget[questEntry.Key].ContainsKey(key))
                    {
                        questTarget[questEntry.Key][key] += value; //값을 추가
                        //QuestClearCheck();
                    }
                }
                else
                {
                    continue;
                }
            }
       
            bool isClear = true; //퀘스트 클리어하는 장치

            for (int i = 0; i < objectives.questEventList.Count; i++)
            {
                //Debug.Log($"당신의 퀘스트 클리어 회수 : {questProgress[questId].questClearPercent[i]}, 내가 클리어한 해당 퀘스트 회수 : {questTarget[questId][int.Parse(objectives.questWantID[i])]}");
                if (questProgress[questId].questClearPercent[i] <= questTarget[questId][int.Parse(objectives.questWantID[i])])
                {
                    //Debug.Log($"퀘스트 {questId}의 목표 {objectives.questWantID[i]} 달성됨");
                }
                else
                {
                    isClear = false; //퀘스트 클리어 조건이 충족되지 않음
                    //Debug.Log($"퀘스트 {questId}의 목표 {objectives.questWantID[i]} 미달성");
                    break; //더 이상 확인할 필요 없음
                }
            }

            if (isClear)
            {
                clearDialog.Add(questId); // 퀘스트 클리어 표시
                QuestLog.Instance.RemoveQuestLog(questId);
                UtilityCode.Logger($"퀘스트 {questId} 완료!");
                QuestClear(questId);
            }
        }
    }

    private void QuestClear(int questId)
    {
        dialogCharNameText.text = player.characterRuntimeData.name_kr;
        dialogContentText.text = questProgress[questId].questClearText[0];
        int key = questProgress[questId].questClearDialog;
        if( key != 0)
        {
            dialogKey = key;
            dialogCash = NPCdialogDict[dialogKey];
        }
        else
        {
            dialogKey = 0; //퀘스트 클리어 다이얼로그가 없다면, 0으로 설정
            dialogCash = null;
        }
        dialogPanel.gameObject.SetActive(true);
    }

    public void DialogList()
    {
        //초기화
        foreach (var item in questBoxes)
        {
            questPool.Release(item);
        }
        questBoxes.Clear();
        questListPanel.gameObject.SetActive(true);

        //퀘스트 리스트 갱신
        #region npcDialog 캐싱
        var npcDialogQuestFillter = NPCdialogDict
            .Where(dialog =>
            !clearDialog.Contains(dialog.Value.questId) &&
            dialog.Value.preDialogueId.All(id => clearDialog.Contains(id))
            );
        #endregion

        //퀘스트 목록 생성
        foreach (var dialog in npcDialogQuestFillter)
        {
            UtilityCode.Logger(dialog.Value.dialogId.ToString());
            var box = questPool.Get();
            box.transform.parent = questListPos;
            box.titleText.text = dialog.Value.questName.ToString();
            box.descriptionText.text = dialog.Value.questDescription.ToString();

            box.questButton.onClick.AddListener(() => 
            { 
                dialogKey = dialog.Key;
                ViewDialog();
                for (int i = 0; i < questBoxes.Count; i++)
                {
                    questPool.Release(questBoxes[i]);
                }
                questBoxes.Clear();
            });
            // box 설정 (예: box.SetData(dialog.Value);)
            questBoxes.Add(box);
            
        }
    }

    public void ViewDialog()
    {
        dialogCash = NPCdialogDict[dialogKey];
        if (clearDialog.Contains(dialogCash.questId)) return;

        dialogPanel.gameObject.SetActive(true);

        if(getDialog.Contains(dialogCash.questId))
        {
            dialogCharNameText.text = DataManager.Instance.CharacterDataByID[dialogCash.NPCID].name_kr;

            string text;
            if(dialogCash.failContexts == null)
            {
                text = "...";
            }
            else
            {
                text = dialogCash.failContexts[curPage];
            }

            dialogContentText.text = text; 
            curPage++;
            return;
        }
        //TODO : PlayerKey를 멈춘다던가 하는 기능, 
        //player.PlayerInput.enabled = false;

        dialogCharNameText.text = dialogCash.talkCharacter[curPage];
        dialogContentText.text = dialogCash.contexts[curPage];
        curPage++;
    }

    public void NextDialog()
    {
        UtilityCode.Logger("당신은 Next를 누르고 있습니다");

        if (dialogKey == 0)
        {
            EndDialog();
        }

        if(getDialog.Contains(dialogCash.questId))
        {
            if (dialogCash.failContexts == null)
            {
                EndDialog();
                return;
            }

            if (dialogCash.failContexts.Count > curPage)
            {
                ViewDialog();
                return;
            }
            else
            {
                EndDialog();
                return;
            }
        }
        
        if (dialogCash.contexts.Count > curPage)
        {
            ViewDialog();
            return;
        }
        else
        {
            if (QuestCheck())
                return;
            EndDialog();
            return;
        }
    }

    private bool QuestCheck()
    {
        if (dialogCash.choiceActions.Count > 0 || dialogCash.choiceActions == null) //선택지가 있다면
        {
            var choice = dialogCash.choiceTexts.Count;
            
            for (int i = 0; i < choice; i++)
            {
                var box = questPool.Get();
                box.transform.parent = questSelectButtonPos;
                //퀘스트 ID
                int.TryParse(dialogCash.choiceActions[i], out int endCheck);
                //퀘스트 Get
                int endCheckCopy = endCheck;

                box.questButton.onClick.AddListener(() =>
                {
                    EndDialog(endCheckCopy);

                    for (int i = 0; i < selectQuestButton.Count; i++)
                    {
                        questPool.Release(selectQuestButton[i]);
                    }
                    selectQuestButton.Clear();
                });
                box.titleText.text = dialogCash.choiceTexts[i];

                selectQuestButton.Add(box);
            }

            return true;
        }
        //선택지가 있다면 false
        return false;
    }

    private void EndDialog(int key = 0) //Key는 dialogID 또는 QuestID로 연결될 수 있음.
    {
        if (DataManager.Instance.NpcDialogueById.ContainsKey(key) && key != 0) //퀘스트 선택 버튼을 눌렀을 때, 다음으로 연결함. 퀘스트의 경우, Contains에 있지 않고 다이얼로그의 경우 Contains에 포함되어 출력됨.
        {
            dialogKey = key;
            ViewDialog();
            return;
        }

        if (dialogCash.questId != 0) //퀘스트가 있는 다이어로그고 퀘스트를 받았다면.
        {
            if (key == dialogCash.questId)
            {
                if (!getDialog.Contains(dialogCash.questId)) //퀘스트를 아직 받지 않았다면 받고서 DialogClear
                {
                    clearDialog.Add(dialogKey);
                    QuestLog.Instance.CreateQuestLog(dialogCash.questId);
                    OnQuestPanel();
                    getDialog.Add(key); //퀘스트를 받아낸다.
                    for(int i =0; i< dialogCash.questEventList.Count; i++)
                    {
                        if (!questProgress.ContainsKey(dialogCash.questId))
                        {
                            questProgress[dialogCash.questId] = new();
                            questTarget[dialogCash.questId] = new();
                        }
                        questProgress[dialogCash.questId] = dialogCash; //퀘스트 목표를 추가한다.
                        questTarget[dialogCash.questId].Add(int.Parse(dialogCash.questWantID[i]), 0); //퀘스트 목표를 추가한다.
                    }
                }
                DialogOff();
                return;
            }
            else
            {
                DialogOff(); //다이얼로그 패널 끄고, dialog Page 0
            }
        }
        else
        {
            clearDialog.Add(dialogKey); //퀘스트 없으면 다이얼로그 클리어
            DialogOff(); //다이얼로그 패널 끄고, dialog Page 0
        }
    }

    private void OnQuestPanel()
    {
        //TODO QuestPanel을 켠다.
        questPanel.SetActive(true);
        //TODO QuestPanel에 동기화한다.
        questCharNameText.text = dialogCash.questName;
        questContentText.text = dialogCash.questDescription;
        questPanelCloseButton.gameObject.SetActive(true);
        //quest Mission
        //quest 보상
    }

    public void DialogOff()
    {
        curPage = 0;
        dialogPanel.gameObject.SetActive(false);
        questListPanel.gameObject.SetActive(false);
    }
}