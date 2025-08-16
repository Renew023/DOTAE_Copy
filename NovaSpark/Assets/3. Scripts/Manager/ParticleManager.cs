using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using UnityEngine.Pool;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ParticleManager : Singleton<ParticleManager>
{
    private Dictionary<string, ObjectPool<ParticleInstance>> _poolDict = new();
    private Dictionary<string, GameObject> _particle = new();
    
    private bool _isInitialized = false; 
    private Task _initTask; 

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        _initTask = InitAsync();
    }

    public async Task InitAsync()
    {
        // 예: "Effect" 라는 라벨을 가진 모든 프리팹 로드
        var locationsHandle = Addressables.LoadResourceLocationsAsync("Effect", typeof(GameObject));
        var locations = await locationsHandle.Task;

        foreach (var location in locations)
        {
            Debug.Log("주소 빌딩" + location.PrimaryKey);
            string key = location.PrimaryKey; // 이게 실제 Addressables 주소 (키)

            var prefabHandle = Addressables.LoadAssetAsync<GameObject>(key);
            var prefab = await prefabHandle.Task;

            if (prefab != null)
            {
                _particle[key] = prefab; // 주소를 기준으로 저장 (원래 하려던 hash 기반 대응 가능)
                
                var pool = new ObjectPool<ParticleInstance>(
                    () => new ParticleInstance(GameObject.Instantiate(prefab)),
                    obj => obj.gameObject.SetActive(true),
                    obj => obj.gameObject.SetActive(false)
                );

                _poolDict[key] = pool;
            }
            else
            {
                Debug.LogError($"프리팹 로드 실패: {key}");
            }

            //Addressables.Release(prefabHandle); // 메모리 관리
        }

        //Addressables.Release(locationsHandle); // 리소스 위치 핸들도 해제
        _isInitialized = true;
    }

    public ParticleInstance CreateEffect(string key, GameObject parent)
    {
        if (!_isInitialized) return null;

        ParticleInstance instance = _poolDict[key].Get();
        instance.gameObject.transform.parent = parent.transform;
        instance.gameObject.transform.localScale = Vector3.one;
        instance.gameObject.transform.localPosition = Vector3.zero;

        instance.particleSystem.Play();

        return instance;
    }

    public void RemoveEffect(string key, ParticleInstance instance)
    {
        if (!_poolDict.ContainsKey(key))
        {
            Debug.LogError($"[ParticleManager] 존재하지 않는 key로 Release 시도: {key}");
            return;
        }
        //_poolDict[key].Release(instance);
        StartCoroutine(StopEffect(key, instance));
    }

    public IEnumerator StopEffect(string key, ParticleInstance instance)
    {
        ParticleSystem particle = instance.particleSystem;
        particle.Stop();
        yield return new WaitWhile(() => particle.IsAlive(true));

        _poolDict[key].Release(instance);
    }
}



public class ParticleInstance
{
    public GameObject gameObject;
    public ParticleSystem particleSystem;

    public ParticleInstance(GameObject prefab)
    {
        this.gameObject = prefab;
        this.particleSystem = prefab.GetComponent<ParticleSystem>();
    }

    public void Play()
    {
        particleSystem.Play();
    }

    public void Stop()
    {
        particleSystem.Stop();
    }
}