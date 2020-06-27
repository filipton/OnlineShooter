using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

[System.Serializable]
public class Strings : SyncList<string> { }

public class LobbyTeamSelection : NetworkBehaviour
{
    public bool AutoBalance = true;

    public Strings PlayersInTeamA = new Strings();
    public Strings PlayersInTeamB = new Strings();

    List<GameObject> gbs = new List<GameObject>();

    public GameObject pText;

    public GameObject ContentTeamA;
    public GameObject ContentTeamB;

	private void Start()
	{
        DontDestroyOnLoad(this);
	}

	[ClientRpc]
    public void RpcRefreshLists()
	{
        if(gbs.Count > 0)
		{
            foreach(GameObject gb in gbs)
			{
                Destroy(gb);
			}
		}

        foreach(string nick in PlayersInTeamA)
		{
            GameObject gb = Instantiate(pText, ContentTeamA.transform);
            gb.GetComponent<TextMeshProUGUI>().text = nick;
            gbs.Add(gb);
		}
        foreach (string nick in PlayersInTeamB)
        {
            GameObject gb = Instantiate(pText, ContentTeamB.transform);
			gb.GetComponent<TextMeshProUGUI>().text = nick;
            gbs.Add(gb);
        }
    }

    [ServerCallback]
    public void SelectTeam(string nick, Team t)
	{
        Team oldTeam = Team.WithoutTeam;

        if (PlayersInTeamA.Count > 0 && PlayersInTeamA.FindIndex(x => x == nick) > -1)
        {
            PlayersInTeamA.Remove(nick);
            oldTeam = Team.Team1;
        }
        if (PlayersInTeamB.Count > 0 && PlayersInTeamB.FindIndex(x => x == nick) > -1)
        {
            PlayersInTeamB.Remove(nick);
            oldTeam = Team.Team2;
        }

        int ta = PlayersInTeamA.Count;
        int tb = PlayersInTeamB.Count;

        if (AutoBalance)
		{
            if (t == Team.Team1) ta++;
            else if (t == Team.Team2) tb++;

            if(Mathf.Abs(ta - tb) > 1)
			{
                t = oldTeam;
			}
		}

        if(t == Team.Team1)
		{
            PlayersInTeamA.Add(nick);
		}
        else if(t == Team.Team2)
		{
            PlayersInTeamB.Add(nick);
		}

        RpcRefreshLists();
    }
}