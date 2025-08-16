using Photon.Pun;
using Photon.Pun.Demo.Procedural;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class CreateGame : Singleton<CreateGame>
{
    public Village CurGround;

    public GameObject curSpawnPoint; // 플레이어 주위 어딘가에 배치되었다가 현재 타일이 Collider로 식별되면 생성하지 않고 이동할거임.

    public List<Village> playerSpawnPoint;
    public List<Village> enemySpawnPoint; //에너미 같은 경우 군집일 때만 해당.
    public List<Village> NPCSpawnPoint;
    
    public Dictionary<int, Village> SpawnPointDict = new(); //단순한 장소를 의미하는 것이기에 int 가능. 
                                                              //튜플을 활용하여 (Enum<Place>, int) 구조로 하면 용의 둥지 몇 번째라는 구조도 가능.
    public LayerMask GroundCheck; //땅 위에만 생성되도록.
    public LayerMask NotSetBlock; //건축물 위, 구조물 위

    public float minDistance = 0.2f;
    public float maxDistacne = 1f;

    public Vector2Int mapSizeMin; //맵의 크기
    public Vector2Int mapSizeMax;

    protected override void Awake()
    {
        base.Awake();

        for (int i = 0; i < playerSpawnPoint.Count; i++)
        {
            //playerSpawnPoint[i].villageNumber = i + 1;
            var key = playerSpawnPoint[i];
            SpawnPointDict[key.villageNumber] = key;
        }

        for (int i = 0; i < enemySpawnPoint.Count; i++)
        {
            enemySpawnPoint[i].villageNumber = i + 501;
            var key = enemySpawnPoint[i];
            SpawnPointDict[key.villageNumber] = key;
        }

        for (int i = 0; i < NPCSpawnPoint.Count; i++)
        {
            NPCSpawnPoint[i].villageNumber = i + 1001;
            var key = NPCSpawnPoint[i];
            SpawnPointDict[key.villageNumber] = key;
        }

        //if (!PhotonNetwork.IsMasterClient) return
        // 150001부터 시작하는 이유는 150000까지는 플레이어가 소유한 블럭이기 때문.
    }

    private void Start()
    {
        MapSpawn(RoomSettingData.Instance.farmingBlockSettingCount);
        GameManager.Instance.TimeManager.OneDayPassed += () => MapSpawn(RoomSettingData.Instance.farmingBlockPerOneDayCount);
    }

    public void MapSpawn(int value)
    {
        int totalValue = (int)(GameManager.Instance.TimeManager.DayCount  
            * RoomSettingData.Instance.farmingBlockPerOneDayWeight) + value;

        MapInit(150001, totalValue / 4); //임시로 4 넣었음
        MapInit(150002, totalValue / 4);
        MapInit(150003, totalValue / 4);
        MapInit(150004, totalValue / 4);
    }

    private void MapInit(int blockID, int count = 100)
    {
        if (RoomSettingData.Instance.farmingBlockMaxBlock < RoomSettingData.Instance.blockCount) return;

        int startCount = 0;
        UtilityCode.Logger2("맵 초기화 시작"+startCount + "시작" + count);

        while (count > startCount)
        {
            startCount++;
            int x = Random.Range(mapSizeMin.x, mapSizeMax.x);
            int y = Random.Range(mapSizeMin.y, mapSizeMax.y);
            Vector2Int randomPosition = new Vector2Int(x, y);
            float randomDistance = Random.Range(minDistance, maxDistacne);
            UtilityCode.Logger(startCount + "블럭 생성" + randomPosition);

            Collider2D ground = Physics2D.OverlapCircle(randomPosition, 0.2f, GroundCheck);
            if (ground == null)
            {
                UtilityCode.Logger2("땅 위가 아님");
                continue; // 땅 위에만 생성
            }

            Collider2D block = Physics2D.OverlapCircle(randomPosition, randomDistance, NotSetBlock);
            if (block != null)
            {
                UtilityCode.Logger2("블럭이 이미 자리잡음");
                continue; // 해당 물체 위에는 생성 불가
            }

            GameObject farmingBlock = Instantiate(AddressableManager.Instance.farmingBlockCache[blockID.ToString()], (Vector2)randomPosition, Quaternion.identity);
            farmingBlock.GetComponent<FarmingBlock>().Initialize(blockID);
            RoomSettingData.Instance.blockCount++;

            //farmingBlockPrefab.AddPhoton();
            //GameObject farmingBlock = PhotonNetwork.Instantiate(
            //    farmingBlockPrefab.name, (Vector2)randomPosition, Quaternion.identity
            //);

            //var interactBlock = farmingBlock.GetComponent<PhotonView>();

            //var key = DataManager.Instance.AllMaterial[blockID]; //아무거나 넣어놨음.

            //interactBlock.
            //(nameof(block.Initialize), RpcTarget.All, key, key);
            //멀티 없는 버전 : 
            //Instantiate(farmingBlockPrefab, (Vector2)randomPosition, Quaternion.identity);
        }
    }

    //public void OnDrawGizmos()
    //{
    //    playerSpawnPoint = gameObject.transform.GetChild(0).transform.GetComponentsInChildren<Village>().ToList();
    //    enemySpawnPoint = gameObject.transform.GetChild(1).transform.GetComponentsInChildren<Village>().ToList();
    //    NPCSpawnPoint = gameObject.transform.GetChild(2).transform.GetComponentsInChildren<Village>().ToList();

    //    Gizmos.color = Color.red;
    //    for (int i = 0; i < playerSpawnPoint.Count; i++)
    //    {
    //        Gizmos.DrawSphere(playerSpawnPoint[i].transform.localPosition, 3f); //10f는 확인을 위한 것.

    //        //Handles.Label(playerSpawnPoint[i].localPosition + Vector3.up * 5f, $"Player 좌표 {playerSpawnPoint[i].gameObject.name}" + playerSpawnPoint[i].localPosition);
            
    //    }
    //    Gizmos.color = Color.yellow;
    //    for (int i = 0; i < enemySpawnPoint.Count; i++)
    //    {
    //        Gizmos.DrawSphere(enemySpawnPoint[i].transform.localPosition, 3f); //10f는 확인을 위한 것.

    //        //Handles.Label(enemySpawnPoint[i].localPosition + Vector3.up * 5f, $"Enemy 좌표 {enemySpawnPoint[i].gameObject.name}" + enemySpawnPoint[i].localPosition);
    //    }
    //    Gizmos.color = Color.green;
    //    for (int i = 0; i < NPCSpawnPoint.Count; i++)
    //    {
    //        Gizmos.DrawSphere(NPCSpawnPoint[i].transform.localPosition, 3f); //10f는 확인을 위한 것.

    //        //Handles.Label(NPCSpawnPoint[i].localPosition + Vector3.up * 5f, $"NPC 좌표 {NPCSpawnPoint[i].gameObject.name}" + NPCSpawnPoint[i].localPosition);
    //    }
    //}
}
