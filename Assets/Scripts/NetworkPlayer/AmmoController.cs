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

public class AmmoController : NetworkBehaviour
{
    [SyncVar] public int MaxInMagazine = 30;

    [SyncVar] public int InPlayer = 0;
    int m_inPlayer = 0;
    int m_inMagazine = 0;

    [SyncVar] public int CurrentInMagazine;

    public WeaponController weaponController;

    [Header("AmmoMagazines")]
    [SyncVar] public short HeavyAmmoInPlayer; 
    [SyncVar] public short LightAmmoInPlayer; 
    [SyncVar] public short RadioactiveAmmoInPlayer; 
    [SyncVar] public short KnifeAmmoInPlayer;

    public GameObject AmmoBoxPrefab;
    public GameObject DroppedWeaponPrefab;


    private void Update()
    {
        if (isLocalPlayer)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                CmdReload();
            }
			if (Input.GetKeyDown(KeyCode.G))
			{
                CmdDropWeapon();
			}

            if (m_inPlayer != InPlayer || m_inMagazine != CurrentInMagazine)
            {
                LocalSceneObjects.singleton.AmmoText.text = $"<color={(CurrentInMagazine < 6 ? "red" : "#51FF00")}>{CurrentInMagazine}</color> {(InPlayer > -1 ? $"<color=#FF3F00>/</color> <color={(InPlayer < MaxInMagazine ? "red" : "#51FF00")}>{InPlayer}</color>" : "")}";
                m_inPlayer = InPlayer;
                m_inMagazine = CurrentInMagazine;
            }
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
                AmmoBox ab = gb.GetComponent<AmmoBox>();
                ab.InMagazine = CurrentInMagazine;
                ab.MagazineAmmoType = weaponController.CurrentAmmoType;
            }
            int toAdd = Mathf.Clamp(GetCurrentAmmoInPlayer(), 0, WeaponStats.GetMaxMagazineSize(weaponController.CurrentAmmoType));

            weaponController.economySystem.ServerSetWeaponAmmo(weaponController.CurrentSelectedWeaponIndex, toAdd);
            ServerAddAmmo(weaponController.CurrentAmmoType, -toAdd);
        }
    }

    [Command]
    public void CmdDropWeapon()
    {
        if(weaponController.economySystem.PlayerWeapons.Count > 0)
		{
            GameObject gb = Instantiate(DroppedWeaponPrefab, transform.position, Quaternion.Euler(0, 0, 0));
            DroppedWeapon ab = gb.GetComponent<DroppedWeapon>();
            ab.InMagazine = CurrentInMagazine;
            ab.WeaponType = weaponController.CurrentWeapon;
            NetworkServer.Spawn(gb);

            int ind = weaponController.economySystem.PlayerWeapons.FindIndex(x => x.weapon == weaponController.CurrentWeapon);

            weaponController.economySystem.PlayerWeapons.RemoveAt(ind);
            ServerSetAmmo(weaponController.CurrentAmmoType, 0);
        }
    }

    [ServerCallback]
    public void RefreshAllInPlayerAmmo()
    {
        InPlayer = GetCurrentAmmoInPlayer();
    }

    [ServerCallback]
    public void RefreshCurrentAmmoInMagazine()
    {
        CurrentInMagazine = weaponController.economySystem.ServerGetWeaponAmmo(weaponController.CurrentSelectedWeaponIndex);
    }

    [Command]
    public void CmdPickupAmmoBox(GameObject gbAmmoBox)
    {
        AmmoBox ab = gbAmmoBox.GetComponent<AmmoBox>();
        if(ab != null && (transform.position - gbAmmoBox.transform.position).sqrMagnitude < 3*3 && GetCurrentAmmoInPlayer() < WeaponStats.GetMaxInPlayer(weaponController.CurrentAmmoType))
        {
            int max = WeaponStats.GetMaxInPlayer(ab.MagazineAmmoType);

            int toAddToPlayer = Mathf.Clamp(ab.InMagazine, 0, max);
            int toSetInMagazine = ab.InMagazine - toAddToPlayer;
            ServerAddAmmo(ab.MagazineAmmoType, toAddToPlayer);

            if(toSetInMagazine > 0)
			{
                ab.InMagazine = toSetInMagazine;
			}
			else
			{
                NetworkServer.UnSpawn(gbAmmoBox);
                NetworkServer.Destroy(gbAmmoBox);
            }
            RefreshAllInPlayerAmmo();
        }
    }


    [ServerCallback]
    public void ServerSetAmmo(AmmoType at, int amount)
    {
        if (amount <= WeaponStats.GetMaxInPlayer(at))
		{
            if (amount > WeaponStats.GetMaxInPlayer(at)) amount = WeaponStats.GetMaxInPlayer(at);

            switch (at)
            {
                case AmmoType.Heavy:
                    HeavyAmmoInPlayer = (short)amount;
                    break;
                case AmmoType.Light:
                    LightAmmoInPlayer = (short)amount;
                    break;
                case AmmoType.Radioactive:
                    RadioactiveAmmoInPlayer = (short)amount;
                    break;
                case AmmoType.KnifeAmmo:
                    KnifeAmmoInPlayer = (short)amount;
                    break;
            }

            RefreshCurrentAmmoInMagazine();
            RefreshAllInPlayerAmmo();
        }
    }

    [ServerCallback]
    public void ServerAddAmmo(AmmoType at, int amount)
    {
        ServerSetAmmo(at, GetAmmoAmount(at) + amount);
    }

    public int GetAmmoAmount(AmmoType at)
    {
        switch (at)
        {
            case AmmoType.Heavy:
                return HeavyAmmoInPlayer;
            case AmmoType.Light:
                return LightAmmoInPlayer;
            case AmmoType.Radioactive:
                return RadioactiveAmmoInPlayer;
            case AmmoType.KnifeAmmo:
                return KnifeAmmoInPlayer;
        }

        return 0;
    }

    public int GetCurrentAmmoInPlayer()
    {
        AmmoType at = weaponController.CurrentAmmoType;
        int am = 0;
        switch (at)
        {
            case AmmoType.Heavy:
                am = HeavyAmmoInPlayer;
                break;
            case AmmoType.Light:
                am = LightAmmoInPlayer;
                break;
            case AmmoType.Radioactive:
                am = RadioactiveAmmoInPlayer;
                break;
            case AmmoType.KnifeAmmo:
                am = KnifeAmmoInPlayer;
                break;
        }

        return am;
    }
}