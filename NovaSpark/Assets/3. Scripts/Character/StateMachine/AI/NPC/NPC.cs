using Cinemachine;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public enum BuildingType //건물의 종류
{
    None,
    House, //일반 집
    Hotel, //여관 집
    Beer, //술집
    Shop, //상점
    CollectorPawn, //전당포
    Pollice, //경찰
    CountryOwner, //총재탑?
    Temple, //신전
    BrokenHouse, //버려진 집, 파괴된 집
    Casino, //도박장
}

//public enum DesignEnums.NPCType
//{
//    //NPC의 종류
//    HotelNPC,//여관집 주인
//    FarmingNPC,//농부
//    FishingNPC, //어부
//    MiningNPC, //광부
//    CutterNPC, //나무꾼
//    EnhanceNPC,//강화상인
//    CasinoNPC,//도박장 선수
//    EnchantNPC, //인첸트 상인
//    WeaponNPC,//무기상인
//    PositionNPC,//물약상인
//    OtherNPC,//잡화상인
//    TrainerNPC,//트레이너? => 능력치 올려주는?
//    QuestNPC,//퀘스트주는 점원? 술집에
//    None

//    //용병 (NPC 친화도, 팀 설정 있어야 가능)
//    //경찰 (경찰도 마을, 팀이라는 느낌이 존재해야함. 그리고 팀?(친밀도?)은 여러개 있을 수 있음)
//    //우두머리
//    //성기사 (NPC 친화도, 팀 설정 있어야 가능)

//}


public class NPC : CharacterObject, IInteractable
{
    [field: SerializeField] public NPCQuestion npcQuestion { get; private set; } //NPC와 대화할 때, 선택지를 제공하는 UI
    Dictionary<int, NPCDialog> NPCdialogDict = new();

    public Image questImage;

    public void NPCQuestionCheck(bool isIdle, bool isDead) //호출 위치 : 생성 시, 퀘스트 클리어 시 재호출. npc 스폰과 같이 관리하는 코드 필요.
    {
        //TODO : 아이콘 초기화 = 퀘스트 없다!
        questImage.gameObject.SetActive(false);

        return;
        if (!isIdle) return;
        if (!isDead) return;


        if(DataManager.Instance.NpcDialogueById.ContainsKey(characterRuntimeData.characterID))
        {
            questImage.gameObject.SetActive(true);
            //퀘스트 있다!
        }
        //foreach(var npcDialog in NPCdialogDict)
        //{
        //    if (DialogManager.Instance.getDialog.Contains(npcDialog.Value.questId))
        //    {
        //        //퀘스트 중이다!
        //        break;
        //    }
        //}
        //완료는 바로 뜨기 때문에 해당사항 없음.
    }

