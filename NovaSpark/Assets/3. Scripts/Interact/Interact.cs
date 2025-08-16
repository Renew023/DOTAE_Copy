using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Interact : MonoBehaviour
{
    //OnInteract 키가 눌렸을 때,
    //레이어를 비교한다. 대화 상대방은 플레이어의 간격 내에 하고 싶은 해당 오브젝트가 존재할 때,
    [field: SerializeField] private PlayerKey playerKey;
    private Player player;
    [field: SerializeField] public LayerMask LayerMask { get; private set; }
    [field: SerializeField] public float distance;

    private void Start()
    {
        playerKey = GetComponentInChildren<PlayerKey>();
        player = GetComponent<Player>();
        //playerKey.Distance;
    }

    public void Update()
    {
        Debug.DrawLine(transform.position, player.Direction, Color.red);
    }

    [PunRPC]
    public void Targeting() //TODO 방향에 대한 값이 필요.Vector2 값에 따라 보는 방향이 바뀌고 들어갈 Ray 방향도 변경.
    {
        Ray2D ray = new Ray2D(transform.position, player.Direction - (Vector2)transform.position); //바라보는 방향에
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, distance, LayerMask);
        //Debug.Log(hit);

        if (hit.collider == null) { Debug.Log("상호작용 가능한게 없습니다"); return; }

        //Debug.Log(hit.collider.name);
        if (hit.collider.TryGetComponent<IInteractable>(out IInteractable interactable))
        {
            //Debug.Log("상호작용 가능한 오브젝트가 있습니다.");
            interactable?.OnInteract(player);
            interactable?.GetInteractObjectType(interactable.IsCan());
            //TODO : Text = PromptText();
        }
        else
        {
            //Debug.Log("상호작용이 빠져있습니다.");
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.up * 2f);
        //Gizmos.DrawWireSphere(playerKey.ClickPos, 0.5f);
    }
}

public enum InteractObjectType
{
    None = 0,
    Read = 1, //읽기, //TODO : 책 문서
    Talk = 2, //대화 //TODO : NPC
    Open = 3, //열기, 닫기 //TODO : 문, 상자
    Break = 4, //부수기 //TODO : 자물쇠, 벽, 문
    Get = 5, //채집 //TODO : 채집할 수 있는 광석, 나무 등;
    UnLock = 6, //잠금해제 //TODO : 잠긴 문, 상자
    Swim = 7,
    Claim = 8,
}
