using System;
using System.Collections.Generic;
[Serializable]
public class NPCData {
    public int characterID;
    public List<int> DiaglogList;
    public DesignEnums.NPCType npcType;
    public List<string> CharacterTalk;
}
