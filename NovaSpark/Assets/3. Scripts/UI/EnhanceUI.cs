using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnhanceUI : PopupUI
{
    private EquipmentItem selectedEquipment;

    [Header("UI")] [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI effectText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI successRateText;
    [SerializeField] private Button enhanceButton;
    [SerializeField] private Transform effectSpawnPoint;
    [SerializeField] private Inventory _targetInventory;
    [SerializeField] private Image iconImage;
    [SerializeField] private Button closeButton;
    [SerializeField] private EnhancePopupUI _enhancePopup;

    private void OnEnable()
    {
        selectedEquipment = null;
        enhanceButton.onClick.AddListener(OnClickEnhance);
        closeButton.onClick.AddListener(HidePanel);
    }

    public override void HidePanel()
    {
        base.HidePanel();
        selectedEquipment = null;
        UpdateUI();
    }

    private void OnDisable()
    {
        enhanceButton.onClick.RemoveListener(OnClickEnhance);
        closeButton.onClick.RemoveListener(HidePanel);
    }

    void Start()
    {
        UpdateUI();
    }

    public void SetSelectedEquipment(EquipmentItem equipment)
    {
        selectedEquipment = equipment;
        selectedEquipment.CacheBaseStat();
        UpdateUI();
    }

    // UI 갱신
    private async void UpdateUI()
    {
        if (selectedEquipment == null)
        {
            ResetUI();
            return;
        }

        var sprite = await AddressableManager.Instance.LoadIcon(selectedEquipment.icon);

        iconImage.sprite = sprite;
        iconImage.enabled = selectedEquipment != null;

        nameText.text = selectedEquipment.name_kr;
        levelText.text = $"+{selectedEquipment.enhanceLevel}";
        effectText.text = $"공격력: {selectedEquipment.GetEnhancedEffect()}";
        costText.text = $"강화 비용: {selectedEquipment.GetEnhanceCost()}G";
        successRateText.text = $"성공 확률: {selectedEquipment.GetCurrentSuccessRate() * 100f:0}%";
    }

    private void ResetUI()
    {
        iconImage.enabled = false;
        iconImage.sprite = null;

        nameText.text = "";
        levelText.text = "";
        effectText.text = "";
        costText.text = "";
        successRateText.text = "";
    }

    private void ShowEnhanceMessage(string msg)
    {
        if (_enhancePopup == null) return;
        _enhancePopup.SetMessage(msg);
        UIManager.Instance.ShowPanel(UIType.EnhancePopupUI);
    }

    // 강화 버튼 클릭 시
    private void OnClickEnhance()
    {
        if (selectedEquipment == null)
        {
            ShowEnhanceMessage("강화할 장비가 없습니다");
            return;
        }

        if (!selectedEquipment.CanEnhance() && selectedEquipment != null)
        {
            ShowEnhanceMessage("최대 강화 레벨입니다");
            return;
        }


        if (_targetInventory == null)
        {
            Debug.LogError("PlayerInventory가 설정되지 않았습니다.");
            return;
        }

        int cost = selectedEquipment.GetEnhanceCost();
        if (!_targetInventory.HasEnoughGold(cost))
        {
            ShowEnhanceMessage("골드가 부족합니다");
            return;
        }

        _targetInventory.AddGold(-cost); // 골드 차감

        bool success = TryEnhance(selectedEquipment);

        ShowEnhanceMessage(success ? "강화 성공!" : "강화 실패");

        UpdateUI();
    }

    // 강화 성공 여부 판단 및 장비 상태 반영
    private bool TryEnhance(EquipmentItem equipment)
    {
        float successRate = equipment.GetCurrentSuccessRate();
        bool isSuccess = Random.value <= successRate;

        // PlayEffect("ada");

        if (isSuccess)
        {
            equipment.enhanceLevel++;
            equipment.UpdateEnhancedStat();
            // 성공 이펙트나 사운드 등 추가 가능
        }
        else
        {
            // 실패 패널티 처리
        }

        return isSuccess;
    }

    // 강화 이펙트
    private async void PlayEffect(string key)
    {
        GameObject effect = await AddressableManager.Instance.GetFromPool(key, effectSpawnPoint);
        if (effect != null)
        {
            StartCoroutine(ReturnToPoolAfterDelay(key, effect, 3f));
        }
    }

    // 일정 시간 후 이펙트 풀로 반환
    private IEnumerator ReturnToPoolAfterDelay(string key, GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        AddressableManager.Instance.ReturnToPool(key, obj);
    }

    public void SetTargetInventory(Inventory target)
    {
        _targetInventory = target;
    }
}