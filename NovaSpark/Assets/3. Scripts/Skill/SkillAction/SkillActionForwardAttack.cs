using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillActionForwardAttack : SkillActionBase
{
    private float length = 1f;
    private void OnEnable()
    {
        length = 1f;
        ForwardAttack();
    }

    private void Update()
    {
        skill.gameObject.transform.localScale = new Vector2(length, 1f);
        //skill.gameObject.transform.position = (Vector2)skill.owner.transform.position - (direction.normalized * length / 2);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
    }

    protected void ForwardAttack() // 1 : DashAttack 
    {

        //테스트 코드
        Debug.Log($"방향 {direction} + 거리 {distance} + 시간 {1f / time} + 총 이동하는 힘 {direction * distance * (1f / time)}");

        //초기 위치
        skill.gameObject.transform.position = (Vector2)skill.owner.transform.position - direction.normalized;

        //움직임
        skill.gameObject.transform.localScale = new Vector2(length, 1f);

        //삭제
        StartCoroutine(UpdateLength());
        //coroutine = StartCoroutine(skill.StartDeleteDelay(time));
        //yield return new WaitForSeconds(_time);
        //_ownerRB.velocity = Vector2.zero;
    }

    protected IEnumerator UpdateLength()
    {
        float timer = 0f;
        float shortTimer = timer / 5f;
        while (shortTimer > timer)
        {
            yield return new WaitForSeconds(0.1f);
            length = 1 + (distance * timer / shortTimer);
            timer += Time.deltaTime;
        }
        length = distance;
    }
}
