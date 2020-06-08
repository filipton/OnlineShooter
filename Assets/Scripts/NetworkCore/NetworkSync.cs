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
    public Camera cam;

    public float MovementThresholdXZ = 0.400002f;
    public float MovementThresholdY = 0.55f;

    Vector3 old_position;

    void Start()
    {
        if (isLocalPlayer)
        {
            Nid = GetComponent<NetworkIdentity>();
            StartCoroutine(SyncMovement());
        }
    }

    [Command]
    public void CmdMovePlayer(Vector3 pos, Quaternion rot, Quaternion camRot)
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

        RpcMovePlayer(pos, rot, camRot);
        transform.position = pos;
        transform.rotation = rot;
        cam.transform.rotation = camRot;
    }

    [ClientRpc]
    public void RpcMovePlayer(Vector3 pos, Quaternion rot, Quaternion camRot)
    {
        if (!isLocalPlayer)
        {
            transform.position = pos;
            transform.rotation = rot;
            cam.transform.rotation = camRot;
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
            CmdMovePlayer(transform.position, transform.rotation, cam.transform.rotation);
            yield return new WaitForSeconds(Interval);
        }
    }
}