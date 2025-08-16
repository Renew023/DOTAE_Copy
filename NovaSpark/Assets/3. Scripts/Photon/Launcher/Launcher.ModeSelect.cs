using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using JetBrains.Annotations;
using Photon.Realtime;

public partial class Launcher
{
    [Header("싱글/멀티")]
    [SerializeField] private CanvasGroup modeSelectPanel;       //싱글,멀티모드 선택 패널
    [SerializeField] private Button singleButton;               //싱글모드 버튼
    [SerializeField] private Button multiButton;                //멀티모드 버튼

    private bool pendingSingleMode = false;                     //싱글 모드 전환이 대기 중인지 표시

    //모드 선택 UI
    private void SetupModeSelectUI()
    {
        isMultiMode = false;    

        // 모드 선택 패널 켜기
        modeSelectPanel.alpha = 1;
        modeSelectPanel.interactable = true;
        modeSelectPanel.blocksRaycasts = true;

        // 나머지 패널 숨기기
        nickPanel.alpha = 0; 
        nickPanel.interactable = false;
        nickPanel.blocksRaycasts = false;
        lobbyPanel.alpha = 0; 
        lobbyPanel.interactable = false; 
        lobbyPanel.blocksRaycasts = false;
        OffRoomDetail();

        // 버튼 리스너
        singleButton.onClick.RemoveAllListeners();
        singleButton.onClick.AddListener(EnterSingleMode);
        multiButton.onClick.RemoveAllListeners();
        multiButton.onClick.AddListener(EnterMultiMode);
    }

    //싱글모드
    public void EnterSingleMode()
    {
        Debug.Log("[Launcher] EnterSingleMode()");
        pendingSingleMode = true;   //싱글몯 전환 대기 상태
        //모드UI 숨기기
        modeSelectPanel.alpha = 0;
        modeSelectPanel.interactable = false;
        modeSelectPanel.blocksRaycasts = false;

        //Photon상태에 따라 순서대로 
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();  //룸에 참여중이면 룸 나가기
        }
        else if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby(); //로비에 참여중이면 룸 나가기
        }
        else if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect(); //연결 해제
        }
        else
        {
            ApplyOfflineMode();         //이미 연결 해제 상태라면 오프라인모드 
        }
    }
    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        if (pendingSingleMode)
        {
            PhotonNetwork.LeaveLobby();     //룸 나간 후 로비도 나가서 연결 해제 
        }
        else
        {
            SceneManager.LoadScene("StartScene");   //싱글모드 전환이 아닌경우 시작 씬으로
        }
    }
    public override void OnLeftLobby()
    {
        base.OnLeftLobby();
        //로비 나간 후에도 연결이 유지된 상태이고 싱글 모드 전환 대기 상태면 연결 해제
        if(pendingSingleMode && PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"OnDisconnected: {cause}");
        if (pendingSingleMode)
        {
            ApplyOfflineMode(); //연결 해제 완료 시 싱글모드 적용
        }
        else
        {
            base.OnDisconnected(cause); //연결이 끊겼을 때는 기본 동작 
        }
    }
    //오프라인 싱글모드 설정 
    private void ApplyOfflineMode()
    {
        pendingSingleMode = false;      //대기 상태 해제
        IsSingleMode = true;            //게임 모드 싱글로

        PhotonNetwork.OfflineMode = true;       //오프라인 모드 활성화
        PhotonNetwork.AutomaticallySyncScene = false;   //자동 동기화 비활성화

        feedbackText.text = "싱글 모드 입장";
        SceneManager.LoadScene(GlobalParameter.sceneName);  //게임씬 로드
    }

    //멀티 모드
    private void EnterMultiMode()
    {
        isMultiMode = true;                         //멀티모드 활성화
        IsSingleMode = false;                       //싱글모드 비활성화

        //모드 선택 및 대기실 UI 숨기기
        //modeSelectPanel.alpha = 0; 
        //modeSelectPanel.interactable = false;
        //modeSelectPanel.blocksRaycasts = false;
        lobbyPanel.alpha = 0; 
        lobbyPanel.interactable = false; 
        lobbyPanel.blocksRaycasts = false;

        PhotonNetwork.OfflineMode = false;  //오프라인모드 비활성화
        //닉네임 패널 활성화
        nickPanel.alpha = 1; 
        nickPanel.interactable = true;
        nickPanel.blocksRaycasts = true;
        //닉네임 설정 버튼 리스너
        setNickButton.onClick.RemoveAllListeners();
        setNickButton.onClick.AddListener(OnClickSetNick);
        feedbackText.text = "닉네임을 입력하세요.";
        PhotonNetwork.ConnectUsingSettings();
    }
}