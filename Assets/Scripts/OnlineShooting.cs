using Mirror;
using Mirror.Examples.Pong;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OnlineShooting : NetworkBehaviour
{
    [SyncVar]
    public float NextTime = 0f;
    float NextTimeP = 0f;

    [SyncVar]
    public float ShootRate = 0.1f;

    public LayerMask HitBoxLayer;
    public HitBox[] HitBoxes;

    public GameObject BulletHit;

    public Material Mat;
    public Camera cam;
    public PlayerMouseLook pml;
    public PlayerHealth PH;

    float LerpTime = -1;
    float RememberY = 0;

    // Start is called before the first frame update
    void Start()
    {
        if(isLocalPlayer)
        {
            pml = cam.GetComponent<PlayerMouseLook>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isLocalPlayer && !PH.PlayerKilled)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                LerpTime = -1;
                RememberY = pml.rotationY;
            }
            if(Input.GetKey(KeyCode.Mouse0) && NextTimeP <= 0)
            {
                CmdShoot(cam.transform.position, cam.transform.forward, gameObject);
                NextTimeP = ShootRate;
                pml.rotationY += 1.5f;
            }
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                LerpTime = 0;
            }

            if (Mathf.Abs(pml.rotationY - RememberY) <= 22.5f && LerpTime > -1 && LerpTime < 1 && pml.CanRotateYAxis)
            {
                LerpTime += Time.deltaTime;
                pml.rotationY += Mathf.Lerp(pml.rotationY, RememberY, LerpTime) - pml.rotationY;
            }
            else if (!pml.CanRotateYAxis)
            {
                LerpTime = -1;
            }
        }

        if (isServer)
        {
            ServerTime();
        }

        if (isClient && NextTimeP > 0)
        {
            NextTimeP -= Time.deltaTime;
        }
    }

    [Server]
    void ServerTime()
    {
        if(NextTime > 0)
            NextTime -= Time.deltaTime;
    }

    [Command]
    public void CmdShoot(Vector3 pos, Vector3 origin, GameObject MyPlayer)
    {
        //pos += origin * 0.4f;
        if(NextTime <= 0)
        {
            RaycastHit[] hits = Physics.RaycastAll(pos, origin);
            List<RaycastHit> FiltredHits = hits.ToList();

            if(hits.Length > 0)
            {
                foreach(RaycastHit hit in hits)
                {
                    if (hit.transform.gameObject.CompareTag("HitBox") && hit.transform.gameObject.GetComponent<HitBox>().plyHealth.gameObject == MyPlayer)
                    {
                        FiltredHits.Remove(hit);
                    }
                }

                for(int i = 0; i < FiltredHits.Count; i++)
                {
                    if (FiltredHits[i].transform.gameObject.CompareTag("HitBox"))
                    {
                        HitBox hitB = FiltredHits[i].transform.gameObject.GetComponent<HitBox>();
                        hitB.plyHealth.CmdRemoveHealth(hitB.HitDmg());
                    }
                    else
                    {
                        if (FiltredHits[i].transform.gameObject.layer != LayerMask.NameToLayer("Player"))
                        {
                            RpcCreateBulletHole(FiltredHits[i].point, Quaternion.FromToRotation(Vector3.up, FiltredHits[i].normal));
                        }
                    }
                }
            }

            NextTime = ShootRate;
        }
    }

    [ClientRpc]
    public void RpcCreateBulletHole(Vector3 pos, Quaternion rot)
    {
        GameObject bh = Instantiate(BulletHit, pos, rot);
        bh.transform.position += bh.transform.up * 0.005f;
        Destroy(bh, 10);
    }
}