    public void NPCInteract(DesignEnums.NPCType npcType, bool isIdle, bool isDead)
    {
        //TODO : NPC와 대화하는 기능 구현, 다이얼로그
        bool isDialog = false;

        if (DataManager.Instance.NpcDialogueById.ContainsKey(characterRuntimeData.characterID))
        {
            NPCdialogDict = DataManager.Instance.NpcDialogueById[characterRuntimeData.characterID];


            foreach (var npcDialog in NPCdialogDict)
            {
                bool isPrev = true; //문제 확인

                if (npcDialog.Value.questId == 0) continue; //퀘스트가 없는 다이얼로그는 건너뛴다.

                if (!DialogManager.Instance.clearDialog.Contains(npcDialog.Key)) // clearDialog에 없다면 해당 키가
                {
                    foreach (var npcPrev in npcDialog.Value.preDialogueId) //그 키의 선행 스킬을 본다
                    {
                        if ((!DialogManager.Instance.clearDialog.Contains(npcPrev)) && npcPrev != 0) //해당 번호의 선행 과정이 지나가지 않았다면
                        {
                            isPrev = false; // 선행과정을 밟았다면
                            break;
                        }
                    }
                }

                if (!isPrev)
                {
                    UtilityCode.Logger("해당 퀘스트를 받으실 수 있습니다" + npcDialog.Value.dialogId);
                    continue; //되돌리기
                }

                isDialog = true;
                //DialogManager.Instance.npcKey = characterRuntimeData.characterID;
                //DialogManager.Instance.dialogKey = npcDialog.Key;
                break;
            }
            DialogManager.Instance.NPCdialogDict = NPCdialogDict;
        }

        DialogManager.Instance.AddQuestSuccess(characterRuntimeData.characterID, DesignEnums.QuestEvent.Talk);
        //for(int j =0; j < NPCdialog.Count; j++)
        //{
        //    if (!DialogManager.Instance.clearDialog.Contains(NPCdialog[j].dialogId)) //해당 번호가 진행하지 않은 다이얼로그 일 경우,
        //    {
        //        bool isPrev = true;
        //        for (int k = 0; k < NPCdialog[j].preDialogueId.Count; k++)
        //        {
        //            if (!DialogManager.Instance.clearDialog.Contains(NPCdialog[j].preDialogueId[k]) && NPCdialog[j].preDialogueId[k] != 0) //해당 번호의 선행 과정이 지나가지 않았다면
        //            {
        //                isPrev = false;
        //                break;
        //            }
        //        }
        //        if (!isPrev) continue; //되돌리기

        //    }
        //isDialog = true;
        //        DialogManager.Instance.npcKey = characterRuntimeData.characterID;
        //        DialogManager.Instance.dialogKey = NPCdialog[j].dialogId;
        //        break;
        //}

        //DialogManager.Instance.ViewDialog(CharacterID, );


        foreach (var npcQuestion in npcQuestion.buttonAllPanel)
        {
            npcQuestion.gameObject.SetActive(false); //모든 버튼을 비활성화
        }
        
        //TODO : NPC와 대화하는 기능 구현, 다이얼로그
        //NPCType에 따라 대화 내용이 달라질 수 있음.
        //예를 들어, HotelNPC는 여관에 대한 정보를 제공하고, FarmingNPC는 농업 관련 정보를 제공할 수 있음.
        //TODO : 선택지 제공, NPC에 따라 다른 선택지 제공 가능
        //TODO : 예시) 상점 주인 : {인사하기, 시비걸기, 물건 사기}
        //TODO : 시비걸기에서는 말풍선으로 시비걸면 재밌을 듯.


        if (isIdle)
        {
            if(isDialog)
            {
                //npcQuestion.questButton.gameObject.SetActive(true);
            }

            switch (npcType)
            {
                case DesignEnums.NPCType.HotelNPC: // 휴식하기 
                    //UtilityCode.Logger("여관 주인과 대화 중입니다. 여관에 대한 정보를 제공합니다.");
                    npcQuestion.hotelButton.gameObject.SetActive(true);
                    break;
                case DesignEnums.NPCType.FarmingNPC: // 농사 배우기
                    //UtilityCode.Logger("농부와 대화 중입니다. 농업 관련 정보를 제공합니다.");
                    npcQuestion.farmingButton.gameObject.SetActive(true);
                    break;
                case DesignEnums.NPCType.FishingNPC: // 낚시 배우기
                    //UtilityCode.Logger("어부와 대화 중입니다. 낚시에 대한 정보를 제공합니다.");
                    npcQuestion.fishingButton.gameObject.SetActive(true);
                    break;
                case DesignEnums.NPCType.MiningNPC: // 광질 배우기
                    //UtilityCode.Logger("광부와 대화 중입니다. 광산에 대한 정보를 제공합니다.");
                    npcQuestion.miningButton.gameObject.SetActive(true);
                    break;
                case DesignEnums.NPCType.CutterNPC:
                    UtilityCode.Logger("나무꾼과 대화 중입니다. 나무에 대한 정보를 제공합니다.");
                    npcQuestion.cutterButton.gameObject.SetActive(true);
                    break;
                case DesignEnums.NPCType.EnhanceNPC:
                    UtilityCode.Logger("강화 상인과 대화 중입니다. 아이템 강화에 대한 정보를 제공합니다.");
                    npcQuestion.upgradeButton.gameObject.SetActive(true);
                    break;
                case DesignEnums.NPCType.CasinoNPC:
                    UtilityCode.Logger("도박장 선수와 대화 중입니다. 도박에 대한 정보를 제공합니다.");
                    npcQuestion.casinoButton.gameObject.SetActive(true);
                    break;
                case DesignEnums.NPCType.EnchantNPC:
                    UtilityCode.Logger("인첸트 상인과 대화 중입니다. 마법 부여에 대한 정보를 제공합니다.");
                    npcQuestion.enchantButton.gameObject.SetActive(true);
                    break;
                case DesignEnums.NPCType.WeaponNPC:
                    UtilityCode.Logger("무기 상인과 대화 중입니다. 무기에 대한 정보를 제공합니다.");
                    npcQuestion.weaponBuyButton.gameObject.SetActive(true);
                    break;
                case DesignEnums.NPCType.ArmorNPC:
                    UtilityCode.Logger("무기 상인과 대화 중입니다. 무기에 대한 정보를 제공합니다.");
                    npcQuestion.armorBuyButton.gameObject.SetActive(true);
                    break;
                case DesignEnums.NPCType.MaterialNPC:
                    UtilityCode.Logger("무기 상인과 대화 중입니다. 무기에 대한 정보를 제공합니다.");
                    npcQuestion.materialBuyButton.gameObject.SetActive(true);
                    break;
                case DesignEnums.NPCType.PositionNPC:
                    UtilityCode.Logger("물약 상인과 대화 중입니다. 물약에 대한 정보를 제공합니다.");
                    npcQuestion.positionBuyButton.gameObject.SetActive(true);
                    break;
                case DesignEnums.NPCType.OtherNPC:
                    UtilityCode.Logger("잡화 상인과 대화 중입니다. 다양한 물건에 대한 정보를 제공합니다.");
                    npcQuestion.otherBuyButton.gameObject.SetActive(true);
                    break;
                case DesignEnums.NPCType.TrainerNPC:
                    UtilityCode.Logger("트레이너와 대화 중입니다. 능력치 향상에 대한 정보를 제공합니다.");
                    npcQuestion.trainerButton.gameObject.SetActive(true);
                    break;
                case DesignEnums.NPCType.QuestNPC:
                    UtilityCode.Logger("퀘스트 주는 점원과 대화 중입니다. 퀘스트에 대한 정보를 제공합니다.");
                    npcQuestion.questButton.gameObject.SetActive(true);
                    break;
                default:
                    UtilityCode.Logger("알 수 없는 NPC 타입입니다.");
                    break;
            }
            
        }
        
    }
    //TODO : 친밀도

