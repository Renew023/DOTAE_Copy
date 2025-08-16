using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyStateMachine : StateMachine //ITalk, IInteractable
{
	public bool isAttack = false; // 선공 몬스터인지 아닌지.
	//몬스터의 패턴:
	// 가만히 있기 : Idle
	// 돌아다니기 : Wander(Move)
	// 집단 따라가기 : Following
	// 추적하기 : Target
	// 공격하기 : Attack
	// 죽었을 때 : Death 조작 불가
	
	public Enemy enemy;
	public EnemyIdleState EnemyIdleState { get; private set; }
	public EnemyWanderState EnemyWanderState { get; private set; }
    public EnemyFollowState EnemyFollowState { get; private set; }
    public EnemyTargetState EnemyTargetState { get; private set; }
    public EnemyAttackState EnemyAttackState { get; private set; }
	public EnemyDeathState EnemyDeathState { get; private set; } // 죽는 애니메이션이 필요할 때 사용
                                                                 //스테이트의 종류를 보유
                                                                 //public PlayerIdleState IdleState; //가만히 있는 모션이 필요가 없음.
    public override void ChangeState(IState state)
    {
        if (stateValue == state) return;
        if (enemy.isStun && enemy.characterRuntimeData.health.Current > 0) return; //기절 되어있을 떄는 움직일 수 없음.
		base.ChangeState(state);
        //Debug.Log($"EnemyStateMachine: ChangeState to {state.GetType().Name}");
        //enemy.RPCChangeState(state);
    }


    public EnemyStateMachine(Enemy enemy)
    {
        this.enemy = enemy;
		EnemyIdleState = new EnemyIdleState(this);
		EnemyWanderState = new EnemyWanderState(this);
		EnemyFollowState = new EnemyFollowState(this); 
		EnemyTargetState = new EnemyTargetState(this);
		EnemyAttackState = new EnemyAttackState(this);
		EnemyDeathState = new EnemyDeathState(this);

        //IdleState = new PlayerIdleState(this);
    }
}
