using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;
using Photon.Pun;

[Serializable]
public class PlayerAttackState : PlayerBaseState
{
    private bool isAttack = false;

    public PlayerAttackState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        StartAnimation(AnimationParameter.attackParameter);
        stateMachine.player.WeaponEnable();
        isAttack = false;
        //stateMachine.player.Weapon.animator.SetBool(AnimationParameter.attackParameter, true);
        SoundManager.Instance.PlaySFXAsync(SoundParameterData.PlayerAttack_SFXParameterHash);
    }
    

    public override void Exit()
    {
        base.Exit();
        StopAnimation(AnimationParameter.attackParameter);
        //stateMachine.player.Weapon.animator.SetBool(AnimationParameter.attackParameter, false);
        isAttack = false;
        stateMachine.player.AttackEnd();
        //stateMachine.player.photonView.RPC(nameof(stateMachine.player.AttackEnd), RpcTarget.All);
        // StopAnimation
    }

    public override void Update()
    {
        base.Update();

        if (stateMachine.GetState() != this) return;

        // 현재 재생 중인 상태 정보 가져오기 (레이어 0 기준)
        AnimatorStateInfo stateInfo = stateMachine.player.Animator.GetCurrentAnimatorStateInfo(0);

        // 재생 중인 애니메이션의 normalizedTime (0 ~ 1 이상, 반복재생 시 1 이상 가능)
        float normalizedTime = stateInfo.normalizedTime;

        //if (normalizedTime < 0.3f && normalizedTime > 0.2f)
        //{
        //    if (isAttack) return;
        //    stateMachine.player.WeaponEnable();
        //    isAttack = true;
        //}

        if (normalizedTime >= 1f)
        {
            // 애니메이션이 100% 재생됨
            stateMachine.ChangeState(stateMachine.PlayerIdleState);
            isAttack = false;
            return;
        }



        //if (stateMachine.player.isAttack)
        //{
        //    // 애니메이션이 100% 재생됨
        //    stateMachine.ChangeState(stateMachine.PlayerIdleState);
        //    return;
        //}
    }
}
