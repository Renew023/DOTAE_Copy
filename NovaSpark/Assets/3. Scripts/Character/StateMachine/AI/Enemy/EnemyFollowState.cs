using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyFollowState : EnemyBaseState //습격에 쓰일 State
{
    public EnemyFollowState(EnemyStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stateMachine.enemy.isFollowState = true; //적이 추적 상태임을 알림
        stateMachine.enemy.SetTarget(stateMachine.enemy.MainTarget);
        StartAnimation(AnimationParameter.MoveParameter);
        // StartAnimation
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(AnimationParameter.MoveParameter);
        // StopAnimation
    }

    public override void Update()
    {
        base.Update();

        if (stateMachine.GetState() != this) return;

        //if (PhotonNetwork.IsMasterClient)
        //{
        Vector2 pos = stateMachine.enemy.Target.transform.position;
        stateMachine.enemy.Move(pos);    
        //stateMachine.enemy.photonView.RPC(nameof(stateMachine.enemy.Move), RpcTarget.All, pos); // 타겟 위치로 이동
        //}
    }
}
