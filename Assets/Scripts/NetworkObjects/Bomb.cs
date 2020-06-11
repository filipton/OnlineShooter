using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    NetworkIdentity nid;

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
        Vector3 lookat = nid.transform.position - new Vector3(0, nid.transform.position.y, 0);
        lookat.y = transform.position.y;

        transform.LookAt(lookat, nid.transform.up);
    }
}