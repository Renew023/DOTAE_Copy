using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class SkillActionTeleportSlash : SkillActionBase
{
    private void OnEnable()
    {
        CharacterTeleport();
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
    }
    protected void CharacterTeleport() //2 : TeleportAttack
    {
        //초기 위치
        Vector2 startPos = skill.owner.transform.position;
        Debug.Log("제발" + startPos);
        //Vector2 direction = (mouseDelta - (Vector2)skill.owner.transform.position).normalized;
        Vector2 targetPos = startPos + direction * distance;
        Debug.Log("타겟" + targetPos );

        //실제 움직임
        skill.ownerRB.MovePosition(targetPos);
        gameObject.transform.localScale = new Vector2(distance-1f, 1f); //1f는 플레이어 크기
        gameObject.transform.position = (startPos + targetPos)/ 2;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        //삭제
        //coroutine = StartCoroutine(skill.StartDeleteDelay(time));
    }
}
