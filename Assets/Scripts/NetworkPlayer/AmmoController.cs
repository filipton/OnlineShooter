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
                RpcSpawnAmmoBox(transform.position + transform.up, InMagazine);

                //spawn in server
                if (isServerOnly)
                {
                    GameObject gb = Instantiate(AmmoBoxPrefab);
                    AmmoBoxPrefab.transform.position = transform.position + transform.up;
                    gb.GetComponent<AmmoBox>().InMagazine = InMagazine;
                    Destroy(gb, 60);
                }
            }

            int min = InPlayer < MaxInMagazine ? InPlayer : MaxInMagazine;
            InPlayer -= min;
            InMagazine = min;
        }
    }

    [ClientRpc]
    public void RpcSpawnAmmoBox(Vector3 position, int inM)
    {
        GameObject gb = Instantiate(AmmoBoxPrefab);
        AmmoBoxPrefab.transform.position = position;
        gb.GetComponent<AmmoBox>().InMagazine = inM;
        gb.GetComponent<AmmoBox>().C = this;
        Destroy(gb, 60);
    }

    [Command]
    public void CmdPickupAmmoBox()
    {
        foreach(AmmoBox ab in FindObjectsOfType<AmmoBox>())
        {
            if((this.transform.position - ab.transform.position).magnitude < 3f)
            {
                InPlayer += ab.InMagazine;
                Destroy(ab.gameObject);
                break;
            }
        }
    }
}