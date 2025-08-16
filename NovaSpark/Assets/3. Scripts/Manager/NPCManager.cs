using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    private Dictionary<int, NPC> npcById = new();
    private AffectionManager affectionManager;

    public void Init(AffectionManager affectionManager)
    {
        this.affectionManager = affectionManager;
    }

    // npc 등록
    public void RegisterNPC(NPC npc)
    {
        if (npc == null)
            return;

        int id = npc.characterRuntimeData.characterID;

        if (!npcById.ContainsKey(id))
        {
            npcById.Add(id, npc);
            affectionManager.RegisterNPC(id);
        }
        else
        {
            Debug.LogWarning($"{id} 이미 존재함.");
        }
    }

    // npc 등록 해제
    public void UnregisterNPC(NPC npc)
    {
        if (npc == null)
            return;

        int id = npc.characterRuntimeData.characterID;

        if (npcById.ContainsKey(id))
        {
            npcById.Remove(id);
        }
    }

    // id로 npc 가져오기
    public NPC GetNPCById(int npcId)
    {
        return npcById.TryGetValue(npcId, out var npc) ? npc : null;
    }

    public List<NPC> GetAllNPCs()
    {
        return new List<NPC>(npcById.Values);
    }

    /* NPC.cs 에 붙여야함
    private void OnEnable()
    {
        NPCManager.Instance.UnregisterNPC(this);
    }

    private void OnDisable()
    {
        NPCManager.Instance.UnregisterNPC(this);
    }
    */
}