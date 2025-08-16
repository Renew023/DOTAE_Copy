using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Linq;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public partial class WaitingRoomManager
{
    [SerializeField] private float totalTimer = 1f;
    [SerializeField] private AnimationCurve animationCurve;
    public void ToggleReady()
    {
        isReady = !isReady;
        PhotonNetwork.LocalPlayer.SetCustomProperties(
            new Hashtable { { "ready", isReady } });

        var btnTxt = readyButton.GetComponentInChildren<TextMeshProUGUI>();
        btnTxt.text = isReady ? "준비 완료" : "준비";
        btnTxt.ForceMeshUpdate();

        UpdatePlayerList();
    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("ready"))
            UpdatePlayerList();

        if (PhotonNetwork.IsMasterClient)
            startGameButton.interactable = AllPlayersReady();
    }

    private bool AllPlayersReady() =>
        PhotonNetwork.PlayerList.All(p =>
            p.CustomProperties.ContainsKey("ready") && (bool)p.CustomProperties["ready"]);

    public void OnStartGameClicked()
    {
        PhotonNetwork.CurrentRoom.SetCustomProperties(
            new Hashtable { { "inProgress", true } });
        photonView.RPC("StartCountdown", RpcTarget.All);
    }

    [PunRPC]
    private void StartCountdown()
    {
        if (hasStartedLoading) return;
        hasStartedLoading = true;
        StartCoroutine(CountdownAndLoad());
    }

    IEnumerator CountdownAndLoad()
    {
        loadingPanel.alpha = 1;
        loadingPanel.interactable = true;
        loadingPanel.blocksRaycasts = true;

        //5초 카운트다운
        float timer = 0f;
        while (timer < totalTimer)
        {
            timer += Time.deltaTime;
            float time = timer / totalTimer;
            var CurveValue = animationCurve.Evaluate(time);
            loadingBar.value = CurveValue;
            //loadingBar.value = Mathf.Lerp(loadingBar.value, timer / totalTimer, Time.deltaTime * 10f);
            //loadingBar.value = timer / (float)total;
            yield return null;
        }
        loadingBar.value = 1f;
        countdownText.text = "게임 시작!";
        yield return new WaitForSeconds(0.5f);
        if (PhotonNetwork.IsMasterClient)
        {
            StartGame();
        }
    }

    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(GlobalParameter.sceneName);
        }
    }
}
