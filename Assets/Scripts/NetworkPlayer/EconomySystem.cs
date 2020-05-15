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
        if (weaponsC < 3 && !WeaponAlready(w))
        {
            bool isK = CheckIfKnife();

            int weaponsCWK = weaponsC - (isK ? 1 : 0);

            if (isK)
            {
                if(weaponsCWK < 2)
                {
                    PlayerWeapons.Add(w);
                    print($"{w} BUGHT! 1");
                }
            }
            else
            {
                if(weaponsC < 2 || w == Weapon.Knfie)
                {
                    PlayerWeapons.Add(w);
                    print($"{w} BUGHT! 2");
                }
            }
        }
    }

    bool CheckIfKnife()
    {
        bool haveknife = false;
        foreach(Weapon w in PlayerWeapons)
        {
            if(w == Weapon.Knfie)
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