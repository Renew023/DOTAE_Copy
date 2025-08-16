using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : Singleton<SceneController>
{
    public async void SceneChange(Scene curScene, Scene loadingScene, Scene nextScene)
    {
        //await Task.Yield();
        SceneManager.SetActiveScene(nextScene);

        SceneManager.UnloadSceneAsync(curScene); //현재 씬 언로드
        SceneManager.UnloadSceneAsync(loadingScene);
    }
}
