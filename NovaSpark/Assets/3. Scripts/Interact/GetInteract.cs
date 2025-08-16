using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetInteract : InteractObject, IInteractable //Read
{

    //private void Awake()
    //{
    //    _interactObjectType = InteractObjectType.Get;
    //}

    //public override void GetInteractObjectType(bool isCan)
    //{
    //    base.GetInteractObjectType(isCan);
    //    //interactObject.GetInteractObjectType(isCan);
    //}
    public override string PromptText()
    {
        return base.PromptText();
        //return interactObject.PromptText();
    }
    public override void OnInteract(CharacterObject owner = null)
    {
        //Panel 열기?
        base.OnInteract(owner);
        //interactObject.OnInteract();
        StartCoroutine(FarmingIng(2f));
    }
    public override bool IsCan()
    {
        return base.IsCan();
        //return interactObject.IsCan();
    }

    private IEnumerator FarmingIng(float time)
    {
        SoundManager.Instance.PlaySFXAsync(SoundParameterData.FarmingIng_SFXParameterHash);

        yield return new WaitForSeconds(time);

        SoundManager.Instance.PlaySFXAsync(SoundParameterData.FarmingSuccess_SFXParameterHash);
    }
}