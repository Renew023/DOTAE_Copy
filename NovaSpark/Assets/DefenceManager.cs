using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DefenceManager : Singleton<DefenceManager>
{
    public int npcCount = 0;
    public int waveMonsterCount = 0;
    public int totalMonsterCount = 0;
    public int waveCount = 1;
    public float nextRoundTimer = 0.0f;

    public TextMeshProUGUI monsterWaveText;
    public TextMeshProUGUI monsterCountText;
    public TextMeshProUGUI npcCountText;
    public GameObject endPanel; // 게임 종료 패널
    
    public Image lifeCount;
    public TextMeshProUGUI lifeCountText;

    [Header("EndPanel 구성")]
    public int killCount = 0;
    public TextMeshProUGUI endText; // 게임 종료 메시지
    public TextMeshProUGUI endDayText; // 일자
    public TextMeshProUGUI endKillCountText; // 처치 수
    public Button reStartButton;
    public Button exitButton;

    public GameObject npcPrefab; // NPC 프리팹

    public List<int> NpcIds = new List<int> { 501, 507, 513, 514 };
    
    public void DeathCycle()
    {
        lifeCount.fillAmount = (float)RoomSettingData.Instance.playerLifeCurCount / (float)RoomSettingData.Instance.playerLifeCount;
        lifeCountText.text = RoomSettingData.Instance.playerLifeCurCount + "/ " + RoomSettingData.Instance.playerLifeCount;
    }

    protected override void Awake()
    {
        base.Awake();
        //Spawn(10);
        //TextReload();
        endPanel.gameObject.SetActive(false); // 게임 종료 패널 비활성화

    }
    private void Start()
    {
        reStartButton.onClick.AddListener(()=>
        {
            //Time.timeScale = 1f; // 게임 시간 재개
            SceneManager.LoadSceneAsync("LoadingScene", LoadSceneMode.Additive);
            });
        exitButton.onClick.AddListener(() => {
            //Time.timeScale = 1f; // 게임 시간 재개
            SceneManager.LoadScene("StartSceneMirror");
        });
        DeathCycle();
    }

    public void TextReload()
    {
        monsterWaveText.text = "Wave : " + waveCount;
        monsterCountText.text = "몬스터 수: " + waveMonsterCount;
        npcCountText.text = "NPC 수: " + npcCount;

        //if(RoomSettingData.Instance.isWaveClearReward)
        //{
        //    GameObject.FindObjectOfType<Player>().AddExp((int)(
        //          RoomSettingData.Instance.waveClearExp 
        //        + RoomSettingData.Instance.waveClearExp
        //        * RoomSettingData.Instance.waveClearCountWeight
        //        * waveCount));
        //}

        if(npcCount <= 0)
        {
            DefenceModeDeath();
        }
    }

    public async void Spawn(int value, Vector2 pos)
    {
        if (npcCount >= RoomSettingData.Instance.npcMaxCount) return;

        for (int i = 0; i < value; i++)
        {
            int randomNpcId = NpcIds[Random.Range(0, NpcIds.Count)];
            var spawnPoint = Random.insideUnitCircle * 10f + pos;

            GameObject npc = Instantiate(npcPrefab, spawnPoint, Quaternion.identity);
            npc.GetComponent<CharacterObject>().Initialize(randomNpcId, 0);
            npc.name = "Npc_" + npcCount;
            npcCount++;
        }

        TextReload(); // UI 업데이트
    }

    public void DefenceModeDeath()
    {
        endPanel.gameObject.SetActive(true); // 게임 종료 패널 활성화
        endText.text = "게임 종료!"; // 게임 종료 메시지 설정
        endDayText.text = "일자: " + GameManager.Instance.TimeManager.DayCount; // 현재 일자 표시
        endKillCountText.text = "처치 수: " + killCount; // 처치 수 표시
        Time.timeScale = 0f; // 게임 일시 정지
    }
}
