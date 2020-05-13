using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Weapon
{
    Defender,
    Breaker
}

public class WeaponController : NetworkBehaviour
{
    [SyncVar]
    public Weapon CurrentWeapon;

    public OnlineShooting onlineShooting;
    public AmmoController ammoController;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Command]
    public void CmdChangeWeapon(Weapon w)
    {
        ServerChangeWeapon(w);
    }

    void ServerChangeWeapon(Weapon w)
    {
        onlineShooting.ShootRate = WeaponStats.GetFireRate(w);
        onlineShooting.DamageMultiplier = WeaponStats.GetDamgeMultiplier(w);
        ammoController.MaxInMagazine = WeaponStats.GetMaxMagazineSize(w);
    }

    private void OnValidate()
    {
        CmdChangeWeapon(CurrentWeapon);
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

    public static int GetMaxMagazineSize(Weapon w)
    {
        switch (w)
        {
            case Weapon.Defender:
                return 20;
            case Weapon.Breaker:
                return 30;
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
}