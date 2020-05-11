﻿using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Team
{
    WithoutTeam = 0,
    Team1 = 1,
    Team2 = 2
}

public class PlayerStats : NetworkBehaviour
{
    [SyncVar]
    public string Nick;

    [SyncVar]
    public int Kills;
    [SyncVar]
    public int Deaths;
    //[SyncVar]
    //public int Assists;

    [SyncVar]
    public Team PlayerTeam;

    private void Start()
    {
        if (isLocalPlayer)
        {
            CustomNetworkManager cnm = FindObjectOfType<CustomNetworkManager>();
            CmdSetNick(cnm.LocalNick);
        }
        if (isServer)
            AutoSelectTeam();
    }

    [ServerCallback]
    void AutoSelectTeam()
    {
        int Team1P = 0;
        int Team2P = 0;

        foreach(PlayerStats ps in FindObjectsOfType<PlayerStats>())
        {
            if(ps.PlayerTeam == Team.Team1)
            {
                Team1P += 1;
                print("T1");
            }
            else if (ps.PlayerTeam == Team.Team2)
            {
                Team2P += 1;
                print("T2");
            }
        }

        if(Team1P == Team2P || Team1P < Team2P)
        {
            PlayerTeam = Team.Team1;
        }
        else if(Team1P > Team2P)
        {
            PlayerTeam = Team.Team2;
        }
    }

    [ServerCallback]
    public void OnlyServerAddKDA(int k, int d)
    {
        Kills += k;
        Deaths += d;
    }

    [Command]
    public void CmdSetNick(string nick)
    {
        if(Nick == string.Empty)
        {
            if (nick.Length < 1)
            {
                nick = $"NG{GetComponent<NetworkIdentity>().netId}";
            }

            Nick = nick;
        }
    }
}