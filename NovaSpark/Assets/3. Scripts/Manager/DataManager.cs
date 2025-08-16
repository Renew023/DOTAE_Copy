using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

public class DataManager : Singleton<DataManager>
{
    private AddressableManager _addressable;
    public int progressCount = 0;
    public int progressTotal = 19;
    public Dictionary<int, Dictionary<int, NPCDialog>> NpcDialogueById { get; private set; } = new();
    public Dictionary<int, NPCDialog> DialogById { get; private set; } = new();


    public bool IsLoaded { get; private set; } = false; // 데이터가 로드되었는지 여부
    public List<SlotData> SlotDataList { get; private set; }

    private Dictionary<int, Equipment> _equipmentById = new();
    public IReadOnlyList<Equipment> AllEquipments { get; private set; } // 전체 목록 순회용
    private Dictionary<int, EquipmentItem> _equipmentItemById = new();
    public IReadOnlyList<EquipmentItem> AllEquipmentItems { get; private set; } // 전체 목록 순회용
    private Dictionary<int, Consumable> _consumableById = new();
    public IReadOnlyList<Consumable> AllConsumable { get; private set; }
    private Dictionary<int, ConsumableItem> _consumableItemById = new();
    public IReadOnlyList<ConsumableItem> AllConsumableItems { get; private set; }
    private Dictionary<int, Material> _materialById = new();
    public IReadOnlyList<Material> AllMaterial { get; private set; }
    private Dictionary<int, MaterialItem> _materialItemById = new();
    public IReadOnlyList<MaterialItem> AllMaterialItems { get; private set; }

    //-------------------------------------------------s
    //public Dictionary<int, CharacterObject> CharacterObjectByID { get; private set; } = new(); // 캐릭터 ID로 찾기용

    //TODO : Character
    public Dictionary<int, CharacterData> CharacterDataByID { get; private set; } = new();

    //TODO : NPC
    public Dictionary<int, NPCData> NPCDataByID { get; private set; } = new();

    //TODO : Blood
    public Dictionary<DesignEnums.BloodType, BloodData> BloodDataByID { get; private set; } = new();

    //TODO : Enemy
    public Dictionary<int, EnemyData> EnemyDataByID { get; private set; } = new();

    //TODO : Player
    public Dictionary<int, PlayerData> PlayerDataByID { get; private set; } = new();

    public Dictionary<string, Sprite> SpriteDict = new();

    public Dictionary<int, FarmingData> farmingData { get; private set; } = new();
    //TODO : Skill
    public Dictionary<int, SkillData> SkillDataByID { get; private set; } = new();

    //TODO : 캐릭터 선택창을 만들어서 보이게 하는 효과를 만들고자 한다면,
    //마찬가지로 보일 Sprite를 int와 연결해야할 듯.

    //public Dictionary<int, UnitStatBase> UnitStatBaseByID { get; private set; } = new(); // 유닛 스탯 베이스 ID로 찾기용
    //public Dictionary<int, UnitStatMax> UnitStatMaxByID { get; private set; } = new(); // 유닛 스탯 최대치 ID로 찾기용
    //public Dictionary<int, TecBase> TecBaseByID { get; private set; } = new(); // 기술 베이스 ID로 찾기용
    //public Dictionary<int, TecMax> TecMaxByID { get; private set; } = new(); // 기술 최대치 ID로 찾기용
    //public Dictionary<int, PartData> PartDataByID { get; private set; } = new(); // 파트 데이터 ID로 찾기용
    //public Dictionary<int, UnitStatWeight> UnitStatWeightByID { get; private set; } = new(); // 유닛 스탯 웨이트 ID로 찾기용
    //public Dictionary<int, List<LevelInfo>> LevelInfoByID { get; private set; } = new(); // 레벨 정보 ID로 찾기용
    //public Dictionary<int, List<TecLevelInfo>> TecLevelInfoByID { get; private set; } = new(); // 기술 레벨 정보 ID로 찾기용

    public List<CharacterObject> TestCharacterView;

    // TODO: 다른 데이터 딕셔너리/리스트도 여기에 추가 예정

    protected override void Awake()
    {
        DontDestroyOnLoad(this);
        base.Awake();
    }

