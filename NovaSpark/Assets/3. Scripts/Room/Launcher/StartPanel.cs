using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartPanel : MonoBehaviour
{
    [SerializeField] private Button singleButton;
    [SerializeField] private Button multiButton;
    [SerializeField] private Button gameDescriptionButton;

    //TODO 임시

    private void Start()
    {
        //singleButton.onClick.AddListener(()=> StartPopupUI.Instance.ShowRoomSetting(StartPopupType.NickNamePanel));
        //multiButton.onClick.AddListener(() =>
        //{
        //    StartPopupUI.Instance.ShowRoomSetting(StartPopupType.NickNamePanel);
        //    PhotonNetwork.OfflineMode = false;  //오프라인모드 비활성화
        //    //PhotonNetwork.ConnectUsingSettings();
        //});
        singleButton.onClick.AddListener(
            () => {
                GameStart();
                singleButton.interactable = false;
            });
        
        gameDescriptionButton.onClick.AddListener(
            () =>
            {
                StartPopupUI.Instance.ShowRoomSetting(StartPopupType.GameDescriptionPanel);
            });
    }

    private void GameStart()
    {
        SceneManager.LoadScene("LoadingScene", LoadSceneMode.Additive);
        //loadingPanel.Init();
    }
}
