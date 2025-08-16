using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Photon.Pun;

public enum DayState
{
    Dawn,
    Day,
    Noon,
    Night,
}

// [DefaultExecutionOrder(-999)]
public class TimeManager : MonoBehaviourPun, ISaveable
{
    [Header("하루 시간 설정")] [SerializeField, Range(10f, 600f)]
    private float _secondsPerDay; // 하루 시간(나중에 하루시간 정해지면 [SerializeField] 삭제해도됨)

    [Header("조명 설정")] [SerializeField] private Light2D _directionalLight; // 게임 전체 조명

    [SerializeField] private Color _dayColor; // 낮 조명 색

    [SerializeField] private Color _nightColor; // 밤 조명 색

    [SerializeField] private float _dayIntensity = 1f; // 낮 조명 밝기

    [SerializeField] private float _nightIntensity = 0.2f; // 밤 조명 밝기

    [SerializeField] private AnimationCurve _intensityCurve; // 조명 전환속도 제어

    [SerializeField] private float _transitionDuration = 2f; // 낮, 밤 전환시 부드럽게 변화되는 시간

    private float _timeOfDay = 0f; // 하루 시간
    private Coroutine _transitionCoroutine; // 조명 변화 코루틴

    private float _syncedTimeOfDay; // MasterClient에서 보낸 시작 시간
    private double _syncedPhotonTime; // 시작 기준 시점(Photon 기준 시간)
    private int _lastDayCount = 0; // 하루 변경 감지용
    private bool _isSynced = false;
    private bool _isSingleMode = false;


    public DayState CurrentState { get; private set; } // 낮, 밤 상태
    public float DayProgress { get; private set; } // 하루 진행도 (0~1)
    public int DayCount { get; private set; } = 1; // 현재 날짜 (0일부터 시작)

    public event Action<DayState> OnDayStateChanged; // 낮, 밤 변화 이벤트
    public event Action OneDayPassed;

    private SaveManager saveManager;

    // 초기화 (SaveManager 등록 + MasterClient가 기준값 RPC 전달)
    public void Init(SaveManager saveManager)
    {
        this.saveManager = saveManager;
        saveManager.Register(this);

        _isSingleMode = !PhotonNetwork.IsConnected;
        if (_isSingleMode)
        {
            _isSynced = true;
            _syncedPhotonTime = Time.time;
            _syncedTimeOfDay = 0f;
        }
        _secondsPerDay = RoomSettingData.Instance.SecondsPerDay;
    }

    public float GetSecondPerDay() => _secondsPerDay;
    public void SendSync()
    {
        float totalTime = DayCount * _secondsPerDay + _timeOfDay;
        //photonView.RPC(nameof(RPC_SyncStartTime), RpcTarget.All, totalTime, PhotonNetwork.Time);
    }

    // 시간 동기화 RPC
    [PunRPC]
    private void RPC_SyncStartTime(float totalTime, double photonTime)
    {
        _syncedTimeOfDay = totalTime;
        _syncedPhotonTime = photonTime;
        _isSynced = true;

        UpdateTime();
        CheckDayState();
        RefreshLighting();
    }


    private void OnDisable() => saveManager.Unregister(this);

    void Update()
    {
        if (!_isSynced) return;

        UpdateTime();
        CheckDayCountChanged();
        CheckDayState();
    }

    //  현재 시간, 날짜 계산
    private void UpdateTime()
    {
        double elapsed = _isSingleMode ? (Time.time - _syncedPhotonTime) : (PhotonNetwork.Time - _syncedPhotonTime);
        float totalTime = _syncedTimeOfDay + (float)elapsed;

        _timeOfDay = totalTime % _secondsPerDay;
        DayCount = (int)(totalTime / _secondsPerDay);
        DayProgress = _timeOfDay / _secondsPerDay;
    }

    // 날짜 변경 감지
    private void CheckDayCountChanged()
    {
        if (DayCount != _lastDayCount)
        {
            _lastDayCount = DayCount;
            OneDayPassed?.Invoke();
        }
    }

    // 낮, 밤 변경 감지
    private void CheckDayState()
    {
        DayState newState = DayProgress < 0.5f ? DayState.Day : DayState.Night;
        if (newState != CurrentState)
        {
            CurrentState = newState;
            OnDayStateChanged?.Invoke(CurrentState); // 연결된 이벤트가 있다면 호출
            ChangeDayState(CurrentState);
        }
    }


    // 낮, 밤 상태에 따라 조명 전환
    void ChangeDayState(DayState state)
    {
        if (PlaceManager.Instance.IsIndoor)
        {
            _directionalLight.enabled = false;
            return;
        }

        _directionalLight.enabled = true;

        if (_transitionCoroutine != null)
            StopCoroutine(_transitionCoroutine);

        Color targetColor = state == DayState.Day ? _dayColor : _nightColor;
        float targetIntensity = state == DayState.Day ? _dayIntensity : _nightIntensity;
        _transitionCoroutine = StartCoroutine(TransitionLight(targetColor, targetIntensity));
    }

    public void RefreshLighting()
    {
        ChangeDayState(CurrentState);
        OnDayStateChanged?.Invoke(CurrentState);
    }

    // 조명 색 부드럽게 전환
    private IEnumerator TransitionLight(Color targetColor, float targetIntensity)
    {
        Color startColor = _directionalLight.color;
        float startIntensity = _directionalLight.intensity;
        float elapsed = 0f;

        while (elapsed < _transitionDuration)
        {
            float t = elapsed / _transitionDuration;
            float curveT = _intensityCurve != null ? _intensityCurve.Evaluate(t) : t;

            _directionalLight.color = Color.Lerp(startColor, targetColor, curveT);
            _directionalLight.intensity = Mathf.Lerp(startIntensity, targetIntensity, curveT);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 마지막 값 정확하게 설정
        _directionalLight.color = targetColor;
        _directionalLight.intensity = targetIntensity;
    }

    // 게임 저장 시 데이터 저장
    public void SaveData(GameSaveData data)
    {
        data.timeData = new TimeSaveData
        {
            dayProgress = DayProgress,
            dayCount = DayCount,
            timeOfDay = _timeOfDay,
        };
    }

    // 저장된 데이터 불러오기
    public void LoadData(GameSaveData data)
    {
        //if (!PhotonNetwork.IsMasterClient) return;

        var timeData = data.timeData;

        _syncedTimeOfDay = timeData.dayCount * _secondsPerDay + timeData.timeOfDay;
        _syncedPhotonTime = _isSingleMode ? Time.time : PhotonNetwork.Time;
        _isSynced = true;

        UpdateTime();
        _lastDayCount = DayCount;

        CheckDayState();
        RefreshLighting();
    }
}

public interface ISaveable
{
    void SaveData(GameSaveData data);
    void LoadData(GameSaveData data);
}