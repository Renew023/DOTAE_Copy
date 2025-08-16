using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBaseState : IState
{
	protected PlayerStateMachine stateMachine;
	public PlayerBaseState(PlayerStateMachine stateMachine)
	{
		this.stateMachine = stateMachine;
	}

	protected void StartAnimation(string animatorHash)
	{
		stateMachine.player.Animator.SetBool(animatorHash, true);
		stateMachine.player.hash = animatorHash;
	}

	protected void StopAnimation(string animatorHash)
	{
		stateMachine.player.Animator.SetBool(animatorHash, false);
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
		//if(stateMachine.player.isTalk)
		//{
		//	stateMachine.ChangeState(stateMachine.PlayerIdleState);
		//	return;
		//}

		if (stateMachine.player.isSkill)
		{
			stateMachine.ChangeState(stateMachine.PlayerSkillActionState);
			//TODO : 임시 키입력 차단
			return;
		}


		if(stateMachine.player.isSit) //앉아있다면
		{
			stateMachine.ChangeState(stateMachine.PlayerSitState);
			return;
        }
    }
}
