using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BanSystem : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        print(GetDUID());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string GetDUID()
    {
        return SystemInfo.deviceUniqueIdentifier;
    }
}