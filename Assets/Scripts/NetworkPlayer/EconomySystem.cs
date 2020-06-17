using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncWeapons : SyncList<Weapon> { }

public class EconomySystem : NetworkBehaviour
{
    public PlayerStats playerStats;
    public AmmoController ammoController;

    public SyncWeapons PlayerWeapons;


    [Command]
    public void CmdBuyWeapon(Weapon w)
    {
        int weaponsC = PlayerWeapons.Count;
        int Wcost = WeaponStats.GetWeaponCost(w);

        if (weaponsC < 3 && !WeaponAlready(w) && playerStats.Money >= Wcost)
        {
            bool isK = CheckIfKnife();

            int weaponsCWK = weaponsC - (isK ? 1 : 0);

            if (isK)
            {
                if(weaponsCWK < 2)
                {
                    playerStats.Money -= Wcost;
                    PlayerWeapons.Add(w);
                }
            }
            else
            {
                if(weaponsC < 2 || w == Weapon.Knife)
                {
                    playerStats.Money -= Wcost;
                    PlayerWeapons.Add(w);
                }
            }
        }

        AmmoType at = WeaponStats.GetAmmoType(w);
        ammoController.SetAmmoInCurrentMagazine(WeaponStats.GetMaxMagazineSize(at), at);
    }

    [Command]
    public void CmdPickupWeapon(GameObject DroppedGun)
	{
        DroppedWeapon dw = DroppedGun.GetComponent<DroppedWeapon>();

        if(dw != null && (transform.position - DroppedGun.transform.position).sqrMagnitude < 3 * 3)
		{
            Weapon w = dw.WeaponType;

            int weaponsC = PlayerWeapons.Count;
            if (weaponsC < 3 && !WeaponAlready(w))
            {
                bool isK = CheckIfKnife();

                int weaponsCWK = weaponsC - (isK ? 1 : 0);

                if (isK)
                {
                    if (weaponsCWK < 2)
                    {
                        PlayerWeapons.Add(w);
                    }
                }
                else
                {
                    if (weaponsC < 2 || w == Weapon.Knife)
                    {
                        PlayerWeapons.Add(w);
                    }
                }
            }

            AmmoType at = WeaponStats.GetAmmoType(w);
            ammoController.SetAmmoInCurrentMagazine(dw.InMagazine, at);

            NetworkServer.UnSpawn(DroppedGun);
            NetworkServer.Destroy(DroppedGun);
        }
    }

    [Command]
    public void CmdBuyAmmo(AmmoType at)
    {
        int atCost = WeaponStats.GetAmmoCost(at);

        if(playerStats.Money >= atCost && ammoController.GetAmmoMagazines(at).Count < ammoController.MaxMagazinesInPlayer)
		{
            ammoController.ServerSetAmmo(at, WeaponStats.GetMaxMagazineSize(at));
            playerStats.Money -= atCost;
		}
    }

    bool CheckIfKnife()
    {
        bool haveknife = false;
        foreach(Weapon w in PlayerWeapons)
        {
            if(w == Weapon.Knife)
            {
                haveknife = true;
            }
        }

        return haveknife;
    }

    bool WeaponAlready(Weapon ww)
    {
        bool havew = false;
        foreach (Weapon w in PlayerWeapons)
        {
            if (w == ww)
            {
                havew = true;
            }
        }

        return havew;
    }
}