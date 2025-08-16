using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public enum RoomSettingType
{
    RoomSetting,
    SpawnSetting,
    EnvironmentSetting
}

public class RoomSettingPanel : MonoBehaviourPunCallbacks, IUIPanel
{
    [field : Header("방 데이터 설정")]
    [SerializeField] private Button roomSettingPanelButton;
    [SerializeField] private Button spawnSettingPanelButton;
    [SerializeField] private Button environmentSettingPanelButton;

    [SerializeField] private CanvasGroup roomSettingPanel;
    [SerializeField] private CanvasGroup spawnSettingPanel;
    [SerializeField] private CanvasGroup environmentSettingPanel;

    [SerializeField] private RoomSettingType curType;

    [field: Header("방 설정 완료")]
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private TextMeshProUGUI feedbackText;

    public string roomName;
    public int maxPlayerCount;
    public Hashtable roomSettings = new();
    public HashSet<string> lobbyProps = new();

    public event Action dataHandler;

    public void Start()
    {
        roomSettingPanelButton.onClick.AddListener(() => ShowRoomSetting(RoomSettingType.RoomSetting));
        spawnSettingPanelButton.onClick.AddListener(() => ShowRoomSetting(RoomSettingType.SpawnSetting));
        environmentSettingPanelButton.onClick.AddListener(() => ShowRoomSetting(RoomSettingType.EnvironmentSetting));
        ShowRoomSetting(RoomSettingType.RoomSetting);

        confirmButton.onClick.AddListener(() =>
        {
            ConfirmCreateRoom();
            //StartPopupUI.Instance.ShowRoomSetting(StartPopupType.LobbyPanel);
        });
        cancelButton.onClick.AddListener(() => StartPopupUI.Instance.ShowRoomSetting(StartPopupType.LobbyPanel));
    }

    public void Init()
    {
        roomSettingPanel.GetComponent<IUIPanel>().Init();
        spawnSettingPanel.GetComponent<IUIPanel>()?.Init();
        environmentSettingPanel.GetComponent<IUIPanel>()?.Init();
    }

    public void ShowRoomSetting(RoomSettingType setting)
    {
        if (curType == setting) return;
        
        GroupSetActive(roomSettingPanel, setting == RoomSettingType.RoomSetting);
        roomSettingPanelButton.interactable = setting != RoomSettingType.RoomSetting;
        
        GroupSetActive(spawnSettingPanel, setting == RoomSettingType.SpawnSetting);
        spawnSettingPanelButton.interactable = setting != RoomSettingType.SpawnSetting;

        GroupSetActive(environmentSettingPanel, setting == RoomSettingType.EnvironmentSetting);
        environmentSettingPanelButton.interactable = setting != RoomSettingType.EnvironmentSetting;
        curType = setting;
    }

    public void ConfirmCreateRoom()
    {
        dataHandler?.Invoke();
        feedbackText.text = $"'{roomName}' 방 생성 중…";
        
        if (string.IsNullOrEmpty(roomName))
        {
            feedbackText.text = "방 이름을 입력하세요.";
            return;
        }

        var opts = new RoomOptions
        {
            MaxPlayers = maxPlayerCount,
            IsOpen = true,
            IsVisible = true,
            EmptyRoomTtl = 0,
            PlayerTtl = 0,
            CleanupCacheOnLeave = true,
            SuppressRoomEvents = false,
            PublishUserId = true,
            CustomRoomProperties = roomSettings,
            CustomRoomPropertiesForLobby = lobbyProps.ToArray()
        };

        PhotonNetwork.CreateRoom(roomName, opts);
        feedbackText.text = $"'{roomName}' 방 생성 중…";
    }

    public override void OnJoinedRoom() //방을 생성하면 자동입장.
    {
        //싱글모드 
        if (PhotonNetwork.OfflineMode)
        {
            feedbackText.text = "싱글 모드 입장";
            SceneManager.LoadScene(GlobalParameter.sceneName);
            return;
        }
        //멀티모드
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.LoadLevel("WaitingRoomTest");
    }

    public void GroupSetActive(CanvasGroup data, bool isTrue)
    {
        if (isTrue)
        {
            data.alpha = 1;
            data.interactable = true;
            data.blocksRaycasts = true;
        }
        else
        {
            data.alpha = 0;
            data.interactable = false;
            data.blocksRaycasts = false;
        }
    }
}
