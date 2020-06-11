using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoundController : NetworkBehaviour
{
    public static RoundController singleton;

    [SyncVar]
    public int Team1Wins;

    [SyncVar]
    public int Team2Wins;


    public int WinRoundCount = 15;

    private void Start()
    {
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
    public void CheckIfAnyTeamWin()
    {
        bool Team1Win = false;
        bool Team2Win = false;

        foreach (PlayerList pl in FindObjectsOfType<PlayerList>())
        {
            if (pl.ps.PlayerTeam == Team.Team1 && !pl.ph.PlayerKilled)
            {
                Team1Win = true;
            }
            else if (pl.ps.PlayerTeam == Team.Team2 && !pl.ph.PlayerKilled)
            {
                Team2Win = true;
            }
        }

        if (Team1Win != Team2Win)
        {
            Team WinTeam = Team.WithoutTeam;
            if (Team1Win) { WinTeam = Team.Team1; }
            else if (Team2Win) { WinTeam = Team.Team2; }
            AddWins(WinTeam);

            bool IsTeam1GameWin = Team1Wins >= WinRoundCount;
            bool IsTeam2GameWin = Team2Wins >= WinRoundCount;

            if (IsTeam1GameWin || IsTeam2GameWin)
            {
                StartCoroutine(EndGame(10f));
            }
            else
            {
                StartCoroutine(EndRound(5f));
            }
        }
    }

    [ServerCallback]
    IEnumerator EndRound(float time)
    {
        yield return new WaitForSeconds(time);

        foreach (AmmoBox gbAmmoBox in FindObjectsOfType<AmmoBox>())
        {
            NetworkServer.UnSpawn(gbAmmoBox.gameObject);
            NetworkServer.Destroy(gbAmmoBox.gameObject);
        }

        foreach (PlayerStats p in FindObjectsOfType<PlayerStats>())
        {
            NetworkSync ns = p.GetComponent<NetworkSync>();
            PlayerHealth ph = p.GetComponent<PlayerHealth>();

            if (p.PlayerTeam == Team.Team1)
            {
                ns.TpPlayer(LocalSceneObjects.singleton.TeamASpawn.position);
            }
            else if (p.PlayerTeam == Team.Team2)
            {
                ns.TpPlayer(LocalSceneObjects.singleton.TeamBSpawn.position);
            }

            ph.PlayerKilled = false;
            ph.Health = 100;
            ph.GetComponent<OnlineShooting>().HitBoxes.ToList().ForEach(delegate (HitBox hb)
            {
                hb.GetComponent<MeshCollider>().enabled = true;
            });
            ph.RpcRespawnPlayer();
        }
    }

    [ServerCallback]
    IEnumerator EndGame(float time)
    {
        yield return new WaitForSeconds(time);

        /*foreach(PlayerStats ps in FindObjectsOfType<PlayerStats>())
        {
            ps.KickPlayerWithMsg($"Server is restarting!");
        }*/

        RoundController rc = FindObjectOfType<RoundController>();
        rc.Team1Wins = 0;
        rc.Team2Wins = 0;

        Destroy(rc.gameObject);

        FindObjectOfType<CustomNetworkManager>().ServerChangeScene("Lobby");
    }
}