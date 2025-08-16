using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AI;
using UnityEngine.ResourceManagement.AsyncOperations;

public enum AttackType
{
    Sword,
    Magic
}
public class CharacterObject : MonoBehaviour, IDamageable, IPlace, ISpawn, IDatable
{
    [Header("ScriptableObject -> Addressable")]
    public CharacterRuntimeData characterRuntimeData;
    public Stats weaponStats = new();
    public TextMeshProUGUI textMeshProUGUI;
    [field: SerializeField] public Weapon Weapon; //적의 무기, 공격을 담당합니다.
    
    public Animator Animator;
    [field: SerializeField] public Animator Front { get; private set; }
    [field: SerializeField] public Animator Back { get; private set; }
    [field: SerializeField] public Animator Side { get; private set; }

    public AttackType attackType;

    [Header("Serializable")]

    public Village spawnVillage;
    public Place curPlace;
    public List<Skill> skills = new(4); //실제로는 게임오브젝트만 있어도 되지만, 값을 전달하기 위해서는 Skill이 필요.
    public List<int> skillNums = new(4);
    public bool isSkill = false;
    //public PhotonView photonView;
    public Transform Parent;
    public bool isAttack = true;
    public bool isKnockBack;

    public string curLook;
    public string hash;
    public float testSpeedValue = 0.1f; //Speed value

    public List<SpriteRenderer> spriteRenderers;

    [SerializeField] private Transform rightHandTransform_Forward;
    [SerializeField] private Transform rightHandTransform_Side;
    [SerializeField] private Transform rightHandTransform_Back;

    private readonly Dictionary<string, GameObject> _weaponGOs = new();

    private float _weightPart;
    public bool isStun;

    //public void SkillAction(int key)
    //{
        
    //    isSkill = true;
    //}

    protected virtual void Awake()
    {
        this.gameObject.AddPhoton();
        //photonView = GetComponent<PhotonView>();
        Parent = FindObjectOfType<NPCQuestion>().transform;
        //Initialize();
       
    }

    public void WeaponEnable()
    {
        Weapon?.Attack(true);
    }

    public void WeaponDisable()
    { 
        Weapon?.Attack(false);
    }

    [PunRPC]
    public void AttackEnd()
    {
        WeaponDisable();
        isAttack = true;
    }

    protected virtual void Start()
    {
        curPlace = PlaceManager.Instance.curPlace;
    }

    protected virtual void OnDisable()
    {
        //spawnVillage.Respawn();
    }

    //캐릭터 초기화 코드
    public virtual void Initialize(int key, int villageNumber = 0)
    {
        int randomAttackType = UnityEngine.Random.Range(0, 2);
        if (randomAttackType == 0)
            attackType = AttackType.Sword;
        if(randomAttackType == 1)
            attackType = AttackType.Magic;

        this.characterRuntimeData = new CharacterRuntimeData(key);
        if (Front != null) return;

        textMeshProUGUI = GetComponentInChildren<Canvas>(true).GetComponentInChildren<TextMeshProUGUI>(true);
        gameObject.transform.parent = Parent;

        textMeshProUGUI.text = characterRuntimeData.name_kr; //임시
        if (CreateGame.Instance.SpawnPointDict.ContainsKey(villageNumber))
        {
            spawnVillage = CreateGame.Instance.SpawnPointDict[villageNumber];
        }

        gameObject.name = characterRuntimeData.characterID +": "+ characterRuntimeData.name_kr;

        //캐릭터의 Front, Side, Back에 해당하는 스프라이트 박스를 넣어주는 코드
        //if문은 해당 애니메이션이 없을 때 강제로 501번을 받도록 하는 예외처리 코드.
        var frontKey = "Forward_" + key;
        if (!AddressableManager.Instance.animatorCache.ContainsKey(frontKey))
        {
            frontKey = "Forward_" + 501;
            key = 501;
        }
        
        GameObject frontPrefab = Instantiate(AddressableManager.Instance.animatorCache[frontKey], transform);
        Front = frontPrefab.GetComponent<Animator>();

        var sideKey = "Side_" + key;
        GameObject sidePrefab = Instantiate(AddressableManager.Instance.animatorCache[sideKey], transform);
        Side = sidePrefab.GetComponent<Animator>();

        var backKey = "Back_" + key;
        GameObject backPrefab = Instantiate(AddressableManager.Instance.animatorCache[backKey], transform);
        Back = backPrefab.GetComponent<Animator>();

        frontPrefab.name = "Forward";
        sidePrefab.name = "Side";
        backPrefab.name = "Back";
        Animator = Front;

        //무기 장착코드
        SetRightHand();
        spriteRenderers = Front.gameObject.GetComponentsInChildren<SpriteRenderer>(true).ToList();

        Side.gameObject.SetActive(false);
        Back.gameObject.SetActive(false);
    }

