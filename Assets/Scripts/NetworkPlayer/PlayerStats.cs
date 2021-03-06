﻿using Discord;
using Dissonance;
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
    [SyncVar] public string Nick;

    public string _token;

    [SyncVar] public int Kills;
    [SyncVar] public int Deaths;
    //[SyncVar]
    //public int Assists;
    [SyncVar] public int Money = 800;
    [SyncVar] public Team PlayerTeam;

    [HideInInspector] public Team PlayerSetTeam;

    private void Start()
    {
        if (isLocalPlayer) GetComponent<OverwatchPlayer>().StartOverWatch();

        if(isServer) SavePlayerStats.singleton.AddPlayerToStats(new SavedPlayerStats(Nick, PlayerTeam, Money, Kills, Deaths));
    }

	[ServerCallback]
    void AutoSelectTeam()
    {
        if(PlayerSetTeam != Team.WithoutTeam)
		{
            PlayerTeam = PlayerSetTeam;
		}
		else
		{
            int Team1P = 0;
            int Team2P = 0;

            foreach (PlayerStats ps in FindObjectsOfType<PlayerStats>())
            {
                if (ps.PlayerTeam == Team.Team1)
                {
                    Team1P += 1;
                }
                else if (ps.PlayerTeam == Team.Team2)
                {
                    Team2P += 1;
                }
            }

            if (Team1P == Team2P || Team1P < Team2P)
            {
                PlayerTeam = Team.Team1;
            }
            else if (Team1P > Team2P)
            {
                PlayerTeam = Team.Team2;
            }
        }

        NetworkSync ns = GetComponent<NetworkSync>();
        if(PlayerTeam == Team.Team1)
        {
            ns.TpPlayer(LocalSceneObjects.singleton.TeamASpawn.position + new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5)));
        }
        else if(PlayerTeam == Team.Team2)
        {
            ns.TpPlayer(LocalSceneObjects.singleton.TeamBSpawn.position + new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5)));
        }

        foreach (PlayerStats ps in FindObjectsOfType<PlayerStats>())
        {
            if(ps.PlayerTeam != Team.WithoutTeam && PlayerTeam == ps.PlayerTeam)
			{
                TargetRpcSetMapIcons(netIdentity.connectionToClient, ps.netIdentity);
                TargetRpcSetMapIcons(ps.netIdentity.connectionToClient, netIdentity);
			}
        }

        TargetRpcSetTeamVoice(netIdentity.connectionToClient, $"Team{((PlayerTeam == Team.Team1) ? "A" : "B")}");
    }

    [TargetRpc]
    public void TargetRpcSetMapIcons(NetworkConnection conn, NetworkIdentity id)
	{
        id.gameObject.GetComponent<PlayerList>().PlayerIconOnMap.SetActive(true);
    }

    [TargetRpc]
    public void TargetRpcSetTeamVoice(NetworkConnection conn, string teamName)
	{
        var comms = FindObjectOfType<DissonanceComms>();

        if (comms == null)
        {
            Debug.Log($"Cannot find voice components for team '{teamName}'");
            return;
        }

        comms.AddToken(teamName);
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
        SavePlayerStats.singleton.AddPlayerToStats(new SavedPlayerStats(Nick, PlayerTeam, Money, Kills, Deaths));
    }

    [Command]
    public void CmdSetNick(string nick) => SetNick(nick);

    [ServerCallback]
    public void SetNick(string nick)
    {
        if (Nick == string.Empty && !string.IsNullOrEmpty(nick))
        {
            Nick = nick;

            //check if player is saved 
            int index = SavePlayerStats.singleton.savedPlayerStats.FindIndex(x => x.Name == nick);

            if (index > -1)
            {
                SavedPlayerStats sps = SavePlayerStats.singleton.savedPlayerStats[index];

                Kills = sps.Kills;
                PlayerSetTeam = sps.Team;
                Deaths = sps.Deaths;
                Money = sps.Money;
            }
            else
            {
                SavePlayerStats.singleton.AddPlayerToStats(new SavedPlayerStats(Nick, PlayerTeam, Money, Kills, Deaths));
            }

            AutoSelectTeam();
        }
        else
        {
            GetComponent<NetworkIdentity>().connectionToClient.Disconnect();
        }
    }

    [ServerCallback]
    public void KickPlayerWithMsg(string msg) => StartCoroutine(KickPlayer(msg));

    [TargetRpc]
    public void TargetRpcKickPlayerMsg(NetworkConnection conn, string desc) => SMessageBox.singleton.ShowMessageBox("Disconected", desc);

    [ServerCallback]
    IEnumerator KickPlayer(string desc)
    {
        TargetRpcKickPlayerMsg(GetComponent<NetworkIdentity>().connectionToClient, desc);
        yield return new WaitForSeconds(0.1f);
        GetComponent<NetworkIdentity>().connectionToClient.Disconnect();
    }
}