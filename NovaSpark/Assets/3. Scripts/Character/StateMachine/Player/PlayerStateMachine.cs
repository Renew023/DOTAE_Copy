using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerStateMachine : StateMachine
{
    //플레이어의 패턴:
    // 걷기 : Move
    // 달리기 : Run
    // 공격하기 : Attack
    // 가만히 있기 : Idle
    public Player player;
    public PlayerIdleState PlayerIdleState { get; private set; }
	public PlayerMoveState PlayerMoveState { get; private set; }
    public PlayerRunState PlayerRunState { get; private set; }
    public PlayerAttackState PlayerAttackState { get; private set; }
    public PlayerSitState PlayerSitState { get; private set; }

    public PlayerDeathState PlayerDeathState { get; private set; }

    public PlayerSkillActionState PlayerSkillActionState { get; private set; }
    //스테이트의 종류를 보유
    //public PlayerIdleState IdleState; //가만히 있는 모션이 필요가 없음.

    public override void ChangeState(IState state)
    {
        if (stateValue == state) return;
        if (player.isParring) return; //기절 되어있을 떄는 움직일 수 없음.
        base.ChangeState(state);
        //Debug.Log(GetState().GetType().Name + " 상태로 변경됨");
        //player.RPCChangeState(state);
    }

    public PlayerStateMachine(Player player)
    {
        this.player = player;
		PlayerIdleState = new PlayerIdleState(this);
		PlayerMoveState = new PlayerMoveState(this);
		PlayerRunState = new PlayerRunState(this);
		PlayerAttackState = new PlayerAttackState(this);
        PlayerSitState = new PlayerSitState(this);
        PlayerSkillActionState = new PlayerSkillActionState(this);
        PlayerDeathState = new PlayerDeathState(this);
        //IdleState = new PlayerIdleState(this);
    }
}
