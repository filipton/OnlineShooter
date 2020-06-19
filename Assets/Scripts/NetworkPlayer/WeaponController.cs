using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Weapon
{
    None = -1,
    Knife,
    Defender,
    Breaker,
    Kernel,
    AWP,
    Shotgun1,
    Shotgun2
}

public enum AmmoType
{
    None = -1,
    Light,
    Heavy,
    Radioactive,
    KnifeAmmo
}

public class WeaponController : NetworkBehaviour
{
    [SyncVar]
    public Weapon CurrentWeapon;

    [SyncVar]
    public AmmoType CurrentAmmoType;

    [SyncVar]
    public int CurrentSelectedWeaponIndex = 0;

    public GameObject[] FPC_WeaponModels;
    public GameObject[] WeaponModels;

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
        if(economySystem.PlayerWeapons.Count > 0)
        {
            CurrentSelectedWeaponIndex -= currWeapon;
            CurrentSelectedWeaponIndex = Mathf.Clamp(CurrentSelectedWeaponIndex, 0, economySystem.PlayerWeapons.Count - 1);
            Weapon w = economySystem.PlayerWeapons[CurrentSelectedWeaponIndex].weapon;
            CurrentWeapon = w;
            CurrentAmmoType = WeaponStats.GetAmmoType(w);
            ServerChangeWeapon(w, WeaponStats.GetAmmoType(w));
            ammoController.RefreshCurrentAmmoInMagazine();
            RpcChangeWeaponModel(CurrentSelectedWeaponIndex);
        }
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
    public void RpcChangeWeaponModel(int ind)
    {
        foreach(GameObject gb in FPC_WeaponModels)
		{
            gb.SetActive(false);
		}
        foreach (GameObject gb in WeaponModels)
        {
            gb.SetActive(false);
        }

        if(ind > -1)
		{
            Weapon currW = economySystem.PlayerWeapons[ind].weapon;
            FPC_WeaponModels[(int)currW].SetActive(true);
            WeaponModels[(int)currW].SetActive(true);
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
            case Weapon.Knife:
                return 0.35f;
            case Weapon.Kernel:
                return 0.25f;
            case Weapon.AWP:
                return 0.95f;
            case Weapon.Shotgun1:
                return 0.416f;
            case Weapon.Shotgun2:
                return 0.585f;
        }

        return 100;
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
            case AmmoType.KnifeAmmo:
                return 25;
        }

        return 0;
    }

    public static int GetAmmoCost(AmmoType at)
    {
        switch (at)
        {
            case AmmoType.Heavy:
                return 300;
            case AmmoType.Light:
                return 350;
            case AmmoType.Radioactive:
                return 400;
            case AmmoType.KnifeAmmo:
                return 50;
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
            case Weapon.Knife:
                return 2;
            case Weapon.Kernel:
                return 0.95f;
            case Weapon.AWP:
                return 4;
            case Weapon.Shotgun1:
                return 1.25f;
            case Weapon.Shotgun2:
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
            case Weapon.Kernel:
                return 400;
            case Weapon.AWP:
                return 4750;
            case Weapon.Shotgun1:
                return 2000;
            case Weapon.Shotgun2:
                return 2250;
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
            case Weapon.Knife:
                return AmmoType.KnifeAmmo;
            case Weapon.Kernel:
                return AmmoType.Light;
            case Weapon.AWP:
                return AmmoType.Heavy;
            case Weapon.Shotgun1:
                return AmmoType.Light;
            case Weapon.Shotgun2:
                return AmmoType.Light;
        }

        return 0;
    }

    public static bool GetWeaponFullAuto(Weapon w)
    {
        switch (w)
        {
            case Weapon.Defender:
                return true;
            case Weapon.Breaker:
                return true;
            case Weapon.Knife:
                return false;
            case Weapon.Kernel:
                return false;
            case Weapon.AWP:
                return false;
            case Weapon.Shotgun1:
                return false;
            case Weapon.Shotgun2:
                return false;
        }

        return false;
    }

    public static float GetMaxDistance(Weapon w)
    {
        switch (w)
        {
            case Weapon.Knife:
                return 2f;
            case Weapon.Shotgun1:
                return 40f;
        }

        return 250;
    }

    public static int GetMaxInPlayer(AmmoType at)
    {
        switch (at)
        {
            case AmmoType.Heavy:
                return 120;
            case AmmoType.Light:
                return 150;
            case AmmoType.Radioactive:
                return 60;
            case AmmoType.KnifeAmmo:
                return 25;
        }

        return 0;
    }
}