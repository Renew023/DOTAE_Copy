using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Place : MonoBehaviour//장소에는 뭐가 있을까?
{
    [field: SerializeField] public List<PlaceGate> PlaceGate { get; private set; } //장소 입구
    [field: SerializeField] public CinemachineVirtualCamera VirtualCamera { get; private set; }
    [field: SerializeField] public List<Renderer> Renderers { get; private set; }

    //public int npcMaxCount; //여관일 경우에는 0이고
    //public int npcCurCount;

    //public List<Vector2> characterWorkSpace { get; private set; } = new(); // 캐릭터가 가지는 일자리 위치.
    //public List<CharacterObject> characterObjects { get; private set; } = new(); //장소에 배정된 character

    //public List<int> characterID { get; private set; } = new();//소환할 몬스터 ID

    private void Awake()
    {
        PlaceGate = GetComponentsInChildren<PlaceGate>(true).ToList(); //장소에 있는 모든 게이트를 받아온다.
        VirtualCamera = GetComponentInChildren<CinemachineVirtualCamera>(true);
        Renderers = GetComponentsInChildren<Renderer>(true).ToList();

    }

    public void PlaceOff()
    {
        if(VirtualCamera != null)
            VirtualCamera.enabled = false;
        foreach(var renderer in Renderers)
        {
            renderer.enabled = false;
        }
        foreach (var gate in PlaceGate)
        {
            gate.GateClose();
        }
    }

    public void PlaceOn()
    {
        if(VirtualCamera != null)
            VirtualCamera.enabled = true;
        foreach (var renderer in Renderers)
        {
            renderer.enabled = true;
        }

        foreach (var gate in PlaceGate)
        {
            gate.GateOpen();
        }
    }
    //public void GateActive()
    //{
    //    foreach (var gate in PlaceGate)
    //    {
    //        gate.GateActive();
    //    }
    //}
}
