using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class NPCDeathState : NPCBaseState
{
    public NPCDeathState(NPCStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation(AnimationParameter.DeathParameter);
        stateMachine.npc.StartMyCoroutine(stateMachine.npc.DeadEnemy());
        //Debug.Log("죽었어요! Baam!");
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
