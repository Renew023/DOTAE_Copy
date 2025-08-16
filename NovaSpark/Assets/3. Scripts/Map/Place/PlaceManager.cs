using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaceManager : Singleton<PlaceManager>
{
    // static PlaceManager Instance { get; private set; }

    //public List<PlaceGate> places = new(); //기본 땅, 마을 내 집, 동굴을 전부 내포.
    public List<Place> places = new();
    public Place curPlace;
    public Place nextPlace;

    //public Dictionary<string, PlaceGate> placeDictionary = new();
    public Dictionary<string, Place> placeDictionary = new();

    public List<CharacterObject> aiObjects = new();

    public bool IsIndoor => curPlace != null && curPlace.name != "Default";


    protected override void Awake()
    {
        DontDestroyOnLoad(gameObject);
        base.Awake();
        //if (Instance == null)
        //{
        //    Instance = this;

        //}
        //else
        //{
        //    Destroy(gameObject);
        //    return;
        //}
    }

    private void Start()
    {
        foreach (var place in places)
        {
            placeDictionary.Add(place.name, place);
            placeDictionary[place.name].PlaceOff(); //모든 장소를 비활성화 시킨다.
        }

        Initialize();
    }

    public Place GetDefaultPlace()
    {
        curPlace = placeDictionary["Default"];
        return curPlace;
    }

    public void Initialize()
    {
        //curPlace = placeDictionary["Default"];
        //curPlace.PlaceOn();
        //PlaceManager.Instance.SetActive(curPlace.name);
    }

    public void SetActive(string placeName)
    {
        nextPlace = placeDictionary[placeName]; //다음 장소를 받아온다.
        curPlace.PlaceOff();
        curPlace = nextPlace;
        curPlace.PlaceOn();
        foreach (var ai in aiObjects)
        {
            ai.CheckPlace();
        }

        GameManager.Instance.TimeManager.RefreshLighting();
    }
}