using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class AddressableManager : Singleton<AddressableManager>
{
    [SerializeField] private AssetReference assetReference;

    [SerializeField] private AssetLabelReference assetLabelReference;
    private readonly Dictionary<string, Sprite> _iconCache = new();
    private readonly Dictionary<string, GameObject> _prefabCache = new();
    private readonly Dictionary<string, AudioClip> _audioCache = new();
    private readonly Dictionary<string, Queue<GameObject>> _objectPool = new();
    private readonly Dictionary<string, GameObject> _animatorCache = new();
    public IReadOnlyDictionary<string, GameObject> animatorCache => _animatorCache;

    private readonly Dictionary<string, GameObject> _farmingBlockCache = new();
    public IReadOnlyDictionary<string, GameObject> farmingBlockCache => _farmingBlockCache;

    public readonly Dictionary<string, GameObject> _prefabCash = new();
    public readonly Dictionary<string, AudioClip> _audioCash = new();

    public Dictionary<string, Sprite> IconCash = new();

    private AesKeyAsset _aesKeyAsset;

    public bool isCashing = false;

    protected override void Awake()
    {
        DontDestroyOnLoad(this);
        base.Awake();
    }

    private async void Start()
    {
        bool success = await LoadAesKeyAsset();
        if (!success)
        {
            // Debug.LogError("AES 키 로딩 실패");
            return;
        }

        // Debug.LogError("[AES Key (Base64)] " + _aesKeyAsset.key);
        // Debug.LogError("[AES IV  (Base64)] " + _aesKeyAsset.iv);

        _ = InitCash();
    }

    private async Task InitCash()
    {
        var animLoaded = await InitAsync<GameObject>("Animator");
        foreach (var kv in animLoaded)
        {
            _animatorCache[kv.Key] = kv.Value;
            Debug.Log("어드래서블 오브젝트 데이터 파밍블럭" + $"{kv.Key}");
        }
        DataManager.Instance.progressCount++;

        var farmLoaded = await InitAsync<GameObject>("FarmingBlock");
        foreach (var kv in farmLoaded)
        {
            _farmingBlockCache[kv.Key] = kv.Value;
            Debug.Log("어드래서블 오브젝트 데이터 파밍블럭" + $"{kv.Key}");
        }
        DataManager.Instance.progressCount++;

        var characterIcons = await InitAsync<Sprite>("CharacterIcon");
        foreach (var kv in characterIcons)
        {
            IconCash[kv.Key] = kv.Value;
            Debug.Log("로드됨"+ kv.Key);
            Debug.Log("로드된 이미지 " + kv.Value);
        }
        DataManager.Instance.progressCount++;

        var Icons = await InitAsync<Sprite>("Icons");
        foreach (var kv in Icons)
        {
            IconCash[kv.Key] = kv.Value;
            Debug.Log("로드됨" + kv.Key);
            Debug.Log("로드된 이미지 " + kv.Value);
        }
        DataManager.Instance.progressCount++;

        var Images = await InitAsync<Sprite>("Images");
        foreach (var kv in Images)
        {
            IconCash[kv.Key] = kv.Value;
            Debug.Log("로드됨" + kv.Key);
            Debug.Log("로드된 이미지 " + kv.Value);
        }
        DataManager.Instance.progressCount++;

        var audioSource = await InitAsync<AudioClip>("Audios");
        foreach (var kv in audioSource)
        {
            _audioCash[kv.Key] = kv.Value;
            Debug.Log("로드됨" + kv.Key);
            Debug.Log("로드된 사운드 " + kv.Value);
        }
        DataManager.Instance.progressCount++;

        var weapon = await InitAsync<GameObject>("Prefabs");
        foreach (var kv in weapon)
        {
            _prefabCash[kv.Key] = kv.Value;
            Debug.Log("로드됨" + kv.Key);
            Debug.Log("로드된 프리팹 " + kv.Value);
        }
        DataManager.Instance.progressCount++;

        isCashing = true;
    }

    public async Task<bool> LoadAesKeyAsset(string key = "AESKeyAsset")
    {
        var handle = Addressables.LoadAssetAsync<AesKeyAsset>(key);
        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            _aesKeyAsset = handle.Result;
            AESUtil.SetKeyAndIV(_aesKeyAsset.key, _aesKeyAsset.iv);
            // Debug.LogError("AES Key/IV 설정 완료");
            return true;
        }
        else
        {
            Debug.LogError("AES Key/IV 로딩 실패");
            return false;
        }
    }

    // 실제 로딩 UI 적용 시
    // private async void Start()
    // {
    //     loadingText.text = "리소스 다운로드 중...";
    //     progressBar.fillAmount = 0f;
    //
    //     await AddressableManager.Instance.DownloadAllWithProgress(progress =>
    //     {
    //         progressBar.fillAmount = progress;
    //         loadingText.text = $"리소스 다운로드 중... {(int)(progress * 100)}%";
    //     });
    //
    //     loadingText.text = "로딩 완료!";
    //     await Task.Delay(1000); // 약간 여유
    //     SceneManager.LoadScene("MainScene");
    // }
    // 또는
    // private void UpdateLoadingProgress(float progress)
    // {
    //     progressBar.fillAmount = progress;
    //     loadingText.text = $"리소스 다운로드 중... {(int)(progress * 100)}%";
    // }
    //
    // await AddressableManager.Instance.DownloadAllWithProgress(UpdateLoadingProgress);

    // 모든 Addressables 리소스 다운로드, 진행률 전달
    public async Task DownloadAllWithProgress(Action<float> onProgress)
    {
        string[] labels = { "Icons", "Prefabs", "Audios", "Jsons" }; // 다운로드할 Addressables label 리스트

        for (int i = 0; i < labels.Length; i++)
        {
            string label = labels[i];
            // 해당 label의 리소스 다운로드 필요한 용량 체크(0이면 이미 로컬캐시에 있음)
            var sizeHandle = Addressables.GetDownloadSizeAsync(label);
            await sizeHandle.Task;

            // 용량 계산 실패 시 다음 label로
            if (sizeHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"{label} 용량 계산 실패");
                continue;
            }

            // 다운로드가 필요한 경우만 진행
            if (sizeHandle.Result > 0)
            {
                Debug.Log($"{label} 다운로드 필요: {sizeHandle.Result / (1024f * 1024f):F2} MB");

                // 이 label의 리소스 다운로드
                var downloadHandle = Addressables.DownloadDependenciesAsync(label);

                // 다운로드 완료될 때까지 대기, 진행률 계속 반영
                while (!downloadHandle.IsDone)
                {
                    float labelProgress = downloadHandle.PercentComplete;
                    float totalProgress = (i + labelProgress) / labels.Length;
                    onProgress?.Invoke(totalProgress);
                    //await Task.Yield();
                }

                // 다운로드 성공 시 진행률 1로 고정, 실패 시 에러 로그
                if (downloadHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    Debug.Log($"{label} 다운로드 완료");
                    onProgress?.Invoke((float)(i + 1) / labels.Length); // 마지막엔 100%로 반영
                }
                else
                {
                    Debug.LogError($"{label} 다운로드 실패");
                }
            }
            // 다운로드 필요 없을 경우 처리(이미 로컬에 있을때)
            else
            {
                Debug.Log($"{label} 은 이미 캐시에 존재");
                onProgress?.Invoke((float)(i + 1) / labels.Length);
            }
        }
    }

    public async Task<GameObject> GetFromPool(string key, Transform parent = null)
    {
        if (!_objectPool.ContainsKey(key))
        {
            _objectPool[key] = new Queue<GameObject>();
        }

        if (_objectPool[key].Count > 0)
        {
            GameObject reused = await LoadPrefab(key);
            reused.SetActive(true);
            reused.transform.SetParent(parent);
            return reused;
        }

        GameObject prefab = _prefabCash[key];
        if (prefab != null)
        {
            GameObject go = GameObject.Instantiate(prefab, parent);
            return go;
        }

        return null;
    }

    public void ReturnToPool(string key, GameObject obj)
    {
        obj.SetActive(false);
        _objectPool[key].Enqueue(obj);
    }

    public async Task<TextAsset> LoadJson(string key)
    {
        var handle = Addressables.LoadAssetAsync<TextAsset>(key);
        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
            return handle.Result;
        Debug.Log($"JSON 로딩  실패: {key}");
        return null;
    }

    public async Task<List<T>> LoadListDataAsync<T>(string key)
    {
        var handle = Addressables.LoadAssetAsync<TextAsset>(key);
        await handle.Task;

        if (handle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"{key} 데이터 로드 실패");
            return null;
        }

        var list = await JsonLoader.LoadAsync<T>(handle.Result);

        //Addressables.Release(handle);
        return list;
    }

    private async Task<T> LoadWithCache<T>(string key, Dictionary<string, T> cache)
        where T : UnityEngine.Object
    {
        if (cache.TryGetValue(key, out var cached))
            return cached;

        var handle = Addressables.LoadAssetAsync<T>(key);
        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            cache[key] = handle.Result;
            return handle.Result;
        }

        Debug.LogError($"{typeof(T).Name} 로딩 실패: {key}");
        return null;
    }

    public Task<Sprite> LoadIcon(string key) => LoadWithCache(key, _iconCache);

    public Task<GameObject> LoadPrefab(string key) => LoadWithCache(key, _prefabCache);

    public Task<AudioClip> LoadAudioClip(string key) => LoadWithCache(key, _audioCache);

    public void ReleaseAllCachedAssets()
    {
        foreach (var icon in _iconCache)
        {
            if (icon.Value != null)
                Addressables.Release(icon.Value);
        }

        foreach (var prefab in _prefabCache)
        {
            if (prefab.Value != null)
                Addressables.Release(prefab.Value);
        }

        foreach (var audio in _audioCache)
        {
            if (audio.Value != null)
                Addressables.Release(audio.Value);
        }

        Debug.Log("모든 메모리 캐시 리소스를 해제했습니다.");
    }

    /* 프리팹 사용시
    GameObject prefab = await AddressableManager.Instance.LoadPrefab(key);
    Instantiate(prefab, Vector3.zero, Quaternion.identity);

    public async void SpawnPrefab(string key)
    {
        GameObject prefab = await AddressableManager.Instance.LoadPrefab(key);
        if (prefab != null)
        {
            Instantiate(prefab, Vector3.zero, Quaternion.identity);
        }
    }*/

    // var handle = Addressables.InstantiateAsync("key", position, rotation);
    // InstantiateAsync()도 있지만 캐시 / 메모리 관리 직접하려면 Load + Instantiate 조합이 더 나음
    // Addressables.ReleaseInstance(obj); << 이걸 직접 해줘야함

    public void ClearIconCache()
    {
        foreach (var sprite in _iconCache.Values)
        {
            Addressables.Release(sprite);
        }

        _iconCache.Clear();
    }

    /// <summary>
    /// 아이콘 이미지를 Addressables로 로드하고, 버전 체크 후 적용
    /// </summary>
    public async void LoadIconToImage(
        Image targetImage,
        string iconKey,
        int version,
        Func<int> getCurrentVersion,
        bool setNativeSize = true)
    {
        if (string.IsNullOrEmpty(iconKey))
        {
            targetImage.enabled = false;
            targetImage.sprite = null;
            return;
        }

        targetImage.enabled = false;
        targetImage.sprite = null;

        var sprite = await LoadIcon(iconKey);
        if (version != getCurrentVersion())
            return;

        if (sprite != null)
        {
            targetImage.sprite = sprite;
            targetImage.enabled = true;
            if (setNativeSize && targetImage.rectTransform.rect.size.sqrMagnitude == 0)
                targetImage.SetNativeSize();
        }
        else
        {
            targetImage.enabled = false;
        }
    }

    public async Task<Dictionary<string, T>> InitAsync<T>(string label)
    {
        var result = new Dictionary<string, T>();
        // 예: "Effect" 라는 라벨을 가진 모든 프리팹 로드
        var locationsHandle = Addressables.LoadResourceLocationsAsync(label, typeof(T));
        var locations = await locationsHandle.Task;

        foreach (var location in locations)
        {
            Debug.Log("주소 빌딩" + location.PrimaryKey);
            string key = location.PrimaryKey; // 이게 실제 Addressables 주소 (키)

            var prefabHandle = Addressables.LoadAssetAsync<T>(key);
            var prefab = await prefabHandle.Task;

            result[key] = prefab;

            if (prefab != null)
            {
                result[key] = prefab;
            }
            else
            {
                Debug.LogError($"프리팹 로드 실패: {key}");
            }

            //Addressables.Release(prefabHandle); // 메모리 관리
        }
        Addressables.Release(locationsHandle); // 리소스 위치 핸들도 해제

        return result;
    }
}