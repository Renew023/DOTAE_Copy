using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Cinemachine;

public partial class NetworkManager
{
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //현재 씬이 게임씬이 아니면 처리안함
        if (scene.name != GlobalParameter.sceneName) return;
        //싱글모드가 아니고 멀티룸에 속해 있지 않으면 스폰안함
        if (!Launcher.IsSingleMode && !PhotonNetwork.InRoom) return;

        // 카메라 참조
        if (gameCamera == null)
        {
            gameCamera = FindObjectOfType<CinemachineVirtualCamera>();
            if (gameCamera == null)
                Debug.LogError("[NetworkManager] 게임 씬에 CinemachineVirtualCamera가 없습니다!");
        }

        // 스폰 위치 결정
        int actorIdx = PhotonNetwork.LocalPlayer.ActorNumber;
        Vector3 spawnPos = actorIdx == 1
            ? new Vector3(-3, 0, 0)
            : actorIdx == 2
                ? new Vector3(3, 0, 0)
                : Vector3.zero;
        //GameObject playerGo;
        //if (Launcher.IsSingleMode)
        //{
        //    playerGo = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
        //}
        //else
        //{
        //    // 플레이어 인스턴스화
        //    playerGo = PhotonNetwork.Instantiate(playerPrefab.name, spawnPos, Quaternion.identity);
        //}

        //var player = playerGo.GetComponent<Player>();

        //// 공통 초기화
        //player.Initialize(4);

        //bool isLocal = Launcher.IsSingleMode || playerGo.GetComponent<PhotonView>().IsMine;

        //// 내 플레이어면 로컬 초기화 & 카메라 추적 설정
        //if (isLocal)
        //{
        //    player.InitializeLocal();
        //    if (gameCamera != null)
        //    {
        //        gameCamera.Follow = playerGo.transform;
        //        gameCamera.LookAt = playerGo.transform;
        //    }
        //}
    }
}