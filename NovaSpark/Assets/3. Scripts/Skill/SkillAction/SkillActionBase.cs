using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class SkillActionBase : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particle;
    [SerializeField] protected Skill skill;
    [SerializeField] protected SkillPool skillPool;
    //[SerializeField] protected Transform target;

    [SerializeField] protected float damage;
    [SerializeField] protected float distance;
    [SerializeField] protected float time;
    [SerializeField] protected Vector2 mouseDelta;
    [SerializeField] protected Vector2 direction;
    [SerializeField] protected Coroutine coroutine;

    private HashSet<IDamageable> _attackedTarget = new();

    public void Initialize(Vector2 mouseDelta, float damage, float distance, float time)
    {
        skillPool = GetComponentInParent<SkillPool>(true);
        skill = skillPool.skill;

        this.mouseDelta = mouseDelta;
        this.damage = damage;
        this.distance = distance;
        this.time = time;
        this.direction = (mouseDelta - (Vector2)skill.owner.transform.position).normalized;
    }

    protected virtual void OnDisable()
    {
        if(coroutine != null)
        StopCoroutine(coroutine);
        _attackedTarget.Clear();
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<IDamageable>(out IDamageable obj))
        {
            if (skill.owner.gameObject == collision.gameObject) return; //스스로 맞은 공격은 돌림.

            if(!_attackedTarget.Contains(obj))
            {
                _attackedTarget.Add(obj);
                obj.TakeDamage(damage, skill.owner);
                StartCoroutine(ResetAttackedTarget(obj));
            }
            //부딫혀도 삭제해서는 아니됨. 그러나 데미지가 딱 한 번만 들어가야함.
        }
    }

    protected IEnumerator ResetAttackedTarget(IDamageable obj)
    {
        yield return new WaitForSeconds(0.2f);
        _attackedTarget.Remove(obj);
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<IDamageable>(out IDamageable obj))
        {
            if (skill.owner.gameObject == collision.gameObject) return; //스스로 맞은 공격은 돌림.

            if (_attackedTarget.Contains(obj))
            {
                _attackedTarget.Remove(obj);
            }
            //부딫혀도 삭제해서는 아니됨. 그러나 데미지가 딱 한 번만 들어가야함.
        }
    }

    //ForwardSpin 은 스킵

    protected IEnumerator ThrowMove() //5 : ThrowSkill
    {
        yield return new WaitForSeconds(time);
        //skillRB.velocity = direction.normalized * distance * (1f / time);
        StartCoroutine(skill.StartDeleteDelay(time));
    }

    
    
    //protected IEnumerator BurnZone() //4 BurnZone + player좌표
    //{
    //    //float curTime = 0f;
    //    StartCoroutine(skill.StartDeleteDelay(time));
    //    while (true)
    //    {
    //        yield return new WaitForSeconds(0.1f);
    //        foreach (var person in _attackedTarget)
    //        {
    //            person.TakeDamage(damage, skill.owner);
    //        }
    //    }
    //}
    
    //protected IEnumerator OverlapDamage() // 6: overlapDamage
    //{
    //    StartCoroutine(skill.StartDeleteDelay(time));
    //    yield return new WaitForSeconds(time);
    //}

    protected IEnumerator OverlapTargetAttack() // 6: overlapDamage
    {
        StartCoroutine(skill.StartDeleteDelay(time));
        //Phsyics2d.OverlapCircle
        
        yield return new WaitForSeconds(time);
    }

}
