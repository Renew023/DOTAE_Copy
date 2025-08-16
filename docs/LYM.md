# NovaSpark
이영민 개발범위 - 스킬트리, 스킬 해금 시스템 / 인벤토리 시스템(플레이어, NPC, 창고) / 장비창, 장비장착 / 제작 시스템 /
		거래 등 아이템 이동시 분기처리 / 소비아이템, 버프아이템 사용 / 플레이어 사망시 아이템드랍 시스템 / 
      		캐릭터 설정 및 시나리오 작성 - 주인공 가일(언데드기사)

# 🧠 스킬 트리 시스템

## SkillData

각 스킬의 정적 정보 (이름, 필요 레벨, 선행 스킬, 필요 숙련도, 최대 레벨, 기본 데미지 등)를 저장하는 스크립트
현재는 ScriptableObject를 이용해 정적 정보를 관리한다.
이후 파싱툴을 이용한 엑셀에서의 데이터관리 체계가 이뤄지면 연결될 예정

## PlayerSkillState

플레이어의 현재 스킬 포인트 투자 상태를 관리

내부 구조:

`Dictionary<string, int> investedPoints;` // 스킬 ID → 투자 포인트
`HashSet<string> unlockedSkills; `        // 해금된 스킬 ID

####해금을 관리하는 bool 메서드 `InvestPoints와 UnlockSkill(data)`
```
 public bool InvestPoints(SkillData data)
 {
     string id = data.skillId;

     if (!investedPoints.ContainsKey(id))
         investedPoints[id] = 0;

     int currentLevel = investedPoints[id];
     if (currentLevel >= data.maxSkillLevel)
         return false; // 최대 레벨 도달

     investedPoints[id] = currentLevel + 1;

     if (currentLevel == 0) // 첫 투자면 해금 처리
         UnlockSkill(id);
     Debug.Log($"[TryInvestPoints] 현재 스킬 레벨({data.skillName}): {currentLevel+1}");
     return true;
 }
```
## SkillTreeManager

해금 조건 판단, 포인트 투자 등 스킬 트리의 핵심 로직을 처리

플레이어 레벨, 숙련도, 보유 스킬포인트 기반으로 해금 가능 여부를 판단

투자 시 `PlayerSkillState`를 자동 갱신

스킬 해금 가능 여부를 체크하는 bool 메서드 `CanUnlockSkill(skill)`
```
public bool CanUnlockSkill(SkillData skill)
{
    // 선행 스킬 해금 여부 체크
    foreach (var preSkill in skill.prerequisiteSkills)
    {
        if (!playerSkillState.IsSkillUnlocked(preSkill.skillId))
            return false;
    }

    // 레벨 체크
    if(playerLevel < skill.requiredPlayerLevel)
        return false;

    // 숙련도 체크 TODO : 플레이어 만든 후 숙련도 세분화 필요
    if(playerProficiency < skill.requiredProficiency)
        return false;

    return true;
}
```
UI의 버튼에서 호출시킬 스킬 포인트 투자 시도에 들어가는 bool 메서드 `TryInvestPoints(skill)`
```
public bool TryInvestPoints(SkillData skill)
{
    Debug.Log($"[TryInvestPoints] 남은 스킬 포인트: {availableSkillPoints}");
    
    if (availableSkillPoints <= 0)
    {
        Debug.Log("[TryInvestPoints] 포인트 부족");
        return false; // 포인트 없음
    }

    if (!CanUnlockSkill(skill))
    {
        Debug.Log($"[TryInvestPoints] 해금 조건 미충족: {skill.skillName}");
        return false; // 해금조건
    }

    int currentPoint = playerSkillState.GetSkillLevel(skill.skillId);
   
    if (currentPoint >= skill.maxSkillLevel)
    {
        if (currentPoint >= skill.maxSkillLevel)
        {
            Debug.Log("[TryInvestPoints] 최대 레벨 도달");
            return false; // 최대레벨
        }
    }

    bool invested = playerSkillState.InvestPoints(skill);
    if(!invested)
    {
        Debug.Log("[TryInvestPoints] 투자 실패");
        return false;
    }

    availableSkillPoints--;
    Debug.Log($"[TryInvestPoints] 투자 성공! 남은 스킬 포인트: {availableSkillPoints}");
    return invested;
}
```


