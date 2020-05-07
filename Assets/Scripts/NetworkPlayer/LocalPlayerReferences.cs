using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class LocalPlayerReferences : NetworkBehaviour
{
    public List<Behaviour> components = new List<Behaviour>();
    public GameObject PlayerModel;
    public GameObject PlayerFPC_Ak;

    void Start()
    {
        if (!isLocalPlayer)
        {
            foreach(Behaviour beh in components)
            {
                beh.enabled = false;
            }
        }
        else
        {
            PlayerModel.SetActive(false);
            PlayerFPC_Ak.SetActive(true);
        }
    }
}