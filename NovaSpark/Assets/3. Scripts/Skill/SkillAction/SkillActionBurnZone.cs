using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillActionBurnZone : SkillActionBase
{
    private void OnEnable()
    {
        BurnZone();
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
    }

    protected void BurnZone()
    {
        //초기 위치 : 현 위치 
        skill.transform.localPosition = mouseDelta;

        //작동 방식 :
        gameObject.transform.localScale = Vector2.one * distance;

        //삭제
        //coroutine = StartCoroutine(skill.StartDeleteDelay(time));
    }
}
