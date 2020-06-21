using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

[Serializable]
public struct BTeamsPerms
{
    public Team PlantingTeam;
    public Team DefusingTeam;
}


public class BombSystem : NetworkBehaviour
{
    public BTeamsPerms bTeamsPerms;

    public float Distance = 100f;

    [SyncVar] public float m_plantingTime;
    public float PlantingTime = 3f;

    [SyncVar] public float m_bombExplosionTime;
    public float BombExplosionTime = 30f;

    [SyncVar] public float m_defusingTime;
    public float DefusingTime = 7f;

    [SyncVar] public bool IsPlanting = false;
    [SyncVar] public bool IsExploding = false;
    [SyncVar] public bool IsDefusing = false;

    public GameObject BombPrefab;

    [Header("Sites")]
    public MeshRenderer SiteA;
    public MeshRenderer SiteB;

    GameObject bomb;
    Vector3 bomb_pos;
    NetworkIdentity nid;

    void Update()
    {
        if (isServer)
		{
			if (IsPlanting && !IsExploding)
			{
                if (SiteA.bounds.Contains(nid.transform.position) || SiteB.bounds.Contains(nid.transform.position))
				{
                    float y = nid.transform.position.y - SiteA.transform.position.y;
                    if (y > 0.8f && y < 0.9f)
                    {
                        m_plantingTime += Time.deltaTime;

                        if (m_plantingTime >= PlantingTime)
                        {
                            IsPlanting = false;
                            m_plantingTime = 0f;

                            bomb_pos = nid.transform.position;
                            bomb = Instantiate(BombPrefab);
                            bomb.transform.position = bomb_pos - new Vector3(0, 0.433f, 0);
                            NetworkServer.Spawn(bomb);

                            TargetRpcTogglePlantBombInClient(nid.connectionToClient, false);

                            IsExploding = true;

                            RoundController.singleton.RoundTimeReaming = BombExplosionTime+1f;
                        }
                    }
                }
            }
            else if (IsExploding)
			{
                m_bombExplosionTime += Time.deltaTime;

                if (m_bombExplosionTime >= BombExplosionTime)
				{
                    RoundController.singleton.CheckIfAnyTeamWin();

                    NetworkServer.UnSpawn(bomb);
                    NetworkServer.Destroy(bomb);

                    IsExploding = false;
                    m_bombExplosionTime = 0f;

                    foreach(PlayerHealth ph in FindObjectsOfType<PlayerHealth>())
					{
                        float BombDmg = 200;
                        float dist = (bomb_pos - ph.transform.position).magnitude;

                        BombDmg /= dist / 10;
                        BombDmg *= 5;

                        ph.CmdRemoveHealth((int)BombDmg, ph.GetComponent<PlayerStats>());
                    }
				}
			}
			if (IsDefusing)
			{
                if ((nid.transform.position - bomb.transform.position).magnitude <= 2)
                {
                    float y = nid.transform.position.y - SiteA.transform.position.y;
                    if (y > 0.8f && y < 0.9f)
                    {
                        m_defusingTime += Time.deltaTime;

                        if (m_defusingTime >= DefusingTime)
                        {
                            RoundController.singleton.CheckIfAnyTeamWin();

                            IsDefusing = false;
                            m_defusingTime = 0f;

                            IsExploding = false;
                            m_bombExplosionTime = 0f;

                            NetworkServer.UnSpawn(bomb);
                            NetworkServer.Destroy(bomb);

                            TargetRpcTogglePlantBombInClient(nid.connectionToClient, false);
                        }
                    }
                }
			}
		}
    }

    [ServerCallback]
    public void CmdTogglePlantBomb(bool tb, NetworkIdentity id)
	{
        if ((!IsExploding && !IsPlanting|| !IsExploding && IsPlanting&& !tb) && id.GetComponent<PlayerStats>().PlayerTeam == bTeamsPerms.PlantingTeam && !id.GetComponent<PlayerHealth>().PlayerKilled)
        {
            if (SiteA.bounds.Contains(id.transform.position) || SiteB.bounds.Contains(id.transform.position) && !IsExploding)
            {
                float y = id.transform.position.y - SiteA.transform.position.y;
                if (y > 0.8f && y < 0.9f)
                {
                    nid = id;
                    IsPlanting = tb;
                    m_plantingTime = 0f;

                    TargetRpcTogglePlantBombInClient(id.connectionToClient, tb);
                }
            }
        }
	}

    [ServerCallback]
    public void CmdToggleDefuseBomb(bool tb, NetworkIdentity id)
    {
        if((IsExploding && !IsDefusing || IsExploding && IsDefusing && !tb) && id.GetComponent<PlayerStats>().PlayerTeam == bTeamsPerms.DefusingTeam && !id.GetComponent<PlayerHealth>().PlayerKilled)
		{
            if ((id.transform.position - bomb.transform.position).magnitude <= 2)
            {
                float y = id.transform.position.y - SiteA.transform.position.y;
                if (y > 0.8f && y < 0.9f)
                {
                    nid = id;
                    IsDefusing = tb;
                    m_defusingTime = 0f;

                    TargetRpcTogglePlantBombInClient(id.connectionToClient, tb);
                }
            }
        }
    }

    [TargetRpc]
    public void TargetRpcTogglePlantBombInClient(NetworkConnection conn, bool tb)
	{
        conn.identity.GetComponent<CharacterController>().enabled = !tb;
        conn.identity.GetComponent<PlayerMovement>().enabled = !tb;
    }
}