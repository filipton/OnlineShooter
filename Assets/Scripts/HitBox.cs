using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    public float Damage;
    public bool HighLight;

    public MeshRenderer Mr;
    public PlayerHealth plyHealth;

    public int HitDmg()
    {
        return (int)Damage;
    }

    public IEnumerator HighLightHit()
    {
        Mr.enabled = true;
        yield return new WaitForSeconds(2);
        Mr.enabled = false;
    }
}