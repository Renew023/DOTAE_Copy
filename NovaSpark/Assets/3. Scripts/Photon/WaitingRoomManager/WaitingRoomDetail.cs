using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaitingRoomDetail : MonoBehaviour
{
    [Header("UI 오브젝트 참조")]
    [SerializeField] private TMP_Text nameText;         // 옵션 이름
    [SerializeField] private Button decreaseButton;     // ← 버튼
    [SerializeField] private Button increaseButton;     // → 버튼
    [SerializeField] private TMP_Text valueText;        // 라벨 표시

    [Header("라벨 설정(인스펙터)")]
    [SerializeField]
    private List<string> labels = new List<string>
    {
        "없음", "적음", "보통", "많음"
    };

    private int currentIndex = 2; 

    // 각 라벨이 매핑될 실제 값(배율) 딕셔너리
    private static readonly Dictionary<string, float> labelToValue = new Dictionary<string, float>
    {
        { "없음", 0f  /*값 수정*/ },
        { "적음", 0.5f /*값 수정*/},
        { "보통", 1f  /*값 수정*/ },
        { "많음", 2f /*값 수정*/  }
    };

    private void Awake()
    {
        decreaseButton.onClick.AddListener(OnDecrease);
        increaseButton.onClick.AddListener(OnIncrease);
    }

    private void Start()
    {
        UpdateUI();
        // 최초 한 번, 나중에 매핑된 값(value)을 매니저에 전달
        var initialValue = labelToValue[labels[currentIndex]];
        WaitingRoomManager.Instance.UpdateSetting(nameText.text, initialValue);
    }

    private void OnDecrease()
    {
        currentIndex = (currentIndex - 1 + labels.Count) % labels.Count;
        UpdateUI();
        var val = labelToValue[labels[currentIndex]];
        WaitingRoomManager.Instance.UpdateSetting(nameText.text, val);
    }

    private void OnIncrease()
    {
        currentIndex = (currentIndex + 1) % labels.Count;
        UpdateUI();
        var val = labelToValue[labels[currentIndex]];
        WaitingRoomManager.Instance.UpdateSetting(nameText.text, val);
    }

    private void UpdateUI()
    {
        // 숫자 대신 라벨만 표시
        valueText.text = labels[currentIndex];
    }
}