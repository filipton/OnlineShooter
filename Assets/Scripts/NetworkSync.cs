using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Networking.Types;

public class NetworkSync : NetworkBehaviour
{
    [Range(0, 0.5f)]
    public float Interval;

    public NetworkIdentity Nid;
    public Camera cam;

    public float MovementTroubleshootXZ = 0.400002f;
    public float MovementTroubleshootY = 0.55f;
    public float MaxXZ;
    public float MaxY;

    Vector3 old_position;

    // Start is called before the first frame update
    void Start()
    {
        if (isLocalPlayer)
        {
            Nid = GetComponent<NetworkIdentity>();
            StartCoroutine(SyncMovement());
        }
    }

    [Command]
    public void CmdMovePlayer(Vector3 pos, Quaternion rot, Quaternion camRot, NetworkIdentity nid)
    {
        if (old_position == null)
        {
            old_position = pos;
        }
        else
        {
            float xzdistspeed = (new Vector2(pos.x, pos.z) - new Vector2(old_position.x, old_position.z)).magnitude;
            float ydistspeed = Mathf.Abs(pos.y - old_position.y);

            if(xzdistspeed > MaxXZ)
            {
                MaxXZ = xzdistspeed;
            }
            if (ydistspeed > MaxY)
            {
                MaxY = ydistspeed;
            }

            if (xzdistspeed > MovementTroubleshootXZ || ydistspeed > MovementTroubleshootY)
            {
                pos = old_position;
                TargetRpcMoveBackPlayer(this.GetComponent<NetworkIdentity>().connectionToClient, pos);
            }
            old_position = pos;
        }

        RpcMovePlayer(pos, rot, camRot, nid);
        transform.position = pos;
        transform.rotation = rot;
        cam.transform.rotation = camRot;
    }

    [ClientRpc]
    public void RpcMovePlayer(Vector3 pos, Quaternion rot, Quaternion camRot, NetworkIdentity nid)
    {
        if (Nid != nid)
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

    IEnumerator SyncMovement()
    {
        while (true)
        {
            CmdMovePlayer(transform.position, transform.rotation, cam.transform.rotation, Nid);
            yield return new WaitForSeconds(Interval);
        }
    }
}