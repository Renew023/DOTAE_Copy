using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;

public class CharacterSpanwer : Singleton<CharacterSpanwer>
{
    //TODO : 진영의 위치에 맞게 몬스터를 소환해주어야함.
    //TODO2 : 마을에서 집이 비어있다면 새로운 NPC가 들어오도록 해야함.
    //TODO3 : 진영은 팀원을 나타내지만, 스폰할 장소에 대한 point는 새롭게 작성되어야함.
    public List<Vector2> monsterSpawnPoint = new();
    public List<GameObject> monsterPrefabs = new();

    public Dictionary<int, ObjectPool<GameObject>> monsterSpawn = new();

    protected override void Awake()
    {
        base.Awake();

        for (int i = 0; i < monsterSpawnPoint.Count; i++)
        {
            var prefabs = monsterPrefabs[i];
            GameObject monster= monsterSpawn[i].Get();
            monster.transform.localPosition = monsterSpawnPoint[i];
        }
    }

    public GameObject CreateEnemy(int key, GameObject parent)
    {
        GameObject instance = monsterSpawn[key].Get();
        instance.transform.parent = parent.transform;
        instance.transform.localScale = Vector3.one;
        instance.transform.localPosition = Vector3.zero;
        return instance;
    }

    public void RemoveEnemy(int key, GameObject instance)
    {
        if (monsterSpawn.ContainsKey(key))
        {
            //Debug.LogError($"[ParticleManager] 존재하지 않는 key로 Release 시도: {key}");
            return;
        }
        monsterSpawn[key].Release(instance);
    }

    public IEnumerator Respawn(string key)
    {
        yield return new WaitForSeconds(15f);

    }
}