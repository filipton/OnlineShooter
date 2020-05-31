using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Discord;
using UnityEngine.Rendering;
using System.IO;
using System;
using System.Text;

public class DiscordRpcController : MonoBehaviour
{
	public static DiscordRpcController singleton;

	public Discord.Discord discord;

	public ServerInfo CurrentServer;

	// Use this for initialization
	void Start()
	{
		if (IsHeadless())
		{
			print("DESTROY DISCORD");
			Destroy(this);
		}

		singleton = this;

		discord = new Discord.Discord(708425982805147658, (System.UInt64)Discord.CreateFlags.Default);
		var activityManager = discord.GetActivityManager();
		var activity = new Discord.Activity
		{
			State = "Wejdz na serwer deklu!",
			Details = "Siedzi w menu...",
			Assets =
			{
				LargeImage = "game_pic"
			},
			Party =
			{
				Id = "foo partyID",
				Size = {
					CurrentSize = 1,
					MaxSize = 4,
				},
			 },
			Secrets =
			{
				Match = "foo matchSecret",
				Join = "foo joinSecret",
				Spectate = "foo spectateSecret",
			},
			Instance = true
		};
		activityManager.UpdateActivity(activity, (res) =>
		{
			if (res == Discord.Result.Ok)
			{
				Debug.Log("Everything is fine!");
			}
		});

		//CreateLobby();

		var lobbyManager = discord.GetLobbyManager();
		activityManager.OnActivityJoin += secret =>
		{
			lobbyManager.ConnectLobbyWithActivitySecret(secret, (Discord.Result result, ref Discord.Lobby lobby) =>
			{
				lobbyManager.ConnectNetwork(lobby.Id);
				lobbyManager.OpenNetworkChannel(lobby.Id, 0, true);

				UpdateActivity(lobby);
			});
		};

		activityManager.RegisterCommand(Path.Combine(Application.dataPath, "OnlineShooter-Online.exe"));
	}

	void CreateLobby()
	{
		var lobbyManager = discord.GetLobbyManager();

		var transaction = lobbyManager.GetLobbyCreateTransaction();
		transaction.SetCapacity(6);
		transaction.SetType(Discord.LobbyType.Public);
		transaction.SetMetadata("ip", "test");
		transaction.SetMetadata("ip", "testowy port");

		lobbyManager.CreateLobby(transaction, (Discord.Result result, ref Discord.Lobby lobby) =>
		{
			if (result != Discord.Result.Ok)
			{
				return;
			}

			// Check the lobby's configuration.
			print(string.Format("lobby {0} with capacity {1} and secret {2}", lobby.Id, lobby.Capacity, lobby.Secret));

			// Check lobby metadata.
			foreach (var key in new string[] { "a", "b", "c" })
			{
				print(string.Format("{0} = {1}", key, lobbyManager.GetLobbyMetadataValue(lobby.Id, key)));
			}

			// Update lobby.
			var lobbyTransaction = lobbyManager.GetLobbyUpdateTransaction(lobby.Id);
			lobbyTransaction.SetMetadata("d", "e");
			lobbyTransaction.SetCapacity(16);
			lobbyManager.UpdateLobby(lobby.Id, lobbyTransaction, (_) =>
			{
				print(string.Format("lobby has been updated"));
			});

			// Update a member.
			var lobbyID = lobby.Id;
			var userID = lobby.OwnerId;
			var memberTransaction = lobbyManager.GetMemberUpdateTransaction(lobbyID, userID);
			memberTransaction.SetMetadata("hello", "there");
			lobbyManager.UpdateMember(lobbyID, userID, memberTransaction, (_) =>
			{
				print(string.Format("lobby member has been updated: {0}", lobbyManager.GetMemberMetadataValue(lobbyID, userID, "hello")));
			});

			// Setup networking.
			lobbyManager.ConnectNetwork(lobby.Id);
			lobbyManager.OpenNetworkChannel(lobby.Id, 0, true);
		});
	}

	// Update is called once per frame
	void Update()
	{
		discord.RunCallbacks();
		discord.GetLobbyManager().FlushNetwork();
	}

	public void ChangeDiscordStatus(string state = null, string details = null, string LargeImage = null, string LargeText = null, string SmallImage = null, string SmallText = null)
	{
		var activityManager = discord.GetActivityManager();
		var lobbyManager = discord.GetLobbyManager();

		var activity = new Discord.Activity
		{
			State = state,
			Details = details,
			Assets =
			{
				LargeImage = LargeImage,
				LargeText = LargeText,
				SmallImage = SmallImage,
				SmallText = SmallText
			}
		};

		activityManager.UpdateActivity(activity, (res) =>
		{
			if (res == Discord.Result.Ok)
			{
				Debug.Log("Everything is fine!");
			}
		});
	}

	void UpdateActivity(Discord.Lobby lobby)
	{
		var activityManager = discord.GetActivityManager();
		var lobbyManager = discord.GetLobbyManager();

		var activity = new Discord.Activity
		{
			State = "olleh",
			Details = "foo details",
			Timestamps =
			{
				Start = 5,
				End = 6,
			},
			Assets =
			{
				LargeImage = "foo largeImageKey",
				LargeText = "foo largeImageText",
				SmallImage = "foo smallImageKey",
				SmallText = "foo smallImageText",
			},
			Party = {
			   Id = lobby.Id.ToString(),
			   Size = {
					CurrentSize = lobbyManager.MemberCount(lobby.Id),
					MaxSize = (int)lobby.Capacity,
				},
			},
			Secrets = {
				Join = lobbyManager.GetLobbyActivitySecret(lobby.Id),
			},
			Instance = true,
		};

		activityManager.UpdateActivity(activity, result =>
		{
			Console.WriteLine("Update Activity {0}", result);

			// Send an invite to another user for this activity.
			// Receiver should see an invite in their DM.
			// Use a relationship user's ID for this.
			// activityManager
			//   .SendInvite(
			//       364843917537050624,
			//       Discord.ActivityActionType.Join,
			//       "",
			//       inviteResult =>
			//       {
			//           Console.WriteLine("Invite {0}", inviteResult);
			//       }
			//   );
		});
	}

	bool IsHeadless()
	{
		return SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null;
	}

	private void OnDisable()
	{
		discord.Dispose();
	}
}