using System.Collections;
using Photon.Pun;
using UnityEngine;


public class GameManager : Singleton<GameManager>
{

    public TimeManager TimeManager { get; private set; }
    public WeatherManager WeatherManager { get; private set; }
    public VillageRelationManager VillageRelationManager { get; private set; }
    public AffectionManager AffectionManager { get; private set; }
    public NPCManager NPCManager { get; private set; }

    private SaveManager _saveManager;

    private void Awake()
    {
        _saveManager = SaveManager.Instance;

        TimeManager = GetComponentInChildren<TimeManager>();
        WeatherManager = GetComponentInChildren<WeatherManager>();
        AffectionManager = GetComponentInChildren<AffectionManager>();
        VillageRelationManager = GetComponentInChildren<VillageRelationManager>();
        NPCManager = GetComponentInChildren<NPCManager>();

        TimeManager.Init(_saveManager);
        WeatherManager.Init(_saveManager, TimeManager);
        AffectionManager.Init(_saveManager, TimeManager, NPCManager);
        NPCManager.Init(AffectionManager);
    }

    private void Start()
    {
        //bool isMulti = PhotonNetwork.IsConnected;
        //bool isMaster = isMulti && PhotonNetwork.IsMasterClient;
        //bool isSingle = !isMulti;
        bool isMulti = false;
        bool isMaster = false;
        bool isSingle = true;

        if (isSingle || isMaster)
        {
            _saveManager.LoadGameFromPath(SaveSData.SelectedSavePath);

            if (isMaster)
                TimeManager.SendSync();
        }
        SoundManager.Instance.StopBGM();
        SoundManager.Instance.PlayBGMAsync(SoundParameterData.IngameBGM);

    }
}