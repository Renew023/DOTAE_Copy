using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillActionTargetAttack: SkillActionBase
{
    private float targetingSpeed = 5f;

    private void OnEnable()
    {
        TargetAttack();
        if (skill.target == null) skill.NextSkill();
    }

    private void Update()
    {
        //구동 방식    
        skill.skillRB.velocity = (skill.target.position - skill.transform.position).normalized * targetingSpeed;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        if (skill.owner.gameObject.name == collision.name) return;

        if (collision.TryGetComponent<CharacterObject>(out CharacterObject _target))
        {
            Debug.Log("맞았어요?");
            //skill.NextSkill();
            gameObject.SetActive(false);
        }
    }

    protected void TargetAttack()
    {
        //초기 위치
        skill.gameObject.transform.position = mouseDelta + (Vector2)skill.gameObject.transform.position;

        //삭제
        //coroutine = StartCoroutine(skill.StartDeleteDelay(time));
    }
}