    public Player player;
    //NavMesh
    [field: SerializeField] public DesignEnums.NPCType NPCType { get; private set; } //NPC의 종류, 예를 들어 HotelNPC, FarmingNPC 등

    [field: SerializeField] public LayerMask targetLayer { get; private set; }
    [field: SerializeField] public Vector3 HomePoint { get; private set; } //NPC 집의 위치
    [field: SerializeField] public Vector3 WorkPoint { get; private set; } //NPC 일터의 위치
    [field: SerializeField] public NPCStateMachine StateMachine { get; private set; }
    [field: SerializeField] public CinemachineVirtualCamera VirtualCamera { get; private set; } //NPC의 카메라 : 대화 걸었을 시 활성화.

    public bool isFirstAttacker = false; //NPC는 기본적으로 비선공
    public float time = 0.0f; //Test : 날짜와 시간
    public float workTime = 9.0f;
    public float restTime = 21.0f;
    [field: SerializeField] public NavMeshAgent Agent { get; protected set; } //NavMeshAgent를 사용하여 이동을 관리합니다. //AI들은.. 따로 관리
    [field: SerializeField] public CharacterObject Target { get; set; }
    
    //[field: SerializeField] public CharacterObject Character { get; private set; }
    //public bool isAttack = false; //NPC가 공격 중인지 여부를 나타냅니다. //TODO : 공격 중일 때, 애니메이션 재생 여부
    //[field: SerializeField] public LayerMask EnemyLayer { get; set; } //적대적인 성향의 레이어, 실시간으로 갱신될 수 있음.
    //[field: SerializeField] public LayerMask LayerMask { get; protected set; } //상호작용 가능한 레이어, 실시간으로 갱신되지는 않음.

