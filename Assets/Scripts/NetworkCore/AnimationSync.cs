using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSync : NetworkBehaviour
{
    public Animator ChAnimator;

    [Command]
    public void CmdSetAnimatorBool(string name, bool b)
    {
        RpcSetAnimatorBool(name, b);
    }

    [ClientRpc]
    public void RpcSetAnimatorBool(string name, bool b)
    {
        ChAnimator.SetBool(name, b);
    }
}