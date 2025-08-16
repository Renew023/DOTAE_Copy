using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NPCTargetState : NPCBaseState
{
    public NPCTargetState(NPCStateMachine stateMachine) : base(stateMachine)
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
        //if(stateMachine.npc.photonView.IsMine)
        StopAnimation(AnimationParameter.MoveParameter);
        stateMachine.npc.Agent.ResetPath();
        stateMachine.npc.Agent.velocity = Vector3.zero;
        stateMachine.npc.Agent.isStopped = true;
        // StopAnimation
    }

    public override void Update()
    {
        base.Update();

        if (stateMachine.GetState() != this) return; // 현재 상태가 아닐 경우 업데이트를 중단

        if (!stateMachine.npc.TargetingEnemy(10f)) //거리가 되지 않는다면
        {
            stateMachine.ChangeState(stateMachine.NPCIdleState); // 이동이 완료되면 대기 상태로 전환
            return;
        }

        Vector2 pos = stateMachine.npc.Target.transform.position;
        stateMachine.npc.Move(pos);


        //if (PhotonNetwork.IsMasterClient)
        //{
        //    Vector2 pos = stateMachine.npc.Target.transform.position;
        //    stateMachine.npc.photonView.RPC(nameof(stateMachine.npc.Move), RpcTarget.All, pos); // 타겟 위치로 이동
        //}

    }
}
