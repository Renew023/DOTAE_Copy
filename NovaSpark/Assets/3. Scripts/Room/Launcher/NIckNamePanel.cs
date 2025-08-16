using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NIckNamePanel : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField nickNameinputField;
    [SerializeField] private Button selectButton;
    [SerializeField] private TextMeshProUGUI descriptionText;

    private void Start()
    {
        selectButton.onClick.AddListener(() => UpdateNickName());
    }

    private void UpdateNickName()
    {
        selectButton.interactable = false;
        string name = nickNameinputField.text.Trim();
        if(string.IsNullOrEmpty(name))
        {
            descriptionText.text = "닉네임을 입력해주세요.";
            selectButton.interactable = true;
            return;
        }

        PhotonNetwork.NickName = name;
        //PhotonNetwork.AuthValues = new AuthenticationValues("myCustomUserId123");

        if (!PhotonNetwork.IsConnected)
        {
            descriptionText.text = $"'{name}'님, Photon에 연결 중…";
            Debug.Log(gameObject.name + "연결 중입니다");
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            Debug.Log(gameObject.name + "방 들어갑니다!");
            PhotonNetwork.JoinLobby();
        }
    }

    #region Photon콜백
    public override void OnConnectedToMaster()
    {
        descriptionText.text = "로비에 입장 중";
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        StartPopupUI.Instance.ShowRoomSetting(StartPopupType.LobbyPanel);
        selectButton.interactable = true;
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        descriptionText.text = $"서버 연결이 끊어졌습니다: {cause}";
        selectButton.interactable = true;
    }
    #endregion
}
