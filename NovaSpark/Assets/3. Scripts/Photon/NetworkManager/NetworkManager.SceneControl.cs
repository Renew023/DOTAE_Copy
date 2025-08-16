using Photon.Pun;

public partial class NetworkManager
{

    // 즉시 게임 씬 로드할 때
    public void LoadGameScene()
    {
        PhotonNetwork.LoadLevel(GlobalParameter.sceneName);
    }
    
}