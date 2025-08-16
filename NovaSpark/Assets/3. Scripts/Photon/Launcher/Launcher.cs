using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public partial class Launcher : MonoBehaviourPunCallbacks
{
    // 공통 로직·상태 변수
    private bool isMultiMode = false;
    private const byte MaxPlayers = 5;
    private bool isLoading = false;

    public static bool ReturnToLobby = false;
    public static bool IsSingleMode = false;

    private void Start()
    {
        // 싱글모드에서 다시 런처로 복귀
        if (IsSingleMode)
        {
            IsSingleMode = false;
            PhotonNetwork.OfflineMode = true;
            SetupModeSelectUI();
            return;
        }

        // 게임 중 로비 복귀 처리
        if (ReturnToLobby)
        {
            ReturnToLobby = false;
            isMultiMode = true;
            SetupLobbyUI();

            if (PhotonNetwork.IsConnectedAndReady)
                PhotonNetwork.JoinLobby();
            return;
        }
        else
        {
            //첫 실행 또는 서버 연결이 끊긴 상태에서 처리
            var state = PhotonNetwork.NetworkClientState;
            //연결 상태가 Disconnected 또는 PeerCreated 일 때 서버 연결 시도
            if (state == ClientState.Disconnected || state == ClientState.PeerCreated)
                PhotonNetwork.ConnectUsingSettings();
            else
                Debug.Log($"ConnectUsingSettings() 스킵: 현재 상태 = {state}");
        }
        SetupModeSelectUI();
    }
}