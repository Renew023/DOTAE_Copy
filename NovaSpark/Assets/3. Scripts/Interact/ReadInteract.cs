using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadInteract : MonoBehaviour, IInteractable //Read
{
    [SerializeField] private InteractObject interactObject;
    [SerializeField] private InteractObjectType interactObjectType;
    [SerializeField] private bool isCan;

    public void GetInteractObjectType(bool isCan)
    {
        interactObject.GetInteractObjectType(isCan);
    }
    public string PromptText()
    {
        return interactObject.PromptText();
    }
    public void OnInteract(CharacterObject owner = null)
    {
        //Panel 열기?
        Debug.Log("ReadInteract가 활성화되었습니다");
        interactObject.OnInteract();
    }
    public bool IsCan()
    {
        return interactObject.IsCan();
    }

    private void Awake()
    {
        interactObject = new InteractObject();
        if (interactObject == null)
        {
            Debug.LogError("NPC에 InteractObject 컴포넌트가 없습니다.");
        }
        else
        {
            //interactObject.Initialize(interactObjectType, isCan); 
        }
    }
}