    [field: SerializeField] public NPCRuntimeData NPCRuntimeData { get; private set; }

    private Coroutine coroutine;

    public void AttackBall()
    {
        GameObject attackBall = Instantiate(AddressableManager.Instance._prefabCash["AttackBall"], transform.position, Quaternion.identity);
        Vector3 posNormalized = (Target.transform.position - transform.position).normalized;
        attackBall.GetComponent<DamageBall>().Initialize(posNormalized, this);
    }
    protected override void Awake()
    {
        base.Awake();
        Animator = GetComponent<Animator>();
        Agent = GetComponent<NavMeshAgent>();
        Agent.updateRotation = false; // NavMeshAgent가 회전을 자동으로 처리하지 않도록 설정합니다.
        Agent.updateUpAxis = false; // NavMeshAgent가 Y축을 자동으로 업데이트하지 않도록 설정합니다.
        VirtualCamera= GetComponentInChildren<CinemachineVirtualCamera>(true);
        VirtualCamera.Follow = transform; //NPC와 대화할 때 카메라가 NPC를 바라보도록 설정합니다.
        questImage = GetComponentInChildren<Image>(true); //NPC의 퀘스트 이미지 UI를 가져옵니다.

        StateMachine = new NPCStateMachine(this);
        StateMachine.ChangeState(StateMachine.NPCIdleState);
    }

    protected override void Start()
    {
        base.Start();
        PlaceManager.Instance.aiObjects.Add(this);
        npcQuestion = GetComponentInParent<NPCQuestion>(); //NPC와 대화할 때, 선택지를 제공하는 UI를 가져옵니다.

        NPCQuestionCheck(true, true); // 초기에 퀘스트가 있는지 확인합니다.

        //Character = DataManager.Instance.CharacterObjectByID[CharacterID];
        if (npcQuestion == null)
        {
            UtilityCode.ErrorLogger("[NPC] npcQuestion is NULL! NPC 오브젝트 부모 중에 NPCQuestion 컴포넌트가 없습니다!");
        }
        else
        {
            UtilityCode.Logger("[NPC] npcQuestion 정상 할당됨: " + npcQuestion.gameObject.name);
        }
    }

    public float App => time; 
    public void OnInitialize(int key, int village) => Initialize(key, village);

    [PunRPC]
    public override void Initialize(int key, int village = 0)
    {
        base.Initialize(key, village);
        this.NPCRuntimeData = new NPCRuntimeData(key);
        NPCType = NPCRuntimeData.npcType;
        textMeshProUGUI.color = Color.green;
        characterRuntimeData.DefenceSpawnStatus(GameManager.Instance.TimeManager.DayCount);
        characterRuntimeData.health.AddStat(characterRuntimeData.health.Current * (RoomSettingData.Instance.enemyHPWeight - 1.0f));
        characterRuntimeData.damage.AddStat(characterRuntimeData.damage.Current * (RoomSettingData.Instance.enemyDamageWeight - 1.0f));
        characterRuntimeData.defence.AddStat(characterRuntimeData.defence.Current * (RoomSettingData.Instance.enemyDefWeight - 1.0f));
        characterRuntimeData.moveSpeed.AddStat(characterRuntimeData.moveSpeed.Current * RoomSettingData.Instance.AISpeedWeight - 1.0f);
    }

    public void FixedUpdate()
    {
        if (characterRuntimeData.health.Current < 0) return;
        if (StateMachine == null) return;
        if (Animator == null) return;

        time += Time.deltaTime; //시간 업데이트, 테스트용
        if (time >= 24f) time = 0f; // 하루가 지나면 시간 초기화
        
        StateMachine?.Update(); //상태 머신 업데이트

        if (Target == null)
        {
            Look(Agent.velocity.normalized);
        }
        else
        {
            Vector2 targetView = (Target.transform.position - transform.position).normalized;
            Look(targetView);
        }
    }

