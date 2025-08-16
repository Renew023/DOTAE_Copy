using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ClockUI : MonoBehaviour
{
    [SerializeField] private RectTransform _clockHand;

    [SerializeField] private TMP_Text _dayCountText; // 하루 경과 횟수를 표시할 UI 텍스트

    TimeManager timeManager;

    private void OnEnable()
    {
        timeManager = GameManager.Instance.TimeManager;
        // 하루 경과 이벤트 구독
        timeManager.OneDayPassed += OnOneDayPassed;
        // 초기 텍스트 설정
        // if (_dayCountText != null)
        //     _dayCountText.text = timeManager.DayCount.ToString();
    }

    IEnumerator Start()
    {
        yield return null;
        if (timeManager != null && _dayCountText != null)
            _dayCountText.text = timeManager.DayCount.ToString();
        else
            Debug.LogError("날짜 초기화 실패");
    }

    private void OnDisable()
    {
        // 이벤트 해제
        if (timeManager != null)
            timeManager.OneDayPassed -= OnOneDayPassed;
    }

    void Update()
    {
        if (timeManager == null) return;
        
        float progress = timeManager.DayProgress;
        float angle = progress * 360f;

        _clockHand.localRotation = Quaternion.Euler(0f, 0f, -angle);
    }

    private void OnOneDayPassed()
    {
        if (_dayCountText != null)
            _dayCountText.text = timeManager.DayCount.ToString();
    }
}