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
        singleton = this;
    }

    [ServerCallback]
    public void AddWins(int t1w, int t2w)
    {
        Team1Wins += t1w;
        Team2Wins += t2w;
    }
}