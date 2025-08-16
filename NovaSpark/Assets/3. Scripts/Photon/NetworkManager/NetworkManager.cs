using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Cinemachine;

public partial class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager Instance { get; private set; }

    [SerializeField] private GameObject playerPrefab;
    private CinemachineVirtualCamera gameCamera;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // 네트워크 송수신 빈도
            PhotonNetwork.SendRate = 30;
            PhotonNetwork.SerializationRate = 30;
            // 고정 프레임 설정
            Time.fixedDeltaTime = 1f / 60f;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
}