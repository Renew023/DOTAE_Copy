using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkSetting : Singleton<NetworkSetting>
{
    // Start is called before the first frame update
    protected override void Awake()
    {
        DontDestroyOnLoad(gameObject);
        base.Awake();

        PhotonNetwork.SendRate = 30;
        PhotonNetwork.SerializationRate = 10;
        // 고정 프레임 설정
        Time.fixedDeltaTime = 1f / 60f;
    }
}
