using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBox : NetworkBehaviour
{
    [SyncVar]
    public int InMagazine = 30;
}