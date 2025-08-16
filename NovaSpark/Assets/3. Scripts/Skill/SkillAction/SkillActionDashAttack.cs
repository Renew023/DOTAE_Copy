using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class SkillActionDashAttack : SkillActionBase
{
    //이동 어택
    private void OnEnable()
    {
        CharacterDash(); //이동
    }

    private void Update()
    {
        gameObject.transform.position = skill.owner.transform.position;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (skill.skillRB != null)
        {
            skill.skillRB.velocity = Vector2.zero;
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision); //어택
    }

    protected void CharacterDash() // 1 : DashAttack 
    {
        //Vector2 direction = target.transform.position - owner.transform.position;
        Debug.Log($"방향 {direction.normalized} + 거리 {distance} + 시간 {1f / time} + 총 이동하는 힘 {direction.normalized * distance * (1f / time)}");
        skill.ownerRB.velocity = direction.normalized * distance * (1f / time);
        
        //StartCoroutine(skill.StartDeleteDelay(time));
        //yield return new WaitForSeconds(_time);
        //_ownerRB.velocity = Vector2.zero;
    }
}
