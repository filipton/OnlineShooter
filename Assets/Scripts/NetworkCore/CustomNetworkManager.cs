using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomNetworkManager : NetworkManager
{
    public string LocalNick;
    public TelepathyTransport Transport;


    public void SetNick(TMP_InputField tmp_if)
    {
        LocalNick = tmp_if.text;
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);

        foreach(PlayerList pl in FindObjectsOfType<PlayerList>())
        {
            pl.RpcGetAllPlayers();
        }
    }

    public void ConnectToServerFromList(ServerInfo sinfo)
    {
        GetComponent<DiscordRpcController>().CurrentServer = sinfo;
        networkAddress = sinfo.Ip;
        Transport.port = (ushort)sinfo.Port;
        StartClient();
    }

    public void ConnectToServerButton()
    {
        ButtonServerInfo sinfo = EventSystem.current.currentSelectedGameObject.GetComponent<ButtonServerInfo>();
        if(sinfo != null)
        {
            ConnectToServerFromList(sinfo.serverInfo);
        }
    }
}