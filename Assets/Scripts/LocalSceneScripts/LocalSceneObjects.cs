using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LocalSceneObjects : MonoBehaviour
{
    public static LocalSceneObjects singleton;

    [Header("Scene Objecs")]
    public TextMeshProUGUI AmmoText;
    public GameObject SniperScope;
    public RawImage HitMarker;
    
    [Header("Tab Player Stats")]
    public GameObject TabPlayerStats;
    public GameObject Team1;
    public GameObject Team2;
    public TextMeshProUGUI RoundsText;

    [Header("KillFeed")]
    public GameObject KF_Parent;
    public GameObject KF_Prefab;

    [Header("Team Spawn Points")]
    public Transform TeamASpawn;
    public Transform TeamBSpawn;



    private void Awake()
    {
        singleton = this;
    }

    private void Start()
    {
        DiscordRpcController.singleton.ChangeDiscordStatus($"Server: {DiscordRpcController.singleton.CurrentServer.ServerName}", "Gra na serwerze", "game_pic");
    }
}