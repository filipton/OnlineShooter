using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking.Types;

public class PlayerHealth : NetworkBehaviour
{
    [SyncVar]
    public int Health = 100;
    [SyncVar]
    public bool PlayerKilled;

    public bool KillPlayer;

    public GameObject KSphere;
    public GameObject Capsule;
    public GameObject Container;

    private void Update()
    {
        if (KillPlayer)
        {
            KillPlayer = false;
            if (isServer)
                RpcKillPlayer();
        }
    }

    public void CmdRemoveHealth(int amount)
    {
        Health -= amount;

        if(Health <= 0 && !PlayerKilled)
        {
            GetComponent<OnlineShooting>().HitBoxes.ToList().ForEach(delegate (HitBox hb)
            {
                hb.GetComponent<Collider>().enabled = false;
            });
            RpcKillPlayer();
        }
    }

    [ClientRpc]
    public void RpcKillPlayer()
    {
        Capsule.SetActive(false);
        for (int i = 0; i < 100; i++)
        {
            PlayerKilled = true;
            GameObject g = Instantiate(KSphere, Vector3.zero, new Quaternion(), Container.transform);
            g.transform.localPosition = new Vector3(Random.Range(-0.35f, 0.35f), Random.Range(-0.8f, 0.85f), Random.Range(-0.35f, 0.35f));
            g.transform.parent = null;
            Destroy(g, 10);

            GetComponent<PlayerMovement>().enabled = false;
        }
    }
}