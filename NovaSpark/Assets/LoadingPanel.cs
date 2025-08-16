using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingPanel : MonoBehaviour
{
    private AsyncOperation asyncLoad;

    [SerializeField] private Image progressBar;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private Button gameStartButton;
    [SerializeField] private AnimationCurve loadingCurve;
    [SerializeField] private float minLoadTime;
    [SerializeField] private float loadAfterTime;
    [SerializeField] private Scene curScene;

    private void Awake()
    {
        gameStartButton = GetComponentInChildren<Button>(true);
    }

    private void OnEnable()
    {
        //await Task.Yield(); //씬이 활성화된 후에 실행되도록 보장
        Init();
    }
    public async void Init()
    {
        //await Task.Yield(); //씬이 활성화된 후에 실행되도록 보장
        //SoundManager.Instance.StopBGM();
        //SoundManager.Instance.PlayBGMAsync(SoundParameterData.LoadingBGM);
        //DataManager.Instance.LoadAllDataAsync();
        curScene = SceneManager.GetActiveScene();
        //DataManager.Instance.LoadAllDataAsync();
        //GameSceneLoad();
        StartCoroutine(GameDataLoad());
        _ = DataManager.Instance.LoadAllDataAsync();
    }

    private void GameSceneLoad()
    {
        SceneManager.LoadScene(GlobalParameter.sceneName);
        //asyncLoad.allowSceneActivation = false; //씬 전환을 막음
    }

    private IEnumerator GameDataLoad()
    {
        while (DataManager.Instance.IsLoaded == false || AddressableManager.Instance.isCashing == false)
        {
            progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, ((float)DataManager.Instance.progressCount / DataManager.Instance.progressTotal), 0.2f);
            progressText.text = $"데이터 불러오는 중... ({DataManager.Instance.progressCount}" + " / " + $"{DataManager.Instance.progressTotal}" + ")"; //로딩 퍼센트 업데이트
            yield return null; //다음 프레임까지 대기
        }
        progressBar.fillAmount = 1.0f;
        yield return null;

        progressText.text = $"게임 접속을 위해 화면을 클릭해주세요"; //로딩 퍼센트 업데이트

        gameStartButton.gameObject.SetActive(true);

        gameStartButton.onClick.RemoveAllListeners(); //기존 리스너 제거
        gameStartButton.onClick.AddListener(() =>
        {
            Time.timeScale = 1f;
            RoomSettingData.Instance.Init();
            StartCoroutine(ButtonReady());
        });

        //StartCoroutine(SceneProgress());
    }

    private IEnumerator SceneProgress()
    {
        float timer = 0f;

        yield return null;

        while(true)
        {
            if (asyncLoad.progress >= 0.9f)
            {
                timer += Time.unscaledDeltaTime * loadAfterTime;
            }
            else
            {
                timer += Time.unscaledDeltaTime;
            }

            float load = Mathf.Min(timer / minLoadTime); // 근데 progress는 점진적으로 오르지 않음. 그냥 0이 떠버림
            var value = 0.5f + load / 2f;

            if (value > 0.9f && asyncLoad.progress < 0.9f) continue;
            //var value = load / 2f;
            //씬 로딩이 아직 완료되지 않았을 때   
            var curveValue = loadingCurve.Evaluate(value); //로딩 곡선 적용
            progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, curveValue, 0.2f); //0.5f는 데이터 로딩의 절반을 차지함
            progressText.text = $"맵 불러오는 중... ({(progressBar.fillAmount* 100):0}%)"; //로딩 퍼센트 업데이트
            yield return null; //다음 프레임까지 대기

            if (asyncLoad.progress >= 0.9f && timer >= minLoadTime) //씬 로딩이 완료되고 최소 로딩 시간이 지났을 때
            {
                progressText.text = $"게임 접속합니다!"; //로딩 퍼센트 업데이트
                progressBar.fillAmount = 1f;
                break; //루프 종료
            }
        }

        yield return new WaitForSecondsRealtime(2f); //2초 대기
        //progressText.text = $"게임 접속하기 위해서 시작 버튼을 눌러주세요"; //로딩 퍼센트 업데이트
        progressText.text = $"게임 접속을 위해 화면을 클릭해주세요"; //로딩 퍼센트 업데이트
        gameStartButton.gameObject.SetActive(true);

        gameStartButton.onClick.RemoveAllListeners(); //기존 리스너 제거
        gameStartButton.onClick.AddListener(() =>
        {
            Time.timeScale = 1f;
            RoomSettingData.Instance.Init();
            StartCoroutine(ButtonReady());
        });

        yield return null; //다음 프레임까지 대기
    }

    private IEnumerator ButtonReady()
    {
        GameSceneLoad();
        //asyncLoad.allowSceneActivation = true; //씬 전환 허용
        yield return null;
        //yield return new WaitUntil(() => asyncLoad.isDone); //씬 로딩이 완료될 때까지 대기
        //Scene loadingScene = this.gameObject.scene;
        //SceneManager.SetActiveScene(loadingScene);
        //SceneController.Instance.SceneChange(curScene, loadingScene, SceneManager.GetSceneByName(GlobalParameter.sceneName));
    }
}
