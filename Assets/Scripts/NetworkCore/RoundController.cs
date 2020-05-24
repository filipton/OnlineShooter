using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundController : NetworkBehaviour
{
    public static RoundController singleton;

    [SyncVar]
    public int Team1Wins;

    [SyncVar]
    public int Team2Wins;


    private void Start()
    {
        DontDestroyOnLoad(this);
        singleton = this;
    }

    [ServerCallback]
    public void AddWins(Team pTeam)
    {
        if(pTeam == Team.Team1)
        {
            Team1Wins += 1;
        }
        else if(pTeam == Team.Team2)
        {
            Team2Wins += 1;
        }
    }

    [ServerCallback]
    public void CheckIfTeamAnyWin()
    {
        bool Team1Win = false;
        bool Team2Win = false;

        foreach(PlayerList pl in FindObjectsOfType<PlayerList>())
        {
            if(pl.ps.PlayerTeam == Team.Team1 && !pl.ph.PlayerKilled)
            {
                Team1Win = true;
            }
            else if(pl.ps.PlayerTeam == Team.Team2 && !pl.ph.PlayerKilled)
            {
                Team2Win = true;
            }
        }

        if(Team1Win != Team2Win)
        {
            Team WinTeam = Team.WithoutTeam;
            if (Team1Win) { WinTeam = Team.Team1; }
            else if (Team2Win) { WinTeam = Team.Team2; }
            AddWins(WinTeam);
        }

        print($"TEAM 1 WIN: {Team1Win}, TEAM 2 WIN: {Team2Win}");
    }
}