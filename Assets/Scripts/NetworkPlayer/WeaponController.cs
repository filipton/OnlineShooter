using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Weapon
{
    Knife,
    Defender,
    Breaker
}

public enum AmmoType
{
    Light,
    Heavy,
    Radioactive
}

public class WeaponController : NetworkBehaviour
{
    [SyncVar]
    public Weapon CurrentWeapon;

    [SyncVar]
    public AmmoType CurrentAmmoType;

    [SyncVar]
    public int CurrentSelectedWeaponIndex = 0;

    public Material[] Mats = new Material[2];
    public GameObject[] Weapons = new GameObject[2];

    public EconomySystem economySystem;
    public OnlineShooting onlineShooting;
    public AmmoController ammoController;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        if (isLocalPlayer)
        {
            int y = (int)(-Input.mouseScrollDelta.y);

            if(y != 0)
            {
                CmdChangeWeapon(y);
            }
        }
    }

    [Command]
    public void CmdChangeWeapon(int currWeapon)
    {
        CurrentSelectedWeaponIndex -= currWeapon;
        CurrentSelectedWeaponIndex = Mathf.Clamp(CurrentSelectedWeaponIndex, 0, 2);
        Weapon w = (Weapon)CurrentSelectedWeaponIndex;
        CurrentWeapon = w;
        CurrentAmmoType = WeaponStats.GetAmmoType(w);
        ServerChangeWeapon(w, WeaponStats.GetAmmoType(w));
        ammoController.RefreshCurrentAmmoInMagazine();
        RpcChangeWeaponColor(CurrentSelectedWeaponIndex);
    }

    [ServerCallback]
    void ServerChangeWeapon(Weapon w, AmmoType at)
    {       
        onlineShooting.ShootRate = WeaponStats.GetFireRate(w);
        onlineShooting.DamageMultiplier = WeaponStats.GetDamgeMultiplier(w);
        ammoController.MaxInMagazine = WeaponStats.GetMaxMagazineSize(at);

        ammoController.RefreshAllInPlayerAmmo();
    }

    [ClientRpc]
    public void RpcChangeWeaponColor(int ind)
    {
        Material m = Mats[0];
        Weapon currW = economySystem.PlayerWeapons[ind];
        switch (currW)
        {
            case Weapon.Defender:
                m = Mats[0];
                break;
            case Weapon.Breaker:
                m = Mats[1];
                break;
            case Weapon.Knife:
                m = null;
                break;

        }
        foreach(GameObject gb in Weapons)
        {
            foreach (MeshRenderer mesh in gb.GetComponentsInChildren<MeshRenderer>())
            {
                mesh.material = m;
            }
        }
    }

    private void OnValidate()
    {
        if (isClient)
        {
            CurrentAmmoType = WeaponStats.GetAmmoType(CurrentWeapon);
            CmdChangeWeapon((int)CurrentWeapon);
        }
    }
}

public class WeaponStats
{
    public static float GetFireRate(Weapon w)
    {
        switch (w)
        {
            case Weapon.Defender:
                return 0.15f;
            case Weapon.Breaker:
                return 0.1f;
        }

        return 0;
    }

    public static int GetMaxMagazineSize(AmmoType at)
    {
        switch (at)
        {
            case AmmoType.Heavy:
                return 20;
            case AmmoType.Light:
                return 30;
            case AmmoType.Radioactive:
                return 10;
        }

        return 0;
    }

    public static float GetDamgeMultiplier(Weapon w)
    {
        switch (w)
        {
            case Weapon.Defender:
                return 1;
            case Weapon.Breaker:
                return 0.85f;
        }

        return 0;
    }

    public static int GetWeaponCost(Weapon w)
    {
        switch (w)
        {
            case Weapon.Defender:
                return 3200;
            case Weapon.Breaker:
                return 2900;
            case Weapon.Knife:
                return 100;
        }

        return 0;
    }

    public static AmmoType GetAmmoType(Weapon w)
    {
        switch (w)
        {
            case Weapon.Defender:
                return AmmoType.Heavy;
            case Weapon.Breaker:
                return AmmoType.Light;
        }

        return 0;
    }
}