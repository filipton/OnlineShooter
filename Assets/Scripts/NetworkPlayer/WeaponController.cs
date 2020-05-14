using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Weapon
{
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

    public OnlineShooting onlineShooting;
    public AmmoController ammoController;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
        {
            int y = (int)Mathf.Clamp01(CurrentSelectedWeaponIndex - Input.mouseScrollDelta.y);
            CmdChangeWeapon(y);
        }
    }

    [Command]
    public void CmdChangeWeapon(int currWeapon)
    {
        CurrentSelectedWeaponIndex = currWeapon;
        Weapon w = (Weapon)CurrentSelectedWeaponIndex;
        CurrentWeapon = w;
        CurrentAmmoType = WeaponStats.GetAmmoType(w);
        ServerChangeWeapon(w, WeaponStats.GetAmmoType(w));
        ammoController.RefreshCurrentAmmoInMagazine();
    }

    [ServerCallback]
    void ServerChangeWeapon(Weapon w, AmmoType at)
    {       
        onlineShooting.ShootRate = WeaponStats.GetFireRate(w);
        onlineShooting.DamageMultiplier = WeaponStats.GetDamgeMultiplier(w);
        ammoController.MaxInMagazine = WeaponStats.GetMaxMagazineSize(at);

        ammoController.RefreshAllInPlayerAmmo();
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
                return 0.25f;
            case Weapon.Breaker:
                return 0.2f;
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