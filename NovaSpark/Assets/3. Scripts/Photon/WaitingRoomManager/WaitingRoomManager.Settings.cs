using System.Collections.Generic;
using Photon.Realtime;
using ExitGames.Client.Photon;  // Hashtable
using Photon.Pun;
using UnityEngine;

public partial class WaitingRoomManager
{
    public void UpdateSetting(string key, float value)
    {
        currentSettings[key] = value;
    }

    public float GetSetting(string key, float defaultValue = 1f)
        => currentSettings.TryGetValue(key, out var value) ? value : defaultValue;


    private void OpenSettingsPanel()
    {
        var room = PhotonNetwork.CurrentRoom;
        int currentCount = room.PlayerCount;
        int absoluteMax = 5;

        var options = new List<string>();
        for (int i = currentCount; i <= absoluteMax; i++)
            options.Add(i.ToString());

        maxPlayersDropdown.ClearOptions();
        maxPlayersDropdown.AddOptions(options);
        maxPlayersDropdown.value = room.MaxPlayers - 1;

        pwdInput.text = room.CustomProperties.TryGetValue("pwd", out var pwd) ? pwd.ToString() : "";
        descInput.text = room.CustomProperties.TryGetValue("desc", out var desc) ? desc.ToString() : "";

        settingPanel.SetActive(true);
        roomNameText.gameObject.SetActive(false);

        roomNameInput.text = room.CustomProperties.TryGetValue("dispName", out var dn2)
            ? dn2.ToString()
            : room.Name;
    }

    public void CloseSettingsPanel()
    {
        settingPanel.SetActive(false);
        roomNameText.gameObject.SetActive(true);

    }

    public void ApplyRoomSettings()
    {
        var room = PhotonNetwork.CurrentRoom;
        string newDispName = roomNameInput.text.Trim();
        string newPwd = pwdInput.text.Trim();
        byte newMax = (byte)(maxPlayersDropdown.value + 1);
        string newDesc = descInput.text.Trim();

        room.MaxPlayers = newMax;
        var props = new Hashtable
        {
            { "dispName", newDispName },
            { "pwd",      newPwd       },
            { "desc",     newDesc      }
        };
        room.SetCustomProperties(props);

        roomNameText.text = newDispName;
        feedbackText.text = "방 설정이 변경되었습니다.";
        CloseSettingsPanel();
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        // 필요에 따라 UI 업데이트 로직 추가 가능
    }
}