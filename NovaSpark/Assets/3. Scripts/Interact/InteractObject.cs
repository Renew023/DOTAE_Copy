using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractObject : MonoBehaviour, IInteractable, IDamageable
{
    //OnInteract 키가 눌렸을 때,
    [SerializeField] private int UniqueKey;
    private Collider2D _collider;
    
    [SerializeField] private int InteractKey;
    [SerializeField] private Sprite sprite;
    //[SerializeField] protected InteractObjectType _interactObjectType;
    [SerializeField] protected bool _isCan = false; //가능한가?
    [SerializeField] protected bool _isActive = true; //활성화 상태인가?

    private void Awake()
    {
        //gameObject.layer = LayerMask.NameToLayer("InteractiveObject");
        //if (!gameObject.TryGetComponent<BoxCollider2D>(out BoxCollider2D boxCollider2D)) 
        //{ 
        //    gameObject.AddComponent<BoxCollider2D>();
        //}
        _collider = GetComponent<Collider2D>();
    }

    public void Initialize(int key)
    {
        
    }

    //레이어를 비교한다. 대화 상대방은 플레이어의 간격 내에 하고 싶은 해당 오브젝트가 존재할 때,
    //public void Initialize(InteractObjectType interactObjectType, bool isCan)
    //{
    //    this._interactObjectType = interactObjectType;
    //    this._isCan = isCan;
    //}

    //public virtual void GetInteractObjectType(bool isCan)
    //{
    //    switch (_interactObjectType)
    //    {
    //        case InteractObjectType.Read:
    //            // NPC와 상호작용 로직
    //            if (!isCan)
    //            {
    //                Debug.Log("읽을 수 없다.");
    //                return;
    //            }

    //            if (_isActive)
    //            {
    //                Debug.Log("책 읽기");
    //            }
    //            else
    //            {
    //                Debug.Log("읽기 취소");
    //            }

    //            break;
    //        case InteractObjectType.Talk:
    //            // 아이템과 상호작용 로직
    //            if (!isCan)
    //            {
    //                Debug.Log("대화할 수 없다.");
    //                //TODO : 캐릭터와 대화할 때 특정 대사가 있다면 그걸 출력해도 좋음.
    //                return;
    //            }

    //            if (_isActive)
    //            {
    //                Debug.Log("대화하기");
    //            }
    //            else
    //            {
    //                Debug.Log("대화할 수 없다.");
    //            }

    //            break;
    //        case InteractObjectType.Open:
    //            // 환경 오브젝트와 상호작용 로직
    //            if (!isCan)
    //            {
    //                Debug.Log("열 수 없다.");
    //                //TODO : 캐릭터와 대화할 때 특정 대사가 있다면 그걸 출력해도 좋음.
    //                return;
    //            }

    //            if (_isActive)
    //            {
    //                Debug.Log("열기");
    //            }
    //            else
    //            {
    //                Debug.Log("닫기");
    //            }

    //            break;
    //        case InteractObjectType.Break:
    //            if(!isCan)
    //            {
    //                Debug.Log("부술 수 없다.");
    //                return;
    //            }

    //            if (_isActive)
    //            {
    //                Debug.Log("부수기");
    //            }
    //            else
    //            {
    //                return; //다시 부술 수 있는 상태가 되선 안됨;
    //            }
    //                break;
    //        case InteractObjectType.Get:
    //            if (!isCan)
    //            {
    //                Debug.Log("너무 무겁습니다.");
    //                return;
    //            }

    //            if (_isActive)
    //            {
    //                Debug.Log("가져오기");
    //                return; //가져오는 것은 언제나 참으로 유지해야되니까.
    //            }

    //            break;
    //        case InteractObjectType.UnLock:
    //            if (!isCan)
    //            {
    //                Debug.Log("풀 수 없습니다"); //TODO :
    //            }

    //            if (isCan)
    //            {
    //                Debug.Log("잠금해제");
    //                return; //잠금 해제하면 해제된 상자인거지.
    //            }

    //            break;
    //    }
    //    _isActive = !_isActive;
    //}

    public virtual void OnInteract(CharacterObject owner = null)
    {
        //TODO : 타입에 따라 바뀔듯? 하지만 다 나눈다고 한다면, 획득, 대화, 열기, 부수기 등등의 타입에 따라 상호작용이 다르게 될 것임.
        //TODO : 해당 코드는 상속하기 좋은 구조가 되기 힘듬. 
        //GetInteractObjectType(_isCan);
    }

    public virtual bool IsCan()
    {
        return _isCan;
    }

    public virtual string PromptText()
    {
        string text = null;
        return text;
    }

    public void TakeDamage(float amount, CharacterObject characterObject)
    {
        throw new NotImplementedException();
    }

    public void CallTakeDamage(float amount, CharacterObject characterObject)
    {
        
    }

    public float GetHp()
    {
        return 0;
    }

    public float GetHpPercent()
    {
        return 0;
        //throw new NotImplementedException();
    }

    public void GetInteractObjectType(bool isCan)
    {
        throw new NotImplementedException();
    }
}