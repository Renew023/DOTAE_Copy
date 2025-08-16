using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stateMachine.player.RigidBody.velocity = Vector2.zero;
        StartAnimation(AnimationParameter.IdleParameter);
    }

    public override void Exit()
    {
        base.Exit();
        StopAnimation(AnimationParameter.IdleParameter);
        // StopAnimation
    }

    public override void Update()
    {
        base.Update();

        if (stateMachine.GetState() != this) return;
    }
}
