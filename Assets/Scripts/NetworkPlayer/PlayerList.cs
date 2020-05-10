using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking.Types;

[Serializable]
public struct Player
{
    public string Name;
    public PlayerList plist;
    public TextMeshProUGUI pStatsText;

    public Player(string name, PlayerList plist, TextMeshProUGUI pStatsText = null)
    {
        Name = name;
        this.plist = plist;
        this.pStatsText = pStatsText;
    }
}

public class PlayerList : NetworkBehaviour
{
    public List<Player> players = new List<Player>();
    public Camera cam;
    public PlayerHealth ph;
    public PlayerStats ps;

    public GameObject PlayerTabStatsText;

    [SyncVar]
    public int PlayersCount;

    public int CurrentPlayer = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer && ph.PlayerKilled)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                CurrentPlayer--;
                UpdateCam();
            }
            else if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                CurrentPlayer++;
                UpdateCam();
            }
        }
        else if (isLocalPlayer)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                LocalSceneObjects.singleton.TabPlayerStats.SetActive(true);
                StartCoroutine(UpdatePlayerList());
            }
            else if (Input.GetKeyUp(KeyCode.Tab))
            {
                LocalSceneObjects.singleton.TabPlayerStats.SetActive(false);
            }

            //LocalSceneObjects.singleton.TabPlayerStats.SetActive(Input.GetKey(KeyCode.Tab));
            //StartCoroutine(UpdatePlayerList());
        }

        if (isServer)
        {
            PlayersCount = NetworkServer.connections.Count;
        }
    }

    IEnumerator UpdatePlayerList()
    {
        LocalSceneObjects.singleton.RoundsText.text = $"{RoundController.singleton.Team1Wins} / {RoundController.singleton.Team2Wins}";
        UpdateList();
        yield return new WaitForSeconds(0.01f);
        foreach (Player p in players.ToArray())
        {
            if (p.pStatsText == null && p.plist.ps.PlayerTeam != Team.WithoutTeam)
            {
                Transform trans = p.plist.ps.PlayerTeam == Team.Team1 ? LocalSceneObjects.singleton.Team1.transform : LocalSceneObjects.singleton.Team2.transform;

                GameObject playerLT = Instantiate(PlayerTabStatsText, trans);
                TextMeshProUGUI pT = playerLT.GetComponent<TextMeshProUGUI>();
                pT.text = $"{p.Name}   {p.plist.ps.Kills}|{p.plist.ps.Deaths}";
                int index = players.FindIndex(x => x.Name == p.Name);
                players[index] = new Player(p.Name, p.plist, pT);
            }
            else
            {
                p.pStatsText.text = $"{p.Name}   {p.plist.ps.Kills}|{p.plist.ps.Deaths}";
            }
        }
    }

    public void UpdateCam()
    {
        UpdateList();

        CurrentPlayer = Mathf.Clamp(CurrentPlayer, 0, PlayersCount - 1);

        cam.enabled = false;
        foreach (Player p in players)
        {
            p.plist.cam.enabled = false;
        }

        players[CurrentPlayer].plist.cam.enabled = true;
    }

    void UpdateList()
    {
        if (players.Count != PlayersCount)
        {
            if (isServer)
            {
                foreach (PlayerList pl in FindObjectsOfType<PlayerList>())
                {
                    pl.RpcGetAllPlayers();
                }
            }
            else
            {
                foreach (NetworkIdentity nid in FindObjectsOfType<NetworkIdentity>())
                {
                    PlayerStats ps = nid.GetComponent<PlayerStats>();
                    PlayerList pl = nid.GetComponent<PlayerList>();

                    if (ps != null && pl != null)
                    {
                        if (players.Find(x => x.Name == ps.Nick).Name == null)
                        {
                            players.Add(new Player(ps.Nick, pl));
                        }
                    }
                }
            }
        }
    }

    [ClientRpc]
    public void RpcGetAllPlayers()
    {
        foreach(NetworkIdentity nid in FindObjectsOfType<NetworkIdentity>())
        {
            PlayerStats ps = nid.GetComponent<PlayerStats>();
            PlayerList pl = nid.GetComponent<PlayerList>();

            if (ps != null && pl != null)
            {
                if (players.Find(x => x.Name == ps.Nick).Name == null)
                {
                    players.Add(new Player(nid.GetComponent<PlayerStats>().Nick, nid.GetComponent<PlayerList>()));
                }
            }
        }
    }
}