using UnityEngine;
using UnityEngine.SceneManagement;

public class UIOnSceneLoadReset : MonoBehaviour
{
    // 씬 전환마다 호출됩니다.
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬 이름에 따라 원하는 초기화 로직
        if (scene.name == "StartScene")
        {
            // 1) 게임 씬 UI 전부 숨기기
            UIManager.Instance.HideAllPanels();  // 옵션, 인벤토리, 스킬창 등 모두 닫기
            // 2) 채팅창도 꺼두고
            ChatManager.Instance.HideChatPanel();

            // (필요하면) UIRoot 자체를 비활성화
            // GameObject.Find("UIRoot").SetActive(false);
        }
        else if (scene.name == GlobalParameter.sceneName)
        {
            // 2) 게임용 기본 HUD만 켜기
            UIManager.Instance.ShowPanel(UIType.HUD);
        }
    }
}