using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void Start()
    {
        if(isLocalPlayer)
            CmdSetNick(FindObjectOfType<CustomNetworkManager>().LocalNick);
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
}