using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

[Serializable]
public class NPCStateMachine : StateMachine //ITalk, IInteractable
{
	public bool isAttack = false; // 선공 몬스터인지 아닌지.
	//NPC의 패턴:
	// 가만히 있기 : Idle
	// 집에 가기 : GoHome
	// 출근하기 : GoWork
	// 추적하기 : Target
	// 공격하기 : Attack
	
	public NPC npc;
	public NPCIdleState NPCIdleState { get; private set; }
	public NPCGoHomeState NPCGoHomeState { get; private set; }
	public NPCGoWorkState NPCGoWorkState { get; private set; }
	public NPCTargetState NPCTargetState { get; private set; }
	public NPCAttackState NPCAttackState { get; private set; }

	public NPCDeathState NPCDeathState { get; private set; }

    public override void ChangeState(IState state)
    {
        if (stateValue == state) return;
        if (npc.isStun && npc.characterRuntimeData.health.Current > 0) return; //기절 되어있을 떄는 움직일 수 없음.
        base.ChangeState(state);
        //Debug.Log($"NPCStateMachine: ChangeState to {state.GetType().Name}");
        //npc.RPCChangeState(state);
    }

    public NPCStateMachine(NPC npc)
    {
		this.npc = npc;
		this.NPCIdleState = new NPCIdleState(this);
		this.NPCGoHomeState = new NPCGoHomeState(this);
		this.NPCGoWorkState = new NPCGoWorkState(this);
		this.NPCTargetState = new NPCTargetState(this);
		this.NPCAttackState = new NPCAttackState(this);
		this.NPCDeathState = new NPCDeathState(this);
    }
    //스테이트의 종류를 보유
    //public PlayerIdleState IdleState; //가만히 있는 모션이 필요가 없음.

}
