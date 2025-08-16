using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class RoomSetting : MonoBehaviour, IUIPanel
{
    [SerializeField] private RoomSettingPanel roomSettingPanel;

    [SerializeField] private TMP_InputField roomNameInputField;
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private TMP_Dropdown maxPersonDropdown;

    [SerializeField] private TMP_InputField descriptionInputField;
    [SerializeField] private TMP_InputField seedInputField;

    [SerializeField] private int maxPlayer = 5;

    public void Init()
    {
        Debug.Log("방 만들기에 접속했습니다");
        roomNameInputField.text = "";
        passwordInputField.text = "";
        maxPersonDropdown.value = maxPlayer - 1;
        descriptionInputField.text = "";
        seedInputField.text = "";

        roomSettingPanel.dataHandler += RoomData;
    }

    public void RoomData()
    {
        var name = roomNameInputField.text.Trim();
        roomSettingPanel.roomName = name;
        AddLobbyProperty(nameof(roomNameInputField), name);

        var password = passwordInputField.text.Trim();
        AddLobbyProperty(nameof(passwordInputField), password);

        var player = maxPersonDropdown.value + 1;
        roomSettingPanel.maxPlayerCount = player;
        AddLobbyProperty(nameof(maxPersonDropdown), player);

        var description = descriptionInputField.text.Trim();
        AddLobbyProperty(nameof(descriptionInputField), description);

        int seed = seedInputField.text.Trim() == null ? int.Parse(seedInputField.text.Trim()) : Random.Range(0, int.MaxValue);
        AddLobbyProperty(nameof(seedInputField), seed);
    }

    private void AddLobbyProperty(string key, object value)
    {
        roomSettingPanel.roomSettings[key] = value;
        roomSettingPanel.lobbyProps.Add(key);
    }
}
