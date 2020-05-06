using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScanableObject : MonoBehaviour
{
    public bool Scanable = true;

    [Range(0, 1)]
    public float ScanMultiplier;
}