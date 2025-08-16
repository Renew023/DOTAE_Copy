using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PartStatusUI : MonoBehaviour
{
    [Header("Data Source")]
    [Tooltip("씬에 있는 CharacterObject를 드래그하세요.")]
    [SerializeField] private CharacterObject _characterObject;

    [Header("Part TextSettings")]
    [SerializeField] private TMP_Text _headHpText;
    [SerializeField] private TMP_Text _bodyHpText;
    [SerializeField] private TMP_Text _armHpText;
    [SerializeField] private TMP_Text _legHpText;

    private void Update()
    {
        if (_characterObject == null || _characterObject.characterRuntimeData == null)
            return;

        RefreshPartUI();
    }

    /// <summary>
    /// CharacterObject.playerStat의 partDictionary와 maxPartDictionary를 읽어
    /// 각 부위별 현재/최대 HP를 UI에 표시
    private void RefreshPartUI()
    {
        var stat = _characterObject.characterRuntimeData;
        // 각 부위별 현재 hp와 최대 hp 가져오기
        //SetPartText(PartType.Head, _headHpText, stat);
        //SetPartText(PartType.Body, _bodyHpText, stat);
        //SetPartText(PartType.Arm, _armHpText, stat);
        //SetPartText(PartType.Leg, _legHpText, stat);
    }

    private void SetPartText(PartType part, TMP_Text uiText, StatInfo stat)
    {
        //if (stat.partDictionary.TryGetValue(part, out var curPart) &&
        //    stat.maxPartDictionary.TryGetValue(part, out var maxPart))
        //{
        //    uiText.text = $"{part}: {curPart.hp:F0} / {maxPart.hp:F0}";
        //}
        //else
        //{
        //    uiText.text = $"{part}: N/A";
        //}
    }
}
