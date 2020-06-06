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
        /*var cryptoServiceProvider = new RSACryptoServiceProvider(4096); //2048 - Długość klucza
        var privateKey = cryptoServiceProvider.ExportParameters(true); //Generowanie klucza prywatnego
        var publicKey = cryptoServiceProvider.ExportParameters(false); //Generowanie klucza publiczny

        string publicKeyString = GetKeyString(publicKey);
        string privateKeyString = GetKeyString(privateKey);

        File.AppendAllText(@"D:\pw", GetKeyString(privateKey));
        File.AppendAllText(@"D:\pb", GetKeyString(publicKey));

        string tte = "HGFesug337+";
        print(tte);
        string x = Encrypt(tte, publicKeyString);
        print(x);
        string d = Decrypt(x, privateKeyString);
        print(d);*/


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

    public static string Encrypt(string textToEncrypt, string publicKeyString)
    {
        var bytesToEncrypt = Encoding.UTF8.GetBytes(textToEncrypt);

        using (var rsa = new RSACryptoServiceProvider(2048))
        {
            try
            {
                rsa.FromXmlString(publicKeyString.ToString());
                var encryptedData = rsa.Encrypt(bytesToEncrypt, true);
                var base64Encrypted = Convert.ToBase64String(encryptedData);
                return base64Encrypted;
            }
            finally
            {
                rsa.PersistKeyInCsp = false;
            }
        }
    }

    public static string Decrypt(string textToDecrypt, string privateKeyString)
    {
        var bytesToDescrypt = Encoding.UTF8.GetBytes(textToDecrypt);

        using (var rsa = new RSACryptoServiceProvider(2048))
        {
            try
            {

                // server decrypting data with private key                    
                rsa.FromXmlString(privateKeyString);

                var resultBytes = Convert.FromBase64String(textToDecrypt);
                var decryptedBytes = rsa.Decrypt(resultBytes, true);
                var decryptedData = Encoding.UTF8.GetString(decryptedBytes);
                return decryptedData.ToString();
            }
            finally
            {
                rsa.PersistKeyInCsp = false;
            }
        }
    }

    public static string GetKeyString(RSAParameters publicKey)
    {

        var stringWriter = new System.IO.StringWriter();
        var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
        xmlSerializer.Serialize(stringWriter, publicKey);
        return stringWriter.ToString();
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