using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking.Types;
using UnityEngine.UI;

public class PlayerHealth : NetworkBehaviour
{
    [SyncVar]
    public int Health = 100;
    [SyncVar]
    public bool PlayerKilled;

    bool KillPlayer;

    public GameObject KSphere;
    public GameObject Capsule;
    public GameObject Container;

    TextMeshProUGUI healthText;
    Button respawnButton;

    private void Start()
    {
        if(isLocalPlayer)
        {
            respawnButton = GameObject.FindGameObjectWithTag("RespawnButton").GetComponent<Button>();
            respawnButton.onClick.AddListener(OnRespawnButtonClick);
            respawnButton.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (KillPlayer)
        {
            KillPlayer = false;
            if (isServer)
                RpcKillPlayer();
        }

        if (isLocalPlayer)
        {
            if(healthText == null)
            {
                healthText = GameObject.FindGameObjectWithTag("HealthText").GetComponent<TextMeshProUGUI>();
                healthText.text = Health.ToString();
            }

            if(healthText.text != Health.ToString())
            {
                GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSync>().clip("death-sound"));
                healthText.text = Health.ToString();
            }
        }
    }

    public void OnRespawnButtonClick()
    {
        CmdRespawnPlayer();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        respawnButton.gameObject.SetActive(false);
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
            Health = 0;
            PlayerKilled = true;
            RpcKillPlayer();
        }
    }

    [Command]
    public void CmdRespawnPlayer()
    {
        if(Health <= 0)
        {
            PlayerKilled = false;
            Health = 100;
            GetComponent<OnlineShooting>().HitBoxes.ToList().ForEach(delegate (HitBox hb)
            {
                hb.GetComponent<Collider>().enabled = true;
            });
            RpcRespawnPlayer();
        }
    }

    [ClientRpc]
    public void RpcKillPlayer()
    {
        Capsule.SetActive(false);
        for (int i = 0; i < 100; i++)
        {
            GameObject g = Instantiate(KSphere, Vector3.zero, new Quaternion(), Container.transform);
            g.transform.localPosition = new Vector3(Random.Range(-0.35f, 0.35f), Random.Range(-0.8f, 0.85f), Random.Range(-0.35f, 0.35f));
            g.transform.parent = null;
            Destroy(g, 10);
        }
        GetComponent<PlayerMovement>().enabled = false;

        if (isLocalPlayer)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            respawnButton.gameObject.SetActive(true);

            GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSync>().clip("real-death-sound"));
        }
    }

    [ClientRpc]
    public void RpcRespawnPlayer()
    {
        if (isLocalPlayer)
        {
            GetComponent<PlayerMovement>().enabled = true;
        }
        else
        {
            Capsule.SetActive(true);
        }
    }
}