    private Coroutine damageCoroutine;
    private Coroutine knockBackCoroutine;
    public virtual void TakeDamage(float damage, CharacterObject characterObject)
    {
        if (characterRuntimeData.missPercent.Current * RoomSettingData.Instance.missBalance > UnityEngine.Random.Range(0f, 100f))
        {
            //UserHelpManager.Instance.CreateText("공격을 회피했습니다!");
            return;
        }

        var totalDamage = damage - characterRuntimeData.defence.Current;

        characterRuntimeData.health.Current -= totalDamage < damage / 4 ? damage / 4 : totalDamage;

        if (RoomSettingData.Instance.isknockBackEnable)
        {
            Debug.Log("넉백이 찍혔고");
            if (characterRuntimeData.criticalPercent.Current * RoomSettingData.Instance.KnockBackBalance > UnityEngine.Random.Range(0, 100))
            {
                Vector2 knockBack = (this.gameObject.transform.position - characterObject.transform.position).normalized;
                Vector2 force = knockBack * RoomSettingData.Instance.KnockBackDistance;
                if (knockBackCoroutine != null)
                {
                    StopCoroutine(knockBackCoroutine);
                    knockBackCoroutine = null;
                }
                Debug.Log("넉백됨");
                StartCoroutine(KnockBackValue(force));
            }
        }

        if (RoomSettingData.Instance.isStunEnable)
        {
            if (characterRuntimeData.criticalPercent.Current * RoomSettingData.Instance.stunBalance > UnityEngine.Random.Range(0, 100))
            {
                Parring();
                return;
            }
        }

        if (isStun) return;
        BodyColorChangeDuration(Color.white, Color.red, 0.3f);
        //UserHelpManager.Instance.CreateText($"데미지 {damage}를 맞아 {characterRuntimeData.health.Current} 남았습니다.");
    }

    private IEnumerator KnockBackValue(Vector2 force)
    {
        if (gameObject.TryGetComponent<NavMeshAgent>(out var agent))
        {
            float timer = 0f;
            float time = 0.2f;
            float weight = 1 / time;
            isKnockBack = true;

            agent.isStopped = true;

            while(time > timer)
            {
                agent.Move(force * Time.deltaTime * weight);
                timer += Time.deltaTime;
                yield return null;
            }
            agent.isStopped = false;
            isKnockBack = false;
            knockBackCoroutine = null;
            yield break;
        }

        if (gameObject.TryGetComponent<Rigidbody2D>(out var rigid))
        {
            float timer = 0f;
            float time = 0.2f;
            float weight = 1 / time;
            isKnockBack = true;

            while (timer < time)
            {
                rigid.MovePosition(rigid.position + force * weight * Time.deltaTime);
                timer += Time.deltaTime;
                yield return null;
            }

            //rigid.AddForce(force * rigid.mass, ForceMode2D.Impulse);
            //yield return null;
            //rigid.velocity = Vector2.zero;

            isKnockBack = false;
            knockBackCoroutine = null;
            yield break;
        }
        yield return null;
    }

    private void BodyColorChangeDuration(Color baseColor, Color changeColor, float duration)
    {
        if (damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
            damageCoroutine = null;
        }

        Color targetColor = Color.Lerp(baseColor, changeColor, 0.8f);
        targetColor.a = 1.0f;
        
        foreach (var sprite in spriteRenderers)
        {
            sprite.color = targetColor; //피격 시 빨간색으로 변경
        }

        //spriteRenderer.color = hitColor; //피격 시 빨간색으로 변경
        damageCoroutine = StartCoroutine(WhileBack(
            duration,
            (timer) =>
            {
                foreach (var sprite in spriteRenderers)
                {
                    float ratio = Mathf.Clamp(timer / duration, 0f, 1f);
                    sprite.color = Color.Lerp(targetColor, baseColor, ratio);
                }
                //spriteRenderer.color = Color.Lerp(hitColor, baseColor, timer / 0.2f);
            }));
    }

    private IEnumerator WhileBack(float time, Action<float> action)
    {
        float timer = 0f;

        while (time > timer)
        {
            timer += Time.deltaTime;
            action(timer);
            yield return null;
        }
        action(time);

        damageCoroutine = null;
    }

    public async void Equip(EquipmentItem item)
    {
        if (item == null)
        {
            Debug.LogError("[Player.Equip] item이 null입니다");
            return;
        }
        var statPairs = item.GetStatPairs();
        if (statPairs == null)
        {
            Debug.LogError("[Player.Equip] item.GetStatPairs()가 null입니다");
            return;
        }

        foreach (var (type, value) in statPairs)
        {
            characterRuntimeData.AddStat(type, value);
            weaponStats.AddStat(type, value);
        }

        // 기존 무기 모두 제거
        if (item.type == DesignEnums.EquipmentType.Weapon)
            ClearAllWeapons();

        if (string.IsNullOrEmpty(item.prefabName))
        {
            Debug.LogWarning("[Equip] prefabName is empty");
            return;
        }

        // Addressables에서 프리팹 로드
        AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(item.prefabName);
        await handle.Task;

        if (handle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"[Equip] 프리펩 로드 실패 : {item.prefabName}");
            return;
        }

        var prefab = handle.Result;

