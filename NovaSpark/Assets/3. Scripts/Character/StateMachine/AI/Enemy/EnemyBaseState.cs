using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBaseState : IState
{
	protected EnemyStateMachine stateMachine;
	public EnemyBaseState(EnemyStateMachine stateMachine)
	{
		this.stateMachine = stateMachine;
	}

	protected void StartAnimation(string animatorHash)
	{
		stateMachine.enemy.Animator.SetBool(animatorHash, true);
	}

	protected void StopAnimation(string animatorHash)
	{
		stateMachine.enemy.Animator.SetBool(animatorHash, false);
	}

	protected float GetNormalizedTime(Animator animator, string tag)
	{
		AnimatorStateInfo currentInfo = animator.GetCurrentAnimatorStateInfo(0);
		//AnimatorStateInfo nextInfo = animator.GetNextAnimatorStateInfo(0);
		//Debug.Log("버그 타임");

		//전환되고 있지 않을 때 현재 애니메이션이 tag라면
		if (!animator.IsInTransition(0) && currentInfo.IsTag(tag))
		{
			return currentInfo.normalizedTime;
		}
		//전환되고 있을 때 && 다음 애니메이션이 tag
		//else if (animator.IsInTransition(0)  && nextInfo.IsTag(tag))
		//{
		//	return nextInfo.normalizedTime;
		//}
		else
		{
			return 0f;
		}
	}


	public virtual void Enter()
	{

	}

	public virtual void Exit()
	{
		
	}


	public virtual void Update()
	{
		if(stateMachine.enemy.AttackDistance())
		{
			stateMachine.ChangeState(stateMachine.EnemyAttackState);
			return;
        }

		if (stateMachine.enemy.TargetingEnemy(10f))//추적거리 또한 몬스터 스탯기반인데.. 10f -> ?
        {
			stateMachine.ChangeState(stateMachine.EnemyTargetState); // 타겟을 찾았을 때 추적 상태로 변경
			return;
		}
    }
}
