using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuyMenu : MonoBehaviour
{
    public GameObject BuyMenuGB;
    public EconomySystem es;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            if(es == null)
            {
                es = FindObjectsOfType<EconomySystem>().ToList().Find(x => x.isLocalPlayer);
            }

            BuyMenuGB.SetActive(!BuyMenuGB.activeSelf);
        }
    }

    public void Buy(int cost)
    {
        Weapon w = (Weapon)Enum.Parse(typeof(Weapon), EventSystem.current.currentSelectedGameObject.name);
        es.CmdBuyWeapon(w);
    }
}