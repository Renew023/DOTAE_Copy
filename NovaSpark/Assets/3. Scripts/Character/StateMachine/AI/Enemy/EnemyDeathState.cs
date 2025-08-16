using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class EnemyDeathState : EnemyBaseState
{

    public EnemyDeathState(EnemyStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation(AnimationParameter.DeathParameter);
        stateMachine.enemy.StartMyCoroutine(stateMachine.enemy.DeadEnemy());
        //Debug.Log("에너미 죽었습니다");
        // StartAnimation
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(AnimationParameter.DeathParameter);
        // StopAnimation
    }

    public override void Update()
    {
        
    }
}
