using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillActionThrowAttack : SkillActionBase
{
    private void OnEnable()
    {
        ThrowSkill();
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
    }

    protected void ThrowSkill() // 1 : DashAttack 
    {
        
        //테스트 코드
        Debug.Log($"방향 {direction} + 거리 {distance} + 시간 {1f / time} + 총 이동하는 힘 {direction * distance * (1f / time)}");
        
        //초기 위치
        skill.gameObject.transform.position = direction + (Vector2)skill.gameObject.transform.position;

        //움직임
        skill.skillRB.velocity = direction * distance * (1f / time);

        //삭제
        //coroutine = StartCoroutine(skill.StartDeleteDelay(time));
        //yield return new WaitForSeconds(_time);
        //_ownerRB.velocity = Vector2.zero;
    }
}
