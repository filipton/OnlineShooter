using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public struct AmmoMagazine
{
    public int InMagazine;

    public AmmoMagazine(int AmmoAmount)
    {
        InMagazine = AmmoAmount;
    }
}

public class SyncAmmoMagazines : SyncList<AmmoMagazine> { }

public class AmmoController : NetworkBehaviour
{
    [SyncVar]
    public int MaxInMagazine = 30;
    [SyncVar]
    public int MaxMagazinesInPlayer = 5;

    [SyncVar]
    public int InPlayer = 0;

    [SyncVar]
    public int CurrentInMagazine;

    public SyncAmmoMagazines AmmoMagazines = new SyncAmmoMagazines();

    public GameObject AmmoBoxPrefab;

    [ServerCallback]
    private void Start()
    {
        CurrentInMagazine = MaxInMagazine;
        for(int i = 0; i < MaxMagazinesInPlayer; i++)
        {
            AmmoMagazines.Add(new AmmoMagazine(MaxInMagazine));
        }

        RefreshAllInPlayerAmmo();
    }

    private void Update()
    {
        if (isLocalPlayer)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                CmdReload();
            }
            LocalSceneObjects.singleton.AmmoText.text = $"<color={(CurrentInMagazine < 6 ? "red" : "#51FF00")}>{CurrentInMagazine}</color> <color=#FF3F00>/</color> <color={(InPlayer < MaxInMagazine ? "red" : "#51FF00")}>{InPlayer}</color>";
        }
    }

    [Command]
    public void CmdReload()
    {
        if(InPlayer > 0)
        {
            if (CurrentInMagazine > 0)
            {
                GameObject gb = Instantiate(AmmoBoxPrefab, transform.position, Quaternion.Euler(0, 0, 0));
                NetworkServer.Spawn(gb);
                gb.GetComponent<AmmoBox>().InMagazine = CurrentInMagazine;
            }

            CurrentInMagazine = AmmoMagazines[0].InMagazine;
            AmmoMagazines.RemoveAt(0);
            RefreshAllInPlayerAmmo();
        }
    }

    [ServerCallback]
    public void RefreshAllInPlayerAmmo()
    {
        int inPlayer = 0;
        foreach (AmmoMagazine am in AmmoMagazines)
        {
            inPlayer += am.InMagazine;
        }
        InPlayer = inPlayer;
    }

    [Command]
    public void CmdPickupAmmoBox(GameObject gbAmmoBox)
    {
        if((transform.position - gbAmmoBox.transform.position).magnitude < 3f && AmmoMagazines.Count < MaxMagazinesInPlayer)
        {
            AmmoMagazines.Add(new AmmoMagazine(gbAmmoBox.GetComponent<AmmoBox>().InMagazine));
            NetworkServer.UnSpawn(gbAmmoBox);
            NetworkServer.Destroy(gbAmmoBox);
            RefreshAllInPlayerAmmo();
        }
    }
}