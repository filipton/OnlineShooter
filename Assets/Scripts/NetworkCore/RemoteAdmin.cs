using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteAdmin : NetworkBehaviour
{
    public Dictionary<string, NetworkIdentity> PlayersIdentities = new Dictionary<string, NetworkIdentity>();

    private void Start()
    {
        GetAllPlayers();
    }

    private void Update()
    {
        if (isLocalPlayer)
        {
            
        }
    }

    [Command]
    public void CmdSendCmd(string cmd, string Nick, string parms)
    {
        string ret = "Unknown command!";
        GetAllPlayers();
        PlayersIdentities.TryGetValue(Nick, out NetworkIdentity id);

        switch (cmd)
        {
            case "add-ammo":
                id.GetComponent<AmmoController>().InPlayer += int.Parse(parms);
                ret = $"Nick: {Nick}, Parms: {parms}";
                break;
            case "play-sound":
                id.GetComponent<AudioSync>().RpcSyncAudioClip(parms);
                ret = $"Nick: {Nick}, Parms: {parms}";
                break;
        }

        TargetRpcCommandReturn(GetComponent<NetworkIdentity>().connectionToClient, cmd);
    }

    [TargetRpc]
    public void TargetRpcCommandReturn(NetworkConnection conn, string ret)
    {
        print(ret);
    }

    void GetAllPlayers()
    {
        PlayersIdentities.Clear();

        foreach (NetworkIdentity id in FindObjectsOfType<NetworkIdentity>())
        {
            PlayersIdentities.Add(id.GetComponent<PlayerStats>().Nick, id);
        }
    }
}