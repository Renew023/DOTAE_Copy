using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

public class NPCInventory : Inventory
{
    public DesignEnums.NPCType npcType;
    public ShopInfo shopInfo;
    public NPC Npc;
    public event Action OnInventoryChanged;

    public override InventoryOwnerType OwnerType => InventoryOwnerType.NPC;

    [SerializeField] private int _InventorySize = 20;

    private void Awake()
    {
        Npc = GetComponent<NPC>();
        Init(_InventorySize);

    }
    private async void Start()
    {
        await DataManager.Instance.WaitUntilLoaded();
        //if (!PhotonNetwork.IsMasterClient) return;
        npcType = Npc.NPCRuntimeData.npcType;
        InitializeInventory();
    }

    public override bool AddItem(Item item, int amount)
    {
        bool result = base.AddItem(item, amount);
        if (result) OnSlotChanged(); // 변경 시 동기화
        return result;
    }

    public void InitializeInventory()
    {
        LoadStartItems();

        switch (npcType)
        {
            case DesignEnums.NPCType.WeaponNPC:
                RestockItems();
                break;
            case DesignEnums.NPCType.PositionNPC:
                RestockItems();
                break;
            case DesignEnums.NPCType.ArmorNPC:
                RestockItems();
                break;
            case DesignEnums.NPCType.MaterialNPC:
                RestockItems();
                break;
        }
    }
    private void LoadStartItems()
    {
        var charData = Npc.NPCRuntimeData;
        if (charData == null)
        {
            return;
        }

        var itemIDs = charData.StartItemIds;
        var itemCounts = charData.StartItemCount;

        for (int i = 0; i < itemIDs.Count; i++)
        {
            int itemId = itemIDs[i];
            int quantity = (i < itemCounts.Count) ? itemCounts[i] : 1;

            Item item = DataManager.Instance.GetItemByID(itemId);

            if (item != null)
                AddItem(item, quantity);
            else
                Debug.LogWarning($"초기 아이템 ID {itemId} 없음");
        }

        gold = UnityEngine.Random.Range(500,1000); // NPC가 가진 돈 초기화

    }

    public void RestockItems() // 특정 일자마다 상점 초기화를 진행해 아이템 넣는 곳에서 불러올 함수
    {
        ClearAllSlots();

        if (shopInfo == null)
        {
            Debug.Log("shopInfo가 null입니다.");
            return;
        }

        if(npcType != shopInfo.type)
        {
            shopInfo.type = npcType; // NPC 타입과 일치하도록 설정
        }

        var pool = DataManager.Instance.GetItemsByType(shopInfo.type);
        var shuffledPool = pool.OrderBy(x => UnityEngine.Random.value).ToList();

        int totalPrice = 0;
        int tries = 0;
        int maxTries = 1000; // 무한루프 방지용 제한

        while (totalPrice < shopInfo.maxTotalValue && tries < maxTries)
        {
            var item = shuffledPool[UnityEngine.Random.Range(0, shuffledPool.Count)];
            int price = item.price;
            // 가격을 넘지 않는 경우에만 추가
            if (totalPrice + price <= shopInfo.maxTotalValue)
            {
                AddItem(item, 1);
                totalPrice += price;
            }
            tries++;
        }
        OnSlotChanged();
    }

    // 슬롯들을 SerializeSlotData 배열로 변환 (직렬화)
    public SerializeSlotData[] SerializeSlots()
    {
        var serialized = new SerializeSlotData[_slots.Count];
        for (int i = 0; i < _slots.Count; i++)
        {
            serialized[i] = new SerializeSlotData(_slots[i]);
        }
        return serialized;
    }

    // SerializeSlotData 배열을 받아 슬롯 데이터를 역직렬화
    public void DeserializeSlots(SerializeSlotData[] data)
    {
        if (data == null || data.Length != _slots.Count)
        {
            Debug.LogWarning("DeserializeSlots: 슬롯 데이터 크기 불일치");
            return;
        }

        for (int i = 0; i < _slots.Count; i++)
        {
            var d = data[i];
            if (d.itemId == 0 || d.quantity <= 0)
            {
                _slots[i].Clear();
                continue;
            }

            var item = DataManager.Instance.GetItemByID(d.itemId);
            if (item == null)
            {
                _slots[i].Clear();
                continue;
            }

            var clone = item.Clone();
            if (clone is EquipmentItem eq)
                eq.enhanceLevel = d.enhanceLevel;

            _slots[i].item = clone;
            _slots[i].quantity = d.quantity;
            _slots[i].itemName = clone.name_kr;
        }
    }

