using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BombSystem : NetworkBehaviour
{
    public float Distance = 100f;

    public float PlantingTime = 3f;
    [SyncVar]
    public float m_plantingTime;

    public float BombExplosionTime = 30f;
    [SyncVar]
    public float m_bombExplosionTime;

    public float DefusingTime = 7f;
    [SyncVar]
    public float m_defusingTime;

    [SyncVar]
    public bool IsPlanting = false;
    [SyncVar]
    public bool IsExploding = false;
    [SyncVar]
    public bool IsDefusing = false;

    Vector3 bomb_pos;
    NetworkIdentity nid;

    void Update()
    {
		if (isServer)
		{
			if (IsPlanting && !IsExploding)
			{
                m_plantingTime += Time.deltaTime;

                if (m_plantingTime >= PlantingTime)
                {
                    IsPlanting = false;
                    m_plantingTime = 0f;

                    bomb_pos = nid.transform.position;

                    if (nid == null)
                    {
                        nid = GetComponent<NetworkIdentity>();
                    }
                    TargetRpcTogglePlantBombInClient(nid.connectionToClient, false);

                    IsExploding = true;
                }
            }
            else if (IsExploding)
			{
                m_bombExplosionTime += Time.deltaTime;

                if(m_bombExplosionTime >= BombExplosionTime)
				{
                    IsExploding = false;
                    m_bombExplosionTime = 0f;

                    foreach(PlayerHealth ph in FindObjectsOfType<PlayerHealth>())
					{
                        float BombDmg = 200;
                        float dist = (bomb_pos - ph.transform.position).magnitude;

                        BombDmg /= dist / 10;
                        BombDmg *= 5;

                        print((int)BombDmg);
                        ph.CmdRemoveHealth((int)BombDmg, ph.GetComponent<PlayerStats>());
                    }
				}
			}
			if (IsDefusing)
			{
                m_defusingTime += Time.deltaTime;

                if(m_defusingTime >= DefusingTime)
				{
                    IsDefusing = false;
                    m_defusingTime = 0f;

                    IsExploding = false;
                    m_bombExplosionTime = 0f;

                    if (nid == null)
                    {
                        nid = GetComponent<NetworkIdentity>();
                    }
                    TargetRpcTogglePlantBombInClient(nid.connectionToClient, false);
                }
			}
		}
        if (isLocalPlayer && isClient)
		{
			if (IsExploding)
			{
                if (Input.GetKeyDown(KeyCode.E)) CmdToggleDefuseBomb(true);
                else if (Input.GetKeyUp(KeyCode.E)) CmdToggleDefuseBomb(false);
            }
			else
			{
                if (Input.GetKeyDown(KeyCode.E)) CmdTogglePlantBomb(true);
                else if (Input.GetKeyUp(KeyCode.E)) CmdTogglePlantBomb(false);
            }
        }
    }

    [Command]
    public void CmdTogglePlantBomb(bool tb)
	{
        IsPlanting = tb;
        m_plantingTime = 0f;

        if (nid == null)
		{
            nid = GetComponent<NetworkIdentity>();
		}

        TargetRpcTogglePlantBombInClient(nid.connectionToClient, tb);
	}

    [Command]
    public void CmdToggleDefuseBomb(bool tb)
    {
        IsDefusing = tb;
        m_defusingTime = 0f;

        if (nid == null)
        {
            nid = GetComponent<NetworkIdentity>();
        }

        TargetRpcTogglePlantBombInClient(nid.connectionToClient, tb);
    }

    [TargetRpc]
    public void TargetRpcTogglePlantBombInClient(NetworkConnection conn, bool tb)
	{
        GetComponent<CharacterController>().enabled = !tb;
        GetComponent<PlayerMovement>().enabled = !tb;
    }
}