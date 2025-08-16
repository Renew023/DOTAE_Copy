using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillActionOverlapDamage : SkillActionBase
{
    private void OnEnable()
    {
        OverlapDamage();
        
    }

    private void Update()
    {
        skill.transform.localPosition = skill.owner.transform.localPosition;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
    }

    protected void OverlapDamage()
    {
        //초기 위치 : 현 위치 

        //작동 방식 :
        gameObject.transform.localScale = Vector2.one * distance;

        //삭제
        //coroutine = StartCoroutine(skill.StartDeleteDelay(time));
    }
}
