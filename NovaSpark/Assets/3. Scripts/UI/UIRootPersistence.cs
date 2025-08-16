using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIRootPersistence : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "StartScene")
        {
            UIManager.Instance.HideAllPanels();
            ChatManager.Instance.HideChatPanel();
        }
        else if (scene.name == GlobalParameter.sceneName)
        {
            // 열려 있는 모든 팝업 끄기
            UIManager.Instance.HideAllPanels();
            // 채팅창 끄기
            ChatManager.Instance.HideChatPanel();
            // 기본 HUD만 켜기
            UIManager.Instance.ShowPanel(UIType.HUD);
        }
    }
}
