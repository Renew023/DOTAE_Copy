using System.Collections;
using System.Collections.Generic;
using Unity.XR.GoogleVr;
using UnityEngine;

public class NPCIdleState : NPCBaseState
{
    public NPCIdleState(NPCStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation(AnimationParameter.IdleParameter);
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(AnimationParameter.IdleParameter);
    }

    public override void Update()
    {
        base.Update();

        if (stateMachine.GetState() != this) return; // 현재 상태가 아닐 경우 업데이트를 중단


        //TODO : 특정 시간대, (낮이나 밤) 
        //if(stateMachine.npc.time > stateMachine.npc.workTime && stateMachine.npc.time < stateMachine.npc.restTime && (1.0f < Vector2.Distance(stateMachine.npc.WorkPoint, stateMachine.npc.transform.position))) // 9시에 출근. 지금 집 이라면.
        //{
        //    stateMachine.ChangeState(stateMachine.NPCGoWorkState);
        //}

        //if(stateMachine.npc.time > stateMachine.npc.restTime && (1.0f < Vector2.Distance(stateMachine.npc.HomePoint, stateMachine.npc.transform.position))) // 밤 9시에 퇴근.
        //{
        //    stateMachine.ChangeState(stateMachine.NPCGoHomeState);
        //}
    }
}
