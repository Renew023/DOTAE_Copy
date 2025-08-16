using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEngine;

public static class UtilityCode
{
    public static void Logger2(string hash) => Logger(hash);
    public static void WaningLogger(string hash) => WarningLogger(hash);

    public static void Logger(string hash)
    {
        Debug.Log(hash);
    }

    public static void WarningLogger(string hash)
    {
        Debug.LogWarning(hash);
    }

    public static void ErrorLogger(string hash)
    {
        Debug.LogError(hash);
    }

    public static void NullComponentCheck<T>(T component)
    {
        if (component == null)
            Debug.LogError("null입니다");
    }

    //public static void Rpc(this PhotonView pv, Action method, object[] parameters, bool isMine)
    //{
    //    if (isMine)
    //    {
    //        // 본인 포함 전체에게 브로드캐스트
    //        pv.RPC(method.Method.Name, RpcTarget.All, parameters);
    //    }
    //    else
    //    {
    //        // 다른 클라이언트에게만 전송
    //        pv.RPC(method.Method.Name, RpcTarget.Others, parameters);
    //    }
    //}

    public static GameObject AddPhoton(this GameObject instance)
    {
        return instance;

        if (instance.TryGetComponent<PhotonView>(out var photonView))
        {
            
        }
        else
        {
            photonView = instance.AddComponent<PhotonView>();
        }
        Reset(photonView);

        //if (instance.TryGetComponent<Rigidbody2D>(out var rigid))
        //{
        //    Reset(rigid);
        //    var component = instance.GetComponent<PhotonRigidbody2DView>();
        //    if (component != null && !photonView.ObservedComponents.Contains(component))
        //    {
        //        photonView.ObservedComponents.Add(component);
        //    }
        //}

        //if (instance.TryGetComponent<CharacterObject>(out var cha))
        //{
        //    Reset(cha);
        //    var component = instance.GetComponent<PhotonTransformView>();
        //    if (component != null && !photonView.ObservedComponents.Contains(component))
        //    {
        //        photonView.ObservedComponents.Add(component);
        //    }
        //}

        //if (instance.TryGetComponent<Animator>(out var anim))
        //{
        //    Reset(anim);
        //    var component = instance.GetComponent<PhotonAnimatorView>();
        //    if (component != null && !photonView.ObservedComponents.Contains(component))
        //    {
        //        photonView.ObservedComponents.Add(component);
        //    }
        //}
        return instance;
    }

    public static void Reset(PhotonView view)
    {
        //Reset(pv)에서 pv의 속성/필드를 바꾸면 component도 바뀜 ✅
        view.Synchronization = ViewSynchronization.UnreliableOnChange;
        view.OwnershipTransfer = OwnershipOption.Takeover;
        view.observableSearch = PhotonView.ObservableSearch.Manual;
        if (view.ObservedComponents == null)
        {
            view.ObservedComponents = new();
        }
        return;
    }
    public static void Reset(Rigidbody2D rigid)
    {
        if (rigid.gameObject.TryGetComponent<PhotonRigidbody2DView>(out var view))
        {

        }
        else
        {
            rigid.gameObject.AddComponent<PhotonRigidbody2DView>();
        }
    }

    public static void Reset(CharacterObject cha)
    {
        if (cha.gameObject.TryGetComponent<PhotonTransformView>(out var view))
        {

        }
        else
        {
            view = cha.gameObject.AddComponent<PhotonTransformView>();
            view.m_SynchronizeRotation = false; //돌아가는 게 없음 =탑뷰
            view.m_SynchronizeScale = true;  //스킬 
        }
    }

    public static void Reset(Animator anim)
    {
        if (anim.gameObject.TryGetComponent<PhotonAnimatorView>(out var view))
        {

        }
        else
        {
            anim.gameObject.AddComponent<PhotonAnimatorView>();
        }
    }


}
