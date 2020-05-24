using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking.Types;
using UnityEngine.SceneManagement;
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
                healthText.text = Health.ToString();
            }
        }
    }

    public void CmdRemoveHealth(int amount, PlayerStats damagingPlayer)
    {
        Health -= amount;

        if(Health <= 0 && !PlayerKilled)
        {
            GetComponent<OnlineShooting>().HitBoxes.ToList().ForEach(delegate (HitBox hb)
            {
                hb.GetComponent<MeshCollider>().enabled = false;
            });
            Health = 0;
            PlayerKilled = true;
            RpcKillPlayer();
            PlayerStats ps = GetComponent<PlayerStats>();

            ps.Deaths += 1;
            ps.AddMoney(300);
            damagingPlayer.Kills += 1;
            damagingPlayer.AddMoney(1000);
            RoundController.singleton.CheckIfTeamAnyWin();

            StartCoroutine(EndRound(5f));
        }
    }

    [ServerCallback]
    IEnumerator EndRound(float time)
    {
        yield return new WaitForSeconds(time);

        foreach(AmmoBox gbAmmoBox in FindObjectsOfType<AmmoBox>())
        {
            NetworkServer.UnSpawn(gbAmmoBox.gameObject);
            NetworkServer.Destroy(gbAmmoBox.gameObject);
        }

        foreach(PlayerStats p in FindObjectsOfType<PlayerStats>())
        {
            NetworkSync ns = p.GetComponent<NetworkSync>();
            PlayerHealth ph = p.GetComponent<PlayerHealth>();

            if (p.PlayerTeam == Team.Team1)
            {
                ns.TpPlayer(LocalSceneObjects.singleton.TeamASpawn.position);
            }
            else if (p.PlayerTeam == Team.Team2)
            {
                ns.TpPlayer(LocalSceneObjects.singleton.TeamBSpawn.position);
            }

            ph.PlayerKilled = false;
            ph.Health = 100;
            ph.GetComponent<OnlineShooting>().HitBoxes.ToList().ForEach(delegate (HitBox hb)
            {
                hb.GetComponent<MeshCollider>().enabled = true;
            });
            ph.RpcRespawnPlayer();
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
                hb.GetComponent<MeshCollider>().enabled = true;
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

            GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSync>().clip("real-death-sound"));
        }
    }

    [ClientRpc]
    public void RpcRespawnPlayer()
    {
        if (isLocalPlayer)
        {
            GetComponent<PlayerMovement>().enabled = true;
            PlayerList pl = GetComponent<PlayerList>();
            pl.CurrentPlayer = pl.players.FindIndex(x => x.Name == GetComponent<PlayerStats>().Nick);
            pl.UpdateCam();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Capsule.SetActive(true);
        }
    }
}