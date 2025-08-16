using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyIdleState : EnemyBaseState
{
    private int _delayTime = 1;
    private Coroutine _changeWander;

    public EnemyIdleState(EnemyStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation(AnimationParameter.IdleParameter);
        //if (PhotonNetwork.IsMasterClient)
        //{
            _changeWander = stateMachine.enemy.StartMyCoroutine(ChangeWander()); // 대기 후 이동 상태로 전환
        //}
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(AnimationParameter.IdleParameter);
        // StopAnimationss
        //if (PhotonNetwork.IsMasterClient)
        //{
            stateMachine.enemy.StopCoroutine(_changeWander); //혹여나 중간에 update로 꺼질 경우, 확실히 종료
        //}
    }


    public override void Update()
    {
        if (stateMachine.enemy.EnemyRuntimeData.isFirst && stateMachine.enemy.TargetingEnemy(10f))//추적거리 또한 몬스터 스탯기반인데.. 10f -> ?
        {
            stateMachine.ChangeState(stateMachine.EnemyTargetState); // 타겟을 찾았을 때 추적 상태로 변경
            return;
        }

        if (stateMachine.enemy.isFollowState)
        {
            stateMachine.ChangeState(stateMachine.EnemyFollowState);
            return;
        }
            //base.Update();
        //몇 초 대기 후 이동하는 등에 설정 필요.
        // Handle idle-specific logic heres
    }

    IEnumerator ChangeWander()
    {
        yield return new WaitForSeconds(_delayTime);

        //if (PhotonNetwork.IsMasterClient)
        //{
            Vector2 point = stateMachine.enemy.GetWanderLocation();
            stateMachine.enemy.WanderPoint(point);
            //stateMachine.enemy.photonView.RPC(nameof(stateMachine.enemy.WanderPoint), RpcTarget.All, point);
        //}

        yield return null;
    }
}
