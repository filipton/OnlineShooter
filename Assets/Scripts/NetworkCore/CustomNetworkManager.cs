using Mirror;
using Mirror.LiteNetLib4Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomNetworkManager : LiteNetLib4MirrorNetworkManager
{
    [Header("Custom Varibles")]
    public LiteNetLib4MirrorTransport Transport;

	public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);

        foreach (PlayerList pl in FindObjectsOfType<PlayerList>())
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

    /*IEnumerator SpawnPlayer(NetworkConnection conn)
	{
        yield return new WaitForSeconds(0.1f);
        GameObject player = Instantiate(LobbyPlayerPrefab);
        NetworkServer.AddPlayerForConnection(conn, player);
    }*/
}