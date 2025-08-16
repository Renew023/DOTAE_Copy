using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SlotMachineController : PopupUI
{
    [Header("UI")] [SerializeField] private Button _spinButton;

    [SerializeField] private Button _closeButton;

    [FormerlySerializedAs("betInputField")] [SerializeField]
    private TMP_InputField _betInputField;

    [SerializeField] private TextMeshProUGUI _goldText;

    [Header("Slot Setting")] [SerializeField]
    private SlotReel[] _reels;

    [SerializeField] private Animator _spinBtnAnimator;

    [SerializeField] private int _playerGold = 1000; // 플레이어 골드

    private const float _baseSpinDuration = 3f; // 첫 릴 기본 회전 시간
    private const float _reelDelay = 1.0f; // 릴 간 회전 지연 시간

    private int betAmount; // 베팅한 금액
    private DataManager dataManager;

    // 초기 슬롯 데이터 로딩 및 릴 초기화
    private async void Start()
    {
        dataManager = DataManager.Instance;

        await dataManager.WaitUntilLoaded();

        var slotData = dataManager.SlotDataList;
        foreach (var reel in _reels)
        {
            await reel.InitAsync(slotData);
        }

        _spinButton.onClick.AddListener(OnSpinButtonClicked);
        _closeButton.onClick.AddListener(OnExitClicked);
        UpdateGoldUI();
    }

    // 닫기 버튼 시
    private void OnExitClicked()
    {
        UIManager.Instance.HidePanel(UIType.SlotMachineUI);
    }

    // 스핀 클릭 시
    private void OnSpinButtonClicked()
    {
        if (!TryPlaceBet(out int inputBet))
            return;

        betAmount = inputBet;
        _playerGold -= inputBet;
        _spinButton.interactable = false;
        _spinBtnAnimator.SetTrigger("PressTrigger");
        SoundManager.Instance.PlaySFXAsync("Sound/SFX/slot_drumroll");

        UpdateGoldUI();
        StartAllReels();
        StartCoroutine(CheckResult(GetTotalSpinDuration()));
    }

    // 베팅 금액 검사
    private bool TryPlaceBet(out int inputBet)
    {
        if (!int.TryParse(_betInputField.text, out inputBet) || inputBet <= 0)
        {
            Debug.Log("베팅금액 확인");
            return false;
        }

        if (_playerGold < inputBet)
        {
            Debug.Log("골드 부족");
            return false;
        }

        return true;
    }

    // 모든 릴 순차적으로 회전
    private void StartAllReels()
    {
        for (int i = 0; i < _reels.Length; i++)
        {
            float duration = _baseSpinDuration + i * _reelDelay;
            _reels[i].StartSpin(duration);
        }
    }


    // 전체 릴 회전 이후 결과 판정
    private IEnumerator CheckResult(float delay)
    {
        yield return new WaitForSeconds(delay + 0.5f);

        var firstResult = _reels[0].GetResult();
        bool isWin = _reels.All(reel => reel.GetResult().icon == firstResult.icon);

        if (isWin)
        {
            int reward = betAmount * firstResult.multiplier;
            _playerGold += reward;
        }
        else
        {
            // TODO: 당첨안됐을 시
        }

        UpdateGoldUI();
        _spinButton.interactable = true;
    }

    // UI 골드 갱신
    private void UpdateGoldUI()
    {
        _goldText.text = _playerGold.ToString();
    }

    // 전체 릴 회전 시간 계산
    private float GetTotalSpinDuration() => _baseSpinDuration + (_reels.Length - 1) * _reelDelay;
}