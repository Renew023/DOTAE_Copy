using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomExitHandler : MonoBehaviourPunCallbacks
{
    // 버튼으로 호출할 메서드
    public void QuitToStartScene()
    {
        if (PhotonNetwork.InRoom)
        {
            // 룸을 나가면 OnLeftRoom 콜백이 자동으로 호출됩니다.
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            // 이미 룸에 없으면 곧바로 씬 이동
            SceneManager.LoadScene("StartScene");
        }
    }

    // MonoBehaviourPunCallbacks 의 콜백 오버라이드
    public override void OnLeftRoom()
    {
        // 룸 나가기 완료 직후 실행
        SceneManager.LoadScene("StartScene");
    }
}