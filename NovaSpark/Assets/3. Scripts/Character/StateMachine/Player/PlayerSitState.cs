using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerSitState : PlayerBaseState
{
    public PlayerSitState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        // StartAnimation
    }

    public override void Exit()
    {
        base.Exit(); 
        stateMachine.player.isSit = false;
        // StopAnimation
    }

    public override void Update()
    {
        base.Update();

        if (stateMachine.GetState() != this) return;

        if (stateMachine.player.isRun && stateMachine.player.isMove)
        {
            stateMachine.ChangeState(stateMachine.PlayerRunState);
        }
        else if (stateMachine.player.isSit == false) //앉아있지 않다면
        {
            stateMachine.ChangeState(stateMachine.PlayerIdleState);
        }
    }
}
