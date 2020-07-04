using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

[System.Serializable]
public struct SavedPlayerStats
{
    public string Name;
	public Team Team;
    public int Money;
    public int Kills;
    //public int Assists;
    public int Deaths;

    public SavedPlayerStats(string name, Team team, int money, int kills, int deaths)
	{
        Name = name;
		Team = team;
        Money = money;
        Kills = kills;
        Deaths = deaths;
	}
}

public class SavePlayerStats : MonoBehaviour
{
    public static SavePlayerStats singleton;

    public List<SavedPlayerStats> savedPlayerStats = new List<SavedPlayerStats>();

	private void Start()
	{
		singleton = this;
	}

	public void AddPlayerToStats(SavedPlayerStats sps)
	{
		int index = savedPlayerStats.FindIndex(x => x.Name == sps.Name);

		if (index > -1)
		{
            savedPlayerStats[index] = sps;
		}
		else
		{
            savedPlayerStats.Add(sps);
		}
	}
}