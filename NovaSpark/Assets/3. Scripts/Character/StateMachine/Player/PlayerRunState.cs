using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerRunState : PlayerBaseState
{
    private ParticleInstance _particle;
    public PlayerRunState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        _particle = ParticleManager.Instance.CreateEffect(ParticleParameterHash.PlayerWalk_Particle, stateMachine.player.gameObject);
        StartAnimation(AnimationParameter.MoveParameter);
        stateMachine.player.Move();
        // StartAnimation
    }

    public override void Exit()
    {
        base.Exit();
        ParticleManager.Instance.RemoveEffect(ParticleParameterHash.PlayerWalk_Particle, _particle);
        StopAnimation(AnimationParameter.MoveParameter);
        stateMachine.player.MoveStop();
        // StopAnimation
    }

    public override void Update()
    {
        base.Update();

        if (stateMachine.GetState() != this) return;

        if (stateMachine.player.isMove == false)
        {
            stateMachine.ChangeState(stateMachine.PlayerIdleState);
            return;
        }
        else if (stateMachine.player.isRun == false)
        {
            stateMachine.ChangeState(stateMachine.PlayerMoveState);
            return;
        }
        else
        {
            stateMachine.player.Move();
        }
    }
}
