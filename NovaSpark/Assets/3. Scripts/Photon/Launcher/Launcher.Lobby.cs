using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public partial class Launcher
{
    [Header("로비 UI")]
    [SerializeField] private CanvasGroup lobbyPanel;
    [SerializeField] private Button createButton;
    [SerializeField] private Button refreshButton;

    private void SetupLobbyUI()
    {
        modeSelectPanel.alpha = 0; modeSelectPanel.interactable = false; modeSelectPanel.blocksRaycasts = false;
        nickPanel.alpha = 0; nickPanel.interactable = false; nickPanel.blocksRaycasts = false;

        lobbyPanel.alpha = 1; lobbyPanel.interactable = true; lobbyPanel.blocksRaycasts = true;
        OffRoomDetail();

        createButton.onClick.RemoveAllListeners();
        createButton.onClick.AddListener(OnClickCreateRoom);
        refreshButton.onClick.RemoveAllListeners();
        refreshButton.onClick.AddListener(RefreshRoomList);
    }

    public override void OnJoinedLobby()
    {
        lobbyPanel.alpha = 1; lobbyPanel.interactable = true; lobbyPanel.blocksRaycasts = true;

        createButton.interactable = true;
        createButton.onClick.RemoveAllListeners();
        createButton.onClick.AddListener(OnClickCreateRoom);

        refreshButton.onClick.RemoveAllListeners();
        refreshButton.onClick.AddListener(RefreshRoomList);
    }

    public void RefreshRoomList()
    {
        feedbackText.text = "방 목록 새로고침";

        foreach (Transform t in roomListContent)
            Destroy(t.gameObject);

        PhotonNetwork.LeaveLobby();
        PhotonNetwork.JoinLobby();
    }

    private void OnClickCreateRoom()
    {
        if (!PhotonNetwork.IsConnectedAndReady || !PhotonNetwork.InLobby) return;

        ShowRoomDetail();
        InitShowOffPasswordButton();

        roomNameText.text = "방 설정";
        roomNameInput.interactable = true;
        playerLimitDropdown.interactable = true;
        descriptionInput.interactable = true;

        usePasswordToggle.onValueChanged.RemoveAllListeners();
        usePasswordToggle.interactable = true;
        usePasswordToggle.isOn = false;

        passwordInput.gameObject.SetActive(false);
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
        roomCreateButton.GetComponentInChildren<TMP_Text>().text = "방 만들기";
        roomCreateButton.onClick.AddListener(ConfirmCreateRoom);

        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(OffRoomDetail);

        roomNameInput.text = "";
        passwordInput.text = "";
        playerLimitDropdown.value = MaxPlayers - 1;
        descriptionInput.text = "";
        feedbackText.text = "방 설정 입력해주세요.";
    }
}