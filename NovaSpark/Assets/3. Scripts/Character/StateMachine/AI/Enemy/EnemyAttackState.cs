using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class EnemyAttackState : EnemyBaseState
{
    private bool isAttack = false;
    public EnemyAttackState(EnemyStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stateMachine.enemy.WeaponEnable();
        isAttack = false;
        //stateMachine.enemy.Attack();
        StartAnimation(AnimationParameter.attackParameter);
        //stateMachine.enemy.Weapon.animator.SetBool(AnimationParameter.attackParameter, true);
        // StartAnimation
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(AnimationParameter.attackParameter);
        //stateMachine.enemy.Weapon.animator.SetBool(AnimationParameter.attackParameter, false);
        isAttack = false;
        stateMachine.enemy.Agent.ResetPath();
        stateMachine.enemy.AttackEnd();

        // StopAnimation
    }

    public override void Update()
    {
        //base.Update();

        if (stateMachine.GetState() != this) return; // 현재 상태가 아닐 경우 업데이트를 중단

        AnimatorStateInfo stateInfo = stateMachine.enemy.Animator.GetCurrentAnimatorStateInfo(0);

        // 재생 중인 애니메이션의 normalizedTime (0 ~ 1 이상, 반복재생 시 1 이상 가능)
        float normalizedTime = stateInfo.normalizedTime;

        if(stateMachine.enemy.attackType == AttackType.Magic)
        {
            if (normalizedTime < 0.3f && normalizedTime > 0.2f)
            {
                if (isAttack) return;
                stateMachine.enemy.AttackBall();
                isAttack = true;
            }
        }


        if (normalizedTime >= 1f)
        {
            // 애니메이션이 100% 재생됨
            isAttack = false;
            //if (stateMachine.enemy.isFollowState)
            //{
            //    stateMachine.ChangeState(stateMachine.EnemyFollowState); // 추적 상태로 전환
            //    return;
            //}
            stateMachine.ChangeState(stateMachine.EnemyIdleState);
            return;
        }
    }
}
