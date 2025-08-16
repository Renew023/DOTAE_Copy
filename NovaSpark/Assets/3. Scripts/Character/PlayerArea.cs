using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerArea : MonoBehaviour
{
    [SerializeField] private Player owner;
    [SerializeField] private CircleCollider2D circleCollider2D;
    [SerializeField] private GameObject monsterPrefab;

    [SerializeField] private float minArea = 15f;
    [SerializeField] private float maxArea = 20f;
    [SerializeField] private float playerDistance = 5f;

    [SerializeField] private TimeManager timeManager;
    [SerializeField] private LayerMask villageLayerMask;
    public LayerMask GroundCheck; //땅 위에만 생성되도록.
    private bool isIngameSound;

    private void Awake()
    {
        circleCollider2D = GetComponent<CircleCollider2D>();
        circleCollider2D.radius = playerDistance;
        monsterPrefab.AddPhoton();
        owner = GetComponentInParent<Player>();
    }

    private void Start()
    {
        timeManager = GameManager.Instance.TimeManager;
        //각 플레이어의 주변에 생기는 거랑 마스터 클라이언트가 관리할 필요가.. 음.
        StartCoroutine(MonsterWave());
        StartCoroutine(MonsterSpawn());       
        DefenceManager.Instance.Spawn(RoomSettingData.Instance.startNpcSpawnCount, this.transform.localPosition);
    }
    
    private void Init()
    {
        //날짜에서 쏴줘야.
        //몬스터 생성.
        // -> 이벤트가 호출되면 하루 지난 걸 알고 이틀 지났는지 3일 지났는지 데이카운트 확인하고
        // -> secondsPerDay를 랜덤 값 돌리고.
    }

    private IEnumerator MonsterWave()
    {
        if (RoomSettingData.Instance.monsterMaxCount < DefenceManager.Instance.totalMonsterCount) yield break;
        
        float timer = 0f;
        while(timer < RoomSettingData.Instance.waveStartDelayTime && !RoomSettingData.Instance.isWaveStartButtonActive)
        {
            timer += Time.deltaTime;
            DefenceManager.Instance.waveCountSecondsText.text = $"웨이브 타이머 시작까지 : {(RoomSettingData.Instance.waveStartDelayTime - timer):F1} 초";
            DefenceManager.Instance.waveCountFill.fillAmount = (RoomSettingData.Instance.waveStartDelayTime - timer) / RoomSettingData.Instance.waveStartDelayTime;
            yield return null;
        }

        DefenceManager.Instance.waveCountFill.fillAmount = 1f;

        if (RoomSettingData.Instance.isWaveStartButtonActive)
        {
            DefenceManager.Instance.waveCountSecondsText.text = $"게임 시작 버튼을 눌러주세요";
            DefenceManager.Instance.waveStartButton.gameObject.SetActive(true);
            yield return new WaitUntil(() => DefenceManager.Instance.isWaveCheck == true);
        }

        DefenceManager.Instance.waveCountSecondsText.text = "타이머가 움직이기 시작합니다";

        while (true)
        {
            DesignEnums.VillageType village = new();
            List<int> family = new();

            foreach (var character in DataManager.Instance.CharacterDataByID.Values)
            {
                if (character.characterID < 100 || character.characterID > 500) continue;
                if (character.Level > 1) continue;
                // if (character.familyType == village)
                // {
                // }
                    family.Add(character.characterID);
            }

            int count = 0;
            int day = timeManager.DayCount;
            float time = timeManager.GetSecondPerDay();

            int monsterCount = Random.Range((RoomSettingData.Instance.spawnMonsterWaveMin)
                , (RoomSettingData.Instance.spawnMonsterWaveMax)) + (int)(RoomSettingData.Instance.monsterWaveSpawnValue * DefenceManager.Instance.waveCount);

            float range = 1.0f;
            float spawnTime = Random.Range(RoomSettingData.Instance.monsterWaveMinSeconds, RoomSettingData.Instance.monsterWaveMaxSeconds) * time
                - time * (RoomSettingData.Instance.monsterWaveSpawnSecondsValue * DefenceManager.Instance.waveCount);

            float timeCount = 0;

            while(timeCount < spawnTime)
            {
                if (DefenceManager.Instance.waveMonsterCount == 0 && isIngameSound == false)
                {
                    isIngameSound = true;
                    SoundManager.Instance.StopBGM(2f);
                    SoundManager.Instance.PlayBGMAsync(SoundParameterData.IngameBGM, 2f);
                }
                if (RoomSettingData.Instance.waveNotClearSpawnTime)
                {
                    if (DefenceManager.Instance.waveMonsterCount != 0)
                    {
                        timeCount += Time.deltaTime * RoomSettingData.Instance.waveNotClearTime;
                    }
                    else
                    {
                        if (spawnTime * RoomSettingData.Instance.waveClearMinTime > timeCount)
                        {
                            timeCount = spawnTime * RoomSettingData.Instance.waveClearMinTime;
                        }

                        timeCount += Time.deltaTime;
                    }

                }
                else
                {
                    timeCount += Time.deltaTime;
                }
                DefenceManager.Instance.waveCountSecondsText.text = $"다음 웨이브까지 : {(spawnTime - timeCount):F1} 초";
                DefenceManager.Instance.waveCountFill.fillAmount = (spawnTime - timeCount) / spawnTime;
                yield return null;
            }

            DefenceManager.Instance.waveCountSecondsText.text = $"몬스터 소환 중";
            DefenceManager.Instance.waveCountFill.fillAmount = 1f;

            if (DefenceManager.Instance.waveCount > 1)
            {
                UserHelpManager.Instance.CreateText($"웨이브 {DefenceManager.Instance.waveCount} 도달 보상");

                owner.AddExp(
                    (int)(RoomSettingData.Instance.waveClearExp
                    + RoomSettingData.Instance.waveClearExp
                    * RoomSettingData.Instance.waveClearCountWeight
                    * DefenceManager.Instance.waveCount));

                owner.GetComponent<PlayerInventory>().AddGold(
                    (int)(RoomSettingData.Instance.waveClearGold
                    + RoomSettingData.Instance.waveClearGold
                    * RoomSettingData.Instance.waveClearCountWeight
                    * DefenceManager.Instance.waveCount));

                UserHelpManager.Instance.CreateText("_");
            }

            //yield return new WaitForSeconds(spawnTime);

            while (count < monsterCount)
            {
                if (RoomSettingData.Instance.monsterMaxCount < DefenceManager.Instance.totalMonsterCount)
                {
                    break;
                }
                int spawnMonsterID = family[Random.Range(0, family.Count)];

                Vector2 point = Random.insideUnitCircle * Random.Range(minArea, maxArea) * range + (Vector2)transform.position;
                //new Vector2(Random.Range(minArea, maxArea)*range, Random.Range(minArea, maxArea)*range);
                
                Collider2D ground = Physics2D.OverlapCircle(point, 0.2f, GroundCheck);
                if (ground == null)
                {
                    UtilityCode.Logger2("땅 위가 아님");
                    continue; // 땅 위에만 생성
                }
                //Debug.Log(point);
                Collider2D target = Physics2D.OverlapCircle(point, 1f, villageLayerMask);
                if (target != null)
                {
                    range += 0.1f;
                    continue;
                }

                GameObject enemyGo = Instantiate(monsterPrefab, point, Quaternion.identity);
                var enemy = enemyGo.GetComponent<Enemy>();

                enemy.Initialize(spawnMonsterID, 0);

                //enemy.Agent.Warp(point);
                enemy.SetTarget(owner);

                //GameObject enemyGo = PhotonNetwork.InstantiateRoomObject(monsterPrefab.name, point, Quaternion.identity);

                //var enemy = enemyGo.GetComponent<Enemy>();
                //enemy.photonView.RPC(nameof(enemy.Initialize), RpcTarget.All, spawnMonsterID);
                //enemy.photonView.RPC(nameof(enemy.SetTarget), RpcTarget.All, owner.photonView.ViewID);

                DefenceManager.Instance.waveMonsterCount++;
                DefenceManager.Instance.totalMonsterCount++;
                count++;
            }
            SoundManager.Instance.StopBGM(2f);
            SoundManager.Instance.PlayBGMAsync(SoundParameterData.BattleBGM, 2f);
            isIngameSound = false;
            DefenceManager.Instance.waveCount++;
            DefenceManager.Instance.TextReload();
            yield return null;

            if (RoomSettingData.Instance.isWaveClearMonsterSpawn)
            {
                DefenceManager.Instance.waveCountSecondsText.text = $"몬스터가 아직 남아있습니다.";
                DefenceManager.Instance.waveCountFill.fillAmount = 1f;
                yield return new WaitUntil(() => DefenceManager.Instance.totalMonsterCount == 0);
            }

            DefenceManager.Instance.Spawn(RoomSettingData.Instance.waveNpcCount, this.transform.localPosition);

        }
    }
    private IEnumerator MonsterSpawn()
    {
        if (!RoomSettingData.Instance.isRandomMonsterSpawn) yield break;
        if (RoomSettingData.Instance.monsterMaxCount < DefenceManager.Instance.totalMonsterCount) yield break;

        while (true)
        {
            DesignEnums.VillageType village = new();
            List<int> family = new();

            foreach (var character in DataManager.Instance.CharacterDataByID.Values)
            {
                if (character.characterID < 100 || character.characterID > 500) continue;
                if(character.Level > 1) continue;
                // if (character.familyType == village)
                // {
                // }
                family.Add(character.characterID);
            }

            int count = 0;

            int monsterCount = 1;
            
            Collider2D curPoint = Physics2D.OverlapCircle(transform.position, 1f, villageLayerMask);
            if (curPoint != null) continue;

            float range = 1.0f;
            float spawnTime = Random.Range(RoomSettingData.Instance.spawnMonsterMinLoopSeconds, RoomSettingData.Instance.spawnMonsterMaxLoopSeconds) * timeManager.GetSecondPerDay();

            yield return new WaitForSeconds(spawnTime);

            while (count < monsterCount)
            {
                if (RoomSettingData.Instance.monsterMaxCount < DefenceManager.Instance.totalMonsterCount) yield break;
                int spawnMonsterID = family[Random.Range(0, family.Count)];

                Vector2 point = Random.insideUnitCircle * Random.Range(minArea, maxArea) * range + (Vector2)transform.position;
                //new Vector2(Random.Range(minArea, maxArea) * range, Random.Range(minArea, maxArea) * range);
                Collider2D ground = Physics2D.OverlapCircle(point, 0.2f, GroundCheck);
                if (ground == null)
                {
                    UtilityCode.Logger2("땅 위가 아님");
                    continue; // 땅 위에만 생성
                }

                Collider2D target = Physics2D.OverlapCircle(point, 1f, villageLayerMask);
                if (target != null)
                {
                    range += 0.1f;
                    continue;
                }

                GameObject enemyGo = Instantiate(monsterPrefab, point, Quaternion.identity);
                var enemy = enemyGo.GetComponent<Enemy>();

                enemy.Initialize(spawnMonsterID, 0);

                //enemy.Agent.Warp(point);

                //GameObject enemyGo = PhotonNetwork.InstantiateRoomObject(monsterPrefab.name, point, Quaternion.identity);

                //var enemy = enemyGo.GetComponent<Enemy>();
                //enemy.photonView.RPC(nameof(enemy.Initialize), RpcTarget.All, spawnMonsterID, 0);
                //enemy.photonView.RPC(nameof(enemy.SetTarget), RpcTarget.All, owner.photonView.ViewID);
                DefenceManager.Instance.totalMonsterCount++;
                DefenceManager.Instance.TextReload();

                count++;
            }
            yield return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Village>(out Village villagePoint))
        {
            DialogManager.Instance.AddQuestSuccess(villagePoint.villageNumber, DesignEnums.QuestEvent.Arrival); //장소를 발견하고 그 장소를 들어가면 체크포인트가 찍힘.
            UtilityCode.ErrorLogger("당신은 사유지를 침범하고 있습니다.");
            //TODO : 이후 최적화 관련으로 사용하기 위해 Collider를 키우고, Arrival 판단은 Player가 해도 상관 없음.
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, minArea);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, maxArea);
    }
}
