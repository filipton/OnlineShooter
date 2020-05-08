﻿using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LocalSceneObjects : MonoBehaviour
{
    public static LocalSceneObjects singleton;

    [Header("Scene Objecs")]
    public TextMeshProUGUI AmmoText;


    private void Awake()
    {
        singleton = this;
    }

    private void Start()
    {
        DiscordRpcController.singleton.ChangeDiscordStatus($"IP: {NetworkManager.singleton.networkAddress}", "Gra na serwerze", "game_pic");
    }
}