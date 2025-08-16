using System;
using System.Collections.Generic;
[Serializable]
public class NPCDialog {
    public List<int> preDialogueId;
    public int dialogId;
    public int NPCID;
    public List<string> contexts;
    public List<string> talkCharacter;
    public List<string> choiceTexts;
    public List<string> questClearText;
    public int questClearDialog;
    public List<string> choiceActions;
    public int questId;
    public DesignEnums.QuestType questType;
    public string questName;
    public string questDescription;
    public List<string> questWantID;
    public List<string> failContexts;
    public List<DesignEnums.QuestEvent> questEventList;
    public List<int> questClearPercent;
    public List<int> rewardItem;
    public float rewardExp;
    public int rewardGold;
}
