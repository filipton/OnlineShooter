using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct ServerInfo
{
    public string Ip;
    public int Port;

    public string ServerName;

    public ServerInfo(string ip, int port, string sname)
    {
        Ip = ip;
        Port = port;
        ServerName = sname;
    }
}

[Serializable]
public struct JsonStruct
{
    public ServerInfo[] Servers;
}

public class ServerList : MonoBehaviour
{
    public List<ServerInfo> servers = new List<ServerInfo>();
    public CustomNetworkManager CNM;

    public GameObject ButtonPrefab;
    public GameObject ListParent;

    // Start is called before the first frame update
    void Start()
    {
        if (DiscordRpcController.singleton != null)
        {
            DiscordRpcController.singleton.ChangeDiscordStatus("Wyszedl dzban z serwera XD", "W Menu", "game_pic");
        }

        CNM = FindObjectOfType<CustomNetworkManager>();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        servers = GetAllServers(new WebClient().DownloadString("http://34.89.232.15/servers.json")).ToList();

        foreach(ServerInfo si in servers)
        {
            GameObject si_gb = Instantiate(ButtonPrefab, ListParent.transform);
            si_gb.GetComponentInChildren<TextMeshProUGUI>().text = si.ServerName;

            si_gb.GetComponent<ButtonServerInfo>().serverInfo = si;

            si_gb.GetComponent<Button>().onClick.AddListener(CNM.ConnectToServerButton);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private ServerInfo[] GetAllServers(string json)
    {
        return JsonUtility.FromJson<JsonStruct>(json).Servers;
    }

    private string ToJson(ServerInfo[] array)
    {
        JsonStruct jsonItem = new JsonStruct();
        jsonItem.Servers = array;
        return JsonUtility.ToJson(jsonItem, true);
    }
}