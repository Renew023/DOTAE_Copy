using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public void GetInteractObjectType(bool isCan); // 상호작용 가능한 오브젝트 중 종류.

    public string PromptText(); // 상호작용 시 표기되는 내용.

    //public void OnInteract(); // 상호작용 시 표기되는 내용.
    //                          // TODO : 상속 클래스로 각 타입에 따라 virtual이 있으면 편할듯.
    public void OnInteract(CharacterObject owner = null); //인터렉트한 대상이 누군지 알아야할때
    public bool IsCan();

}