    // RPC 호출용 함수 - 네트워크 상에 슬롯 정보를 동기화
    [PunRPC]
    public void RPC_SyncInventory(string json)
    {

        var wrapper = JsonUtility.FromJson<SerializeSlotDataList>(json);
        if (wrapper == null || wrapper.slots == null)
        {
            Debug.LogWarning("RPC_SyncInventory: 역직렬화 실패");
            return;
        }

        var slotData = wrapper.ToArray();
        DeserializeSlots(slotData);
        OnInventoryChanged?.Invoke();
    }

    // 슬롯 데이터 동기화 요청 (보통 MasterClient 또는 소유자에서 호출)
    public override void SyncInventoryNetwork()
    {
        if (photonView == null)
        {
            Debug.LogWarning("SyncInventoryNetwork: photonView가 없음");
            return;
        }
        var serializedSlots = SerializeSlots(); // SerializeSlotData[]
        var wrapper = new SerializeSlotDataList(serializedSlots);
        string json = JsonUtility.ToJson(wrapper);

        //photonView.RPC(nameof(RPC_SyncInventory), RpcTarget.OthersBuffered, json);
    }

    // 슬롯 변경이 있을 때마다 호출하는 예시
    protected virtual void OnSlotChanged()
    {
        OnInventoryChanged?.Invoke();
        SyncInventoryNetwork();
    }

    public override void SendAllItem(IInventory targetInventory)
    {
        if (targetInventory == null)
        {
            Debug.LogError("targetInventory is NULL!!");
            return;
        }

        if (targetInventory is PlayerInventory playerInv)
        {
            float currentWeight = playerInv.GetTotalWeight();
            float maxWeight = playerInv.PlayerMaxForce;

            var targetSlots = targetInventory.GetSlots();

            for (int i = 0; i < _slots.Count; i++)
            {
                var fromSlot = _slots[i];
                if (fromSlot.IsEmpty)
                    continue;

                float itemWeight = fromSlot.item.weight * fromSlot.quantity;

                // 1. 같은 아이템 병합 가능하면 우선 병합
                foreach (var targetSlot in targetSlots)
                {
                    if (!targetSlot.IsEmpty &&
                        targetSlot.item.id == fromSlot.item.id &&
                        fromSlot.item.isStackable)
                    {
                        if (currentWeight + itemWeight > maxWeight)
                        {
                            Debug.LogWarning("무게 초과로 전송 중단");
                            return;
                        }

                        targetSlot.quantity += fromSlot.quantity;
                        fromSlot.Clear();
                        currentWeight += itemWeight;
                        goto NextSlot;
                    }
                }

                // 2. 빈 슬롯에 넣기
                foreach (var targetSlot in targetSlots)
                {
                    if (targetSlot.IsEmpty)
                    {
                        if (currentWeight + itemWeight > maxWeight)
                        {
                            Debug.LogWarning("무게 초과로 전송 중단");
                            return;
                        }

                        targetSlot.item = fromSlot.item;
                        targetSlot.quantity = fromSlot.quantity;
                        targetSlot.itemName = fromSlot.item.name_kr;
                        fromSlot.Clear();
                        currentWeight += itemWeight;
                        goto NextSlot;
                    }
                }

                Debug.LogWarning("타겟 인벤토리에 공간 부족");
                return;

            NextSlot:
                continue;
            }
        }
        else
        {
            // 플레이어가 아닌 경우는 그냥 원래처럼 전송
            base.SendAllItem(targetInventory);
        }
    }
    public override void MergeSameItem(IInventory targetInventory)
    {
        if (targetInventory is PlayerInventory playerInv)
        {
            float currentWeight = playerInv.GetTotalWeight();
            float maxWeight = playerInv.PlayerMaxForce;

            foreach (var fromSlot in _slots)
            {
                if (fromSlot.IsEmpty)
                    continue;

                var targetSlots = playerInv.GetSlots();

                foreach (var targetSlot in targetSlots)
                {
                    if (!targetSlot.IsEmpty &&
                        targetSlot.item.id == fromSlot.item.id &&
                        fromSlot.item.isStackable)
                    {
                        float addedWeight = fromSlot.item.weight * fromSlot.quantity;

                        // 무게 초과 시: 이번 병합만 skip, 나머지는 계속 진행
                        if (currentWeight + addedWeight > maxWeight)
                        {
                            Debug.LogWarning($"[{fromSlot.item.name_kr}] 병합 불가: 무게 초과");
                            break;
                        }

                        targetSlot.quantity += fromSlot.quantity;
                        currentWeight += addedWeight;
                        fromSlot.Clear();
                        break;
                    }
                }
            }
        }
        else
        {
            base.MergeSameItem(targetInventory); // 기본 병합 로직
        }
    }
}
