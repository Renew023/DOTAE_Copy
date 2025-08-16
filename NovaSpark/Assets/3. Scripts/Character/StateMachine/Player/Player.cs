using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Linq;
//using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class Player : CharacterObject
{
    [field: SerializeField]
    public int CharacterID { get; private set; }


    //[field : SerializeField] public CharacterObject Character { get; private set; }
    [field: SerializeField] public PlayerStateMachine StateMachine { get; private set; }
    [field: SerializeField] public PlayerKey InputData { get; private set; }
    [field: SerializeField] public SkillSlotKey SkillSlotKey { get; private set; }
    [field: SerializeField] public PlayerInput PlayerInput { get; private set; } // 플레이어의 입력을 받는 컴포넌트, Input System 사용
    [field: SerializeField] public Interact Interact { get; private set; }
    [field: SerializeField] public LayerMask LayerMask { get; private set; } //상호작용 가능한 레이어, 실시간으로 갱신될 수 있음.
    [field: SerializeField] public Rigidbody2D RigidBody { get; private set; }
    [field: SerializeField] public PlayerRuntimeData PlayerRuntimeData { get; private set; }
    [field: SerializeField] public Inventory Inventory { get; private set; }
    [field: SerializeField] public PlayerRecipe PlayerRecipe { get; private set; } // 플레이어가 해금한 레시피를 저장 및 관리
    [field: SerializeField] public PlayerBuffData BuffData { get; private set; }

    [field: SerializeField] public CinemachineVirtualCamera VirtualCamera;

    [field: SerializeField] public CinemachineBasicMultiChannelPerlin noise;
    
    [field: SerializeField] private GameObject deadBodyPrefab;

    
    [field: Header("플레이어 애니메이션 파츠")]

    [SerializeField]
    private Light2D playerLight;

    [SerializeField]
    private float nightIntensity = 2f;

    [SerializeField]
    private float transitionDuration = 2f;

    [SerializeField]
    private AnimationCurve intensityCurve;
    private Coroutine _lightCoroutine;

    public bool isTalk = false;
    public bool isDeath = false;

    private TimeManager timeManager;

    protected override void Awake()
    {
        base.Awake();
        //photonView = GetComponent<PhotonView>();
        Interact = GetComponent<Interact>();
        PlayerInput = GetComponentInChildren<PlayerInput>();
        InputData = GetComponentInChildren<PlayerKey>();
        SkillSlotKey = GetComponentInChildren<SkillSlotKey>();
        Weapon = GetComponentInChildren<Weapon>();
        RigidBody = GetComponent<Rigidbody2D>();
        Inventory = GetComponent<PlayerInventory>();
        PlayerRecipe = GetComponent<PlayerRecipe>();
        //Animator = GetComponent<Animator>();
        BuffData = new PlayerBuffData(this);
        //Animator = Front; //기본 애니메이션은 Front로 설정
    }

    public void AddExp(int value)
    {
        UserHelpManager.Instance.CreateText($"경험치 획득 : {value}");
        var data = PlayerRuntimeData;
        data.exp += value;
        while (data.curLevel * RoomSettingData.Instance.playerLevelUpExp <= data.exp)
        {
            data.exp -= data.curLevel * RoomSettingData.Instance.playerLevelUpExp;

            LevelUp();
            //photonView.RPC(nameof(LevelUp), RpcTarget.Other);
            data.curLevel++;
        }
    }

    public void AddAbilityExp(int value)
    {
    }

    [PunRPC]
    public void LevelUp()
    {
        var data = PlayerRuntimeData;
        var chaData = characterRuntimeData;
        var weight = 1 + RoomSettingData.Instance.playerLevelUpPerStatWeights * (data.curLevel - 1);

        chaData.health.AddStat(DataManager.Instance.CharacterDataByID[characterRuntimeData.characterID].health
            * RoomSettingData.Instance.playerLevelUpStatWeight * weight * RoomSettingData.Instance.playerHPWeight);
        chaData.damage.AddStat(DataManager.Instance.CharacterDataByID[characterRuntimeData.characterID].damage
            * RoomSettingData.Instance.playerLevelUpStatWeight * weight * RoomSettingData.Instance.playerDamageWeight);
        chaData.defence.AddStat(DataManager.Instance.CharacterDataByID[characterRuntimeData.characterID].defence
            * RoomSettingData.Instance.playerLevelUpStatWeight * weight * RoomSettingData.Instance.playerDefWeight);
        //chaData.attackSpeed.AddStat(data.attackSpeed * weight);
        //chaData.criticalPercent.AddStat(data.criticalPercent * weight);
        //chaData.aimPercent.AddStat(data.aimPercent * weight);
        //chaData.missPercent.AddStat(data.missPercent * weight);
        //chaData.force.AddStat(data.force * weight);
        //chaData.moveSpeed.AddStat(data.moveSpeed * weight);
    }

    [PunRPC]
    public void AbilityLevelUp(int value)
    {

    }



    //[PunRPC]
    //public void PlayerAttack()
    //{
    //    isMove = false;
    //    Animator.SetTrigger(AnimationParameter.attackParameter);
    //    SoundManager.Instance.PlaySFXAsync(SoundParameterData.PlayerAttack_SFXParameterHash);
    //}
    //[PunRPC]
    //public void StopAttack()
    //{
    //    Animator.ResetTrigger(AnimationParameter.attackParameter);
    //    Animator.Play("Idle");
    //}

    [PunRPC]
    public override void Initialize(int key, int village = 0)
    {
        base.Initialize(key, village);
        this.PlayerRuntimeData = new PlayerRuntimeData(key);
        DialogManager.Instance.clearDialog.Add(PlayerRuntimeData.preQuestID); //플레이어 퀘스트 ID로 변경 예정.
        DialogManager.Instance.clearDialog.Add(0); //공통 Dialog
        characterRuntimeData.health.AddStat(characterRuntimeData.health.Current * (RoomSettingData.Instance.playerHPWeight - 1.0f));
        characterRuntimeData.damage.AddStat(characterRuntimeData.damage.Current * (RoomSettingData.Instance.playerDamageWeight - 1.0f));
        characterRuntimeData.defence.AddStat(characterRuntimeData.defence.Current * (RoomSettingData.Instance.playerDefWeight - 1.0f));

        playerIcon = GameObject.FindObjectOfType<FogPlayer>(true)?.gameObject;
        StateMachine = new PlayerStateMachine(this);
        StateMachine.ChangeState(StateMachine.PlayerIdleState);
        //playerIcon = UIManager.Instance.GetComponentInChildren<FogSystem>(true).gameObject;
        PlayerInput.enabled = true;

        skillNums.Clear();
        skills.Clear();

        skillNums.Add(4001);
        skills.Add(Instantiate(AddressableManager.Instance._prefabCash["4001Skill"]).GetComponent<Skill>());
        skills[0].gameObject.SetActive(false);
        
        skillNums.Add(4002);
        skills.Add(Instantiate(AddressableManager.Instance._prefabCash["4002Skill"]).GetComponent<Skill>());
        skills[1].gameObject.SetActive(false);

        skillNums.Add(4001);
        skills.Add(Instantiate(AddressableManager.Instance._prefabCash["4003Skill"]).GetComponent<Skill>());
        skills[2].gameObject.SetActive(false);

        skillNums.Add(4002);
        skills.Add(Instantiate(AddressableManager.Instance._prefabCash["4004Skill"]).GetComponent<Skill>());
        skills[3].gameObject.SetActive(false);
    }

    private IEnumerator CameraShake(float shakeSize, float shakeSpeed, float time)
    {
        noise.m_AmplitudeGain = shakeSize;
        noise.m_FrequencyGain = shakeSpeed;
        float timer = 0f;
        float curWeight = 0f;

        while(timer < time)
        {
            timer += Time.deltaTime;
            curWeight = 1 - (timer / time);

            noise.m_AmplitudeGain = shakeSize * curWeight;
            noise.m_FrequencyGain = shakeSpeed * curWeight;

            yield return null;
        }

        noise.m_AmplitudeGain = 0;
        noise.m_FrequencyGain = 0;
    }

    private void OnEnable()
    {
        timeManager = GameManager.Instance.TimeManager;
        timeManager.OnDayStateChanged += HandleDayStateChanged;
        HandleDayStateChanged(timeManager.CurrentState);
        PlayerInput.enabled = true;
    }

    protected override void OnDisable()
    {
        timeManager.OnDayStateChanged -= HandleDayStateChanged;
        base.OnDisable();
        spawnVillage.Respawn(this);
    }

    private void HandleDayStateChanged(DayState state)
    {
        if (PlaceManager.Instance.IsIndoor)
        {
            playerLight.enabled = false;
            return;
        }
        float target = state == DayState.Night ? nightIntensity : 0f;
        if (_lightCoroutine != null)
            StopCoroutine(_lightCoroutine);

        _lightCoroutine = StartCoroutine(TransitionPlayerLight(target));
    }

    IEnumerator TransitionPlayerLight(float targetIntensity)
    {
        float start = playerLight.intensity;
        float elapsed = 0f;

        playerLight.enabled = true;

        while (elapsed < transitionDuration)
        {
            float t = elapsed / transitionDuration;
            float curveT = intensityCurve != null ? intensityCurve.Evaluate(t) : t;
            playerLight.intensity = Mathf.Lerp(start, targetIntensity, curveT);
            elapsed += Time.deltaTime;
            yield return null;
        }
        playerLight.intensity = targetIntensity;
    }

    protected override void Start()
    {
        base.Start();
        StartCoroutine(GameActive());
        //if (PhotonNetwork.OfflineMode || photonView.IsMine)
        //{
        //    PlayerInput.enabled = true;
        //    Debug.Log("PlayerInput.enabled = true");
        //    DialogManager.Instance.player = this;
        //}
        //else
        //{
        //    PlayerInput.enabled = false;
        //    Debug.Log("PlayerInput.enabled = false");
        //}
        //Character = DataManager.Instance.CharacterObjectByID[CharacterID];
    }

    public void PlayerDataLoad() { }

    public void PlayerDataSave()
    {
        // 데이터 저장 시점 : 레벨업 시, 아이템 구매 시, 피해 입을 시
    }

    public void FixedUpdate()
    {
        if (isDeath == true) return;
        if (characterRuntimeData.health.Current <= 0) return;
        if (StateMachine == null) return;
        if (Animator == null) return;
        //if (PhotonNetwork.OfflineMode || photonView.IsMine)
        //{
        StateMachine?.Update();
        //}
        Look();
        BuffData.UpdateBuff(Time.fixedDeltaTime);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            transform.position = (Vector3)stream.ReceiveNext();
            transform.rotation = (Quaternion)stream.ReceiveNext();
        }
    }
    [PunRPC]
    public override void TakeDamage(float damage, CharacterObject attacker)
    {
        if (isDeath == true)
        {
            return;
        }
        if(PlayerInput.enabled == false)
        {
            return;
        }

        //SoundManager.Instance.PlaySFXAsync(SoundParameterData.PlayerTakeDamage_SFXParameterHash);

        var curHp = characterRuntimeData.health.Current;
        base.TakeDamage(damage, attacker);

        if (curHp == characterRuntimeData.health.Current)
        {
            UserHelpManager.Instance.CreateText($"회피 성공");
            return;
        }

        StartCoroutine(CameraShake(5f, 5f, 0.2f));

        if (characterRuntimeData.health.Current <= 0)
        {
            PlayerDeath();  
            //키입력 차단 (일정시간동안 안된다거나)
            //TODO : 부활 패널 생성 혹은 캐릭터 생성 씬으로 이동(방은 그대로)
        }

        if (isStun)
        {
            StateMachine.ChangeState(StateMachine.PlayerIdleState);
            return;
        }
    }

    private void PlayerDeath()
    {
        isDeath = true;
        //죽으면
        SoundManager.Instance.PlaySFXAsync(SoundParameterData.PlayerDeath_SFXParameterHash);
        PlayerInput.enabled = false;
        RoomSettingData.Instance.playerLifeCurCount -= 1;
        DefenceManager.Instance.DeathCycle();

        if (RoomSettingData.Instance.isPlayerRespawn == true && RoomSettingData.Instance.playerLifeCurCount > 0)
        {
            StateMachine.ChangeState(StateMachine.PlayerDeathState);
            spawnVillage.Respawn(this);
        }
        //PlaceManager.Instance.Initialize();
        else
        {
            //정말 죽었을 때
            DefenceManager.Instance.DefenceModeDeath();
            OnPlayerDeath(this);
        }
    }

    //Image hungryBar;
    //Image thirstyBar;

    public IEnumerator GameActive()
    { 
        while(true)
        {
            if (isDeath == true) yield break;
            yield return new WaitForSeconds(1f);
            if (PlayerRuntimeData.thirsty.Current <= 0)
            {
                if (RoomSettingData.Instance.hungryThirstyZeroDeath == true)
                {
                    PlayerDeath();
                    continue;
                }
                else
                {
                    characterRuntimeData.health.Current -= RoomSettingData.Instance.thirstyPerSeconds
                        * RoomSettingData.Instance.hungryThirstyZeroHealthMinus; 
                }
            }
            else
            {
                PlayerRuntimeData.thirsty.Current -= RoomSettingData.Instance.thirstyPerSeconds;
            }

            if(PlayerRuntimeData.hungry.Current <= 0)
            {
                if (RoomSettingData.Instance.hungryThirstyZeroDeath == true)
                {
                    PlayerDeath();
                    continue;
                }
                else
                {
                    characterRuntimeData.health.Current -= RoomSettingData.Instance.hungryPerSeconds
                        * RoomSettingData.Instance.hungryThirstyZeroHealthMinus;
                }
            }
            else
            {
                PlayerRuntimeData.hungry.Current -= RoomSettingData.Instance.hungryPerSeconds;
            }

            //thirstyBar.fillAmount = PlayerRuntimeData.thirsty.Current / PlayerRuntimeData.thirsty.Max;
            //hungryBar.fillAmount = PlayerRuntimeData.hungry.Current / PlayerRuntimeData.hungry.Max;

            if (PlayerRuntimeData.hungry.Current < 0 || PlayerRuntimeData.thirsty.Current < 0) continue;

            var healHp = characterRuntimeData.health.Current
                + RoomSettingData.Instance.regenerationPerSeconds;
                
                //+ characterRuntimeData.ability[DesignEnums.Ability.Regeneration].GetAbility()
                //* RoomSettingData.Instance.regenerationPerSeconds;

            characterRuntimeData.health.Current = Mathf.Min(healHp, characterRuntimeData.health.Max);
        }
    }

    [PunRPC]
    public override void Respawn()
    {
        base.Respawn();
        isDeath = false;
        StatReset();

        VirtualCamera.m_Lens.OrthographicSize = VirtualCamera.m_Lens.OrthographicSize * 2;
        StartCoroutine(GameActive());

        Vector2 randomPos = transform.position + (Vector3)(Random.insideUnitCircle * 5f);
        RigidBody.MovePosition(randomPos);
        //if (photonView.IsMine)
        //{
        //}
        StateMachine.ChangeState(StateMachine.PlayerIdleState);
        PlayerInput.enabled = true;
    }
    
    private void StatReset()
    {
        PlayerRuntimeData.hungry.Current = PlayerRuntimeData.hungry.Max;
        PlayerRuntimeData.thirsty.Current = PlayerRuntimeData.thirsty.Max;
        characterRuntimeData.health.Reset();
    }

    public void RPCChangeState(IState state)
    {
        //state
        string name = state.GetType().Name;
        //photonView.RPC(name, RpcTarget.Others);
    }

    [PunRPC]
    public void PlayerIdleState()
    {
        StateMachine.ChangeState(StateMachine.PlayerIdleState);
    }

    [PunRPC]
    public void PlayerMoveState()
    {
        StateMachine.ChangeState(StateMachine.PlayerMoveState);
    }

    [PunRPC]
    public void PlayerRunState()
    {
        StateMachine.ChangeState(StateMachine.PlayerRunState);
    }

    [PunRPC]
    public void PlayerAttackState()
    {
        StateMachine.ChangeState(StateMachine.PlayerAttackState);
    }

    public override void CallTakeDamage(float amount, CharacterObject attacker)
    {
        TakeDamage(amount, attacker);
        //photonView.RPC(nameof(TakeDamage), RpcTarget.Other, amount, attackerViewID);
    }

    [SerializeField] private GameObject playerIcon; // 미니맵 속 플레이어
    [field: SerializeField] public Vector2 ClickPos { get; private set; }
    [field: SerializeField] public Vector2 Direction { get; private set; }
    public float angle;
    private float _distance;

    public bool isRun = false;
    //public bool isAttack = true;
    public bool isMove = false;
    public bool isSit = false;

    public void Point()
    {
        //if (photonView.IsMine)
        //{
            Vector2 mousePosition = Input.mousePosition;
            ClickPos = Camera.main.ScreenToWorldPoint(mousePosition);
            SetPoint(ClickPos);
            //photonView.RPC(nameof(SetPoint), RpcTarget.Other, ClickPos);
        //}
    }

    public bool isClickPos = false;

    [PunRPC]
    public void SetPoint(Vector2 clickPos)
    {
        isMove = true;
        isClickPos = true;
        ClickPos = clickPos;
        StateMachine.ChangeState(StateMachine.PlayerIdleState);
        StateMachine.ChangeState(StateMachine.PlayerMoveState);
    }

    public void MoveStop()
    {
        this.RigidBody.velocity = Vector2.zero;
        isClickPos = false;
    }

    public void MoveIdle(Vector2 moveSize)
    {
        if (isClickPos) MoveStop();

        Vector2 speed = moveSize.normalized
            * characterRuntimeData.moveSpeed.Current *
            (isRun ? 2f : 1f);

        if (characterRuntimeData.ability.ContainsKey(DesignEnums.Ability.Run))
        {
            speed += moveSize.normalized * Vector2.one * characterRuntimeData.ability[DesignEnums.Ability.Run].GetAbility()
                    * PlayerRuntimeData.moveSpeed;
        }

        this.RigidBody.velocity = speed * testSpeedValue;
        StateMachine.ChangeState(StateMachine.PlayerMoveState);
    }

    public void Move() // 도착지 + 이동까지 엮여잇음.
    {
        Vector2 angle = (ClickPos - (Vector2)this.transform.position).normalized;

        Vector2 speed = angle
            * characterRuntimeData.moveSpeed.Current *
            (isRun ? 2f : 1f)
            * Mathf.Clamp(characterRuntimeData.force.Max / characterRuntimeData.force.Current, 0.2f, 1f);

        if(characterRuntimeData.ability.ContainsKey(DesignEnums.Ability.Run))
        {
            speed += angle * Vector2.one * characterRuntimeData.ability[DesignEnums.Ability.Run].GetAbility()
                    * PlayerRuntimeData.moveSpeed;
        }

        this.RigidBody.velocity = speed * testSpeedValue;
        ; //무게에 따른 속도
        _distance = Vector2.Distance(this.transform.position, ClickPos);

        if (_distance <= this.RigidBody.velocity.magnitude * 0.05f)
        {
            MoveStop();
            isMove = false;
        }

        if (playerIcon != null) //&& playerIcon.transform.parent != null
        {
            float parentScaleX = playerIcon.transform.parent.localScale.x;
            playerIcon.transform.localPosition =
            (Vector2)this.transform.localPosition / parentScaleX;
        }
        else
        {
            Debug.LogWarning("[PlayerKey] playerIcon or its parent is null, skipping minimap update.");
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if(StateMachine.GetState() == StateMachine.PlayerMoveState ||
           StateMachine.GetState() == StateMachine.PlayerRunState)
        {
            StateMachine.ChangeState(StateMachine.PlayerIdleState);
        }
    }

    [PunRPC]
    public void Sit()
    {
        isSit = !isSit;

        if (isSit)
        {
            ClickPos = transform.position;
        }
    }

    [PunRPC]
    public void Run(bool isRun)
    {
        this.isRun = isRun;
    }

    public void Look()
    {
        Vector2 mouseDelta = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Direction = mouseDelta;
        float middlePoint = Camera.main.transform.position.x;
        SetLook(mouseDelta, middlePoint);
        //photonView.RPC(nameof(SetLook), RpcTarget.Other, mouseDelta, middlePoint);
    }

    [PunRPC]
    public async void SetLook(Vector2 mouseDelta, float middlePoint)
    {

        //await Task.Yield();

        gameObject.transform.localScale = new Vector2(1, 1);
        textMeshProUGUI.transform.localScale = new Vector2(1, 1);

        string getLook = "";
        var normalizeMouseDelta = (mouseDelta- (Vector2)gameObject.transform.position).normalized;
        
        if (Mathf.Abs(normalizeMouseDelta.y) <= Mathf.Abs(normalizeMouseDelta.x))
        {
            getLook = nameof(Side);

            if (mouseDelta.x > middlePoint)
            {
                gameObject.transform.localScale = new Vector2(-1, 1);
                textMeshProUGUI.transform.localScale = new Vector2(-1, 1);
            }
        }
        else if (normalizeMouseDelta.y > 0)
        {
            getLook = nameof(Back);
        }
        else if(normalizeMouseDelta.y <= 0)
        {
            getLook = nameof(Front);
        }


        if(curLook == getLook) return;

        float stateInfoTime = Animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        foreach(var sprite in spriteRenderers)
        {
            sprite.color = Color.white;
        }

        //await Task.Yield();

        Animator.SetBool(hash, false);

        curLook = getLook;

        if (StateMachine.GetState() == StateMachine.PlayerAttackState)
        {
            WeaponEnable();
        }

        //TODO : Front, Side, Back에 대한 전이가 한 번 더 일어나야만 함.
        if (curLook == nameof(Front))
        {
            Front.gameObject.SetActive(true);
            Animator = Front;
        }

        if (curLook == nameof(Side))
        {

            Side.gameObject.SetActive(true);
            Animator = Side;
        }

        if (curLook == nameof(Back))
        {
            Back.gameObject.SetActive(true);
            Animator = Back;
        }

        Front.gameObject.SetActive(curLook == nameof(Front));
        Side.gameObject.SetActive(curLook == nameof(Side));
        Back.gameObject.SetActive(curLook == nameof(Back));

        spriteRenderers.Clear();
        spriteRenderers = Animator.gameObject.GetComponentsInChildren<SpriteRenderer>(true).ToList();

        //await Task.Yield();

        Animator.StopPlayback();
        Animator.Rebind();
        Animator.SetBool(hash, true);
        
        //await Task.Yield();
        int animHash = Animator.GetCurrentAnimatorStateInfo(0).shortNameHash;
        Animator.Play(animHash, 0, stateInfoTime);
    }

    [PunRPC]
    public void Attack()
    {
        if (isTalk) return;
        if (isAttack)
        {
            //공격
            isAttack = false;
            Animator.speed = characterRuntimeData.attackSpeed.Current;
            StateMachine.ChangeState(StateMachine.PlayerAttackState);
            return;
        }
        return;
    }

    public void OnPlayerDeath(Player player)
    {
        return;
        Vector3 deathPos = player.transform.position;

        // 데드바디 생성
        GameObject deadBodyObj = Instantiate(deadBodyPrefab, deathPos, Quaternion.identity);

        // 데드바디 컨트롤러 컴포넌트 가져오기
        DeadBodyController dbc = deadBodyObj.GetComponent<DeadBodyController>();
        if (dbc != null)
        {
            dbc.InitFromPlayer(player);
        }
        else
        {
            Debug.LogError("DeadBodyController 컴포넌트가 데드바디에 없습니다!");
        }

        // 플레이어 오브젝트 비활성화 또는 제거
        gameObject.SetActive(false);
    }

    public void AddBuffStat(DesignEnums.StatType statType, float value)
    {
        switch (statType)
        {
            case DesignEnums.StatType.Damage:
                characterRuntimeData.damage.AddStat(value);
                break;
            case DesignEnums.StatType.Defence:
                characterRuntimeData.defence.AddStat(value);
                break;
            case DesignEnums.StatType.Hp:
                characterRuntimeData.health.AddStat(value);
                break;
            case DesignEnums.StatType.AttackSpeed:
                characterRuntimeData.attackSpeed.AddStat(value);
                break;
            case DesignEnums.StatType.Speed:
                characterRuntimeData.moveSpeed.AddStat(value);
                break;
            // 필요한 스탯 계속 추가
            default:
                Debug.LogWarning($"[AddStat] 정의되지 않은 스탯 타입: {statType}");
                break;
        }
    }
}
