using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using static DesignEnums;

public class DeadBodyController : MonoBehaviour
{
    [SerializeField] private NPCInventory npcInventory;
    private GameObject npcQuestionParent;

    public float lootDuration = 180f; // 3분 후 자동 제거

    private float timer;
    private void Awake()
    {
        npcInventory = GetComponent<NPCInventory>();

        // 씬에서 미리 있는 NPCQuestion 오브젝트 찾아서 부모로 설정 (혹은 외부에서 SetParent 호출 가능)
        npcQuestionParent = GameObject.Find("NPCQuestion");
        if (npcQuestionParent != null)
        {
            transform.SetParent(npcQuestionParent.transform, true);
        }
        else
        {
            Debug.LogWarning("NPCQuestion 부모를 찾지 못했습니다!");
        }
    }
    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
            OnDestroyBody();
    }

    public void OnDestroyBody()
    {
        Destroy(gameObject);
        //PhotonNetwork.Destroy(gameObject);
    }

    public void InitFromPlayer(Player player)
    {
        if (npcInventory == null)
        {
            Debug.LogError("NPCInventory가 없습니다!");
            return;
        }

        npcInventory.Init(100);
        player.Inventory.SendAllItem(npcInventory);
        npcInventory.SyncInventoryNetwork();
    }
}
