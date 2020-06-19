using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class RemoteAdmin : NetworkBehaviour
{
    public Dictionary<string, NetworkIdentity> PlayersIdentities = new Dictionary<string, NetworkIdentity>();

    [Command]
    public void CmdSendCmd(string cmd)
    {
        string ret = "Unknown command!";

        GetAllPlayers();

        List<string> args = new List<string>();
        foreach (string arg in Regex.Split(cmd, "(?<=^[^\"]*(?:\"[^\"]*\"[^\"]*)*) (?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)"))
        {
            args.Add(arg.Replace("\"", ""));
        }

        if(args.Count > 0)
        {
            NetworkIdentity id;
            string Nick = string.Empty;

            if (args[0] == "sudo")
            {
                Nick = args[2];
                if (PlayersIdentities.TryGetValue(Nick, out id))
                {
                    switch (args[1])
                    {
                        case "add-ammo":
                            int count = int.Parse(args[3]);
                            AmmoController ac = id.GetComponent<AmmoController>();
                            WeaponController wc = id.GetComponent<WeaponController>();
                            ac.ServerAddAmmo(wc.CurrentAmmoType, count * WeaponStats.GetMaxMagazineSize(wc.CurrentAmmoType));
                            ac.RefreshAllInPlayerAmmo();
                            ret = $"{Nick} got {args[3]} ammo magazines! ({args[3]}x{WeaponStats.GetMaxMagazineSize(wc.CurrentAmmoType)} ammo)";
                            break;
                        case "play-sound":
                            id.GetComponent<AudioSync>().RpcSyncAudioClip(args[3]);
                            ret = $"Played at position of {Nick} sound: {args[3]}";
                            break;
                        case "add-money":
                            int moneyCount = int.Parse(args[3]);
                            PlayerStats ps = id.GetComponent<PlayerStats>();
                            ps.AddMoney(moneyCount);
                            ret = $"Player {Nick} got {moneyCount} money! Player money: {ps.Money}.";
                            break;
                    }
                }
                else
                {
                    ret = "Player not found!";
                }
            }
            else
            {
                Nick = args[1];
                if (PlayersIdentities.TryGetValue(Nick, out id))
                {
                    //non sudo commands
                }
            }
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