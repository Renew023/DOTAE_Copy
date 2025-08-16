using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public partial class Launcher : MonoBehaviourPunCallbacks
{
    public override void OnJoinedRoom()
    {
        //싱글모드 
        if (PhotonNetwork.OfflineMode)
        {
            feedbackText.text = "싱글 모드 입장";
            SceneManager.LoadScene(GlobalParameter.sceneName);
            return;
        }
        //멀티모드
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.LoadLevel("WaitingRoom");
    }

    //메인화면으로 돌아가기 버튼 클릭 시 
    public void OnBackToMainClicked()
    {
        //룸 나가기
        if (PhotonNetwork.InRoom)
            PhotonNetwork.LeaveRoom();
        //로비 나가기
        else if (PhotonNetwork.InLobby)
            PhotonNetwork.LeaveLobby();

    }
}