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

    public GameObject AmmoBoxPrefab;

    private void Update()
    {
        if (isLocalPlayer)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                CmdReload();
            }

            LocalSceneObjects.singleton.AmmoText.text = $"<color={(InMagazine < 6 ? "red" : "#51FF00")}>{InMagazine}</color> <color=#FF3F00>/</color> <color={(InPlayer < MaxInMagazine ? "red" : "#51FF00")}>{InPlayer}</color>";
        }
    }

    [Command]
    public void CmdReload()
    {
        if(InPlayer > 0)
        {
            if (InMagazine > 0)
            {
                GameObject gb = Instantiate(AmmoBoxPrefab, transform.position, Quaternion.Euler(0, 0, 0));
                NetworkServer.Spawn(gb);
                gb.GetComponent<AmmoBox>().InMagazine = InMagazine;
            }

            int min = InPlayer < MaxInMagazine ? InPlayer : MaxInMagazine;
            InPlayer -= min;
            InMagazine = min;
        }
    }

    [Command]
    public void CmdPickupAmmoBox(GameObject gbAmmoBox)
    {
        if((transform.position - gbAmmoBox.transform.position).magnitude < 3f)
        {
            InPlayer += gbAmmoBox.GetComponent<AmmoBox>().InMagazine;
            NetworkServer.UnSpawn(gbAmmoBox);
            NetworkServer.Destroy(gbAmmoBox);
        }
    }
}