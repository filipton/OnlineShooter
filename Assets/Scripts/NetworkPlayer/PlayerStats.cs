using Mirror;
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
            CmdChooseTeam(cnm.LocalTeam == Team.WithoutTeam ? Team.Team1 : cnm.LocalTeam);
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
                nick = $"NIENAZWANY GRACZZ! {GetComponent<NetworkIdentity>().netId}";
            }

            Nick = nick;
        }
    }

    [Command]
    public void CmdChooseTeam(Team team)
    {
        PlayerTeam = team;
    }
}