    public override void SetPlace(Place place)
    {
        base.SetPlace(place);
        CheckPlace();
    }

    public override void CheckPlace()
    {
        // curPlace가 플레이어, 또는 현재 활성화된 맵과 다를 경우,
        if (curPlace == PlaceManager.Instance.curPlace)
        {
            foreach (var sprite in spriteRenderers)
            {
                sprite.enabled = true;
            }
            Weapon.ChangePlace(true);
            //Weapon.spriteRenderer.enabled = false;
        }
        else
        {
            foreach (var sprite in spriteRenderers)
            {
                sprite.enabled = false;
            }
            Weapon.ChangePlace(false);
        }
    }

    public bool AttackDistance() //TODO : 친한 애인지 선공 몹인지에 대한 식별 여부 필요.
    {
        if (Target == null) return false;
        if (!isFirstAttacker) return false;

        float distance = Vector3.Distance(transform.position, Target.gameObject.transform.position);

        float attackDistanceWeight = 1.0f;
        if (attackType == AttackType.Magic)
        {
            attackDistanceWeight *= 5.0f;
        }

        if (distance <= Weapon.gameObject.transform.localScale.x * RoomSettingData.Instance.attackDistance * attackDistanceWeight)
        {
            return true;
        }

        return false;
    }

    public bool TargetingEnemy(float radius)
    {
        //if(RoomSettingData.Instance.isNPCAttackMonster)
        //{
        //    Target = null;
        //    Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, targetLayer); //NonAlloc 방식으로 Collider2D를 가져오는 코드가 필요.
        //    foreach (var collider in colliders)
        //    {
        //        if (collider.gameObject == this.gameObject) continue; // 자기 자신은 제외
        //        if (collider.gameObject.tag == gameObject.tag) continue;

        //        if (collider.TryGetComponent<Enemy>(out var characterObject))
        //        {
        //            Target = characterObject;
        //        }
        //    }
        //}

        if (Target == null) return false;
        if (!isFirstAttacker) return false;

        float distance = Vector3.Distance(transform.position, Target.gameObject.transform.position);

        if (distance <= radius)
        {
            return true;
        }
        isFirstAttacker = false;
        Target = null; // 타겟이 범위 밖에 있으면 타겟을 초기화
        return false;
    }

    public Coroutine StartMyCoroutine(IEnumerator enumerator)
    {
        return StartCoroutine(enumerator);
    }

    public void StopMyCoroutine(Coroutine coroutine)
    {
        StopCoroutine(coroutine);
        return;
    }

    [PunRPC]
    public override void TakeDamage(float damage, CharacterObject attacker)
    { 
        if(characterRuntimeData.health.Current <= 0)
        {
            return; // 이미 죽은 상태라면 데미지를 받지 않습니다.
        }

        var curHp = characterRuntimeData.health.Current;
        base.TakeDamage(damage, attacker);

        if (curHp == characterRuntimeData.health.Current)
        {
            UserHelpManager.Instance.CreateText($"회피 성공");
            return;
        }

        if (characterRuntimeData.health.Current <= 0)
        {
            DefenceManager.Instance.npcCount--;
            DefenceManager.Instance.TextReload();
            if (attacker.TryGetComponent(out Player player))
            {
                //if (player.photonView.IsMine)
                //{
                var data = NPCRuntimeData;
                player.AddExp((int)(RoomSettingData.Instance.isKillExpPercent));
                DialogManager.Instance.AddQuestSuccess(characterRuntimeData.characterID, DesignEnums.QuestEvent.Kill);
                //}
            }
            StateMachine.ChangeState(StateMachine.NPCDeathState);
            return;
        }

        if (attacker.GetComponent<Player>() != null) return;
        if (attacker.GetComponent<NPC>() != null) return;

        //PhotonView photon = PhotonView.Find(attackerViewID);
        //CharacterObject attacker = photon.GetComponent<CharacterObject>();

        //맞으면 대화중인거 취소
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            npcQuestion.OffButton();
            TalkOut();
            //DialogManager.Instance.DialogOff();
        }

        SoundManager.Instance.PlaySFXAsync(SoundParameterData.AITakeDamage_SFXParameterHash);

