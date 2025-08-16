using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;

public class StatusUI : PopupUI
{
    [Header("UI CloseButton")]
    [SerializeField] private Button _closeButton;

    [Header("Data Source")]
    [Tooltip("씬에 있는 Player 오브젝트를 드래그하세요.")]
    [SerializeField] private Player _player;

    [Header("Stat Texts")]
    [SerializeField] private TMP_Text _levelText; // 데미지 텍스트
    [SerializeField] private TMP_Text _hp_Text; // 데미지 텍스트
    [SerializeField] private TMP_Text _damageText; // 데미지 텍스트
    [SerializeField] private TMP_Text _speedText; // 스피드 텍스트
    [SerializeField] private TMP_Text _defenceText; // 방어력 텍스트
    [SerializeField] private TMP_Text _attackSpeedText; // 공격 속도 텍스트
    [SerializeField] private TMP_Text _attackRangeText; // 공격 범위 텍스트 > 없어도 됨
    [SerializeField] private TMP_Text _criticalPercentText; // 크리티컬 확률 텍스트
    [SerializeField] private TMP_Text _accuracyText; // 명중률 텍스트
    [SerializeField] private TMP_Text _missingText; // 회피율 텍스트

    public override void Awake()
    {
        base.Awake();
        _closeButton.onClick.AddListener(HidePanel);
    }

    private void OnEnable()
    {
        RefreshUI();
    }
    private void Update()
    {
        // CanvasGroup alpha 기준으로 패널 보임 상태를 확인
        if (_panel != null && _panel.alpha > 0f)
        {
            RefreshUI();
        }
        if (_player == null)
        {
            _player = FindObjectOfType<Player>();
        }
    }
    /// <summary>
    /// Player.Character.playerStat 에서 값을 읽어 텍스트에 반영
    public void RefreshUI()
    {
        if (_player == null || _player == null || _player.characterRuntimeData == null)
        {
            return;
        }
        
        var s = _player.characterRuntimeData;
        var level = _player.PlayerRuntimeData.curLevel;

        _levelText.text = $"{level}";
        _hp_Text.text = $"{s.health.Current:F0} / {s.health.Max:F0} (+{_player.weaponStats.health.Current:F0})";
        _damageText.text = $"{s.damage.Current:F0} (+{_player.weaponStats.damage.Current:F0})";
        _defenceText.text = $"{s.defence.Current:F0} (+{_player.weaponStats.defence.Current:F0})";
        _speedText.text = $"{s.moveSpeed.Current:F1} (+{_player.weaponStats.moveSpeed.Current:F1})";
        _attackSpeedText.text = $"{s.attackSpeed.Current:F1}";
        _attackRangeText.text = $"{s.attackRange.Current:F1}";
        _criticalPercentText.text = $"{s.criticalPercent.Current:F1}%";
        _accuracyText.text = $"{s.aimPercent.Current:F1}%";
        _missingText.text = $"{s.missPercent.Current:F1}%";
    }
}
