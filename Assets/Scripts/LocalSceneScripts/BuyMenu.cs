using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuyMenu : MonoBehaviour
{
    public GameObject BuyMenuGB;
    public TextMeshProUGUI MoneyText;
    EconomySystem es;
    PlayerStats ps;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            if(es == null || ps == null)
            {
                es = FindObjectsOfType<EconomySystem>().ToList().Find(x => x.isLocalPlayer);
                ps = es.GetComponent<PlayerStats>();
            }

            CursorManager.RefreshLock("_bm", BuyMenuGB.activeSelf);

            MoneyText.text = ps.Money.ToString();
            BuyMenuGB.SetActive(!BuyMenuGB.activeSelf);
        }
    }

    public void Buy()
    {
        Weapon w = (Weapon)Enum.Parse(typeof(Weapon), EventSystem.current.currentSelectedGameObject.name);
        es.CmdBuyWeapon(w);

        MoneyText.text = ps.Money.ToString();
    }

    public void BuyAmmo(string ammoType)
    {
        AmmoType at = (AmmoType)Enum.Parse(typeof(AmmoType), ammoType);
        es.CmdBuyAmmo(at);

        MoneyText.text = ps.Money.ToString();
    }
}