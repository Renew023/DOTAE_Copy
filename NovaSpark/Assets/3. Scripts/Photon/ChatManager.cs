using System.Collections;
using System.Collections.Generic;
using Photon.Chat;
using Photon.Pun;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using System.Linq;

public class ChatManager : MonoBehaviour, IChatClientListener
{
    public static ChatManager Instance {  get; private set; }
    public bool IsChatActive => chatActive;
    [Header("UI")]
    [SerializeField] GameObject chatPanel;          //채팅패널
    [SerializeField] TMP_InputField chatInput;      //채팅 입력창
    [SerializeField] Button sendButton;             //전송 버튼
    [SerializeField] Transform content;             //채팅메시지가 들어갈 content
    [SerializeField] GameObject messagePrefab;      //채팅메시지 프리팹
    [SerializeField] Toggle chatToggle;             //채팅 온오프 토글
    [SerializeField] int maxMessages = 50;          //채팅 최대 개수 ( 초과시 오래된 것부터 삭제)

    private bool chatActive = false;                //채팅 입력중인지 bool
    private string chatChannel;                     //구독중인 채널(방이름)

    private ChatClient chatClient;                  //채팅클라이언트
    private string userId;                          //유저 아이디(닉네임)

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        if (Launcher.IsSingleMode)
        {
            Debug.Log("[ChatManager] 싱글 모드 채팅 비활성화");
            chatToggle.gameObject.SetActive(false);
            chatPanel.SetActive(false);
            this.enabled = false;
            return;
        }
     chatPanel.SetActive(false);                            //채팅패널숨김
     chatInput.gameObject.SetActive(false);                 //입력창 숨김
    //토글버튼에 연결
     chatToggle.onValueChanged.AddListener(OnToggleChat);

        //유저 id가 비어있으면 User로 초기화
        if (string.IsNullOrEmpty(userId))
        {
            userId = "User";
        }
        //Photon에서 지정한 닉네임 가져옴
        userId = PhotonNetwork.NickName;
        //Photon ChatClient 생성 및 연결
        chatClient = new ChatClient(this);
        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, "1.0",
            new Photon.Chat.AuthenticationValues(userId));

        chatInput.onSubmit.AddListener((_) => OnSendClicked());     //채팅 입력창에 엔터로 연결
        //전송 연결
        sendButton.onClick.RemoveAllListeners();
        sendButton.onClick.AddListener(OnSendClicked);

        StartCoroutine(TryJoinChannelAfterReady());
    }

    private void Update()
    {
        //Photon ChatClient 내부 서비스 처리
        chatClient.Service();
        //토글이 켜져있을때만 입력
        if (chatPanel.activeSelf)
        {
            //채팅이 열려있고 인풋필드가 비활성화 상태에서 엔터입력
            if(!chatInput.gameObject.activeSelf && Input.GetKeyDown(KeyCode.Return))
            {
                chatInput.gameObject.SetActive(true);
                chatInput.text = "";
                chatInput.ActivateInputField();         //입력창에 커서 활성
                chatActive = true;
            }
            //채팅 중 엔터 누르면 메시지 전송
            else if(chatInput.gameObject.activeSelf && Input.GetKeyDown(KeyCode.Return))
            {
                if (!string.IsNullOrEmpty(chatInput.text))
                {
                    OnSendClicked();                    //메시지 전송
                }
                chatInput.DeactivateInputField();       //입력창 비활성화
                chatInput.gameObject.SetActive(false);
                chatActive = false;
            }
            //Esc로 채팅 취소 
            else if(chatInput.gameObject.activeSelf && Input.GetKeyDown(KeyCode.Escape))
            {
                chatInput.DeactivateInputField();
                chatInput.gameObject.SetActive(false);
                chatActive = false;
            }
        }
    }

    IEnumerator TryJoinChannelAfterReady()
    {
        //채팅준비/네트워크 입장 될 때까지 대기
        while (chatClient == null || !chatClient.CanChat || !PhotonNetwork.InRoom)
            yield return null;

        chatChannel = PhotonNetwork.CurrentRoom.Name;   //현재 방 이름을 채팅채널로
        chatClient.Subscribe(new[] { chatChannel });    //해당채널 입장
    }


    //토글버튼
    private void OnToggleChat(bool isOn)
    {
        chatPanel.SetActive(isOn);
        if (!isOn)
        {
            chatInput.gameObject.SetActive(false);//꺼질 때 인풋도 꺼짐
            chatActive=false;
        }
    }
    //방 채널로 채팅 입장
    public void JoinRoomChannel()
    {
        chatChannel = PhotonNetwork.CurrentRoom.Name;
        if (chatClient != null && chatClient.CanChat)
        {
            chatClient.Subscribe(new[] { chatChannel });
        }
    }

    //메시지 전송 입력시
    private void OnSendClicked()
    {
        string msg = chatInput.text;
        if (string.IsNullOrWhiteSpace(msg)) return;     //빈 메시지는 무시
        chatClient.PublishMessage(chatChannel, msg);    //현재 채널로 메시지 전송
        chatInput.text = "";                            //입력창 초기화
    }
    //채팅상태변화 호출
    public void OnChatStateChange(ChatState state) { }
    //채팅서버 연결 성공시 호출
    public void OnConnected()
    {
        bool ok =chatClient.Subscribe(new[] { "Global" });
    }
    //채팅서버 연결 끊겼을 때 호출
    public void OnDisconnected()
    {
        Debug.LogWarning("채팅 서버와 연결이 끊겼습니다. 재연결을 시도합니다.");
        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, "1.0",
            new Photon.Chat.AuthenticationValues(userId));
    }
    //메시지 수신시 호출
    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        if (channelName != chatChannel) return;     //내 채널이 아니면 무시

        for (int i = 0; i < senders.Length; i++)
        {
            var go = Instantiate(messagePrefab, content);
            var text = go.GetComponent<TMP_Text>();

            if (senders[i] == userId)
            {
                //내 메시지는 색상 처리
                text.text = $"<color=green>[나]</color>{messages[i]}";
            }
            else
            {
                //다른사람 메시지
                text.text = $"[{senders[i]}]{messages[i]}";
            }
            go.transform.SetAsLastSibling();        //맨 아래로 정렬
        }
        //레이아웃 업데이트
        LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
        Canvas.ForceUpdateCanvases();
        //메시지가 maxMessages보다 많아지면 오래된 것부터 삭제
        while (content.childCount > maxMessages)
        {
            Destroy(content.GetChild(content.childCount - 1).gameObject);
        }
        //스크롤 맨 아래로 내림
        var scroll = content.GetComponentInParent<ScrollRect>();
        if(scroll != null)
        {
            //1= 맨위 0= 맨 아래
            scroll.verticalNormalizedPosition = 0f;
        }
    }
    public void HideChatPanel()
    {
        // 토글 해제
        chatToggle.isOn = false;

        // 패널과 입력창 비활성화
        chatPanel.SetActive(false);
        chatInput.gameObject.SetActive(false);

        // 채팅 중 플래그 해제
        chatActive = false;
    }
    //필수 구현에 필요함
    public void OnPrivateMessage(string sender, object message, string channelName) { }
    public void OnSubscribed(string[] channels, bool[] results) { }
    public void OnUnsubscribed(string[] channels) { }
    public void OnStatusUpdate(string user, int status, bool gotMessage, object message) { }
    public void DebugReturn(DebugLevel level, string message) { }
    public void OnUserSubscribed(string channelName, string user) { }
    public void OnUserUnsubscribed(string channelName, string user) { }
}
