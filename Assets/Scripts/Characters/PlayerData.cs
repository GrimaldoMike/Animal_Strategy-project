using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    private Dictionary<string, int> playerStats;
    private Dictionary<string, int> playerItems;
    private List<string> playerJobs;

    public PlayerData()
    {
        playerStats = new Dictionary<string, int>();
        playerItems = new Dictionary<string, int>();
        playerJobs = new List<string>();
    }

    public void SetStat(string statName, int value)
    {
        if (playerStats.ContainsKey(statName))
        {
            playerStats[statName] = value;
        }
        else
        {
            playerStats.Add(statName, value);
        }
    }

    public int GetStat(string statName)
    {
        if (playerStats.ContainsKey(statName))
        {
            return playerStats[statName];
        }
        else
        {
            // Handle the case when the stat doesn't exist
            // You can return a default value or throw an exception
            return 0; // Default value
        }
    }

    public void SetItem(string itemName, int value)
    {
        if (playerItems.ContainsKey(itemName))
        {
            playerItems[itemName] = value;
        }
        else
        {
            playerItems.Add(itemName, value);
        }
    }

    public int GetItem(string itemName)
    {
        if (playerItems.ContainsKey(itemName))
        {
            return playerItems[itemName];
        }
        else
        {
            // Handle the case when the stat doesn't exist
            // You can return a default value or throw an exception
            return 0; // Default value
        }
    }

    public void AddJob(string jobName)
    {
        playerJobs.Add(jobName);
    }

    public void RemoveJob(string jobName)
    {
        playerJobs.Remove(jobName);
    }

    public List<string> GetJobs()
    {
        return playerJobs;
    }

}