using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData : MonoBehaviour
{
    private Dictionary<string, int> characterStats;
    private Dictionary<string, int> characterItems;
    private List<string> characterJobs;

    public CharacterData()
    {
        characterStats = new Dictionary<string, int>();
        characterItems = new Dictionary<string, int>();
        characterJobs = new List<string>();
    }

    public void SetStat(string statName, int value)
    {
        if (characterStats.ContainsKey(statName))
        {
            characterStats[statName] = value;
        }
        else
        {
            characterStats.Add(statName, value);
        }
    }

    public int GetStat(string statName)
    {
        if (characterStats.ContainsKey(statName))
        {
            return characterStats[statName];
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
        if (characterItems.ContainsKey(itemName))
        {
            characterItems[itemName] = value;
        }
        else
        {
            characterItems.Add(itemName, value);
        }
    }

    public int GetItem(string itemName)
    {
        if (characterItems.ContainsKey(itemName))
        {
            return characterItems[itemName];
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
        characterJobs.Add(jobName);
    }

    public void RemoveJob(string jobName)
    {
        characterJobs.Remove(jobName);
    }

    public List<string> GetJobs()
    {
        return characterJobs;
    }
}
