using Mirror;
using Mirror.Websocket;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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
    public int Money = 800;

    [SyncVar]
    public Team PlayerTeam;

    private void Start()
    {
        if (isLocalPlayer)
        {
            CustomNetworkManager cnm = FindObjectOfType<CustomNetworkManager>();
            CmdSetNick(cnm.LocalNick, Application.version);
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
            }
            else if (ps.PlayerTeam == Team.Team2)
            {
                Team2P += 1;
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

        NetworkSync ns = GetComponent<NetworkSync>();
        if(PlayerTeam == Team.Team1)
        {
            ns.TpPlayer(LocalSceneObjects.singleton.TeamASpawn.position);
        }
        else if(PlayerTeam == Team.Team2)
        {
            ns.TpPlayer(LocalSceneObjects.singleton.TeamBSpawn.position);
        }
    }

    [ServerCallback]
    public void OnlyServerAddKDA(int k, int d)
    {
        Kills += k;
        Deaths += d;
    }

    [ServerCallback]
    public void AddMoney(int amount)
    {
        Money += amount;

        if(Money > 15000)
        {
            Money = 15000;
        }
    }

    [Command]
    public void CmdSetNick(string nick, string version)
    {
        if(Nick == string.Empty && !string.IsNullOrEmpty(nick) && !string.IsNullOrEmpty(version))
        {
            int vCont = ServerVsClientVersion(version, Application.version);
            print(vCont);
            if (vCont != 0)
            {
                //TODO: Show Kick Message To Client
                print(VersionKickMessage(vCont));

                //Kick player (Disconnect)
                GetComponent<NetworkIdentity>().connectionToClient.Disconnect();
            }

            Nick = nick;
        }
        else
        {
            GetComponent<NetworkIdentity>().connectionToClient.Disconnect();
        }
    }

    public int ServerVsClientVersion(string cV, string sV)
    {
        try
        {
            string[] cVs = cV.Split('.');
            string[] sVs = sV.Split('.');

            for (int i = 0; i < cVs.Length; i++)
            {
                int a = int.Parse(sVs[i]);
                int b = int.Parse(cVs[i]);

                if (a > b)
                {
                    //client outdated
                    return -1;
                }
                else if (a < b)
                {
                    //server outdated
                    return 1;
                }
            }
        }
        catch { return -1; }

        return 0;
    }

    public string VersionKickMessage(int VersionControl)
    {
        switch (VersionControl)
        {
            case -1:
                return "CLIENT OUTDATED";
            case 1:
                return "SERVER OUTDATED";
        }

        return string.Empty;
    }
}