        if (isFirstAttacker != true)
        {
            isFirstAttacker = true;
            Target = attacker;
        }

        if (isStun)
        {
            StateMachine.ChangeState(StateMachine.NPCIdleState);
            return;
        }

        StateMachine.ChangeState(StateMachine.NPCTargetState);
        //TODO : 친화도 떨어지는 코드
    }

    public IEnumerator DeadEnemy()
    {
        SoundManager.Instance.PlaySFXAsync(SoundParameterData.AIDeath_SFXParameterHash);
        yield return new WaitForSeconds(60f);
        gameObject.SetActive(false);

        //오디오 
        //TODO : 오브젝트 풀링 가능
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        TalkOut();
        Agent.ResetPath();
        StateMachine.ChangeState(StateMachine.NPCIdleState);
        //spawnVillage.Respawn(this);
    }

    private IEnumerator TalkOutDistance(GameObject owner)
    {
        Vector3 startPos = owner.transform.position;

        while(true)
        {
            if(owner.transform.position != startPos)
            {
                TalkOut();
                yield break;
            }
            yield return null;
        }
    }

    private void TalkOut()
    {
        npcQuestion.OffButton();
        DialogManager.Instance.DialogOff();
        VirtualCamera.gameObject.SetActive(false);
        UIManager.Instance.HidePanel(UIType.NPCInventoryUI);
        UIManager.Instance.HidePanel(UIType.EnhanceUI);
        UIManager.Instance.HidePanel(UIType.ShopUI);
        UIManager.Instance.HidePanel(UIType.QuantitySelectUI);

        player.isTalk = false;
    }

    public void Attack()
    {
        if (isAttack)
        {
            //공격
            isAttack = false;
            Animator.speed = characterRuntimeData.attackSpeed.Current;
            //StartCoroutine(StartAttack());
        }
        return;
    }

    public void GetInteractObjectType(bool isCan)
    {
        return;
    }

    public string PromptText()
    {
        return null;
    }

    public void OnInteract(CharacterObject owner)
    {
        //TODO : IF: NPC가 특정상태(나와 적대적일 때, 혹은 맞고 있을 때)일 때, 말을 거는 것이 불가능
        //TODO Idle 상태가 아니면 물건 구매나 기타 상호작용은 불가능, 말과 시비, 전투만 가능.
        VirtualCamera.Follow = owner.gameObject.transform; //NPC와 대화할 때 카메라가 NPC를 따라가도록 설정합니다.
        player = owner as Player;
        player.isTalk = true;
        PlayerInventory playerInventory = owner.GetComponent<PlayerInventory>();

        bool isIdle = StateMachine.GetState() == StateMachine.NPCIdleState;
        bool isDead = StateMachine.GetState() == StateMachine.NPCDeathState;

        //현재 Idle 상태인지, 무슨 다른 상태인지 확인 / 죽었는지 확인 추가
        NPCInteract(NPCType, isIdle, isDead);
        VirtualCamera.gameObject.SetActive(true); //NPC와 대화할 때 카메라 활성화

        if (StateMachine.GetState() != StateMachine.NPCAttackState && StateMachine.GetState() != StateMachine.NPCTargetState)
        {
            coroutine = StartCoroutine(TalkOutDistance(owner.gameObject));
            UIManager.Instance.OpenPlayerUI(GetComponent<NPCInventory>(), playerInventory);
        }
        //Debug.Log("Interact With: " + gameObject.name);
    }

    public bool IsCan()
    {
        return true; // 상호작용 가능한 상태인지 여부를 반환합니다.
    }

    public async void Look(Vector2 normalizeMouseDelta)
    {
        //await Task.Yield();

        gameObject.transform.localScale = new Vector2(1, 1);
        textMeshProUGUI.transform.localScale = new Vector2(1, 1);

        string getLook = "";
        //var normalizeMouseDelta = Agent.velocity.normalized;
        float middlePoint = 0;

        if (Mathf.Abs(normalizeMouseDelta.y) <= Mathf.Abs(normalizeMouseDelta.x))
        {
            getLook = nameof(Side);

        if (normalizeMouseDelta.x > middlePoint)
            {
                gameObject.transform.localScale = new Vector2(-1, 1);
                textMeshProUGUI.transform.localScale = new Vector2(-1, 1);
            }
        }
        else if (normalizeMouseDelta.y > 0)
        {
            getLook = nameof(Back);
        }
        else if (normalizeMouseDelta.y <= 0)
        {
            getLook = nameof(Front);
        }


        if (curLook == getLook) return;

        float stateInfoTime = Animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        //await Task.Yield();

        Animator.SetBool(hash, false);

        curLook = getLook;

        if (StateMachine.GetState() == StateMachine.NPCAttackState)
        {
            WeaponEnable();
        }

        //TODO : Front, Side, Back에 대한 전이가 한 번 더 일어나야만 함.
        if (curLook == nameof(Front))
        {
            Front.gameObject.SetActive(true);
            Animator = Front;
        }
        else
        {
            Front.gameObject.SetActive(false);
        }

        if (curLook == nameof(Side))
        {

            Side.gameObject.SetActive(true);
            Animator = Side;
        }
        else
        {
            Side.gameObject.SetActive(false);
        }

        if (curLook == nameof(Back))
        {
            Back.gameObject.SetActive(true);
            Animator = Back;
        }
        else
        {
            Back.gameObject.SetActive(false);
        }

        spriteRenderers.Clear();
        spriteRenderers = Animator.gameObject.GetComponentsInChildren<SpriteRenderer>().ToList();

        //await Task.Yield();

        Animator.StopPlayback();
        Animator.Rebind();
        Animator.SetBool(hash, true);

        //await Task.Yield();
        int animHash = Animator.GetCurrentAnimatorStateInfo(0).shortNameHash;
        Animator.Play(animHash, 0, stateInfoTime);
    }

    public void RPCChangeState(IState state)
    {
        //state
        string name = state.GetType().Name;
        //photonView.RPC(name, RpcTarget.Others);
    }

    [PunRPC]
    public void NPCIdleState()
    {
        StateMachine.ChangeState(StateMachine.NPCIdleState);
    }

    [PunRPC]
    public void NPCTargetState()
    {
        StateMachine.ChangeState(StateMachine.NPCTargetState);
    }

    [PunRPC]
    public void NPCAttackState()
    {
        StateMachine.ChangeState(StateMachine.NPCAttackState);
    }

    [PunRPC]
    public void NPCDeathState()
    {
        StateMachine.ChangeState(StateMachine.NPCDeathState);
    }

    public override void CallTakeDamage(float amount, CharacterObject attacker)
    {
        TakeDamage(amount, attacker);
        //photonView.RPC(nameof(TakeDamage), RpcTarget.All, amount, attacker);
    }

    [PunRPC]
    public override void Respawn()
    {
        Agent.ResetPath(); // NavMeshAgent의 경로를 초기화

        base.Respawn();
        SetPlace(PlaceManager.Instance.GetDefaultPlace());
        StateMachine.ChangeState(StateMachine.NPCIdleState);
    }

    [PunRPC]
    public void Move(Vector2 pos)
    {
        float speed = characterRuntimeData.moveSpeed.Current
            * Mathf.Clamp(characterRuntimeData.force.Max / characterRuntimeData.force.Current, 0.2f, 1f);

        if (characterRuntimeData.ability.ContainsKey(DesignEnums.Ability.Run))
        {
            speed += characterRuntimeData.ability[DesignEnums.Ability.Run].GetAbility()
                    * characterRuntimeData.moveSpeed.Current;
        }

        Agent.speed = speed * testSpeedValue;

        Agent.SetDestination(pos);
        //if (pos.x > transform.position.x)
        //{
        //    transform.localScale = new Vector2(1, 1);
        //    textMeshProUGUI.transform.localScale = new Vector2(1, 1);
        //}
        //else if (pos.x < transform.position.x)
        //{
        //    transform.localScale = new Vector2(-1, 1);
        //    textMeshProUGUI.transform.localScale = new Vector2(-1, 1);
        //}
    }
}
