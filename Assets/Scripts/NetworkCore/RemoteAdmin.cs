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
                int count = int.Parse(parms);
                AmmoController ac = id.GetComponent<AmmoController>();
                WeaponController wc = id.GetComponent<WeaponController>();
                for (int i = 0; i < count; i++)
                {
                    ac.GetAmmoMagazines(wc.CurrentAmmoType).Add(new AmmoMagazine(ac.MaxInMagazine));
                }
                ac.RefreshAllInPlayerAmmo();
                ret = $"{Nick} got {parms} ammo magazines! ({parms}x30 ammo)";
                break;
            case "play-sound":
                id.GetComponent<AudioSync>().RpcSyncAudioClip(parms);
                ret = $"Played at position of {Nick} sound: {parms}";
                break;
            case "add-money":
                int moneyCount = int.Parse(parms);
                PlayerStats ps = id.GetComponent<PlayerStats>();
                ps.AddMoney(moneyCount);
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