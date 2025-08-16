using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyTargetState : EnemyBaseState
{
    public EnemyTargetState(EnemyStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation(AnimationParameter.MoveParameter);
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(AnimationParameter.MoveParameter);
        stateMachine.enemy.Agent.ResetPath();
        // StopAnimation
    }

    public override void Update()
    {
        base.Update();

        if (stateMachine.GetState() != this) return;


        if (!stateMachine.enemy.TargetingEnemy(10f)) //거리가 되지 않는다면
        {
            if(stateMachine.enemy.isFollowState)
            {
                stateMachine.ChangeState(stateMachine.EnemyFollowState);
            }

            stateMachine.ChangeState(stateMachine.EnemyIdleState); // 이동이 완료되면 대기 상태로 전환
            return;
        }
        
        if (stateMachine.enemy.AttackDistance())
        {
            stateMachine.ChangeState(stateMachine.EnemyAttackState); // 공격 거리에 도달하면 공격 상태로 전환
            return;
        }

        //if (PhotonNetwork.IsMasterClient)
        //{
            Vector2 pos = stateMachine.enemy.Target.transform.position;
            stateMachine.enemy.Move(pos);
        //stateMachine.enemy.photonView.RPC(nameof(stateMachine.enemy.Move),RpcTarget.All, pos); // 타겟 위치로 이동
        //}
    }
}
