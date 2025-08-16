using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public partial class NetworkManager
{
    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}