using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class NPCAttackState : NPCBaseState
{
    private bool isAttack = false;
    public NPCAttackState(NPCStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stateMachine.npc.WeaponEnable();
        isAttack = false;
        //stateMachine.npc.Attack();
        StartAnimation(AnimationParameter.attackParameter);
        //stateMachine.npc.Weapon.animator.SetBool(AnimationParameter.attackParameter, true);
        // StartAnimation
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(AnimationParameter.attackParameter);
        //stateMachine.npc.Weapon.animator.SetBool(AnimationParameter.attackParameter, false);
        isAttack = false;
        stateMachine.npc.Agent.ResetPath();
        stateMachine.npc.AttackEnd();
        // StopAnimation
    }

    public override void Update()
    {
        //base.Update();

        if(stateMachine.GetState() != this) return; // 현재 상태가 아닐 경우 업데이트를 중단

        // 현재 재생 중인 상태 정보 가져오기 (레이어 0 기준)
        AnimatorStateInfo stateInfo = stateMachine.npc.Animator.GetCurrentAnimatorStateInfo(0);

        // 재생 중인 애니메이션의 normalizedTime (0 ~ 1 이상, 반복재생 시 1 이상 가능)
        float normalizedTime = stateInfo.normalizedTime;

        if (stateMachine.npc.attackType == AttackType.Magic)
        {
            if (normalizedTime < 0.3f && normalizedTime > 0.2f)
            {
                if (isAttack) return;
                stateMachine.npc.AttackBall();
                isAttack = true;
            }
        }

        if (normalizedTime >= 1f)
        {
            // 애니메이션이 100% 재생됨
            stateMachine.ChangeState(stateMachine.NPCIdleState);
            isAttack = false;
            return;
        }
        //if (!stateMachine.npc.isAttack)
        //{
        //    stateMachine.npc.StartMyCoroutine(stateMachine.npc.Attack());
        //}
        //Move(stateMachine.npc.Target.transform.position); // 움직이면서 공격하는지에 대한 여부는 나중에 리팩토링

        //if (!stateMachine.npc.AttackDistance())
        //{
        //    stateMachine.ChangeState(stateMachine.NPCTargetState); // 공격 거리에 도달하면 공격 상태로 전환
        //}
    }
}
