using Mirror;
using Mirror.Examples.Pong;
using System.Collections;
using System.Collections.Generic;
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
                CmdShoot(cam.transform.position, cam.transform.forward);
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
    public void CmdShoot(Vector3 pos, Vector3 origin)
    {
        pos += origin;

        if(NextTime <= 0)
        {
            if (Physics.Raycast(pos, origin, out RaycastHit hit, 10000, HitBoxLayer))
            {
                print($"TRAFILES W COS! {hit.transform.gameObject.name} {hit.transform.gameObject.tag}");
                if (hit.transform.gameObject.CompareTag("SO"))
                {
                    RpcChangeMat(hit.transform.gameObject);
                }
                else if (hit.transform.gameObject.CompareTag("HitBox"))
                {
                    print("Trafiles w cokolwiek");

                    HitBox hitB = hit.transform.gameObject.GetComponent<HitBox>();
                    hitB.plyHealth.CmdRemoveHealth(hitB.HitDmg());

                    print($"TRAFILES W {hitB.name}");

                    if (hitB.HighLight)
                    {
                        hitB.StartCoroutine("HighLightHit");
                    }
                }
            }
            NextTime = ShootRate;
        }
    }

    [ClientRpc]
    public void RpcChangeMat(GameObject gb)
    {
        gb.GetComponent<MeshRenderer>().material = Mat;
    }

    [TargetRpc]
    public void TargetRpcRecoil(NetworkConnection conn, Vector3 amount)
    {
        pml.rotationY += amount.y;
    }
}