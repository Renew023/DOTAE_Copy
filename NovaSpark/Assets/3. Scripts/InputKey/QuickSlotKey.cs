using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
public class QuickSlotKey : MonoBehaviour
{
    public bool isCoolTime1 = false;
    public bool isCoolTime2 = false;
    public bool isCoolTime3 = false;
    public bool isCoolTime4 = false;
    public void OnQuickSlot1(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started)
        {
            if(!isCoolTime1)
            {
                //Debug.Log("사용");
                isCoolTime1 = true;
                StartCoroutine(CooltimeDelay(() => isCoolTime1 = false, 5f));
                //QuickSlot 아이템의 쿨타임 동안 사용 불가.
                //장착한 해당 아이템 사용.
            }
            // Handle quick slot 1 input here
            //Debug.Log("Quick Slot 1 Activated");
        }
    }
    public void OnQuickSlot2(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (!isCoolTime2)
            {
                //Debug.Log("사용");
                isCoolTime2 = true;
                StartCoroutine(CooltimeDelay(() => isCoolTime2 = false, 5f));
                //QuickSlot 아이템의 쿨타임 동안 사용 불가.
                //장착한 해당 아이템 사용.
            }

            //Debug.Log("Quick Slot 2 Activated");
        }
    }

    public void OnQuickSlot3(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (!isCoolTime3)
            {
                //Debug.Log("사용");
                isCoolTime3 = true;
                StartCoroutine(CooltimeDelay(() => isCoolTime3 = false, 5f));
                //QuickSlot 아이템의 쿨타임 동안 사용 불가.
                //장착한 해당 아이템 사용.
            }
            //Debug.Log("Quick Slot 3 Activated");
        }
    }

    public void OnQuickSlot4(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (!isCoolTime4)
            {
                //Debug.Log("사용");
                isCoolTime4 = true;
                StartCoroutine(CooltimeDelay(() => isCoolTime4 = false, 5f)); 
                //QuickSlot 아이템의 쿨타임 동안 사용 불가.
                //장착한 해당 아이템 사용.
            }
            // Handle quick slot 1 input here
            //Debug.Log("Quick Slot 4 Activated");
        }
    }

    IEnumerator CooltimeDelay(Action coolTimeReset, float delay)
    {
        yield return new WaitForSeconds(delay);
        coolTimeReset?.Invoke();
        //들어온 bool 값에 따라
        //Debug.Log("쿨타임 종료");
        yield return null;
    }
}
