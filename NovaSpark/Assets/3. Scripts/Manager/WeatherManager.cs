using System;
using UnityEngine;
using Photon.Pun;

public enum WeatherType
{
    Clear,
    Rain,
    Snow,
}

public class WeatherManager : MonoBehaviourPun, ISaveable
{
    [SerializeField] private WeatherType _startWeather;
    [SerializeField] private GameObject _rainEffect;
    [SerializeField] private GameObject _snowEffect;

    private float _followXOffset = 2f;
    private float _followYOffset = 12f;
    private Vector3 _effectRotation = new Vector3(80f, -90f, -90f);
    private bool _isSingleMode = false;

    [SerializeField] private Camera _camera;
    private SaveManager saveManager;
    private TimeManager timeManager;
    public WeatherType CurrentWeather { get; private set; }

    public event Action<WeatherType> OnWeatherChanged;


    // 테스트용
    [ContextMenu("날씨변경-랜덤")]
    public void TestRandomWeather()
    {
        if (_isSingleMode || PhotonNetwork.IsMasterClient)
        {
            ChangeRandomWeather();
        }
    }

    [ContextMenu("날씨변경-비")]
    public void TestRain()
    {
        ApplyWeather(WeatherType.Rain);
    }

    [ContextMenu("날씨변경-눈")]
    public void TestSnow()
    {
        ApplyWeather(WeatherType.Snow);
    }

    // protected override void Awake()
    // {
    //     base.Awake();
    //     _camera = Camera.main;
    //     _rainEffect = transform.Find("Rain_Particle")?.gameObject;
    //     _snowEffect = transform.Find("Snow_Particle")?.gameObject;
    //
    //     CurrentWeather = _startWeather;
    //     // ApplyWeather(CurrentWeather);
    // }

    public void Init(SaveManager saveManager, TimeManager timeManager)
    {
        this.saveManager = saveManager;
        this.timeManager = timeManager;
        Debug.LogWarning($"Photon 연결 상태: {PhotonNetwork.IsConnected}");
        _isSingleMode = !PhotonNetwork.IsConnected;

        if (_camera == null)
            _camera = Camera.main;
        _rainEffect = transform.Find("Rain_Particle")?.gameObject;
        _snowEffect = transform.Find("Snow_Particle")?.gameObject;

        CurrentWeather = _startWeather;

        saveManager.Register(this);
        timeManager.OneDayPassed += ChangeRandomWeather;
    }

    private void LateUpdate()
    {
        // 카메라 기준으로 파티클 위치/회전 갱신
        if (_camera == null) return;

        Vector3 camPos = _camera.transform.position;
        Vector3 followPos = new Vector3(camPos.x + _followXOffset, camPos.y + _followYOffset, 0f);
        Quaternion followRot = Quaternion.Euler(_effectRotation);

        UpdateEffectTransform(_rainEffect, followPos, followRot);
        UpdateEffectTransform(_snowEffect, followPos, followRot);
    }

    // private void OnEnable()
    // {
    //     saveManager = SaveManager.Instance;
    //     timeManager = TimeManager.Instance;
    //
    //     saveManager.Register(this); // 저장 시스템에 등록
    //     timeManager.OneDayPassed += ChangeRandomWeather; // 하루 경과 시 날씨 변경
    // }

    private void OnDisable()
    {
        saveManager.Unregister(this); // 저장 시스템에서 해제

        if (timeManager != null)
            timeManager.OneDayPassed -= ChangeRandomWeather;
    }

    // 파티클 오브젝트의 위치와 회전을 카메라에 맞춰 조정
    private void UpdateEffectTransform(GameObject effect, Vector3 pos, Quaternion rot)
    {
        if (effect != null && effect.activeSelf)
        {
            effect.transform.position = pos;
            effect.transform.rotation = rot;
        }
    }

    // 랜덤하게 날씨를 변경.
    private void ChangeRandomWeather()
    {
        WeatherType newWeather = GetRandomWeather();
        if (newWeather == CurrentWeather) return;

        if (_isSingleMode)
        {
            ApplyWeather(newWeather);
            CurrentWeather = newWeather;
            OnWeatherChanged?.Invoke(newWeather);
        }
        //else if (PhotonNetwork.IsMasterClient)
        //{
        //    photonView.RPC(nameof(RPC_ApplyWeather), RpcTarget.All, (int)newWeather);
        //}
    }

    [PunRPC]
    private void RPC_ApplyWeather(int weatherInt)
    {
        WeatherType newWeather = (WeatherType)weatherInt;
        CurrentWeather = newWeather;
        ApplyWeather(newWeather);
        OnWeatherChanged?.Invoke(newWeather);
    }

    // 랜덤으로 날씨 타입을 반환
    private WeatherType GetRandomWeather()
    {
        var values = Enum.GetValues(typeof(WeatherType));
        return (WeatherType)values.GetValue(UnityEngine.Random.Range(0, values.Length));
    }

    // 주어진 날씨에 맞춰 파티클 효과를 활성/비활성화
    private void ApplyWeather(WeatherType weather)
    {
        Debug.Log($"[날씨변경] : {weather}");

        bool isRain = weather == WeatherType.Rain;
        bool isSnow = weather == WeatherType.Snow;

        if (_rainEffect != null) _rainEffect.SetActive(isRain);
        if (_snowEffect != null) _snowEffect.SetActive(isSnow);
    }

    // 현재 날씨 상태를 저장
    public void SaveData(GameSaveData data)
    {
        data.weatherData = new WeatherSaveData
        {
            currentWeather = CurrentWeather,
        };
    }

    // 저장된 날씨 상태를 불러와 적용
    public void LoadData(GameSaveData data)
    {
        CurrentWeather = data.weatherData.currentWeather;
        ApplyWeather(CurrentWeather);
        OnWeatherChanged?.Invoke(CurrentWeather);
    }


    /*
    private void OnEnable() => WeatherManager.Instance.OnWeatherChanged += OnWeatherChanged;
    private void OnDisable() => WeatherManager.Instance.OnWeatherChanged -= OnWeatherChanged;
    private void OnWeatherChanged(WeatherType weather)
    {
        switch (weather)
        {
            case WeatherType.Clear:
                _currentSpeed = 기본이동속도;
                break;
            case WeatherType.Rain:
                _currentSpeed = 기본이동속도 * 0.8f;
                break;
            case WeatherType.Snow:
                _currentSpeed = 기본이동속도 * 0.6f;
                break;
        }
    }
    */
}