using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Tayx.Graphy;
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
            foreach (Behaviour beh in components)
            {
                beh.enabled = false;
            }
        }
        else
        {
            GraphyManager gm = FindObjectOfType<GraphyManager>();
            if(gm != null)
            {
                gm.AudioListener = GetComponent<AudioListener>();
            }

            PlayerModel.SetActive(false);
            PlayerFPC_Ak.SetActive(true);
        }
    }
}