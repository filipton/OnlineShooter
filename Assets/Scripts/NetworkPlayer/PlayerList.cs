using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Types;

[Serializable]
public struct Player
{
    public string Name;
    public PlayerList plist;

    public Player(string name, PlayerList plist)
    {
        Name = name;
        this.plist = plist;
    }
}

public class PlayerList : NetworkBehaviour
{
    public List<Player> players = new List<Player>();
    public Camera cam;
    public PlayerHealth ph;

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

        if (isServer)
        {
            PlayersCount = NetworkServer.connections.Count;
        }
    }

    public void UpdateCam()
    {
        if(players.Count != PlayersCount)
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
                players.Clear();

                foreach (NetworkIdentity nid in FindObjectsOfType<NetworkIdentity>())
                {
                    PlayerStats ps = nid.GetComponent<PlayerStats>();
                    PlayerList pl = nid.GetComponent<PlayerList>();

                    if (ps != null && pl != null)
                    {
                        players.Add(new Player(ps.Nick, pl));
                    }
                }
            }
        }

        CurrentPlayer = Mathf.Clamp(CurrentPlayer, 0, PlayersCount - 1);

        cam.enabled = false;
        foreach (Player p in players)
        {
            p.plist.cam.enabled = false;
        }

        players[CurrentPlayer].plist.cam.enabled = true;
    }

    [ClientRpc]
    public void RpcGetAllPlayers()
    {
        players.Clear();

        foreach(NetworkIdentity nid in FindObjectsOfType<NetworkIdentity>())
        {
            players.Add(new Player(nid.GetComponent<PlayerStats>().Nick, nid.GetComponent<PlayerList>()));
        }
    }
}