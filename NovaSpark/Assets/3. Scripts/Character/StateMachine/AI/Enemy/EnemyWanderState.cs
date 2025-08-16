using Photon.Pun;
using Photon.Pun.Demo.Cockpit;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class EnemyWanderState : EnemyBaseState
{
    public EnemyWanderState(EnemyStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation(AnimationParameter.MoveParameter);
        // 몬스터가 이동할 랜덤 위치를 설정
        // StartAnimation
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

        if (stateMachine.GetState() != this) return; // 현재 상태가 아닐 경우 업데이트를 중단

        //Debug.Log(stateMachine.enemy.Agent.remainingDistance + " / " + stateMachine.enemy.Agent.stoppingDistance);
        if (stateMachine.enemy.Agent.remainingDistance <= stateMachine.enemy.Agent.stoppingDistance)
        {
            stateMachine.ChangeState(stateMachine.EnemyIdleState); // 이동이 완료되면 대기 상태로 전환
            return;
        }

        //Debug.Log(stateMachine.enemy.isFirstAttacker + " / " + stateMachine.enemy.TargetingEnemy(10f));
        if (stateMachine.enemy.EnemyRuntimeData.isFirst && stateMachine.enemy.TargetingEnemy(10f))//추적거리 또한 몬스터 스탯기반인데.. 10f -> ?
        {
            stateMachine.ChangeState(stateMachine.EnemyTargetState); // 타겟을 찾았을 때 추적 상태로 변경
            return;
        }
    }
}
