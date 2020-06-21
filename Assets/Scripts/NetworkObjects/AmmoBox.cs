using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AmmoBox : NetworkBehaviour
{
    [SyncVar]
    public int InMagazine = 30;
    [SyncVar]
    public AmmoType MagazineAmmoType;

    public AmmoController C;
    public PlayerHealth ph;

    private void Start()
    {
        C = FindObjectsOfType<AmmoController>().ToList().Find(x => x.isLocalPlayer);
        if (C != null)
            ph = C.GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if(C == null) C = FindObjectsOfType<AmmoController>().ToList().Find(x => x.isLocalPlayer);
            if(ph == null) ph = C.GetComponent<PlayerHealth>();

            if (!ph.PlayerKilled && (transform.position - C.transform.position).sqrMagnitude < 3*3)
            {
                C.CmdPickupAmmoBox(gameObject);
            }
        }
    }
}