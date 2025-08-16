using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerSkillActionState : PlayerBaseState
{
    public PlayerSkillActionState(PlayerStateMachine stateMachine) : base(stateMachine)
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
        // StopAnimation
    }

    public override void Update()
    {
        if (!stateMachine.player.isSkill)
        {
            stateMachine.ChangeState(stateMachine.PlayerIdleState);
        }
    }
}
