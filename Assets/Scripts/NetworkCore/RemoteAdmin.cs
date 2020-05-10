using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteAdmin : NetworkBehaviour
{
    public Dictionary<string, NetworkIdentity> PlayersIdentities = new Dictionary<string, NetworkIdentity>();

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    [Command]
    public void CmdSendCmd(string cmd, string Nick, string parms)
    {
        GetAllPlayers();

        string ret = "Unknown command!";
        PlayersIdentities.TryGetValue(Nick, out NetworkIdentity id);

        switch (cmd)
        {
            case "add-ammo":
                id.GetComponent<AmmoController>().InPlayer += int.Parse(parms);
                ret = $"Ammo added to: Nick: {Nick}, Amount: {parms}";
                break;
            case "play-sound":
                id.GetComponent<AudioSync>().RpcSyncAudioClip(parms);
                ret = $"Nick: {Nick}, Parms: {parms}";
                break;
        }

        TargetRpcCommandReturn(GetComponent<NetworkIdentity>().connectionToClient, ret);
    }

    [TargetRpc]
    public void TargetRpcCommandReturn(NetworkConnection conn, string ret)
    {
        RemoteConsole.singleton.AddLog($"<color=red><b>[SUDO SERVERCALLBACK]</b></color> {ret}");
    }

    void GetAllPlayers()
    {
        PlayersIdentities.Clear();

        foreach (NetworkIdentity id in FindObjectsOfType<NetworkIdentity>())
        {
            if (id.GetComponent<PlayerStats>() != null)
            { 
                PlayersIdentities.Add(id.GetComponent<PlayerStats>().Nick, id); 
            }
        }
    }
}