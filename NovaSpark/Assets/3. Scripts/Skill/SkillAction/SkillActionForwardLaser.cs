using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillActionForwardLaser : SkillActionBase
{
    private void OnEnable()
    {
        ForwardLaser();
    }
    private void Update()
    {
        //구동 방식    
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        direction = ((Vector2)skill.owner.transform.position - mousePos).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        skill.gameObject.transform.position = (Vector2)skill.owner.transform.position - (direction.normalized * distance / 2);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        if (skill.owner.gameObject.name == collision.name) return;
    }

    protected void ForwardLaser()
    {
        //초기 위치
        //direction = ((Vector2)skill.owner.transform.position - mouseDelta).normalized;
        //Vector2 targetPos = ((Vector2)skill.owner.transform.position - mouseDelta).normalized * distance + startPos;

        ////실제 움직임
        //skill.owner.transform.position = targetPos;
        gameObject.transform.localScale = new Vector2(distance - 1f, 1f); //1f는 플레이어 크기
        skill.gameObject.transform.position = (Vector2)skill.owner.transform.position - (direction.normalized * distance / 2);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        //삭제
        //coroutine = StartCoroutine(skill.StartDeleteDelay(time));
    }
}
