using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
           
public class PlayerKey : MonoBehaviour
{
    public Player player;
    private void Awake()
    {
        player = GetComponentInParent<Player>();
    }

    private void Start()
    {
        //photonView = GetComponentInParent<PhotonView>();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            player.Interact.Targeting();
            //player.photonView.RPC(nameof(player.Interact.Targeting), RpcTarget.All);
        }
    
    }

    public void OnSit(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            player.Sit();
            //player.photonView.RPC(nameof(player.Sit), RpcTarget.All);
        }
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            player.Run(true);
            //player.photonView.RPC(nameof(player.Run), RpcTarget.All, true);
        }
        if (context.phase == InputActionPhase.Canceled)
        {
            player.Run(false);
            //player.photonView.RPC(nameof(player.Run), RpcTarget.All, false);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (UserHelpManager.Instance.SelectObject()) return;
            //player.isMove = true;
            player.Point();
            //Direction = ClickPos;
            //angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        }

        if (context.phase == InputActionPhase.Canceled)
        {
            //player.isMove = false;
        }
    }

    public void OnKeyMove(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            player.isMove = true;
        }

        if(context.phase == InputActionPhase.Performed)
        {
            Vector2 moveInput = context.ReadValue<Vector2>();
            player.MoveIdle(moveInput);
        }

        if(context.phase == InputActionPhase.Canceled)
        {
            player.isMove = false;
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            //player.Look();
        }
        //angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Handle mouse look input here
        //Debug.Log($"Mouse Look: {mouseDelta}");
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            //공격딜레이
            player.Attack();
            //player.photonView.RPC(nameof(player.Attack), RpcTarget.All);
        }
    }
}
