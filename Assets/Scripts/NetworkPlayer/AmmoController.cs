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
    [SyncVar] public int MaxInMagazine = 30;
    [SyncVar] public int MaxMagazinesInPlayer = 5;

    [SyncVar] public int InPlayer = 0;
    int m_inPlayer = 0;
    int m_inMagazine = 0;

    [SyncVar] public int CurrentInMagazine;

    public WeaponController weaponController;

    public SyncAmmoMagazines HeavyAmmoMagazines = new SyncAmmoMagazines();
    public SyncAmmoMagazines LightAmmoMagazines = new SyncAmmoMagazines();
    public SyncAmmoMagazines RadioactiveAmmoMagazines = new SyncAmmoMagazines();

    [Header("CurrentMagazine Of Ammo Types")]
    [SyncVar] public AmmoMagazine CurrentKnifeDurability = new AmmoMagazine();
    [SyncVar] public AmmoMagazine CurrentHeavyAmmoMagazine = new AmmoMagazine();
    [SyncVar] public AmmoMagazine CurrentLightAmmoMagazine = new AmmoMagazine();
    [SyncVar] public AmmoMagazine CurrentRadiactiveAmmoMagazine = new AmmoMagazine();

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
                ab.MagazineAmmoType= weaponController.CurrentAmmoType;
            }

            SetAmmoInCurrentMagazine(GetAmmoMagazines(weaponController.CurrentAmmoType)[0].InMagazine);
            GetAmmoMagazines(weaponController.CurrentAmmoType).RemoveAt(0);
            RefreshAllInPlayerAmmo();
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

            weaponController.economySystem.PlayerWeapons.Remove(weaponController.CurrentWeapon);
            RemoveAmmoInCurrentMagazine(GetCurrentAmmoInMagazine());

            RefreshAllInPlayerAmmo();
        }
    }

    [ServerCallback]
    public void RefreshAllInPlayerAmmo()
    {
        int inPlayer = 0;
        SyncAmmoMagazines sam = GetAmmoMagazines(weaponController.CurrentAmmoType);
        if(sam != null)
        {
            foreach (AmmoMagazine am in sam)
            {
                inPlayer += am.InMagazine;
            }
        }

        if (sam == null || weaponController.CurrentAmmoType == AmmoType.KnifeAmmo)
        {
            inPlayer = -1;
        }
        InPlayer = inPlayer;
    }

    [ServerCallback]
    public void RefreshCurrentAmmoInMagazine()
    {
        CurrentInMagazine = GetCurrentAmmoInMagazine();
    }

    [Command]
    public void CmdPickupAmmoBox(GameObject gbAmmoBox)
    {
        AmmoBox ab = gbAmmoBox.GetComponent<AmmoBox>();
        if(ab != null && (transform.position - gbAmmoBox.transform.position).sqrMagnitude < 3*3 && GetAmmoMagazines(ab.MagazineAmmoType).Count < MaxMagazinesInPlayer)
        {
            GetAmmoMagazines(ab.MagazineAmmoType).Add(new AmmoMagazine(ab.InMagazine));
            NetworkServer.UnSpawn(gbAmmoBox);
            NetworkServer.Destroy(gbAmmoBox);
            RefreshAllInPlayerAmmo();
        }
    }

    public SyncAmmoMagazines GetAmmoMagazines(AmmoType at)
    {
        return at.Equals(AmmoType.Heavy) ? HeavyAmmoMagazines : (at.Equals(AmmoType.Light) ? LightAmmoMagazines : RadioactiveAmmoMagazines);
    }


    [ServerCallback]
    public void ServerSetAmmo(AmmoType at, int amount)
    {
        if (GetAmmoMagazines(at).Count < MaxMagazinesInPlayer)
		{
            amount = Mathf.Clamp(amount, 0, WeaponStats.GetMaxMagazineSize(at));

            switch (at)
            {
                case AmmoType.Heavy:
                    HeavyAmmoMagazines.Add(new AmmoMagazine(amount));
                    break;
                case AmmoType.Light:
                    LightAmmoMagazines.Add(new AmmoMagazine(amount));
                    break;
                case AmmoType.Radioactive:
                    RadioactiveAmmoMagazines.Add(new AmmoMagazine(amount));
                    break;
                case AmmoType.KnifeAmmo:
                    CurrentKnifeDurability.InMagazine = amount;
                    break;
            }

            RefreshCurrentAmmoInMagazine();
            RefreshAllInPlayerAmmo();
        }
    }

    [ServerCallback]
    public void RemoveAmmoInCurrentMagazine(int amount)
    {
        AmmoType at = weaponController.CurrentAmmoType;
        switch (at)
        {
            case AmmoType.Heavy:
                CurrentHeavyAmmoMagazine.InMagazine -= amount;
                break;
            case AmmoType.Light:
                CurrentLightAmmoMagazine.InMagazine -= amount;
                break;
            case AmmoType.Radioactive:
                CurrentRadiactiveAmmoMagazine.InMagazine -= amount;
                break;
            case AmmoType.KnifeAmmo:
                CurrentKnifeDurability.InMagazine -= amount;
                break;
        }

        RefreshCurrentAmmoInMagazine();
    }

    [ServerCallback]
    public void SetAmmoInCurrentMagazine(int amount, Nullable<AmmoType> type = null)
    {
        AmmoType at = weaponController.CurrentAmmoType;

        if (type != null)
            at = type.Value;

        switch (at)
        {
            case AmmoType.Heavy:
                CurrentHeavyAmmoMagazine.InMagazine = amount;
                break;
            case AmmoType.Light:
                CurrentLightAmmoMagazine.InMagazine = amount;
                break;
            case AmmoType.Radioactive:
                CurrentRadiactiveAmmoMagazine.InMagazine = amount;
                break;
            case AmmoType.KnifeAmmo:
                CurrentKnifeDurability.InMagazine = amount;
                break;
        }

        RefreshCurrentAmmoInMagazine();
    }

    public int GetCurrentAmmoInMagazine()
    {
        AmmoType at = weaponController.CurrentAmmoType;
        int am = 0;
        switch (at)
        {
            case AmmoType.Heavy:
                am = CurrentHeavyAmmoMagazine.InMagazine;
                break;
            case AmmoType.Light:
                am = CurrentLightAmmoMagazine.InMagazine;
                break;
            case AmmoType.Radioactive:
                am = CurrentRadiactiveAmmoMagazine.InMagazine;
                break;
            case AmmoType.KnifeAmmo:
                am = CurrentKnifeDurability.InMagazine;
                break;
        }

        return am;
    }
}