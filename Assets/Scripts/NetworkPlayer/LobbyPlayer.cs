using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyPlayer : NetworkBehaviour
{
    [SyncVar] public string Nick;

    public string _token;

    public GameObject PlayerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        if (isLocalPlayer)
            CmdSendToken(FindObjectOfType<LoginSystem>().LoginToken, Application.version);
    }

    // Update is called once per frame
    void Update()
    {
		if(isLocalPlayer && SceneManager.GetActiveScene().name != "Lobby")
		{
			try
			{
                CmdReplacePlayer();
			}
			catch { }
		}
    }

    [Command]
    public void CmdReplacePlayer()
	{
        NetworkConnection conn = GetComponent<NetworkIdentity>().connectionToClient;

        GameObject oldPlayer = conn.identity.gameObject;
        if (oldPlayer.GetComponent<PlayerStats>() == null)
        {
            string nick = oldPlayer.GetComponent<LobbyPlayer>().Nick;
            string token = oldPlayer.GetComponent<LobbyPlayer>()._token;

            LobbyTeamSelection stl = FindObjectOfType<LobbyTeamSelection>();
            int ta = stl.PlayersInTeamA.FindIndex(x => x == nick);
            int tb = stl.PlayersInTeamB.FindIndex(x => x == nick);
            Team selectedTeam = Team.WithoutTeam;

            if(ta > -1)
			{
                selectedTeam = Team.Team1;
			}
            else if (tb > -1)
            {
                selectedTeam = Team.Team2;
            }

            print(selectedTeam);

            NetworkServer.ReplacePlayerForConnection(conn, Instantiate(PlayerPrefab));
            PlayerStats ps = conn.identity.GetComponent<PlayerStats>();
            if(selectedTeam != Team.WithoutTeam)
			{
                ps.PlayerSetTeam = selectedTeam;
            }
            ps.SetNick(nick);
            ps._token = token;

            NetworkServer.Destroy(oldPlayer);
            NetworkServer.UnSpawn(oldPlayer);
        }
    }

    [Command]
    public void CmdSendToken(string token, string version)
	{
		if (!string.IsNullOrEmpty(token))
		{
            Nick = new WebClient().DownloadString("https://login.filipton.space/UserManagment.php?mode=gn&t=" + token);
            _token = token;

            int msV = ServerVsClientVersion(version, Application.version);
			if (string.IsNullOrEmpty(Nick) || msV != 0)
			{
                KickPlayerWithMsg($"Kicked from server: {VersionKickMessage(msV)}");
			}
        }
	}

    [Command]
    public void CmdSelectTeam(Team t)
	{
        LobbyTeamSelection lts = FindObjectOfType<LobbyTeamSelection>();

        lts.SelectTeam(Nick, t);
	}

    [ServerCallback]
    public void KickPlayerWithMsg(string msg)
    {
        StartCoroutine(KickPlayer(msg));
    }

    [TargetRpc]
    public void TargetRpcKickPlayerMsg(NetworkConnection conn, string desc)
    {
        SMessageBox.singleton.ShowMessageBox("Disconected", desc);
    }

    [ServerCallback]
    IEnumerator KickPlayer(string desc)
    {
        TargetRpcKickPlayerMsg(GetComponent<NetworkIdentity>().connectionToClient, desc);
        yield return new WaitForSeconds(0.1f);
        GetComponent<NetworkIdentity>().connectionToClient.Disconnect();
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