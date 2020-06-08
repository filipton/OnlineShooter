using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombSystem : NetworkBehaviour
{
    public float PlantingTime = 3f;
    float m_plantingTime;

    public float BombExplosionTime = 30f;
    float m_bombExplosionTime;

    public float DefusingTime = 7f;
    float m_defusingTime;

    public bool IsPlanting = false;
    public bool IsExploding = false;
    public bool IsDefusing = false;

    NetworkIdentity nid;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
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

                    print("EXPLODED!");
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