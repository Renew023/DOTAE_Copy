using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectPlayerValue : MonoBehaviour
{
    [Header("UI데이터")]
    [SerializeField] private Button prevButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button selectButton;

    [Header("UI - 가변성 데이터")]
    [SerializeField] private Image charImage;
    [SerializeField] private TextMeshProUGUI charName;
    [SerializeField] private TextMeshProUGUI charDescription;
    [SerializeField] private TextMeshProUGUI statValue;
    [SerializeField] private TextMeshProUGUI tecList;

    [Header("데이터")]
    [SerializeField] private int initValue = 0;
    [SerializeField] private List<int> playerData = new();

    [field : Header("프리팹")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject virtualCamera;

    private void Awake()
    {
        playerData.Add(DataManager.Instance.CharacterDataByID[4].characterID);
        playerData.Add(DataManager.Instance.CharacterDataByID[6].characterID);
        //playerData = DataManager.Instance.CharacterDataByID
        //    .Where(cha => cha.Value.characterType == DesignEnums.CharacterType.Player)
        //    .Select(cha => cha.Key)
        //    .ToList();

        ResetText();
        prevButton.onClick.AddListener(() 
            => {
                initValue -= 1;
                if (initValue < 0) initValue = playerData.Count-1;
                ResetText(); //초기화 메서드
            });
        nextButton.onClick.AddListener(() 
            => {
                initValue += 1;
                if (initValue >= playerData.Count) initValue = 0;
                ResetText(); //초기화 메서드
            });
        selectButton.onClick.AddListener(() 
            => {
                SpawnPlayer();
                //TODO : 로딩창? 선택하고도 있긴 해야함.
                gameObject.SetActive(false);
            });
    }

    private void ResetText()
    {
        var key = playerData[initValue];
        var characterIconKey = key + "_characterIcon";
        Debug.Log(characterIconKey);
        charImage.sprite = AddressableManager.Instance.IconCash[characterIconKey];
        charName.text = DataManager.Instance.CharacterDataByID[key].name_kr;
        charDescription.text = DataManager.Instance.CharacterDataByID[key].description;
        statValue.text = "기대치: " + DataManager.Instance.CharacterDataByID[key].damage.ToString(); //나중에 추가합니다.

        string abilitys = "보유 어빌리티: " + string.Join(", ", DataManager.Instance.CharacterDataByID[key].abilityName);
        tecList.text = abilitys;
    }

    private void SpawnPlayer()
    {
        GameObject playerGo = Instantiate(
            playerPrefab, Vector3.zero, Quaternion.identity
        );

        var cameraObj = Instantiate(virtualCamera, playerGo.transform);
        cameraObj.gameObject.GetComponent<CinemachineVirtualCamera>().Follow = playerGo.transform;
        var player = playerGo.GetComponent<Player>();
        
        var key = playerData[initValue];
        player.Initialize(key);
        player.VirtualCamera = cameraObj.GetComponent<CinemachineVirtualCamera>();
        player.noise = player.VirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        //CreateGame.Instance.MapSpawn(RoomSettingData.Instance.farmingBlockSettingCount);
        //GameManager.Instance.TimeManager.OneDayPassed += () => CreateGame.Instance.MapSpawn(RoomSettingData.Instance.farmingBlockPerOneDayCount);
        //playerPrefab.AddPhoton();
        //GameObject playerGo = PhotonNetwork.Instantiate(
        //    playerPrefab.name, Vector3.zero, Quaternion.identity
        //);

        //var pv = playerGo.GetComponent<PhotonView>();
        //var player = playerGo.GetComponent<Player>();

        //var key = playerData[initValue];
        //player.photonView.RPC(nameof(player.Initialize), RpcTarget.All, key, key);

        //if (pv.IsMine)
        //{
        //    var cameraObj = Instantiate(virtualCamera, playerGo.transform);
        //    cameraObj.gameObject.GetComponent<CinemachineVirtualCamera>().Follow = playerGo.transform;
        //    player.InitializeLocal();
        //}
    }
}
