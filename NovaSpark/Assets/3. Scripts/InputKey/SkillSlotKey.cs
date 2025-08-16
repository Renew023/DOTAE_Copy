using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
public class SkillSlotKey : MonoBehaviour
{
    public Player player;

    public bool isCoolTime1 = false;
    public bool isCoolTime2 = false;
    public bool isCoolTime3 = false;
    public bool isCoolTime4 = false;

    private void Awake()
    {
        player = GetComponentInParent<Player>(true);
    }

    public void OnSkillAction1(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started)
        {
            if (player.isTalk == true) return;
            //Debug.Log("스킬 1번이 눌림");
            if(!isCoolTime1 && !player.isSkill)
            {
                Vector2 mouseDelta = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                //Vector2 direction = mouseDelta - (Vector2)player.transform.position; 

                Collider2D catchObj = Physics2D.OverlapCircle(mouseDelta, 1f);
                Transform target = catchObj?.transform;
                //if (target == null) return;
                
                //Debug.Log("사용");
                isCoolTime1 = true;
                player.skills[0].Initialize(player, target, mouseDelta, player.skillNums[0]);
                player.skills[0].gameObject.SetActive(true);
                StartCoroutine(CooltimeDelay(() => isCoolTime1 = false, DataManager.Instance.SkillDataByID[player.skillNums[0]].cooltime));
                //QuickSlot 아이템의 쿨타임 동안 사용 불가.
                //장착한 해당 아이템 사용.
            }
            // Handle quick slot 1 input here
            //Debug.Log("Quick Slot 1 Activated");
        }
    }
    public void OnSkillAction2(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (player.isTalk == true) return;

            if (!isCoolTime2 && !player.isSkill)
            {
                //Debug.Log("사용");
                isCoolTime2 = true;
                Vector2 mouseDelta = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                //Vector2 direction = mouseDelta - (Vector2)player.transform.position;

                Collider2D catchObj = Physics2D.OverlapCircle(mouseDelta, 1f);
                Transform target = catchObj?.transform;

                player.skills[1].Initialize(player, target, mouseDelta, player.skillNums[1]);
                player.skills[1].gameObject.SetActive(true);
                StartCoroutine(CooltimeDelay(() => isCoolTime2 = false, DataManager.Instance.SkillDataByID[player.skillNums[1]].cooltime));
                //QuickSlot 아이템의 쿨타임 동안 사용 불가.
                //장착한 해당 아이템 사용.
            }

            //Debug.Log("Quick Slot 2 Activated");
        }
    }

    public void OnSkillAction3(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (player.isTalk == true) return;

            if (!isCoolTime3 && !player.isSkill)
            {
                //Debug.Log("사용");
                isCoolTime3 = true;

                Vector2 mouseDelta = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                //Vector2 direction = mouseDelta - (Vector2)player.transform.position;

                Collider2D catchObj = Physics2D.OverlapCircle(mouseDelta, 1f);
                Transform target = catchObj?.transform;

                player.skills[2].Initialize(player, target, mouseDelta, player.skillNums[2]);
                player.skills[2].gameObject.SetActive(true);
                StartCoroutine(CooltimeDelay(() => isCoolTime3 = false, DataManager.Instance.SkillDataByID[player.skillNums[2]].cooltime));
                //QuickSlot 아이템의 쿨타임 동안 사용 불가.
                //장착한 해당 아이템 사용.
            }
            //Debug.Log("Quick Slot 3 Activated");
        }
    }

    public void OnSkillAction4(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (player.isTalk == true) return;

            if (!isCoolTime4 && !player.isSkill)
            {
                //Debug.Log("사용");
                isCoolTime4 = true;

                Vector2 mouseDelta = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                //Vector2 direction = mouseDelta - (Vector2)player.transform.position;

                Collider2D catchObj = Physics2D.OverlapCircle(mouseDelta, 1f);
                Transform target = catchObj?.transform;

                player.skills[3].Initialize(player, target, mouseDelta, player.skillNums[3]);
                player.skills[3].gameObject.SetActive(true);
                StartCoroutine(CooltimeDelay(() => isCoolTime4 = false, DataManager.Instance.SkillDataByID[player.skillNums[3]].cooltime)); 
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
        player.isSkill = false;
        //들어온 bool 값에 따라
        //Debug.Log("쿨타임 종료");
        yield return null;
    }
}
