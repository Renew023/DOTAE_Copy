using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPanel : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button roomCreateButton;
    [SerializeField] private Button gotoMainButton;

    [SerializeField] private Button refreshButton;

    [SerializeField] private TextMeshProUGUI feedbackText;

    [SerializeField] private Transform roomListParent;
    [SerializeField] private GameObject roomPrefab;

    private void Start()
    {
        roomCreateButton.onClick.AddListener(OnClickRoomCreate);
        gotoMainButton.onClick.AddListener(OnClickGotoMain);
        refreshButton.onClick.AddListener(OnClickRefresh);
        feedbackText.text = "로비에 오신 것을 환영합니다!";
    }
    private void OnClickRoomCreate()
    {
        // 방 생성 로직을 여기에 추가s

        if (!PhotonNetwork.IsConnectedAndReady || !PhotonNetwork.InLobby) return;

        StartPopupUI.Instance.ShowRoomSetting(StartPopupType.RoomDetailPanel);
    }

    private void OnClickGotoMain()
    {
        StartPopupUI.Instance.ShowRoomSetting(StartPopupType.StartPanel);
    }

    private void OnClickRefresh()
    {
        Debug.Log("방 목록 새로고침 버튼 클릭됨");
        feedbackText.text = "방 목록 새로고침";

        foreach (Transform t in roomListParent)
            Destroy(t.gameObject);

        PhotonNetwork.LeaveLobby();
        PhotonNetwork.JoinLobby();
        // 방 목록 새로고침 로직을 여기에 추가
        // 예: PhotonNetwork.LeaveLobby(); PhotonNetwork.JoinLobby();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        feedbackText.text = $"입장 실패: {message}";
    }

    public override void OnJoinedRoom()
    {
        feedbackText.text = "방에 성공적으로 입장했습니다!";
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        feedbackText.text = $"생성 실패: {message}";
    }

    public override void OnCreatedRoom()
    {
        feedbackText.text = $"생성 성공";
    }
}
