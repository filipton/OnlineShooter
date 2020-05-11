using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AmmoBox : NetworkBehaviour
{
    [SyncVar]
    public int InMagazine = 30;
    public AmmoController C;

    private void Start()
    {
        C = FindObjectsOfType<AmmoController>().ToList().Find(x => x.isLocalPlayer);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (C == null)
                C = FindObjectsOfType<AmmoController>().ToList().Find(x => x.isLocalPlayer);

            if((transform.position - C.transform.position).magnitude < 3f)
            {
                C.CmdPickupAmmoBox(gameObject);
            }
        }
    }
}