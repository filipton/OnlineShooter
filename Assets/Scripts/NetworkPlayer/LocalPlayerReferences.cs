using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class LocalPlayerReferences : NetworkBehaviour
{
    public List<Behaviour> components = new List<Behaviour>();

    void Start()
    {
        if (!isLocalPlayer)
        {
            foreach(Behaviour beh in components)
            {
                beh.enabled = false;
            }
        }
    }
}