    private void Start()
    {
        _addressable = AddressableManager.Instance;
        //LoadAllDataAsync();
        //_ = LoadAllDataAsync();
    }

    // public async void Init(AddressableManager addressable)
    // {
    //     _addressable = addressable;
    //     await LoadAllDataAsync();
    // }

    public async Task LoadAllDataAsync()
    {
        // LoadCharacterData();
        if (IsLoaded) return;
        await LoadSlotDataAsync();
        await LoadEquipmentDataAsync();
        await LoadConsumableDataAsync();
        await LoadDialogueDataAsync();
        await LoadMaterialDataAsync();
        //TODO : Character
        await LoadCharacterDataAsync();
        //TODO : NPC
        await LoadNPCDataAsync();
        //TODO : Blood
        await LoadBloodDataAsync();
        //TODO : Enemy
        await LoadEnemyDataAsync();
        //TODO : Player
        await LoadPlayerDataAsync();
        await LoadImageResource();
        
        //TODO : FarmingData
        await LoadFarmingDataAsync();
        await LoadSkillDataAsync();

        IsLoaded = true; // 데이터 로드 완료 상태 설정
    }

    public async Task WaitUntilLoaded()
    {
        //while (!IsLoaded)
        //{
        //    await Task.Yield(); // 데이터가 로드될 때까지 대기
        //}
    }

    private async Task LoadImageResource()
    {
        var locationsHandle = Addressables.LoadResourceLocationsAsync("Images", typeof(Sprite[]));
        var locations = await locationsHandle.Task;

        foreach (var location in locations)
        {
            var spritesHandle = Addressables.LoadAssetAsync<Sprite[]>(location);
            Sprite[] sprites = await spritesHandle.Task;

            foreach (var sprite in sprites)
            {
                if (!SpriteDict.ContainsKey(sprite.name))
                {
                    SpriteDict[sprite.name] = sprite;
                }
                else
                {
                    Debug.LogWarning($"Duplicate sprite name: {sprite.name}");
                }
                //await Task.Yield();
            }
            //await Task.Yield();
        }
        progressCount++;
    }

    private async Task LoadFarmingDataAsync()
    {
        var list = await _addressable.LoadListDataAsync<FarmingData>("FarmingData");
        if (list == null)
            return;
        farmingData.Clear();
        foreach (var item in list)
        {
            if (!farmingData.ContainsKey(item.FarmingBlockID))
            {
                farmingData[item.FarmingBlockID] = item;
            }
            else
            {
                Debug.LogWarning($"Duplicate farming data id: {item.FarmingBlockID}");
            }
            //await Task.Yield();
        }
        progressCount++;
    }

    private async Task LoadSkillDataAsync()
    {
        var list = await _addressable.LoadListDataAsync<SkillData>("SkillData");
        if (list == null)
            return;
        SkillDataByID.Clear();
        foreach (var item in list)
        {
            if (!farmingData.ContainsKey(item.id))
            {
                SkillDataByID[item.id] = item;
            }
            else
            {
                Debug.LogWarning($"Duplicate farming data id: {item.id}");
            }
            //await Task.Yield();
        }
        progressCount++;
    }

    private async Task LoadSlotDataAsync()
    {
        var list = await _addressable.LoadListDataAsync<SlotData>("SlotData");
        if (list != null)
            SlotDataList = list;
        progressCount++;
    }

    private async Task LoadEquipmentDataAsync()
    {
        var list = await _addressable.LoadListDataAsync<Equipment>("Equipment");
        if (list == null)
            return;

        _equipmentById.Clear();
        _equipmentItemById.Clear();

        foreach (var item in list)
        {
            _equipmentById[item.id] = item;
            _equipmentItemById[item.id] = new EquipmentItem(item);
            //await Task.Yield();
        }

        AllEquipments = new List<Equipment>(_equipmentById.Values);
        AllEquipmentItems = new List<EquipmentItem>(_equipmentItemById.Values);
        progressCount++;
    }

