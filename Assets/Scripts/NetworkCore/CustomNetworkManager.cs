using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CustomNetworkManager : NetworkManager
{
    public string LocalNick;

    public void SetNick(TMP_InputField tmp_if)
    {
        LocalNick = tmp_if.text;
    }
}