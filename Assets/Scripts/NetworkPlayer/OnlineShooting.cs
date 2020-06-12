using Mirror;
using Mirror.Examples.Pong;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OnlineShooting : NetworkBehaviour
{
    float NextTimeP = 0f;

    [SyncVar]
    public float ShootRate = 100f;
    [SyncVar]
    public float DamageMultiplier = 1;

    float DamagePlayerScanThreshold = 0.2f;
    float CreateBulletScanThreshold = 0.1f;

    public LayerMask HitBoxLayer;
    public HitBox[] HitBoxes;

    public GameObject BulletHit;

    public Camera cam;
    public PlayerMouseLook pml;
    public PlayerHealth PH;
    public AudioSync As;
    public AmmoController ammo;
    public WeaponController weaponController;

    float LerpTime = -1;
    float RememberY = 0;

    float NormalFov = 0;

    bool CanShoot = true;

    // Start is called before the first frame update
    void Start()
    {
        if(isLocalPlayer)
        {
            NormalFov = cam.fieldOfView;
            pml = cam.GetComponent<PlayerMouseLook>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isLocalPlayer && !PH.PlayerKilled)
        {
			//scope
			if (Input.GetKeyDown(KeyCode.Mouse1) && weaponController.CurrentWeapon == Weapon.AWP)
			{
                bool b = !LocalSceneObjects.singleton.SniperScope.activeSelf;
                LocalSceneObjects.singleton.SniperScope.SetActive(b);

                cam.fieldOfView = b ? 30 : NormalFov;
                weaponController.FPC_WeaponModels[(int)Weapon.AWP].SetActive(!b);
            }

            //shooting
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                LerpTime = -1;
                RememberY = pml.rotationY;
                CanShoot = true;
            }
            if(Input.GetKey(KeyCode.Mouse0) && NextTimeP <= 0 && ammo.CurrentInMagazine > 0 && CanShoot)
            {
                CmdShoot();
                As.CmdSyncAudioClip("AkShot");
                NextTimeP = ShootRate;
                pml.rotationY += 1.5f;

                if (!WeaponStats.GetWeaponFullAuto(weaponController.CurrentWeapon))
                {
                    LerpTime = 0;
                    CanShoot = false;
                }
            }
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                LerpTime = 0;
                CanShoot = false;
            }

            //recoil back
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

        if (!isServer && isClient && NextTimeP > 0)
        {
            NextTimeP -= Time.deltaTime;
        }
    }

    [Server]
    void ServerTime()
    {
        if(NextTimeP > 0)
            NextTimeP -= Time.deltaTime;
    }

    [Command]
    public void CmdShoot()
    {
        //pos += origin * 0.4f;
        if(NextTimeP <= 0 && ammo.CurrentInMagazine > 0)
        {
            if(weaponController.CurrentWeapon != Weapon.Knife)
			{
                //RaycastHit[] hits = Physics.RaycastAll(pos, origin);
                RaycastHit[] hits = RaycastAllSort(cam.transform.position, cam.transform.forward);
                List<RaycastHit> FiltredHits = hits.ToList();

                if (hits.Length > 0)
                {
                    foreach (RaycastHit hit in hits)
                    {
                        if (hit.transform.gameObject.CompareTag("HitBox") && hit.transform.gameObject.GetComponent<HitBox>().plyHealth.gameObject == gameObject)
                        {
                            FiltredHits.Remove(hit);
                        }
                    }

                    float BulletInpact = 1;

                    for (int i = 0; i < FiltredHits.Count; i++)
                    {
                        if (BulletInpact > 0)
                        {
                            if (FiltredHits[i].transform.gameObject.CompareTag("HitBox"))
                            {
                                if (BulletInpact >= DamagePlayerScanThreshold && FiltredHits[i].distance <= WeaponStats.GetMaxDistance(weaponController.CurrentWeapon))
                                {
                                    HitBox hitB = FiltredHits[i].transform.gameObject.GetComponent<HitBox>();
                                    hitB.plyHealth.CmdRemoveHealth((int)(hitB.HitDmg() * BulletInpact * DamageMultiplier), GetComponent<PlayerStats>());
                                    if (hitB.plyHealth.Health > 0)
                                    {
                                        As.RpcSyncAudioClip("death-sound");
                                    }
                                }
                            }
                            else
                            {
                                if (FiltredHits[i].transform.gameObject.layer != LayerMask.NameToLayer("Player") && BulletInpact >= CreateBulletScanThreshold)
                                {
                                    RpcCreateBulletHole(FiltredHits[i].point, Quaternion.FromToRotation(Vector3.up, FiltredHits[i].normal));
                                }
                            }

                            //scan calculator
                            ScanableObject so = FiltredHits[i].transform.GetComponent<ScanableObject>();
                            if (so == null)
                            {
                                BulletInpact *= 1;
                            }
                            else
                            {
                                BulletInpact *= so.Scanable ? so.ScanMultiplier : 0;
                            }

                            MeshRenderer pbm = FiltredHits[i].transform.GetComponent<MeshRenderer>();
                            if (pbm != null)
                            {
                                Vector3 hitNormal = FiltredHits[i].normal;

                                //side "X"
                                if (hitNormal.x != 0)
                                {
                                    BulletInpact /= pbm.bounds.size.x * 2;
                                }

                                //side "Y"
                                if (hitNormal.y != 0)
                                {
                                    BulletInpact /= pbm.bounds.size.y * 2;
                                }

                                //side "Z"
                                if (hitNormal.z != 0)
                                {
                                    BulletInpact /= pbm.bounds.size.z * 2;
                                }
                            }
                        }
                    }
                }
            }
			else
			{
                float thickness = 1f;
                Vector3 origin = transform.position + new Vector3(0, 0.6f, 0) + transform.forward;
                Vector3 direction = transform.TransformDirection(Vector3.forward);

                RaycastHit[] hits = Physics.SphereCastAll(origin, thickness, direction, WeaponStats.GetMaxDistance(Weapon.Knife)).OrderBy(x => x.distance).ToArray();
                List<RaycastHit> FiltredHits = hits.ToList();

                if (hits.Length > 0)
                {
                    foreach (RaycastHit hit in hits)
                    {
                        if (hit.transform.gameObject.CompareTag("HitBox") && hit.transform.GetComponent<HitBox>().plyHealth.gameObject == gameObject)
                        {
                            FiltredHits.Remove(hit);
                        }
                    }

                    HitBox hitB = FiltredHits[0].transform.GetComponent<HitBox>();
                    if(hitB != null)
					{
                        hitB.plyHealth.CmdRemoveHealth((int)(hitB.HitDmg() * DamageMultiplier), GetComponent<PlayerStats>());
                        if (hitB.plyHealth.Health > 0)
                        {
                            As.RpcSyncAudioClip("death-sound");
                        }
					}
                }
            }

            ammo.RemoveAmmoInCurrentMagazine(1);
            NextTimeP = ShootRate - 0.1f;
        }
    }

    RaycastHit[] RaycastAllSort(Vector3 position, Vector3 origin)
    {
        return Physics.RaycastAll(position, origin).OrderBy(h => h.distance).ToArray();
    }

    [ClientRpc]
    public void RpcCreateBulletHole(Vector3 pos, Quaternion rot)
    {
        GameObject bh = Instantiate(BulletHit, pos, rot);
        bh.transform.position += bh.transform.up * 0.005f;
        bh.transform.rotation *= Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up);
        Destroy(bh, 10);
    }
}