using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking.Types;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[Serializable]
public struct DamageFromPlayer
{
    public string Nick;
    public int Damage;

    public DamageFromPlayer(string nick, int damage)
	{
        Nick = nick;
        Damage = damage;
	}
}

public class PlayerHealth : NetworkBehaviour
{
    [SyncVar]
    public int Health = 100;
    [SyncVar]
    public bool PlayerKilled;

    public List<DamageFromPlayer> damageFromPlayers = new List<DamageFromPlayer>();

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
            }
			else
			{
                if (healthText.text != Health.ToString())
                {
                    healthText.text = Health.ToString();
                }
            }
        }
    }

    public void CmdRemoveHealth(int amount, PlayerStats damagingPlayer)
    {
        Health -= amount;
		if (isServerOnly)
		{
            damageFromPlayers.Add(new DamageFromPlayer(damagingPlayer.Nick, (short)amount));
        }
        RpcAddToDamagesFromPlayers(damagingPlayer.Nick, (short)amount);

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

            RpcKillFeed(ps.Nick, damagingPlayer.Nick, damagingPlayer.GetComponent<WeaponController>().CurrentWeapon);

            ps.Deaths += 1;
            ps.AddMoney(300);

            if (ps != damagingPlayer)
			{
                damagingPlayer.Kills += 1;
                damagingPlayer.AddMoney(1000);
            }

            RoundController.singleton.CheckIfAnyTeamWin();
        }
    }

    [ClientRpc]
    public void RpcKillFeed(string playerKilled, string playerDealtDamage, Weapon w)
    {
        GameObject kf = Instantiate(LocalSceneObjects.singleton.KF_Prefab, LocalSceneObjects.singleton.KF_Parent.transform);
        kf.GetComponentInChildren<TextMeshProUGUI>().text = $"{playerDealtDamage} ︻╦╤─ {playerKilled}";
        Destroy(kf, 5f);

		if (isLocalPlayer)
		{
            foreach (PlayerStats ps in FindObjectsOfType<PlayerStats>())
            {
                if (ps.Nick == playerDealtDamage)
                {
                    int hitsCount = 0;
                    int hitsDamage = 0;

                    List<DamageFromPlayer> dfps = ps.GetComponent<PlayerHealth>().damageFromPlayers;

                    foreach (DamageFromPlayer dfp in dfps)
                    {
                        if (dfp.Nick == playerKilled)
                        {
                            hitsCount++;
                            hitsDamage += dfp.Damage;
                        }
                    }

                    if (hitsCount > 0)
                    {
                        SMessageBox.singleton.ShowMessageBox($"Player {playerDealtDamage} killed you with {w}", $"You dealt him {hitsDamage} hp in {hitsCount} shots.", 2.5f);
                    }
                }
            }
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
            CursorManager.RefreshLock("_pdead", false);

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

            CursorManager.RefreshLock("_pdead", true);
        }
        else
        {
            Capsule.SetActive(true);
        }
    }

    [ClientRpc]
    public void RpcAddToDamagesFromPlayers(string nick, short dmg)
	{
        damageFromPlayers.Add(new DamageFromPlayer(nick, dmg));
	}

    [ClientRpc]
    public void RpcClearDamagesFromPlayers()
    {
        damageFromPlayers.Clear();
    }
}