using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomNetworkManager : NetworkManager
{
    [Header("Custom Varibles")]
    public string LocalNick;
    public Team LocalTeam;
    public TelepathyTransport Transport;

    public override void Awake()
    {
        base.Awake();
        LocalNick = PlayerPrefs.GetString("nick");
    }

    public void SetNick(TMP_InputField tmp_if)
    {
        LocalNick = tmp_if.text;
        PlayerPrefs.SetString("nick", tmp_if.text);
    }

    public void SetTeam(int TeamId)
    {
        LocalTeam = (Team)TeamId;
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