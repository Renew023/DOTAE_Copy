using UnityEngine;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;  // Hashtable
using Photon.Pun;
using TMPro;

public partial class WaitingRoomManager
{
    private bool isLeavingRoom = false;
    private void UpdatePlayerList()
    {
        foreach (Transform t in playerListContent)
            Destroy(t.gameObject);

        foreach (var p in PhotonNetwork.PlayerList)
        {
            var go = Instantiate(playerNameItemPrefab, playerListContent);
            var txt = go.GetComponentInChildren<TextMeshProUGUI>();
            bool ready = p.CustomProperties.ContainsKey("ready") && (bool)p.CustomProperties["ready"];
            txt.text = p.NickName + (ready ? "   (준비 완료)" : "");
        }
    }

    public void OnLeaveRoomClicked()
    {
        if (isLeavingRoom) return;
        if(PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InRoom)
        {
            isLeavingRoom = true;
            Launcher.ReturnToLobby = true;
         PhotonNetwork.LeaveRoom();
        }
    }

    public override void OnLeftRoom()
    {
        isLeavingRoom=false;
        Destroy(gameObject);
        SceneManager.LoadScene("StartScene");
    }

    public override void OnJoinedRoom()
    {
        ResetReadyState();
        var disp = PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("dispName", out var dn)
            ? dn.ToString() : PhotonNetwork.CurrentRoom.Name;
        roomNameText.text = disp;
        UpdatePlayerList();
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer) => UpdatePlayerList();
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer) => UpdatePlayerList();

    private void ResetReadyState()
    {
        isReady = false;
        PhotonNetwork.LocalPlayer.SetCustomProperties(
            new Hashtable { { "ready", isReady } });

        var btnTxt = readyButton.GetComponentInChildren<TextMeshProUGUI>();
        btnTxt.text = "준비";
        btnTxt.ForceMeshUpdate();

        UpdatePlayerList();
    }
}