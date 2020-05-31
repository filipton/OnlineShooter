using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OverwatchPlayer : NetworkBehaviour
{
    public GameObject OP_Prefab;

    public void StartOverWatch()
    {
        PlayerStats ps = FindObjectsOfType<PlayerStats>().ToList().Find(x => x.isLocalPlayer);

        if (ps != null)
        {
            CmdDespawnPlayer(ps.gameObject);
        }
    }

    [Command]
    public void CmdDespawnPlayer(GameObject pGb)
    {
        PlayerStats ps = pGb.GetComponent<PlayerStats>();
        if (ps.Nick == "f12")
        {
            NetworkIdentity id = ps.netIdentity;
            TargetRpcSpawnOverwatchPlayer(ps.connectionToClient);

            NetworkServer.UnSpawn(pGb);
            NetworkServer.Destroy(pGb);
        }
    }

    [TargetRpc]
    public void TargetRpcSpawnOverwatchPlayer(NetworkConnection conn)
    {
        Instantiate(OP_Prefab);
    }
}