## 💾 저장 고려 사항
멀티플레이의 가능성을 고려한 구조 설계

각 플레이어마다 독립된 PlayerSkillState 저장

## 🧪 테스트 환경
테스트용 UI, 플레이어 객체, 임시 스킬 3종 구성

플레이어 초기 상태:

레벨: 10

스킬 포인트: 10

### 🔥 테스트용 스킬 설정
스킬 이름	해금 조건	최대 레벨
FireBall	레벨 ≥ 1, 선행 스킬 없음	1
FireArrow	레벨 ≥ 5, 선행: FireBall	5
FireWall	레벨 ≥ 11, 선행: FireBall	1

### 🖼️ 테스트 UI 

![image](https://github.com/user-attachments/assets/d8e01932-94b7-4346-95d5-60e787d73941)

🔹 FireBall 첫 해금 클릭 (위 - 해금, 아래 - 해금 후 클릭, 최대 레벨에 도달해 포인트 할당이 되지 않음)

![image](https://github.com/user-attachments/assets/3b171970-f14d-475b-b501-eced22c0864a)
![image](https://github.com/user-attachments/assets/bacc7507-b16f-42e4-836a-f675e9591a8a)


🔹 FireBall 해금 후 다시 클릭 (최대 레벨 도달 상태)

![image](https://github.com/user-attachments/assets/e6e85ff3-57cc-4ba6-8977-4c4cc23b724e)


🔹 FireArrow 해금 및 레벨업 테스트 (최대 레벨 5까지 도달)

![image](https://github.com/user-attachments/assets/2895f2e6-7aaa-4927-9c3b-c23300b907b2)


### ✅ 정리 포인트
스킬 해금은 레벨, 선행 스킬, 숙련도 조건을 모두 고려

투자한 포인트는 `Dictionary<string, int>`로 관리하여 스킬별 레벨 추적

추후 저장/불러오기, 멀티플레이 대응을 위한 구조적 준비 완료

--------------------
# 📦 인벤토리 시스템
개요
이 인벤토리 시스템은 플레이어, NPC, 창고 등 다양한 주체가 사용할 수 있는 **공통 기반 클래스(Inventory)**를 중심으로 설계되었습니다.
모든 인벤토리는 **슬롯 리스트(List<Slot>)**를 사용하며, 아이템 이동, 정렬, 직렬화, 멀티플레이 동기화까지 지원합니다.

또한, 드래그 앤 드롭, 무게 제한, 거래 로직을 OnDrop 이벤트 기반으로 처리하며, 중첩 아이템의 경우 수량 입력 UI를 통해 개별 수량 조정이 가능합니다.

Ondrop이벤트가 발생한 후의 계산은 TradeManager를 통해서 합니다.

클래스 구조

1. Inventory (기반 클래스)
모든 인벤토리의 기본 동작을 정의.

슬롯 초기화, 아이템 추가/삭제, 정렬, 전송 기능을 포함.

무게 계산 및 네트워크 동기화를 위한 인터페이스 제공.

주요 메서드

메서드	설명
`Init(int size)`		지정한 슬롯 개수만큼 빈 슬롯 생성
`AddItem(Item item, int amount)`중첩 가능한 경우 합치거나 빈 슬롯에 추가
`SortItem()`			같은 아이템 병합 후 타입/이름 순으로 정렬
`SendAllItem(IInventory target)`모든 아이템을 대상 인벤토리로 전송
`MergeSameItem(IInventory target)`대상과 동일한 아이템만 병합 전송
`GetTotalWeight()`		현재 인벤토리 무게 합계 계산
`RemoveItem(Item item, int amount)`지정 아이템 수량만큼 제거

2. NPCInventory
상점 NPC 또는 특정 프리셋 아이템을 가진 NPC 전용 인벤토리.

랜덤 재입고(RestockItems) 및 NPC 타입별 아이템 세팅 지원.

네트워크 동기화를 위한 슬롯 직렬화/역직렬화 기능 포함.

추가 기능

메서드	설명
`RestockItems()`	상점 타입/가격 제한/아이템 수 제한에 맞춰 랜덤 재입고
`SerializeSlots()`	슬롯 데이터를 직렬화 객체로 변환
`DeserializeSlots(data)` 직렬화 데이터를 다시 슬롯 상태로 복원
`RPC_SyncInventory(json)`네트워크 RPC를 통한 슬롯 동기화

3. StorageInventory
플레이어가 대량의 아이템을 보관할 수 있는 창고 전용 인벤토리.

플레이어와 상호작용 시 UI를 통해 아이템 입출고 가능.

아이템의 초기화 타이밍 지연 - 데이터 로드 전 아이템을 넣어주려고 해서 리스트 내에 아이템이 없는 것으로 됨
```
private async void Start()
{
    await DataManager.Instance.WaitUntilLoaded(); // 데이터 완전 로드 대기
    InitializeInventory();     // NPCType 에 따라 Restock / Preset
}
```

# 💱 TradeManager – 인벤토리 간 거래 처리 전담 클래스
TradeManager는 인벤토리 간 아이템 이동 및 거래 로직을 통합 관리하는 전담 클래스입니다.
원래는 `InventoryItemSlot.OnDrop()` 안에 복잡하게 들어있던 판매/구매/이동 로직을 전부 분리하여 코드의 가독성과 유지보수성을 향상시켰습니다.

주요 기능

`GetMaxSellAmount(item, playerQty, shopGold, unitSellPrice)`
플레이어가 가진 아이템 수량과 상점 골드를 기반으로 판매 가능한 최대 수량을 계산
스택 불가능 아이템 또는 가격이 0 이하일 때는 1개만 가능

`GetMaxBuyAmount(item, npcQty, playerGold, playerFreeWeight, unitBuyPrice)`
NPC 상점 재고, 플레이어 골드, 플레이어 무게 공간을 고려해 구매 가능한 최대 수량 산출

`GetMaxStorageTransferAmount(item, sourceQty, playerFreeWeight)`
창고 재고와 플레이어 무게 공간에 따라 창고에서 플레이어로 이동 가능한 최대 수량 산출

`DoSell(player, shop, item, amount)`
플레이어 인벤토리에서 아이템 제거 후 상점에 추가
플레이어 골드 증가, 상점 골드 차감
상점 골드 부족 시 거래 취소

`DoBuy(shop, player, item, amount)`
상점 인벤토리에서 아이템 제거 후 플레이어 인벤토리에 추가
플레이어 골드 차감, 상점 골드 증가
플레이어 무게 초과나 골드 부족 시 거래 취소

`DoStorageTransfer(storage, player, item, amount)`
창고에서 플레이어 인벤토리로 아이템 이동 처리
플레이어 무게 초과 시 거래 취소

# 🔢 QuantitySelectUI
스택 가능한 아이템을 거래/이동할 때 수량을 입력하는 UI입니다.

무게, 소지골드와 아이템 개당 가격을 계산하여 최대로 거래/이동 할 수 있는 최대 수량을 표시합니다

동작 과정

`OnDrop()` 시 스택 가능 아이템이면 QuantitySelectUI 활성화

최대수량보다 작은 숫자를 입력 (큰 숫자를 입력시 최대 수량으로 수정)

확인 버튼을 통해 `OnConfirmButton()` 호출

`OnConfirmButton()` 내의 onConfirm 이벤트 활성화

```
public void OnConfirmButton()
{
    Debug.Log("확인 버튼 눌림");

    if (int.TryParse(numberInput.text, out int value))
    {
        Debug.Log($"입력된 값: {value}, 최대값: {maxValue}");

        if (value < 1) value = 1;
        if (value > maxValue) value = maxValue;

        if (onConfirm != null)
        {
            Debug.Log("onConfirm 실행됨");
            onConfirm?.Invoke(value);
        }
        else
        {
            Debug.LogWarning("onConfirm이 null입니다.");
        }

        HidePanel();
    }
    else
    {
        Debug.LogWarning("숫자 입력이 올바르지 않습니다.");
    }
}
```

# ⚖ 무게 시스템
각 Item에 weight 속성 추가

`Inventory.GetTotalWeight()`에서 현재 무게를 계산

무게 제한(maxWeight) 초과 시 아이템 추가 불가

구매/이동 시 무게 제한 체크 로직 추가

```
public virtual float GetTotalWeight()
{
    float totalWeight = 0f;
    foreach (var slot in GetSlots())
    {
        if (!slot.IsEmpty && slot.item != null)
        {
            totalWeight += slot.item.weight * slot.quantity;
        }
    }
    return totalWeight;
}
```

## NPCInventory와 Storage의 멀티동기화

# SerializeSlotData & SerializeSlotDataList

인벤토리 슬롯 내의 아이템을 직렬화하기 위한 클래스

생성자 - 아이템의 아이디와 변동가능한 변수를 저장
```
public SerializeSlotData(Slot slot)
{
    if (slot == null || slot.IsEmpty || slot.item == null)
    {
        itemId = 0;
        quantity = 0;
        enhanceLevel = 0;
        return;
    }

    itemId = slot.item.id;
    quantity = slot.quantity;
    // 강화 레벨 같은 커스텀 데이터는 아이템에 따라 다르게 처리
    if (slot.item is EquipmentItem eq)
        enhanceLevel = eq.enhanceLevel;
    else
        enhanceLevel = 0;
}
```

# 포톤 연결용 데이터 직렬화와 RPC함수 

아이템 슬롯의 정보를 바꿀만한 로직엔 `SyncInventoryNetwork();`를 호출해주어야한다.

슬롯들을 SerializeSlotData 배열로 변환 (직렬화)
```
 public SerializeSlotData[] SerializeSlots()
 {
     var serialized = new SerializeSlotData[_slots.Count];
     for (int i = 0; i < _slots.Count; i++)
     {
         serialized[i] = new SerializeSlotData(_slots[i]);
     }
     return serialized;
 }
```

SerializeSlotData 배열을 받아 슬롯 데이터를 역직렬화
```
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
```

RPC 호출용 함수 - 네트워크 상에 슬롯 정보를 동기화
```
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
```

슬롯 데이터 동기화 요청
```

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

     photonView.RPC(nameof(RPC_SyncInventory), RpcTarget.OthersBuffered, json);
 }
```
## 인벤토 전체 흐름도
아이템 이동/거래 시

OnDrop (슬롯) 
    ↓
TradeManager.ProcessTrade(...)
    ↓
수량 선택 필요 시 QuantitySelectUI 호출
    ↓
거래/이동 수행 (무게, 금액 검사)
    ↓
인벤토리 변경 → SyncInventoryNetwork()
    ↓
SerializeSlots() → JSON 변환
    ↓
Photon RPC → 다른 클라이언트 RPC_SyncInventory(json)
    ↓
DeserializeSlots() → 슬롯 복원

--------------------------------------
## 🦾 Equipment System – 장비 착용 및 무기 프리팹 생성
`EquipmentInventory`는 플레이어가 착용하는 장비 슬롯을 관리하는 전용 인벤토리입니다.
부위별(`EquipmentType`)로 1개의 장비만 착용 가능하며, 착용 시 해당 장비의 스탯이 플레이어에게 즉시 반영됩니다.
무기 장착 시에는 Addressables를 통해 무기 프리팹을 동적으로 로드하여 캐릭터 모델에 부착합니다.

# EquipmentInventory

`Dictionary<EquipmentType, Slot>` 구조로 각 부위별 슬롯 보유

장비 착용(`Equip`) / 해제(`Unequip`) 기능 제공

플레이어 스탯 반영 및 무기 프리팹 생성 호출

# EquipmentSlot

UI 슬롯으로, 해당 슬롯이 담당하는 부위(`slotType`) 지정

드래그 앤 드롭(`OnDrop`) 이벤트를 통해 인벤토리에서 장비 장착

장비 해제 시 `RequestUnequip()` 호출

# Player

장비 스탯 적용 및 해제

무기 장착 시 Addressables로 프리팹 로드 후 소켓에 부착

무기 해제 시 생성된 모든 무기 오브젝트 제거

# 주요 메서드
1. 장비 착용 – `EquipmentInventory.Equip(EquipmentItem item`)
해당 부위에 이미 장비가 있으면 자동으로 해제
슬롯에 아이템 등록 후 `Player.Equip(item)` 호출

2. 장비 해제 – `EquipmentInventory.Unequip(EquipmentType type)`
장착된 장비의 스탯 제거 및 무기 오브젝트 정리
아이템을 플레이어 인벤토리로 반환

3. 스탯 적용 & 무기 프리팹 생성 – `Player.Equip(EquipmentItem item)`
- 장비 유효성 검증 및 스탯 적용
`item.GetStatPairs()`를 호출해 장비가 가진 능력치 쌍(스탯 타입과 값)을 얻습니다.
각 능력치를 `characterRuntimeData.AddStat(type, value)`로 플레이어 데이터에 추가합니다.

-장착한 무기가 있는 상태에서 무기 장착시 기존 장비 제거
만약 새로 장착하는 장비가 무기(Weapon) 타입이면, 이전에 장착된 무기 프리팹들을 모두 `ClearAllWeapons()`를 통해 삭제합니다.

-Addressables에서 무기 프리팹 비동기 로드
장비에 연결된 프리팹 이름(prefabName)을 기준으로 
`Addressables.LoadAssetAsync<GameObject>(prefabName)` 호출.
`await`를 통해 비동기 로드 완료까지 대기합니다.
로드 실패 시 에러 로그 출력.

-무기 프리팹 3종 소켓 위치에 인스턴스 생성
`TrySpawn()` 메서드를 통해 미리 지정된 오른손 소켓(Forward, Side, Back)에 각각 프리팹을 인스턴스화합니다.
인스턴스는 부모를 소켓 트랜스폼으로 지정하여 위치와 회전을 초기화합니다.
로드 핸들 해제
메모리 관리를 위해 Addressables 로드 핸들을 `Addressables.Release(handle)`로 해제합니다.
이미 인스턴스화된 게임오브젝트들은 씬에 유지됩니다.

-장비 해제 시 프리팹 제거
`Player.UnEquip(EquipmentItem item)` 메서드에서 스탯을 제거하고 무기 타입이면 `ClearAllWeapons()`를 호출해 무기 프리팹을 삭제합니다.


# 🛠️ 제작 시스템 (Crafting System)

## Craft (작업대 컴포넌트)
작업대 오브젝트에 붙는 실제 제작 로직을 담당하는 컴포넌트.

주요 기능:
플레이어와 연결된 제작 가능 레시피 목록 조회

재료 충분 여부 판단 CanCraft()

재료 소모 및 결과 아이템 지급 TryCraft()

대표 메서드

CanCraft: 재료가 충분한지 확인
```
public bool CanCraft(EquipmentItem item, PlayerInventory player)
{
    for (int i = 0; i < item.producedMaterial.Count; i++)
    {
        int materialId = item.producedMaterial[i];
        int requiredQty = item.requiredQuantity[i];

        if (!player.HasItem(materialId, requiredQty))
            return false;
    }
    return true;
}
```

TryCraft: 실제 제작 시도 (재료 소모 → 결과 지급)
```
public bool TryCraft(EquipmentItem item, PlayerInventory player)
{
    if (!CanCraft(item, player)) return false;

    for (int i = 0; i < item.producedMaterial.Count; i++)
    {
        int materialId = item.producedMaterial[i];
        int requiredQty = item.requiredQuantity[i];
        player.ConsumeItem(materialId, requiredQty);
    }

    player.AddItem(item, 1);
    Debug.Log($"[제작 성공] {item.name_kr}");
    return true;
}
```

## CraftUI
플레이어가 작업대 UI를 통해 실제로 제작하는 UI 관리 컴포넌트.

슬롯 프리팹을 생성하여 CraftSlotUI로 구성

클릭된 아이템의 재료를 확인하고 제작 버튼 활성화

CraftButton을 통해 TryCraft() 호출

대표 메서드
Init: UI 초기화
```
public void Init(Craft craft, PlayerInventory playerInventory, PlayerRecipe recipe)
{
    this.craft = craft;
    this.player = playerInventory;

    craft.ConnectPlayer(player, recipe);
    var slots = craft.GetPlayerUnlockedSlots();

    foreach (var slot in slots)
    {
        if (slot.item is EquipmentItem equipment)
        {
            var go = Instantiate(slotPrefab, slotParent);
            var slotUI = go.GetComponent<CraftSlotUI>();
            slotUI.Setup(equipment, OnSlotClicked);
        }
    }

    craftButton.onClick.AddListener(OnCraftButtonClicked);
}
```

OnSlotClicked: 선택한 제작 아이템 정보 출력
```
private void OnSlotClicked(Item item)
{
    if (item is not EquipmentItem equipment) return;

    selectedItem = equipment;
    itemNameText.text = equipment.name_kr;
    descriptionText.text = equipment.description;

    for (int i = 0; i < materialTexts.Length; i++)
    {
        if (i < equipment.producedMaterial.Count)
        {
            int id = equipment.producedMaterial[i];
            int required = equipment.requiredQuantity[i];
            int owned = player.CountItem(id);

            var materialData = DataManager.Instance.GetMaterialItem(id);
            materialTexts[i].text = $"{materialData.name_kr}: {owned} / {required}";
            materialTexts[i].color = (owned >= required) ? Color.white : Color.red;
        }
    }

    craftButton.interactable = craft.CanCraft(equipment, player);
}
```

# CraftSlotUI
제작 가능한 아이템을 표시하는 UI 슬롯 구성 요소

IPointerClickHandler를 구현하여 클릭 시 선택 이벤트 발생

아이템 이름 및 아이콘 출력, 아이템 정보 저장

SetUp : 받아온 아이템의 정보 세팅
```
public void Setup(Item item, System.Action<EquipmentItem> onClick)
{
    this.item = item;
    this.onClick = onClick;
    _icon.enabled = true;
    Debug.Log($"[CraftSlotUI] Setup 호출: {item?.name_kr}");
    _nameText.text = item.name_kr;
    // 텍스트 등 UI 갱신 코드

    Debug.Log($"[CraftSlotUI] 슬롯 세팅 완료: {item.name_kr}");
}
```

# PlayerRecipe
플레이어가 해금한 제작 레시피 ID를 관리하는 컴포넌트

GetUnlockedRecipeIds()로 해금된 아이템 ID 목록 반환

Unlock(id)로 특정 아이템의 제작을 해금

```
   [SerializeField] private List<int> unlockedRecipeIds = new();

    /// <summary>
    /// 새 레시피 해금
    /// </summary>
    public void Unlock(int id)
    {
        if (!unlockedRecipeIds.Contains(id))
            unlockedRecipeIds.Add(id);
    }

    /// <summary>
    /// 현재 해금된 레시피들의 ID 목록을 반환
    /// </summary>
    public List<int> GetUnlockedRecipeIds()
    {
        return unlockedRecipeIds.ToList();
    }
}

```
# ✅ 제작 시스템 정리
해금된 아이템만 제작 가능 (PlayerRecipe)

인벤토리에 충분한 재료가 있을 때만 제작 가능

재료는 아이템 데이터 내에서 개별 재료 ID와 수량 리스트로 관리

UI에서 재료 보유 현황 및 제작 가능 여부를 실시간으로 표시

제작 성공 시 재료 차감 및 결과 아이템 지급

# 🖼 AddressableManager 아이콘 로드
슬롯에 아이템 아이콘을 표시하기 위해 Addressables를 사용합니다.

아이콘은 아이템 데이터의 iconKey를 기반으로 로드
Addressables 캐싱으로 성능 최적화

```
public async void LoadIconToImage(
    Image targetImage,
    string iconKey,
    int version,
    Func<int> getCurrentVersion,
    bool setNativeSize = true)
{
    if (string.IsNullOrEmpty(iconKey))
    {
        targetImage.enabled = false;
        targetImage.sprite = null;
        return;
    }

    targetImage.enabled = false;
    targetImage.sprite = null;

    var sprite = await LoadIcon(iconKey);
    if (version != getCurrentVersion())
        return;

    if (sprite != null)
    {
        targetImage.sprite = sprite;
        targetImage.enabled = true;
        if (setNativeSize && targetImage.rectTransform.rect.size.sqrMagnitude == 0)
            targetImage.SetNativeSize();
    }
    else
    {
        targetImage.enabled = false;
    }
}
```

---

## ⚰️ 플레이어 사망 시 – 시체 생성 및 아이템 전달
플레이어가 사망하면 해당 위치에 시체 오브젝트를 생성하고, 플레이어 인벤토리에 있던 모든 아이템을 시체 인벤토리로 이동시킵니다.
이 시체 인벤토리는 다른 플레이어나 본인이 다시 접근하여 아이템을 회수할 수 있습니다.

# DeadBodyController
사망시 생성될 시체를 컨트롤하는 스크립트
초기화 및 삭제시간 설정 가능

주요 매서드 `InitFromPlayer(Player player)`
```
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
```
플레이어인벤토리의 SendAllItem 재사용


동작 과정
1.사망 감지
플레이어 HP가 0 이하가 되면 Player의 OnPlayerDeath(player) 호출.

2.시체 생성
프리팹(시체 모델)을 Instantiate하여 플레이어 위치에 배치
시체에 Inventory 컴포넌트 부착

3.아이템 이동
플레이어 인벤토리 → 시체 인벤토리로 모든 아이템 전송

4.회수 로직
다른 플레이어 또는 본인이 시체와 상호작용 시 인벤토리 UI를 열어 아이템 회수 가능
시체는 설정한 시간이 지난 후 자동으로 삭제

---

## 🍖 소비아이템 & 버프아이템 사용
소비아이템은 즉시 효과와 **지속 효과(버프)**를 모두 지원합니다.
아이템 사용 시 ConsumableItem.Use()가 호출되며, 효과 타입(EffectType)에 따라 즉시 효과 또는 버프가 적용됩니다.

📌 주요 클래스
클래스			역할
ConsumableItem		소비아이템 데이터 및 사용 로직 정의
Buff			버프의 효과 타입, 수치, 지속시간을 저장
BuffEffectTypeConverter	버프 효과 타입(EffectType)을 스탯 타입(StatType)으로 변환
PlayerBuffData		플레이어에게 적용된 버프를 관리하고 시간 경과에 따라 해제
InventoryItemSlot (UI)	우클릭 시 아이템 사용 처리

⚡ 즉시 효과
ConsumableItem.Use()에서 effectType을 순회하며 효과 타입에 따라 즉시 적용합니다.
Heal → HP 회복
thirsty → 목마름 수치 회복
hunger → 배고픔 수치 회복
UnlockRecipe → 제작 레시피 해금
기타 커스텀 효과 타입 확장 가능

⏳ 지속 효과(버프)
duration 값이 0보다 큰 효과는 버프로 등록됩니다.
Buff 객체 생성 후 PlayerBuffData.AddBuff() 호출
BuffEffectTypeConverter를 통해 스탯 타입 변환 후 해당 스탯을 즉시 증가시킵니다.
PlayerBuffData.UpdateBuff()에서 매 프레임 지속 시간을 감소시키며, 0 이하가 되면 효과 제거

소비아이템 (Consumable)
즉시 효과 적용 (예: HP 회복, MP 회복)
사용 후 수량 감소
스택 수량이 0이 되면 슬롯 비움

💻 사용 절차 (UI 기준)
인벤토리 UI에서 우클릭
OnPointerClick()에서 Use() 호출
즉시 효과 적용
ConsumableItem.Use() 내부에서 HP/목마름/배고픔/레시피 해금 처리
버프 적용
지속 시간이 있는 효과는 PlayerBuffData에 추가
소모 처리
스택 가능 → 수량 1 감소, 0이면 슬롯 비움
스택 불가 → 즉시 슬롯 비움
UI 갱신
슬롯 Refresh 및 무게 표시 업데이트

## 퀘스트 시나리오 및 플레이어 캐릭터 서사

이름 : 가일
나이／성별	: 29살 / 남
외모 :	190cm, 넓은 어깨, 굵은 뼈, 기사의 복장을 갖추고있다. 
성격 :	언데드로서 산 자에 대한 적대감을 가지고있다. 생전에 꽤나 강한 기사였던 탓에 강해지는 것에 대한 갈망이 크고 전투에 대한 두려움         이 전혀 없다. 이것저것 저울질 할 시간에 그냥 때려부수는걸 선호한다.
캐릭터 :	높은 체력과 지치지 않는 스테미나를 가지고 있다. 마법에 대한 적성이 낮고 모든 냉병기엔 숙달되어있다. 

배경 스토리

"부활 전 : 인간과 언데드의 전쟁, 참전한 기사 가일
대륙을 뒤흔든 대전쟁.
살아있는 자들의 연합군은 ‘언데드는 자연에 반하는 재앙’이라 규정하고 성전을 선포한다.
젊은 기사 가일 블레이크는 충성과 정의를 믿으며 그 전쟁에 참전한다.
그는 최전선에서 수많은 스켈레톤과 좀비를 파괴했고, 그때마다 이렇게 되뇌었다.
“저들은 이미 죽은 자. 무의미한 껍데기일 뿐.”
하지만 한 마을에서 전투에서, 가일은 네크로맨서의 죽음의 파장에 휘말려 전사한다."

"부활: 무의지의 스켈레톤
그의 정신은 깨어난다.
하지만 몸은 식었고, 감각은 없다.
거울조차 볼 수 없는 두개골, 썩은 갑옷, 그리고 자신이 파괴했던 것들과 똑같은 스켈레톤이 되어 있었다.

그리고 머릿속엔 단 하나의 목소리만이 맴돌았다.

“전진하라. 명령에 복종하라.”

가일은 더 이상 생각하지 못했고, 자신이 누구였는지도 몰랐다.
단지 죽음의 군대 속 한 조각일 뿐이었다."

진영 :	언데드 왕국(가제)
	
서사	: 인간으로서의 삶은 끝났고 다른 선택지는 없다. 그렇다면 차라리 네크로맨서 따위에게 휘둘리는 저급한 스켈레톤에서 벗어나 언데드 위에 군림하겠다. - 네크로맨서의 명령에 따라 움직이던 스켈레톤은 여러 사건을 겪으며 자아를 되찾고 네크로맨서의 명령으로부터 자유로워진다.
	
	









