using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaitingRoomToggle : MonoBehaviour
{
    [SerializeField] private Button gameSettingBtn;
    [SerializeField] private Button gameDetailBtn;

    [SerializeField] private GameObject gameSettingPanel;
    [SerializeField] private GameObject gameDetailPanel;

    private void Start()
    {
        gameSettingBtn.onClick.AddListener(() => 
        {
            ShowRoomSettingPanel(); 
        });
        gameDetailBtn.onClick.AddListener(() =>
        {
            ShowGameDetailPanel();
        });
    }

    private void ShowRoomSettingPanel()
    {
        gameSettingPanel.SetActive(true);
        gameDetailPanel.SetActive(false);
    }

    private void ShowGameDetailPanel()
    {
        gameSettingPanel.SetActive(false);
        gameDetailPanel.SetActive(true);
    }
}