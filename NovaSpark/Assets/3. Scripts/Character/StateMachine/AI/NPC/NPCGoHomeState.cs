using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCGoHomeState : NPCBaseState
{
    public NPCGoHomeState(NPCStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        //Move(stateMachine.npc.HomePoint); // NPC의 집 위치로 이동
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        if(stateMachine.npc.Agent.remainingDistance <= stateMachine.npc.Agent.stoppingDistance)
        {
            // 집에 도착했을 때, Idle 상태로 전환
            stateMachine.ChangeState(stateMachine.NPCIdleState);
        }
    }
}
