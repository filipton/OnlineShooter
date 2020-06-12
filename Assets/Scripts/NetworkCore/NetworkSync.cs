using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Networking.Types;
using System;

public class NetworkSync : NetworkBehaviour
{
    [Range(0, 0.5f)]
    public float Interval;

    NetworkIdentity Nid;
    PlayerMouseLook pml;
    PlayerMovement pm;
    public Camera cam;

    public float MovementThresholdXZ = 0.400002f;
    public float MovementThresholdY = 0.55f;

    public float Velocity;
    public bool isSneaking;

    Vector3 old_position;

    void Start()
    {
        if (isLocalPlayer)
        {
            Nid = GetComponent<NetworkIdentity>();
            pml = cam.GetComponent<PlayerMouseLook>();
            pm = GetComponent<PlayerMovement>();
        }
    }

	private void Update()
	{
        if(isLocalPlayer) CmdMovePlayer(transform.position, cam.transform.eulerAngles.x, transform.eulerAngles.y, Convert.ToByte(pm.isSneaking));
    }

	[Command]
    public void CmdMovePlayer(Vector3 pos, float xRot, float yRot, byte sneaking)
    {
        if (old_position == null)
        {
            old_position = pos;
        }
        else
        {
            if (pos != old_position)
            {
                float xzdistspeed = (new Vector2(pos.x, pos.z) - new Vector2(old_position.x, old_position.z)).magnitude;
                float ydistspeed = Mathf.Abs(pos.y - old_position.y);

                Velocity = xzdistspeed / Time.deltaTime;
                isSneaking = Convert.ToBoolean(sneaking);

                if (xzdistspeed > MovementThresholdXZ || ydistspeed > MovementThresholdY)
                {
                    pos = old_position;
                    TargetRpcMoveBackPlayer(Nid.connectionToClient, pos);
                }
                old_position = pos;
            }
        }

        RpcMovePlayer(pos, xRot, yRot);
        if (isServer && !isClient && !isLocalPlayer)
        {
            transform.position = pos;
            cam.transform.localRotation = Quaternion.Euler(xRot, 0, 0);
            transform.rotation = Quaternion.Euler(0, yRot, 0);
        }
    }

    [ClientRpc]
    public void RpcMovePlayer(Vector3 pos, float xRot, float yRot)
    {
        if (!isLocalPlayer)
        {
            transform.position = pos;
            cam.transform.localRotation = Quaternion.Euler(xRot, 0, 0);
            transform.rotation = Quaternion.Euler(0, yRot, 0);
        }
    }

    [TargetRpc]
    public void TargetRpcMoveBackPlayer(NetworkConnection conn, Vector3 pos)
    {
        GetComponent<CharacterController>().enabled = false;
        GetComponent<PlayerMovement>().enabled = false;
        this.transform.position = pos;
        GetComponent<CharacterController>().enabled = true;
        GetComponent<PlayerMovement>().enabled = true;
    }

    [ServerCallback]
    public void TpPlayer(Vector3 pos)
    {
        if (Nid == null)
            Nid = GetComponent<NetworkIdentity>();

        old_position = pos;
        TargetRpcMoveBackPlayer(Nid.connectionToClient, pos);
    }
}