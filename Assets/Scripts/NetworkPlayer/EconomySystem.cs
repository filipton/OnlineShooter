using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct MWeapon
{
    public Weapon weapon;
    public int AmmoInMagazine;

    public MWeapon(Weapon w, int aim)
    {
        weapon = w;
        AmmoInMagazine = aim;
    }
}

public class SyncWeapons : SyncList<MWeapon> { }

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
        AmmoType at = WeaponStats.GetAmmoType(w);

        if (weaponsC < 3 && !WeaponAlready(w) && playerStats.Money >= Wcost)
        {
            bool isK = CheckIfKnife();

            int weaponsCWK = weaponsC - (isK ? 1 : 0);

            if (isK)
            {
                if(weaponsCWK < 2)
                {
                    playerStats.Money -= Wcost;
                    PlayerWeapons.Add(new MWeapon(w, WeaponStats.GetMaxMagazineSize(at)));
                }
            }
            else
            {
                if(weaponsC < 2 || w == Weapon.Knife)
                {
                    playerStats.Money -= Wcost;
                    PlayerWeapons.Add(new MWeapon(w, WeaponStats.GetMaxMagazineSize(at)));
                }
            }
        }
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
                        PlayerWeapons.Add(new MWeapon(w, dw.InMagazine));

                        NetworkServer.UnSpawn(DroppedGun);
                        NetworkServer.Destroy(DroppedGun);
                    }
                }
                else
                {
                    if (weaponsC < 2 || w == Weapon.Knife)
                    {
                        PlayerWeapons.Add(new MWeapon(w, dw.InMagazine));

                        NetworkServer.UnSpawn(DroppedGun);
                        NetworkServer.Destroy(DroppedGun);
                    }
                }
            }
        }
    }

    [Command]
    public void CmdBuyAmmo(AmmoType at)
    {
        int atCost = WeaponStats.GetAmmoCost(at);

        if(playerStats.Money >= atCost && ammoController.GetAmmoAmount(at) < WeaponStats.GetMaxInPlayer(at))
		{
            ammoController.ServerAddAmmo(at, WeaponStats.GetMaxMagazineSize(at));
            playerStats.Money -= atCost;
		}
    }

    [ServerCallback]
    public int ServerGetWeaponAmmo(int index)
	{
        if(index > -1 && PlayerWeapons.Count >= 0 && index < PlayerWeapons.Count)
		{
            return PlayerWeapons[index].AmmoInMagazine;
        }
        return 0;
	}

    [ServerCallback]
    public void ServerSetWeaponAmmo(int index, int amount)
    {
        if (index > -1 && PlayerWeapons.Count >= 0 && index < PlayerWeapons.Count)
        {
            MWeapon mw = PlayerWeapons[index];
            PlayerWeapons[index] = new MWeapon(mw.weapon, amount);
        }
    }

    [ServerCallback]
    public void ServerAddWeaponAmmo(int index, int amount)
    {
        if(PlayerWeapons.Count >= 0 && index < PlayerWeapons.Count)
		{
            ServerSetWeaponAmmo(index, PlayerWeapons[index].AmmoInMagazine + amount);
        }
    }

    bool CheckIfKnife()
    {
        bool haveknife = false;
        foreach(MWeapon w in PlayerWeapons)
        {
            if(w.weapon == Weapon.Knife)
            {
                haveknife = true;
            }
        }

        return haveknife;
    }

    bool WeaponAlready(Weapon ww)
    {
        bool havew = false;
        foreach (MWeapon w in PlayerWeapons)
        {
            if (w.weapon == ww)
            {
                havew = true;
            }
        }

        return havew;
    }
}