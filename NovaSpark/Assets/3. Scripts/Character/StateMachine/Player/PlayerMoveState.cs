using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class PlayerMoveState : PlayerBaseState
{
    private ParticleInstance _particle;
    private float _footstepTimer = 0f;
    private float _footstepInterval = 0.5f;
    public PlayerMoveState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        _particle = ParticleManager.Instance.CreateEffect(ParticleParameterHash.PlayerWalk_Particle, stateMachine.player.gameObject);
        StartAnimation(AnimationParameter.MoveParameter);
        stateMachine.player.Move();
        //_footstepTimer = 0f;
    }

    public override void Exit()
    {
        base.Exit();
        ParticleManager.Instance.RemoveEffect(ParticleParameterHash.PlayerWalk_Particle, _particle);
        StopAnimation(AnimationParameter.MoveParameter);
        stateMachine.player.MoveStop();
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
        else if (stateMachine.player.isRun == true)
        {
            stateMachine.ChangeState(stateMachine.PlayerRunState);
            return;
        }
        else
        {
            stateMachine.player.Move();
        }

        _footstepTimer += Time.fixedDeltaTime;
        if (_footstepTimer >= _footstepInterval)
        {
            _footstepTimer = 0f;
            SoundManager.Instance.PlaySFXAsync(SoundParameterData.PlayerFootstep_1);
            SoundManager.Instance.PlaySFXAsync(SoundParameterData.PlayerFootstep_2);
        }
    }
}
