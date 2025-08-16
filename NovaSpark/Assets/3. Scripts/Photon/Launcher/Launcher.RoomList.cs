using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Pun;

public partial class Launcher
{
    [Header("방 목록")]
    [SerializeField] private Transform roomListContent;
    [SerializeField] private GameObject roomListItemPrefab;

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (!isMultiMode) return;

        foreach (Transform t in roomListContent)
            Destroy(t.gameObject);

        foreach (var room in roomList)
        {
            if (room.RemovedFromList) continue;

            var item = Instantiate(roomListItemPrefab, roomListContent);
            var input = item.transform.Find("RoomNameInput").GetComponent<TMP_InputField>();
            string disp = room.CustomProperties.ContainsKey("dispName")
                ? room.CustomProperties["dispName"].ToString()
                : room.Name;
            input.text = $"{disp} ({room.PlayerCount}/{room.MaxPlayers})";
            input.interactable = false;

            var statusText = item.transform.Find("StatusText").GetComponent<TMP_Text>();
            bool inProg = room.CustomProperties.TryGetValue("inProgress", out var ip) && (bool)ip;
            statusText.text = inProg ? "게임중" : "대기중";

            var joinBtn = item.transform.Find("JoinButton").GetComponent<Button>();
            joinBtn.onClick.RemoveAllListeners();
            joinBtn.onClick.AddListener(() =>
            {
                if (inProg)
                {
                    feedbackText.text = "이미 게임이 진행 중...";
                    return;
                }
                OnRoomListItemClicked(room);
            });
        }
    }

    public override void OnRoomPropertiesUpdate(Hashtable propsThatChanged)
    {
        base.OnRoomPropertiesUpdate(propsThatChanged);
        if (propsThatChanged.ContainsKey("dispName") && PhotonNetwork.InLobby)
            RefreshRoomList();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        feedbackText.text = $"입장 실패: {message}";
    }

    private void OnRoomListItemClicked(RoomInfo room)
    {
        ShowRoomDetail();
        InitShowOffPasswordButton();

        roomNameText.text = room.Name;
        roomNameInput.text = room.Name;
        roomNameInput.interactable = false;

        playerLimitDropdown.value = room.MaxPlayers - 1;
        playerLimitDropdown.interactable = false;

        descriptionInput.text = room.CustomProperties.ContainsKey("desc")
            ? (string)room.CustomProperties["desc"] : "";
        descriptionInput.interactable = false;

        bool hasPwd = room.CustomProperties.TryGetValue("pwd", out var pwdObj)
                      && !string.IsNullOrEmpty((string)pwdObj);

        usePasswordToggle.onValueChanged.RemoveAllListeners();
        usePasswordToggle.isOn = hasPwd;
        usePasswordToggle.interactable = hasPwd;

        passwordInput.gameObject.SetActive(hasPwd);
        passwordInput.text = "";

        usePasswordToggle.onValueChanged.AddListener(isOn =>
        {
            passwordInput.gameObject.SetActive(isOn);
            if (isOn)
            {
                passwordInput.inputType = TMP_InputField.InputType.Password;
                passwordInput.ForceLabelUpdate();
            }
            else passwordInput.text = "";
        });

        roomCreateButton.onClick.RemoveAllListeners();
        roomCreateButton.GetComponentInChildren<TMP_Text>().text = "입장하기";
        roomCreateButton.onClick.AddListener(() =>
        {
            if (hasPwd && passwordInput.text.Trim() != (string)pwdObj)
            {
                feedbackText.text = "비밀 번호가 틀렸습니다.";
                return;
            }
            feedbackText.text = $"'{room.Name}' 방에 입장중..";
            PhotonNetwork.JoinRoom(room.Name);
            OffRoomDetail();
        });

        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(OffRoomDetail);
        backButton.interactable = true;
    }
}