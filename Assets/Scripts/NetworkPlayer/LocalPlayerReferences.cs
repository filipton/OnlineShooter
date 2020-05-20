using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Tayx.Graphy;
using UnityEngine;

public class LocalPlayerReferences : NetworkBehaviour
{
    public LocalPlayerReferences singleton;

    [Header("LocalPlayerRefs")]
    public AmmoController ammoController;
    public EconomySystem economySystem;
    public OnlineShooting onlineShooting;
    public PlayerHealth playerHealth;
    public PlayerList playerList;
    public PlayerStats playerStats;
    public WeaponController weaponController;

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

            singleton = this;
        }
    }
}