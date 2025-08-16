using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public partial class WaitingRoomManager : MonoBehaviourPunCallbacks
{
    public static WaitingRoomManager Instance { get; private set; }
    private Dictionary<string, float> currentSettings;

    [SerializeField] private Button readyButton;
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button leaveButton;
    [SerializeField] private CanvasGroup loadingPanel;
    [SerializeField] private Slider loadingBar;
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private Transform playerListContent;
    [SerializeField] private GameObject playerNameItemPrefab;
    [SerializeField] private TMP_Text roomNameText;

    [Header("방 설정(호스트)")]
    [SerializeField] private TMP_InputField roomNameInput;
    [SerializeField] private Button roomSettingsButton;
    [SerializeField] private GameObject settingPanel;
    [SerializeField] private TMP_InputField pwdInput;
    [SerializeField] private TMP_Dropdown maxPlayersDropdown;
    [SerializeField] private TMP_InputField descInput;
    [SerializeField] private Button confirmSettingsButton;
    [SerializeField] private Button cancelSettingsButton;
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private TMP_InputField seedInput;

    private bool isReady = false;
    private bool hasStartedLoading = false;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            currentSettings = new Dictionary<string, float>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        currentSettings = new Dictionary<string, float>();
    }
    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        // 준비 버튼
        readyButton.onClick.RemoveAllListeners();
        readyButton.onClick.AddListener(ToggleReady);

        // 시작(호스트 전용)
        startGameButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        startGameButton.interactable = false;
        startGameButton.onClick.AddListener(OnStartGameClicked);

        // 로딩 패널 초기화
        loadingPanel.alpha = 0;
        loadingPanel.interactable = false;
        loadingPanel.blocksRaycasts = false;

        //방 속성 패널
        //gameDetailPanel.alpha = 0;
        //gameDetailPanel.interactable = false;
        //gameDetailPanel.blocksRaycasts = false;

        //gamePropertiesButton.onClick.RemoveAllListeners();
        //gamePropertiesButton.onClick.AddListener(OpenGameDetailPanel);

        // 방 설정(호스트 전용)
        roomSettingsButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        roomSettingsButton.onClick.AddListener(OpenSettingsPanel);
        confirmSettingsButton.onClick.AddListener(ApplyRoomSettings);
        cancelSettingsButton.onClick.AddListener(CloseSettingsPanel);
        settingPanel.SetActive(false);

        // 입장 시 방 이름/목록 세팅
        if (PhotonNetwork.InRoom)
        {
            roomNameText.text = PhotonNetwork.CurrentRoom.Name;
            UpdatePlayerList();
        }

        // 나가기 버튼
        leaveButton.onClick.RemoveAllListeners();
        leaveButton.onClick.AddListener(OnLeaveRoomClicked);
    }
}