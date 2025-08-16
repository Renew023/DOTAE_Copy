using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Pun;
using Photon.Realtime;

public partial class Launcher
{
    [Header("방 정보 UI")]
    [SerializeField] private CanvasGroup roomDetailPanel;
    [SerializeField] private TMP_Text roomNameText;
    [SerializeField] private TMP_InputField roomNameInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private Toggle usePasswordToggle;
    [SerializeField] private TMP_Dropdown playerLimitDropdown;
    [SerializeField] private TMP_InputField descriptionInput;
    [SerializeField] private Button roomCreateButton;
    [SerializeField] private Button backButton;

    [Header("비밀번호 표시 토글")]
    [SerializeField] private Button togglePwdButton;
    private bool pwdVisible = false;

    private void InitShowOffPasswordButton()
    {
        togglePwdButton.onClick.RemoveAllListeners();

        pwdVisible = false;
        passwordInput.inputType = TMP_InputField.InputType.Password;
        passwordInput.ForceLabelUpdate();

        var txt = togglePwdButton.GetComponentInChildren<TMP_Text>();
        txt.text = "표시";

        togglePwdButton.onClick.AddListener(TogglePasswordVisibility);
    }

    private void ShowRoomDetail()
    {
        roomDetailPanel.alpha = 1;
        roomDetailPanel.interactable = true;
        roomDetailPanel.blocksRaycasts = true;
    }

    private void OffRoomDetail()
    {
        roomDetailPanel.alpha = 0;
        roomDetailPanel.interactable = false;
        roomDetailPanel.blocksRaycasts = false;
    }

    private void ConfirmCreateRoom()
    {
        var name = roomNameInput.text.Trim();
        if (string.IsNullOrEmpty(name))
        {
            feedbackText.text = "방 이름을 입력하세요.";
            return;
        }

        var pwd = passwordInput.text.Trim();
        byte maxP = (byte)(playerLimitDropdown.value + 1);
        var desc = descriptionInput.text.Trim();

        var props = new Hashtable
        {
            { "dispName", name },
            { "pwd", pwd },
            { "desc", desc },
            { "inProgress", false }
        };
        var opts = new RoomOptions
        {
            MaxPlayers = maxP,
            CustomRoomProperties = props,
            CustomRoomPropertiesForLobby = new string[] { "dispName", "desc", "pwd", "inProgress" }
        };

        PhotonNetwork.CreateRoom(name, opts);
        feedbackText.text = $"'{name}' 방 생성 중…";
        OffRoomDetail();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        feedbackText.text = $"생성 실패: {message}";
    }

    private void TogglePasswordVisibility()
    {
        pwdVisible = !pwdVisible;
        passwordInput.inputType = pwdVisible
            ? TMP_InputField.InputType.Standard
            : TMP_InputField.InputType.Password;
        passwordInput.ForceLabelUpdate();

        var txt = togglePwdButton.GetComponentInChildren<TMP_Text>();
        txt.text = pwdVisible ? "숨기기" : "표시";
    }
}