using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameDescriptionPanel : MonoBehaviour
{
    [Header("UI데이터")] [SerializeField] private Button prevButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI pageText;

    [Header("데이터")] [SerializeField] private List<GameObject> gameObjectPanels;
    [SerializeField] private int initValue = 0;
    [SerializeField] private int curValue = 0;

    private void Awake()
    {
        prevButton.onClick.AddListener(()
            =>
        {
            if (initValue > 0)
            {
                initValue--;
                ResetText();
            }
        });
        nextButton.onClick.AddListener(()
            =>
        {
            if (initValue < gameObjectPanels.Count - 1)
            {
                initValue++;
                ResetText();
            }
        });
        closeButton.onClick.AddListener(()
            =>
        {
            initValue = 0;
            ResetText();
            StartPopupUI.Instance.ShowRoomSetting(StartPopupType.StartPanel);
        });
        ResetText();
    }

    private void ResetText()
    {
        if (gameObjectPanels.Count == 0) return;

        gameObjectPanels[curValue].SetActive(false);
        gameObjectPanels[initValue].SetActive(true);
        curValue = initValue;

        pageText.text = $"{initValue + 1} / {gameObjectPanels.Count}";

        prevButton.gameObject.SetActive(initValue > 0);
        nextButton.gameObject.SetActive(initValue < gameObjectPanels.Count - 1);
    }
}