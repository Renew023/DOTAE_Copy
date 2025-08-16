using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCGoWorkState : NPCBaseState
{
    public NPCGoWorkState(NPCStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        //Move(stateMachine.npc.WorkPoint); // NPC의 일터로 이동.
    }

    public override void Exit()
    {
        base.Exit();
        stateMachine.npc.Agent.ResetPath();
    }

    public override void Update()
    {
        base.Update();
        if (stateMachine.npc.Agent.remainingDistance <= stateMachine.npc.Agent.stoppingDistance)
        {
            // 일터에 도착했을 때, Idle로 변경.
            stateMachine.ChangeState(stateMachine.NPCIdleState);
        }
    }
}