        // 2) 각 소켓에 인스턴스 생성
        TrySpawn(prefab, rightHandTransform_Forward, "Forward");
        TrySpawn(prefab, rightHandTransform_Side, "Side");
        TrySpawn(prefab, rightHandTransform_Back, "Back");

        // 3) 로드 핸들 해제 (인스턴스는 유지됨)
        Addressables.Release(handle);

    }

    public void UnEquip(EquipmentItem item)
    {
        if (item == null)
        {
            Debug.LogWarning("[Player.UnEquip] item이 null입니다");
            return;
        }


        foreach (var (type, value) in item.GetStatPairs())
        {
            characterRuntimeData.RemoveStat(type, value);
            weaponStats.RemoveStat(type, value);
        }
        
        if(item.type == DesignEnums.EquipmentType.Weapon)
            ClearAllWeapons();

    }
    private void TrySpawn(GameObject prefab, Transform socket, string key)
    {
        if (socket == null) return;

        var go = Instantiate(prefab, socket);
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f); 
        _weaponGOs[key] = go;
    }

    private void ClearAllWeapons()
    {
        foreach (var go in _weaponGOs.Values)
            if (go) Destroy(go);
        _weaponGOs.Clear();
        Weapon = null;
    }

    public void AddWeight()
    {
        //TODO : AddInventory와 Item에 넣어주세요!
        //TODO : 무게 추가
    }

    public float GetHp()
    {
        return characterRuntimeData.health.Current;
    }

    public virtual void SetPlace(Place place)
    {
        curPlace = place;
    }
    public virtual void CheckPlace()
    {

    }

    public virtual void CallTakeDamage(float amount, CharacterObject characterObject)
    {
        return;
    }

    public virtual void Respawn()
    {
        gameObject.SetActive(true);
        //Debug.Log("가일이 부활했습니다.");
    }

    private async void SetRightHand()
    {
        string animAddress = "root/hip/Torso_Down/Torso_Up/Neck_Joint/R_ArmJoint/R_UpArm/R_ArmJoint_1/R_HandJoint/R_Hand";
        GameObject weapon = AddressableManager.Instance._prefabCash["RustySword"];
        float size = 1.5f;

        if (rightHandTransform_Forward == null)
        {
            rightHandTransform_Forward = transform.Find($"Forward/{animAddress}") ?? transform;
            //rightHandTransform_Forward = transform.Find("Forward/bone_1/bone_2/bone_9/bone_10") ?? transform;
        }

        if (rightHandTransform_Side == null)
        {
            rightHandTransform_Side = transform.Find($"Side/{animAddress}") ?? transform;
            //rightHandTransform_Side = transform.Find("Side/bone_1/bone_2/bone_5/bone_6") ?? transform;
        }

        if (rightHandTransform_Back == null)
        {
            rightHandTransform_Back = transform.Find($"Back/{animAddress}") ?? transform;
            //rightHandTransform_Back = transform.Find("Back/bone_1/bone_2/bone_5/bone_6") ?? transform;
        }

        if (!(this is Player))
        {
            GameObject forwardWeapon = Instantiate(weapon, rightHandTransform_Forward);
            forwardWeapon.transform.localScale = Vector3.one * size; // 크기 조정
            GameObject sideWeapon = Instantiate(weapon, rightHandTransform_Side);
            sideWeapon.transform.localScale = Vector3.one * size; // 크기 조정
            GameObject backWeapon = Instantiate(weapon, rightHandTransform_Back);
            backWeapon.transform.localScale = Vector3.one * size; // 크기 조정

            if (this.attackType == AttackType.Magic)
            {
                forwardWeapon.SetActive(false);
                sideWeapon.SetActive(false);
                backWeapon.SetActive(false);
            }
        }
        else if(this is Player)
        {
            var equipInventory = GetComponent<EquipmentInventory>();
            equipInventory?.EquipDefaultWeapon();
        }
    }

    private Coroutine parringCoroutine;

    public void Parring()
    {
        isStun = true;
        BodyColorChangeDuration(Color.white, Color.yellow, RoomSettingData.Instance.stunTime);

        if(parringCoroutine != null)
        {
            StopCoroutine(parringCoroutine);
            parringCoroutine = null;
        }
        parringCoroutine = StartCoroutine(ChangeParringState());
    }

    private IEnumerator ChangeParringState()
    {
        yield return new WaitForSeconds(RoomSettingData.Instance.stunTime);
        isStun = false;
        parringCoroutine = null;
    }

    public float GetHpPercent()
    {
        return characterRuntimeData.health.Current / characterRuntimeData.health.Max;
    }

    public int GetID()
    {
        return characterRuntimeData.characterID;
    }

    public string GetName()
    {
        return characterRuntimeData.name_kr;
    }

    public int GetLevel()
    {
        return characterRuntimeData.initLevel;
    }

    public Sprite GetIcon()
    {
        var characterIconKey = characterRuntimeData.characterID + "_characterIcon";
        if(!AddressableManager.Instance.IconCash.ContainsKey(characterIconKey))
        {
            characterIconKey = 501 + "_characterIcon";
        }

        var icon = AddressableManager.Instance.IconCash[characterIconKey];
        return icon;
    }
}
