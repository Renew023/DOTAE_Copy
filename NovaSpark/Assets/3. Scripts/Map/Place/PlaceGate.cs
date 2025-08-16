using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceGate : MonoBehaviour
{
    //TODO : 해당하는 장소명,
    [field: SerializeField] public Place NextPlace { get; private set; }
    [field: SerializeField] public Place CurPlace { get; private set; } 

    //TODO : 문 반복 이동 방지하는 예외처리
    [SerializeField] private bool _isGateActive = true;

    private void Awake()
    {
        CurPlace = GetComponentInParent<Place>();
    }

    //[field: SerializeField] public 

    // TileMap Gate는 활성화되어있고 => 밟아졌을 때, 다른 게임오브젝트를 전부 비활성화 시키고, 해당 맵에 관련된 거 활성화.

    //public void SetActive(string placeName)
    //{
    //    gameObject.SetActive(placeName == this.placeName);
    //}
    public void GateOpen()
    {
        _isGateActive = true; //활성화시킨다.
    }

    public void GateClose()
    {
        _isGateActive = false;
    }

    public void InGate()
    {
        PlaceManager.Instance.SetActive(NextPlace.name); //장소 매니저에서 이전 장소를 비활성화시킨다.
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {

        Debug.Log("무언가 통과함1");
        if (collision.TryGetComponent<IPlace>(out IPlace character))
        {
            Debug.Log("무언가 통과함2");
            character.SetPlace(NextPlace);
        }

        if (collision.TryGetComponent<Player>(out Player player)) //플레이어가 들어간 공간을 활성화시킨다.
        {
            if (!_isGateActive) return; //활성화되어있지 않으면 그냥 리턴한다.
            PlaceManager.Instance.SetActive(NextPlace.name);
            //TODO : 플레이어가 입구를 들어가면 자동으로 다른 오브젝트들은 다 꺼지도록 해야함.
        }
        //else
        ////플레이어를 제외하고는 그냥 표시되지 않게만 바꾼다.
        //{
        //    //spriteRenderer.enabled = false;   //이것도 잘못된 구조, 카메라에서 감지 못하는 레이어로 가거나 그런 방식ss을 생각해야됨. 컬링 기능
        //    //collision.transform.SetParent(NextPlace.transform); //건물 안으로 이동시킨다.
        //}
    }

    //public void OnTriggerExit2D(Collider2D collision)
    //{
    //    CurPlace.GateActive();
    //}
}
