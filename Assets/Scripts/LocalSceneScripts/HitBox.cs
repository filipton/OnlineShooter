﻿using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    public float Damage;
    public bool HighLight;

    public PlayerHealth plyHealth;

    public int HitDmg()
    {
        return (int)Damage;
    }
}