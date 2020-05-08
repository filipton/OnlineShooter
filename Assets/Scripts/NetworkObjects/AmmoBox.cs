using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBox : MonoBehaviour
{
    public int InMagazine = 30;
    public AmmoController C;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            C.CmdPickupAmmoBox();
        }
    }
}