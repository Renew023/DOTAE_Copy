using Photon.Pun;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;


public class Enemy : CharacterObject
{
    //TODO : 친밀도

    //NavMesh

    [field: SerializeField] public int CharacterID { get; private set; }
    [field: SerializeField] public EnemyStateMachine StateMachine { get; private set; }

    //public bool isAttack = false; //공격 중인지? TODO : 나중에 애니메이터로 판별
    [field: SerializeField] public NavMeshAgent Agent { get; protected set; } //NavMeshAgent를 사용하여 이동을 관리합니다. //AI들은.. 따로 관리

    [field: SerializeField] public CharacterObject MainTarget { get; private set; }
    [field: SerializeField] public CharacterObject Target { get; protected set; }
    [field: SerializeField] public LayerMask EnemyLayer { get; set; } //적대적인 성향의 레이어, 실시간으로 갱신될 수 있음.
    [field: SerializeField] public LayerMask LayerMask { get; protected set; } //상호작용 가능한 레이어, 실시간으로 갱신되지는 않음.

    [field :SerializeField] public EnemyRuntimeData EnemyRuntimeData { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        Agent = GetComponent<NavMeshAgent>();
        Animator = GetComponent<Animator>();
        Agent.updateRotation = false; // NavMeshAgent가 회전을 자동으로 처리하지 않도록 설정합니다.
        Agent.updateUpAxis = false; // NavMeshAgent가 Y축을 자동으로 업데이트하지 않도록 설정합니다.

        StateMachine = new EnemyStateMachine(this);
    }

    [PunRPC]
    public override void Initialize(int key, int village=0)
    {
        base.Initialize(key, village);
        this.EnemyRuntimeData = new EnemyRuntimeData(key);
        textMeshProUGUI.color = Color.red;
        characterRuntimeData.DefenceSpawnStatus(GameManager.Instance.TimeManager.DayCount);
        characterRuntimeData.health.AddStat(characterRuntimeData.health.Current * (RoomSettingData.Instance.enemyHPWeight - 1.0f));
        characterRuntimeData.damage.AddStat(characterRuntimeData.damage.Current * (RoomSettingData.Instance.enemyDamageWeight - 1.0f));
        characterRuntimeData.defence.AddStat(characterRuntimeData.defence.Current * (RoomSettingData.Instance.enemyDefWeight - 1.0f));
        characterRuntimeData.moveSpeed.AddStat(characterRuntimeData.moveSpeed.Current * RoomSettingData.Instance.AISpeedWeight - 1.0f);
    }

    //TODO : 오브젝트 풀용 Enabled
    private void OnEnable()
    {
        StateMachine.ChangeState(StateMachine.EnemyIdleState);
    }

    protected override void Start()
    {
        base.Start();
        PlaceManager.Instance.aiObjects.Add(this);
        //Character = DataManager.Instance.CharacterObjectByID[CharacterID];
    }

    public void FixedUpdate()
    {
        if (characterRuntimeData.health.Current < 0) return;
        if (StateMachine == null) return;
        if (Animator == null) return;
        
        StateMachine?.Update();

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
        if (curPlace = PlaceManager.Instance.curPlace)
        {
            foreach(var sprite in spriteRenderers)
            {
                sprite.enabled = true;
            }
        }
        else
        {
            foreach (var sprite in spriteRenderers)
            {
                sprite.enabled = false;
            }
        }
    }

    public void AttackBall()
    {
        GameObject attackBall = Instantiate(AddressableManager.Instance._prefabCash["AttackBall"], transform.position, Quaternion.identity);
        Vector3 posNormalized = (Target.transform.position - transform.position).normalized;
        attackBall.GetComponent<DamageBall>().Initialize(posNormalized, this);
    }

    public bool AttackDistance() //TODO : 친한 애인지 선공 몹인지에 대한 식별 여부 필요.
    {
        if (Target == null) return false;
        if (!EnemyRuntimeData.isFirst) return false;

        float distance = Vector3.Distance(transform.position, Target.transform.position);

        //playerStat.attackRange

        float attackDistanceWeight = 1.0f;
        if(attackType == AttackType.Magic)
        {
            attackDistanceWeight *= 5.0f;
        }

        if (distance <= Weapon.gameObject.transform.localScale.x * RoomSettingData.Instance.attackDistance * attackDistanceWeight)
        {
            return true;
        }

        return false;
    }

    [PunRPC]
    public void SetTarget(CharacterObject characterObject)
    {
        //if(PhotonNetwork.IsMasterClient == false) return; // 마스터 클라이언트만 타겟을 설정할 수 있습니다.
        MainTarget = characterObject;
        Target = MainTarget;
        //Target = PhotonView.Find(targetViewID).GetComponent<CharacterObject>();
        StateMachine.ChangeState(StateMachine.EnemyFollowState);
    }
    
