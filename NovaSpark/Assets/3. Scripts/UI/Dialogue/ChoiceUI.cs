using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChoiceUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private RectTransform contentArea;
    [SerializeField] private GameObject buttonPrefab;

    public Action<string> OnChoiceSelected;
    private List<GameObject> spawnedButtons = new List<GameObject>();

    private void Awake()
    {
        panel.SetActive(false);
    }

    /// <summary>
    /// rawTexts/rawActions 에 파이프로 붙어온 문자열이 들어올 때,
    /// 런타임에 '|' 로 분리해서 각각 버튼으로 만듭니다.
    /// </summary>
    public void ShowChoices(List<string> rawTexts, List<string> rawActions)
    {
        ClearChoices();

        // '|' 기준으로 나눠서 진짜 리스트 생성
        var texts = rawTexts
            .SelectMany(t => t.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
            .Select(t => t.Trim())
            .ToList();

        var actions = rawActions
            .SelectMany(a => a.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
            .Select(a => a.Trim())
            .ToList();

        for (int i = 0; i < texts.Count; i++)
        {
            var btnGO = Instantiate(buttonPrefab, contentArea);
            var btnText = btnGO.GetComponentInChildren<TMP_Text>();
            btnText.text = texts[i];

            string actionKey = (i < actions.Count) ? actions[i] : null;
            btnGO.GetComponent<Button>().onClick.AddListener(() =>
            {
                OnChoiceSelected?.Invoke(actionKey);
                HideChoices();
            });

            spawnedButtons.Add(btnGO);
        }

        panel.SetActive(true);
    }

    public void HideChoices()
    {
        panel.SetActive(false);
        ClearChoices();
    }

    private void ClearChoices()
    {
        foreach (var btn in spawnedButtons)
            Destroy(btn);
        spawnedButtons.Clear();
    }
}
