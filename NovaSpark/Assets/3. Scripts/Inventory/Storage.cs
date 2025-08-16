using Photon.Pun;
using UnityEngine;

public class Storage : Inventory, IInteractable
{
    [SerializeField] private int _InventorySize = 100; // 후에 상자의 종류에 따른 사이즈 조절로 변경 가능

    private void Awake()
    {
        Init(_InventorySize); // 슬롯 수 설정
    }
    public void OnInteract(CharacterObject owner)
    {
        var playerInventory = owner.GetComponent<PlayerInventory>();

        if (playerInventory == null)
        {
            Debug.LogWarning("[Storage] 상호작용한 객체에 PlayerInventory가 없음");
            return;
        }

        UIManager.Instance.OpenStorageUI(this, playerInventory);
    }

    void IInteractable.GetInteractObjectType(bool isCan)
    {
        Debug.Log($"[Storage] 상호작용 타입: {(isCan ? "Open" : "None")}");
    }
    
    public bool IsCan()
    {
        return true;
    }

    public string PromptText()
    {
        return "창고를 열기";
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
                Debug.LogWarning($"DeserializeSlots: ID {d.itemId}에 해당하는 아이템 없음");
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
