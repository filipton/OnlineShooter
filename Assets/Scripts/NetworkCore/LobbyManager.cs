using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : NetworkBehaviour
{
    [SyncVar]
    public float ReamingTime;

    [SyncVar]
    public float LobbyTime = 30;

    [SyncVar]
    public int PlayersCount;

    public GameObject StartHostButton;

    public string GameSceneName;

    public Image ImageProgressBar;
    public TextMeshProUGUI PlayersCountText;

    bool Y = true;

    // Start is called before the first frame update
    void Start()
    {
        CursorManager.RemoveAll();
        CursorManager.RefreshLock("_lm", false);

        if (isServer)
		{
            ReamingTime = LobbyTime;
		}
		if (isServer)
		{
            StartHostButton.SetActive(true);
		}
    }

    // Update is called once per frame
    void Update()
    {
		if (isServer)
		{
            ServerUpdate();
		}
		if (isClient)
		{
            ImageProgressBar.fillAmount = ReamingTime / LobbyTime;
            PlayersCountText.text = PlayersCount.ToString();
		}
    }

    [ServerCallback]
    void ServerUpdate()
	{
        PlayersCount = NetworkServer.connections.Count;

        if (PlayersCount >= 1 && Y)
        {
            ReamingTime -= Time.deltaTime;
        }

        if(ReamingTime <= 0)
		{
            FindObjectOfType<CustomNetworkManager>().ServerChangeScene(GameSceneName);

            ReamingTime = LobbyTime;
            Y = false;
		}
	}

    public void StartGame()
	{
        ReamingTime = -1;
	}

    public void Disconnect()
	{
        NetworkClient.Disconnect();
		if (NetworkServer.active)
		{
            NetworkServer.DisconnectAll();
		}

        SceneManager.LoadScene("Menu");
	}

    public void ButtonSelectTeam(int i)
    {
        FindObjectsOfType<LobbyPlayer>().ToList().Where(x => x.isLocalPlayer).First().CmdSelectTeam((Team)i);
    }
}