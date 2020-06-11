using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombPlayerController : NetworkBehaviour
{
    BombSystem bs;

    // Start is called before the first frame update
    void Start()
    {
        bs = FindObjectOfType<BombSystem>();
    }

    // Update is called once per frame
    void Update()
    {
		if (isLocalPlayer)
		{
            if (bs.IsExploding)
            {
                if (Input.GetKeyDown(KeyCode.E)) CmdToggleDefuseBomb(true, netIdentity);
                else if (Input.GetKeyUp(KeyCode.E)) CmdToggleDefuseBomb(false, netIdentity);
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.E)) CmdTogglePlantBomb(true, netIdentity);
                else if (Input.GetKeyUp(KeyCode.E)) CmdTogglePlantBomb(false, netIdentity);
            }
        }
    }

    [Command]
    public void CmdTogglePlantBomb(bool b, NetworkIdentity id)
	{
        bs.CmdTogglePlantBomb(b, id);
    }

    [Command]
    public void CmdToggleDefuseBomb(bool b, NetworkIdentity id)
	{
        bs.CmdToggleDefuseBomb(b, id);

    }
}