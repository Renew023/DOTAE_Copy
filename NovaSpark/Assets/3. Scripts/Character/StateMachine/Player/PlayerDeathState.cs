using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class PlayerDeathState : PlayerBaseState
{
    public PlayerDeathState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation(AnimationParameter.DeathParameter);
        stateMachine.player.VirtualCamera.m_Lens.OrthographicSize = stateMachine.player.VirtualCamera.m_Lens.OrthographicSize / 2;
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
