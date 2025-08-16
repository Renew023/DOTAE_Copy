using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.TextCore.Text;

public class Village : MonoBehaviour
{
    //TODO : 캐릭터 스포너를 만들면서 어떤 특정할 지역이 필요하다고 느꼈습니다.
    public int villageNumber;
    public List<int> spawnEnemyNumber; //이 마을에서 나올 수 있는 종류
    public int spawnCount;
    //public ObjectPool<GameObject> charObjectPool; //오브젝트마다 프리팹이 달라서 풀을 달리 해주긴 해야함.
    public GameObject npcPrefab;
    public Transform npcParent;
    public GameObject monsterPrefab;
    public GameObject playerPrefab;

    private void Start()
    {
        monsterPrefab = monsterPrefab.AddPhoton();
        npcPrefab = npcPrefab.AddPhoton();
        playerPrefab = playerPrefab.AddPhoton();
        StartCoroutine(DataLoadAfter());
    }

    IEnumerator DataLoadAfter()
    {
        yield return new WaitUntil(() => DataManager.Instance.IsLoaded == true);

        yield break;
        //charObjectPool = new ObjectPool<GameObject>(
        //    () => {
        //        Debug.Log("데이터 저장이 끝났습니다. 소환합니다. ");
        //        int key = Random.Range(0, spawnEnemyNumber.Count);
        //        int RandomKey = spawnEnemyNumber[key];
        //        GameObject charPrefab = null;

        //        if(RandomKey > 0 && RandomKey <= 100)
        //        {
        //            charPrefab = Instantiate(playerPrefab);
        //            charPrefab.GetComponent<Player>().Initialize(RandomKey);
        //            charPrefab.GetComponent<Player>().spawnVillage = this;
        //        }
        //        else if(RandomKey > 100 && RandomKey <= 500)
        //        {
        //            charPrefab = Instantiate(monsterPrefab);
        //            charPrefab.GetComponent<Enemy>().Initialize(RandomKey);
        //            charPrefab.GetComponent<Enemy>().spawnVillage = this;
        //        }
        //        else if (RandomKey > 500 && RandomKey <= 1000)
        //        {
        //            charPrefab = Instantiate(npcPrefab, npcParent);
        //            charPrefab.GetComponent<NPC>().Initialize(RandomKey);
        //            charPrefab.GetComponent<NPC>().spawnVillage = this;

        //        }

        //        return charPrefab;
        //        },
        //    obj =>
        //    {
        //        int key = obj.GetComponent<CharacterObject>().characterRuntimeData.characterID;

        //        if (key > 0 && key <= 100)
        //        {
        //            obj.GetComponent<Player>().Initialize(key);
        //        }
        //        else if (key > 100 && key <= 500)
        //        {
        //            obj.GetComponent<Enemy>().Initialize(key);
        //        }
        //        else if (key > 500 && key <= 1000)
        //        {
        //            obj.GetComponent<NPC>().Initialize(key);
        //        }
        //        obj.gameObject.SetActive(true);

        //    },
        //    obj =>
        //    {
        //        obj.gameObject.SetActive(false);
        //        obj.transform.position = new Vector2(10000, 10000);
        //    }
        //    );
        //if (!PhotonNetwork.IsMasterClient)
        //{

            for (int i = 0; i < spawnCount; i++)
            {
                if (spawnEnemyNumber.Count == 0) break;
                int key = Random.Range(0, spawnEnemyNumber.Count);
                int RandomKey = spawnEnemyNumber[key];
                GameObject charPrefab = null;

                if (RandomKey > 0 && RandomKey <= 100)
                {
                    charPrefab = Instantiate(playerPrefab);
                    //charPrefab = PhotonNetwork.InstantiateRoomObject(playerPrefab.name, Vector3.zero, Quaternion.identity);
                }
                else if (RandomKey > 100 && RandomKey <= 500)
                {
                    charPrefab = Instantiate(monsterPrefab);
                    //charPrefab = PhotonNetwork.InstantiateRoomObject(monsterPrefab.name, Vector3.zero, Quaternion.identity);

                }
                else if (RandomKey > 500 && RandomKey <= 1000)
                {
                    charPrefab = Instantiate(npcPrefab, npcParent);
                    //charPrefab = PhotonNetwork.InstantiateRoomObject(npcPrefab.name, Vector3.zero, Quaternion.identity);
                }
                //charPrefab = charPrefab.AddPhoton();
                charPrefab.transform.parent = npcParent;
                //Debug.Log("village" + villageNumber);
                //charPrefab.GetComponent<CharacterObject>().Initialize(RandomKey, villageNumber);
                //charPrefab.GetComponent<CharacterObject>().photonView.RPC(Initialize, RpcTarget.All, RandomKey, this);
                var ch = charPrefab.GetComponent<CharacterObject>();
                ch.Initialize(RandomKey, villageNumber);
                //ch.photonView.RPC(nameof(ch.Initialize), RpcTarget.All, RandomKey, villageNumber);
            }
        //}
    }

    //TODO2: Village는 Place를 포함 시킨 마을을 만들고.
    //    TODO3: 해당 플레이스의 NPC들이 있어야하는 지정위치를 만들고.
    //    TODO4: 맞는 위치에 NPC를 소환해야한다. 

    public void Respawn(CharacterObject character)
    {
        StartCoroutine(RespawnStart(character));
        
        //Debug.Log("Respawn");
        //character.gameObject.SetActive(true);
        //character.Initialize(character.characterRuntimeData.characterID, character.spawnVillage.villageNumber);
    }

    IEnumerator RespawnStart(CharacterObject character)
    {
        yield return new WaitForSeconds(2f);

        character.Respawn();
        //character.photonView.RPC(nameof(character.Respawn), RpcTarget.All);
    }
}