    public bool TargetingEnemy(float radius)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, EnemyLayer); //NonAlloc 방식으로 Collider2D를 가져오는 코드가 필요.
        foreach (var collider in colliders)
        {
            if (collider.gameObject == this.gameObject) continue; // 자기 자신은 제외
            if (collider.gameObject.tag == gameObject.tag) continue;

            if (collider.TryGetComponent<CharacterObject>(out var characterObject))
            {
                Target = characterObject;
                return true;
            }
        }
        if (isFollowState)
        {
            Target = MainTarget;
        }
        else
            Target = null; // 적을 찾지 못했을 경우 Target을 null로 설정

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
        if (characterRuntimeData.health.Current <= 0)
        {
            return; // 이미 죽은 상태라면 데미지를 받지 않습니다.
        }

        var curHp = characterRuntimeData.health.Current;
        base.TakeDamage(damage, attacker);

        if (curHp == characterRuntimeData.health.Current)
        {
            UserHelpManager.Instance.CreateText($"회피 성공");
            StateMachine.ChangeState(StateMachine.EnemyAttackState);
            return;
        }
        //효과음
        //SoundManager.Instance.PlaySFXAsync(SoundParameterData.AITakeDamage_SFXParameterHash);

        if (characterRuntimeData.health.Current <= 0)
        {
            DefenceManager.Instance.totalMonsterCount--;
            if(isFollowState == true)
            {
                DefenceManager.Instance.waveMonsterCount--;
            }
            DefenceManager.Instance.TextReload();
            DefenceManager.Instance.killCount++;
            if(attacker.TryGetComponent(out Player player))
            {
                //if (player.photonView.IsMine)
                //{
                    var data = EnemyRuntimeData;
                    player.AddExp((int)(RoomSettingData.Instance.isKillExpPercent));
                    DialogManager.Instance.AddQuestSuccess(characterRuntimeData.characterID, DesignEnums.QuestEvent.Kill);
                //}
                DropItemsAndGold(player);
            }
            //죽으면 
            //TODO : 죽는 애니메이션
            //15초 후 맵에서 삭제
            StateMachine.ChangeState(StateMachine.EnemyDeathState);
            return;
        }
        //패링 기능
        if (isStun)
        {
            StateMachine.ChangeState(StateMachine.EnemyIdleState);
            return;
        }
    }

    public IEnumerator DeadEnemy()
    {
        SoundManager.Instance.PlaySFXAsync(SoundParameterData.AIDeath_SFXParameterHash);
        yield return new WaitForSeconds(10f);
        StateMachine.ChangeState(StateMachine.EnemyIdleState);
        gameObject.SetActive(false);
        //TODO : 오브젝트 풀링 가능
    }

    protected override void OnDisable()
    {
        base.OnDisable();
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
    public void RPCChangeState(IState state)
    {
        //state
        string name = state.GetType().Name;
        //photonView.RPC(name, RpcTarget.Others);
    }

    [PunRPC]
    public void EnemyIdleState()
    {
        StateMachine.ChangeState(StateMachine.EnemyIdleState);
    }

    [PunRPC]
    public void EnemyDeathState()
    {
        StateMachine.ChangeState(StateMachine.EnemyDeathState);
    }

    [PunRPC]
    public void EnemyWanderState()
    {
        StateMachine.ChangeState(StateMachine.EnemyWanderState);
    }

    [PunRPC]
    public void EnemyTargetState()
    {
        StateMachine.ChangeState(StateMachine.EnemyTargetState);
    }

    [PunRPC]
    public void EnemyAttackState()
    {
        StateMachine.ChangeState(StateMachine.EnemyAttackState);
    }

    public bool isFollowState = false;

    [PunRPC]
    public void EnemyFollowState()
    {
        StateMachine.ChangeState(StateMachine.EnemyFollowState);
    }

    public override void CallTakeDamage(float amount, CharacterObject attacker)
    {
        TakeDamage(amount, attacker);
        //photonView.RPC(nameof(TakeDamage), RpcTarget.All, amount, attacker);
    }

    [PunRPC]
    public void WanderPoint(Vector2 point)
    {
        Move(point);
        StateMachine.ChangeState(StateMachine.EnemyWanderState);
    }

    public Vector2 GetWanderLocation() //이것만 MasturClient에서.
    {
        // 랜덤한 위치를 생성하여 NavMesh 상에서 유효한 위치로 반환

        float minDistance = 3f;
        float maxDistance = 10f;
        Vector3 randomDirection = UnityEngine.Random.insideUnitCircle * maxDistance;
        float distance = Vector2.Distance(Vector3.zero, randomDirection);

        while (distance < minDistance) // 3f 이상인 위치를 찾을 때까지 반복
        {
            randomDirection = UnityEngine.Random.insideUnitCircle * maxDistance;
            distance = Vector2.Distance(Vector3.zero, randomDirection);
        }

        randomDirection += transform.position;

        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, maxDistance, NavMesh.AllAreas);

        return hit.position;
    }

    [PunRPC]
    public override void Respawn()
    {
        base.Respawn();
        SetPlace(PlaceManager.Instance.GetDefaultPlace());
        StateMachine.ChangeState(StateMachine.EnemyIdleState);
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

        if(StateMachine.GetState() == StateMachine.EnemyAttackState)
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

    private void DropItemsAndGold(Player player)
    {
        var data = EnemyRuntimeData;
        var enemyData = DataManager.Instance.EnemyDataByID[characterRuntimeData.characterID];

        for (int i = 0; i < enemyData.DropItemIDs.Count; i++)
        {
            int itemID = enemyData.DropItemIDs[i];
            float dropRate = enemyData.DropRates[i];
            int maxCount = enemyData.DropCounts[i];

            float rand = Random.Range(0f, 100f); // 0~1

            if (rand <= dropRate)
            {
                Item item = DataManager.Instance.GetItemByID(itemID);
                int count = Random.Range(1, maxCount + 1); // 1~maxCount
                player.Inventory.AddItem(item, count);
                Debug.Log($"플레이어에게 아이템 {itemID} x{count} 지급");
            }
        }

        int gold = Random.Range(enemyData.DropGoldMin, enemyData.DropGoldMax + 1);
        player.Inventory.AddGold(gold);
        Debug.Log($"플레이어에게 골드 {gold} 지급");
    }
}
