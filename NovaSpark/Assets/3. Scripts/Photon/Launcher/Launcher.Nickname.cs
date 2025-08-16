using UnityEngine;
using TMPro;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public partial class Launcher
{
    [Header("닉네임 설정 UI")]
    [SerializeField] private CanvasGroup nickPanel;
    [SerializeField] private TMP_InputField nickInput;
    [SerializeField] private Button setNickButton;
    [SerializeField] private TextMeshProUGUI feedbackText;

    public void OnClickSetNick()
    {
        var nick = nickInput.text.Trim();
        if (string.IsNullOrEmpty(nick))
        {
            feedbackText.text = "닉네임을 입력해주세요.";
            return;
        }

        PhotonNetwork.NickName = nick;
        nickPanel.alpha = 0; nickPanel.interactable = false; nickPanel.blocksRaycasts = false;

        var state = PhotonNetwork.NetworkClientState;
        if (state == ClientState.Disconnected)
        {
            feedbackText.text = $"'{nick}'님, Photon에 연결 중…";
            PhotonNetwork.ConnectUsingSettings();
            setNickButton.interactable = false;
        }
        else if (state == ClientState.ConnectedToMasterServer)
        {
            feedbackText.text = $"'{nick}'님, 로비에 입장 중";
            PhotonNetwork.JoinLobby();
            setNickButton.interactable = false;
        }
        else
        {
            feedbackText.text = "로비 서버 연결 완료!";
            OnJoinedLobby();
        }

        //TO 추가
        modeSelectPanel.alpha = 0;
        modeSelectPanel.interactable = false;
        modeSelectPanel.blocksRaycasts = false;
    }

    public override void OnConnectedToMaster()
    {
        if (isMultiMode)
            PhotonNetwork.JoinLobby();

        feedbackText.text = "로비에 입장했습니다.";
    }
}