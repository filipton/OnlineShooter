using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Discord;
using UnityEngine.Rendering;

public class DiscordRpcController : MonoBehaviour
{
	public static DiscordRpcController singleton;

	public Discord.Discord discord;

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

	// Update is called once per frame
	void Update()
	{
		discord.RunCallbacks();
	}

	public void ChangeDiscordStatus(string state = null, string details = null, string LargeImage = null, string LargeText = null, string SmallImage = null, string SmallText = null)
	{
		var activityManager = discord.GetActivityManager();

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

	bool IsHeadless()
	{
		return SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null;
	}

	private void OnDisable()
	{
		discord.Dispose();
	}
}