    private async Task LoadConsumableDataAsync()
    {
        var list = await _addressable.LoadListDataAsync<Consumable>("Consumable");
        if (list == null)
            return;

        _consumableById.Clear();
        _consumableItemById.Clear();

        foreach (var item in list)
        {
            _consumableById[item.id] = item;
            _consumableItemById[item.id] = new ConsumableItem(item);
            //await Task.Yield();
        }

        AllConsumable = new List<Consumable>(_consumableById.Values);
        AllConsumableItems = new List<ConsumableItem>(_consumableItemById.Values);
        progressCount++;
    }

    private async Task LoadDialogueDataAsync()
    {
        // NpcDialogue.json을 불러와 NpcDialogue 리스트로 파싱
        var list = await _addressable.LoadListDataAsync<NPCDialog>("NPCDialog");
        if (list == null)
            return;

        NpcDialogueById.Clear();

        foreach (var dialog in list)
        {
            if (!NpcDialogueById.ContainsKey(dialog.NPCID))
            {
                NpcDialogueById[dialog.NPCID] = new Dictionary<int, NPCDialog>();
            }

            UtilityCode.Logger($"{dialog} + {dialog.dialogId} + {dialog.preDialogueId}");

            DialogById[dialog.dialogId] = dialog;

            NpcDialogueById[dialog.NPCID].Add(dialog.dialogId, dialog);
            //await Task.Yield();
        }
        //NpcDialogueById[dialog.] = list[i]; // id 필드 기준으로 저장 :contentReference[oaicite:0]{index=0}
        progressCount++;
    }

    private async Task LoadMaterialDataAsync()
    {
        var list = await _addressable.LoadListDataAsync<Material>("Material");
        if (list == null)
            return;

        _materialById.Clear();
        _materialItemById.Clear();

        foreach (var item in list)
        {
            _materialById[item.id] = item;
            _materialItemById[item.id] = new MaterialItem(item);
            //await Task.Yield();
        }

        AllMaterial = new List<Material>(_materialById.Values);
        AllMaterialItems = new List<MaterialItem>(_materialItemById.Values);
        progressCount++;
    }

    public Equipment GetEquipment(int id) =>
        _equipmentById.TryGetValue(id, out var item) ? item : null;

    public EquipmentItem GetEquipmentItem(int id) =>
        _equipmentItemById.TryGetValue(id, out var item) ? item : null;

    public Consumable GetConsumable(int id) =>
        _consumableById.TryGetValue(id, out var item) ? item : null;

    public ConsumableItem GetConsumableItem(int id) =>
        _consumableItemById.TryGetValue(id, out var item) ? item : null;

    public NpcDialogue[] GetDialogues(int startIndex, int endIndex)
    {
        //var result = new List<NpcDialogue>();
        //for (int i = startIndex; i <= endIndex; i++)
        //    if (NpcDialogueById.TryGetValue(i, out var dlg))
        //        result.Add(dlg);
        //return result.ToArray();
        return null;
    }

    public Material GetMaterial(int id) =>
        _materialById.TryGetValue(id, out var item) ? item : null;

    public MaterialItem GetMaterialItem(int id) =>
        _materialItemById.TryGetValue(id, out var item) ? item : null;

    //-------------------------------------

    //private async Task LoadCharacterDataAsync()
    //{
    //    var list = await LoadListDataAsync<CharacterData>("CharacterData");
    //    if (list != null)
    //        characterDatas = list;
    //}

    private async Task LoadCharacterDataAsync()
    {
        var list = await _addressable.LoadListDataAsync<CharacterData>("CharacterData");
        if (list == null)
            return;

        //클리어 하는 이유 매번 게임 최신화 할 때
        //기존의 데이터를 없앨 필요가 있기 때문. 어떤 게 업데이트 될지 모름.
        CharacterDataByID.Clear();

        foreach (var character in list)
        {
            Debug.Log(character.characterID + " : " + character.name_kr);
            CharacterDataByID[character.characterID] = character;
            //await Task.Yield();
        }
        progressCount++;
    }

    private async Task LoadNPCDataAsync()
    {
        var list = await _addressable.LoadListDataAsync<NPCData>("NPCData");
        if (list == null)
            return;

        //클리어 하는 이유 매번 게임 최신화 할 때
        //기존의 데이터를 없앨 필요가 있기 때문. 어떤 게 업데이트 될지 모름.
        NPCDataByID.Clear();

        foreach (var character in list)
        {
            NPCDataByID[character.characterID] = character;
            //await Task.Yield();
        }
        progressCount++;
    }

