using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Networking.Types;

public class NetworkSync : NetworkBehaviour
{
    [Range(0, 0.5f)]
    public float Interval;

    NetworkIdentity Nid;
    PlayerMouseLook pml;
    public Camera cam;

    public float MovementThresholdXZ = 0.400002f;
    public float MovementThresholdY = 0.55f;

    Vector3 old_position;

    void Start()
    {
        if (isLocalPlayer)
        {
            Nid = GetComponent<NetworkIdentity>();
            pml = cam.GetComponent<PlayerMouseLook>();
            StartCoroutine(SyncMovement());
        }
    }

    [Command]
    public void CmdMovePlayer(Vector3 pos, sbyte xRot, short yRot)
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
            transform.rotation = Quaternion.Euler(0, yRot, 0);
            cam.transform.rotation = Quaternion.Euler(xRot, 0, 0);
        }
    }

    [ClientRpc]
    public void RpcMovePlayer(Vector3 pos, sbyte xRot, short yRot)
    {
        if (!isLocalPlayer)
        {
            transform.position = pos;
            cam.transform.localRotation = Quaternion.AngleAxis(xRot, -Vector3.right);
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

    IEnumerator SyncMovement()
    {
        while (true)
        {
            CmdMovePlayer(transform.position, (sbyte)pml.rotationY, (short)transform.rotation.eulerAngles.y);
            yield return new WaitForSeconds(Interval);
        }
    }
}