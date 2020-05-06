using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AmmoController : NetworkBehaviour
{
    [SyncVar]
    public int MaxInMagazine = 30;
    [SyncVar]
    public int MaxInPlayer = 100;

    [SyncVar]
    public int InMagazine = 30;
    [SyncVar]
    public int InPlayer = 100;

    TextMeshProUGUI ammoText;

    private void Start()
    {
        ammoText = GameObject.FindGameObjectWithTag("AmmoText").GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (isLocalPlayer)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                CmdReload();
            }

            ammoText.text = $"<color={(InMagazine < 6 ? "red" : "#51FF00")}>{InMagazine}</color> <color=#FF3F00>/</color> <color={(InPlayer < MaxInMagazine ? "red" : "#51FF00")}>{InPlayer}</color>";
        }
    }

    [Command]
    public void CmdReload()
    {
        if(InPlayer > 0)
        {
            int min = InPlayer < MaxInMagazine ? InPlayer : MaxInMagazine;
            InPlayer -= min;
            InMagazine = min;
        }
    }
}