    private async Task LoadEnemyDataAsync()
    {
        var list = await _addressable.LoadListDataAsync<EnemyData>("EnemyData");
        if (list == null)
            return;

        //클리어 하는 이유 매번 게임 최신화 할 때
        //기존의 데이터를 없앨 필요가 있기 때문. 어떤 게 업데이트 될지 모름.
        EnemyDataByID.Clear();

        foreach (var character in list)
        {
            EnemyDataByID[character.characterID] = character;
            //await Task.Yield();
        }
        progressCount++;
    }

    private async Task LoadPlayerDataAsync()
    {
        var list = await _addressable.LoadListDataAsync<PlayerData>("PlayerData");
        if (list == null)
            return;

        //클리어 하는 이유 매번 게임 최신화 할 때
        //기존의 데이터를 없앨 필요가 있기 때문. 어떤 게 업데이트 될지 모름.
        PlayerDataByID.Clear();

        foreach (var character in list)
        {
            PlayerDataByID[character.characterID] = character;
            //await Task.Yield();
        }
        progressCount++;
    }

    private async Task LoadBloodDataAsync()
    {
        var list = await _addressable.LoadListDataAsync<BloodData>("BloodData");
        if (list == null)
            return;

        //클리어 하는 이유 매번 게임 최신화 할 때
        //기존의 데이터를 없앨 필요가 있기 때문. 어떤 게 업데이트 될지 모름.
        BloodDataByID.Clear();

        foreach (var character in list)
        {
            BloodDataByID[character.BloodName] = character;
            //await Task.Yield();
        }
        progressCount++;
    }

    // 아이템 타입에 따라 아이템 리스트 가져오기 - 상점용
    public List<Item> GetItemsByType(DesignEnums.NPCType type)
    {
        switch (type)
        {
            case DesignEnums.NPCType.WeaponNPC:
                return AllEquipmentItems
                    .Where(e => e.type == DesignEnums.EquipmentType.Weapon) // && e.isShopItem)
                    .Cast<Item>()
                    .ToList();

            case DesignEnums.NPCType.ArmorNPC:
                var armorTypes = new[]
                {
                     DesignEnums.EquipmentType.Armor,
                     DesignEnums.EquipmentType.Arm,
                     DesignEnums.EquipmentType.Head,
                     DesignEnums.EquipmentType.Leg
                 };
                return AllEquipmentItems
                    .Where(e => armorTypes.Contains(e.type))
                    .Cast<Item>()
                    .ToList();

            case DesignEnums.NPCType.PositionNPC:
                return AllConsumableItems
                    //.Where(c => c.isShopItem)
                    .Cast<Item>()
                    .ToList();

            case DesignEnums.NPCType.MaterialNPC:
                return AllMaterialItems
                    // .Where(m => m.isShopItem)
                    .Cast<Item>()
                    .ToList();
            //case DesignEnums.NPCType:
            //    return AllEquipmentItems
            //        // .Where(i => i.isShopItem)
            //        .Cast<Item>()
            //        .Concat(
            //            AllConsumableItems //.Where(i => i.isShopItem)
            //                .Cast<Item>()
            //        )
            //        .Concat(
            //            AllMaterialItems //.Where(i => i.isShopItem)
            //                .Cast<Item>()
            //        )
            //        .ToList();

            default:
                Debug.LogWarning($"지원되지 않는 상점 타입: {type}");
                return new List<Item>();
        }
    }

    /// 아이템 ID로 아이템 가져오기
    public Item GetItemByID(int id)
    {
        var eq = _equipmentItemById.TryGetValue(id, out var equipmentItem) ? equipmentItem : null;
        if (eq != null)
            return eq;

        var con = _consumableItemById.TryGetValue(id, out var consumableItem) ? consumableItem : null;
        if (con != null)
            return con;

        var mat = _materialItemById.TryGetValue(id, out var materialItem) ? materialItem : null;
        if (mat != null)
            return mat;

        Debug.LogWarning($"[DataManager] ID {id}에 해당하는 아이템 없음");
        return null;
    }
}