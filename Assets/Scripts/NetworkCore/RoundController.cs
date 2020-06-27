using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

[Serializable]
public struct RTeamsPerms
{
    public Team team;
    public bool WinAfterRunOutOfTimeAndBombNotExploeded;
    public bool WinAfterBombExploded;
    public bool WinAfterBombDefused;
}

public class RoundController : NetworkBehaviour
{
    public static RoundController singleton;

    [SyncVar] public int Team1Wins;
    [SyncVar] public int Team2Wins;
    [SyncVar] public float RoundTimeReaming;

    public float RoundTime = 150f;
    public int WinRoundCount = 15;

    bool RoundWin = false;

    [Header("Permissions")]
    public List<RTeamsPerms> teamsPermissions = new List<RTeamsPerms>();

    private void Start()
    {
        singleton = this;
        CursorManager.RemoveAll();

        if(isServer)
		{
            RoundTimeReaming = RoundTime;
		}
    }

	private void Update()
	{
        if (isServer) ServerUpdate();

		if (isClient)
		{
            TimeSpan ts = TimeSpan.FromSeconds(RoundTimeReaming);
            string minutes = ts.Minutes.ToString();
            string seconds = (ts.Seconds < 10 && ts.Seconds >= 0) ? $"0{ts.Seconds}" : ts.Seconds.ToString();
            LocalSceneObjects.singleton.RoundTime.text = $"{minutes}:{seconds}";
        }
    }

    [ServerCallback]
    void ServerUpdate()
	{
        RoundTimeReaming -= Time.deltaTime;

        if(RoundTimeReaming <= 0)
		{
            CheckIfAnyTeamWin();
        }
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
		if (!RoundWin)
		{
            bool Team1Win = false;
            bool Team2Win = false;

            Team WinTeam = Team.WithoutTeam;
            BombSystem bs = FindObjectOfType<BombSystem>();
            foreach (RTeamsPerms tp in teamsPermissions)
            {
                if ((tp.WinAfterRunOutOfTimeAndBombNotExploeded && RoundTimeReaming <= 0) || (tp.WinAfterBombExploded && bs.IsExploding && bs.m_bombExplosionTime >= bs.BombExplosionTime) || (tp.WinAfterBombDefused && bs.IsDefusing && bs.m_defusingTime >= bs.DefusingTime))
                {
                    if (tp.team == Team.Team1) Team1Win = true;
                    else if (tp.team == Team.Team2) Team2Win = true;
                    goto skip_point;
                }
            }

            if (!bs.IsExploding)
            {
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

                if (!Team1Win && !Team2Win)
                {
                    Team1Win = true;
                }
            }


            skip_point:

            if (Team1Win != Team2Win)
            {
                RoundWin = true;

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

                RoundTimeReaming = RoundTime + 5f;
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
        foreach (DroppedWeapon dWeapon in FindObjectsOfType<DroppedWeapon>())
        {
            NetworkServer.UnSpawn(dWeapon.gameObject);
            NetworkServer.Destroy(dWeapon.gameObject);
        }

        foreach (PlayerStats p in FindObjectsOfType<PlayerStats>())
        {
            NetworkSync ns = p.GetComponent<NetworkSync>();
            PlayerHealth ph = p.GetComponent<PlayerHealth>();

            if (p.PlayerTeam == Team.Team1)
            {
                ns.TpPlayer(LocalSceneObjects.singleton.TeamASpawn.position + new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5)));
            }
            else if (p.PlayerTeam == Team.Team2)
            {
                ns.TpPlayer(LocalSceneObjects.singleton.TeamBSpawn.position + new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5)));
            }

            ph.PlayerKilled = false;
            ph.Health = 100;
            ph.GetComponent<OnlineShooting>().HitBoxes.ToList().ForEach(delegate (HitBox hb)
            {
                hb.GetComponent<MeshCollider>().enabled = true;
            });
            ph.RpcRespawnPlayer();
            ph.RpcClearDamagesFromPlayers();

			if (isServerOnly)
			{
                ph.damageFromPlayers.Clear();
            }
        }

        RoundWin = false;
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