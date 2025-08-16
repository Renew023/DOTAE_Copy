using System;
using System.Collections.Generic;
[Serializable]
public class NpcDialogue {
    public int id;
    public string characterName;
    public DesignEnums.DialogueType DialogueType;
    public List<string> contexts;
    public List<string> choiceTexts;
    public List<string> choiceActions;
    public int questId;
    public string questName;
    public string questDescription;
    public int prerequisiteQuestId;
    public List<string> failContexts;
}
