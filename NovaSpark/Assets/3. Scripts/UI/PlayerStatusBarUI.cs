using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Player의 상태(Hp / Thirsty / Hungry)를 받아서 UI에 게이지로 표시.
/// </summary>
public class PlayerStatusBarUI : MonoBehaviour
{
    [Header("References")]
    [Tooltip("FillAmount로 쓸 이미지 (슬라이더처럼 좌→우 채워짐)")]
    [SerializeField] private Image _hpBar;
    [SerializeField] private Image _thirstBar;
    [SerializeField] private Image _hungerBar;

    [Tooltip("감지할 Player. 비어있으면 씬에서 첫 번째 Player를 자동 탐색")]
    [SerializeField] private Player _player;


    public void Start()
    {
       
    }
    private void Update()
    {
        if (_player == null)
        {
            _player = FindObjectOfType<Player>();
            return;
        }


        UpdateHP();
        UpdateThirst();
        UpdateHunger();
    }

    private void UpdateHP()
    {
        if (_hpBar == null) return;
        var healthData = _player.characterRuntimeData.health;
        float current = healthData.Current;
        float max = healthData.Max;
        _hpBar.fillAmount = (max > 0f) ? Mathf.Clamp01(current / max) : 0f;
    }

    private void UpdateThirst()
    {
        if (_thirstBar == null) return;
        var thirsty = _player.PlayerRuntimeData.thirsty;
        float current = thirsty.Current;
        float max = thirsty.Max;
        _thirstBar.fillAmount = (max > 0f) ? Mathf.Clamp01(current / max) : 0f;
    }

    private void UpdateHunger()
    {
        if (_hungerBar == null) return;
        var hungry = _player.PlayerRuntimeData.hungry;
        float current = hungry.Current;
        float max = hungry.Max;
        _hungerBar.fillAmount = (max > 0f) ? Mathf.Clamp01(current / max) : 0f;
    }
}
