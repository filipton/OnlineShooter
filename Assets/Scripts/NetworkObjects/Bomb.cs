using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    NetworkIdentity nid;
    BombSystem bs;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(nid == null)
		{
            nid = FindObjectsOfType<NetworkIdentity>().Where(x => x.isLocalPlayer).First();
		}
        if(bs == null)
		{
            bs = FindObjectOfType<BombSystem>();
		}

        Vector3 lookat = nid.transform.position - new Vector3(0, nid.transform.position.y, 0);
        lookat.y = transform.position.y;

        transform.LookAt(lookat, nid.transform.up);

        GetComponentInChildren<TextMeshPro>().text = (bs.BombExplosionTime - bs.m_bombExplosionTime).ToString("0");
    }
}