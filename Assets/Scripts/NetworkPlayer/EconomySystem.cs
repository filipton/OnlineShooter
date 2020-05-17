using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncWeapons : SyncList<Weapon> { }

public class EconomySystem : NetworkBehaviour
{
    public PlayerStats playerStats;

    public SyncWeapons PlayerWeapons;

    void Start()
    {
        
    }

    void Update()
    {
        if (isLocalPlayer)
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                CmdBuyWeapon((Weapon)Random.Range(0, 3));
            }
        }
    }

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