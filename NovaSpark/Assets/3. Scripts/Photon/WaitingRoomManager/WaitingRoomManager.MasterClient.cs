using Photon.Realtime;
using Photon.Pun;

public partial class WaitingRoomManager
{
    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        bool amIHost = PhotonNetwork.IsMasterClient;

        roomSettingsButton.gameObject.SetActive(amIHost);
        startGameButton.gameObject.SetActive(amIHost);

        if (amIHost)
            startGameButton.interactable = AllPlayersReady();
    }
}