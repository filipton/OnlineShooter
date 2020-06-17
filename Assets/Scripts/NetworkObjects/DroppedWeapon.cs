using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DroppedWeapon : NetworkBehaviour
{
    [SyncVar]
    public int InMagazine = 0;
    [SyncVar]
    public Weapon WeaponType;

    public GameObject[] weapons;

    public EconomySystem E;

    private void Start()
    {
        weapons[(int)WeaponType].SetActive(true);

        E = FindObjectsOfType<EconomySystem>().ToList().Find(x => x.isLocalPlayer);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (E == null)
                E = FindObjectsOfType<EconomySystem>().ToList().Find(x => x.isLocalPlayer);

            if ((transform.position - E.transform.position).sqrMagnitude < 3*3)
            {
                E.CmdPickupWeapon(gameObject);
            }
        }
    }
}