using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData
{
    public CharacterStats CharacterStats { get; set; }
    public GeneralStats GeneralStats { get; set; }
    public CurrentStats CurrentStats { get; set; }
    public BattleActions BattleActions { get; set; }
    public BattleStates BattleStates { get; set; }


    //public CharacterData CreateNewCharacterData(CharacterStats charStats)
    public CharacterData CreateNewCharacterData(string charName, string charType, string charSubType, int charColor)
    {
        if (charName == null ||
            charType == null ||
            charSubType == null || 
            charColor.ToString() == null)
        {
            Debug.Log("estuve entrando aqui todo este tiempo");
            return null;
        }

        CharacterStats charStatsInitial = new CharacterStats()
        {
            Name = charName,
            CharacterType = charType,
            CharacterSubType = charSubType,
            Color = charColor
        };

        GeneralStats generalStatsInitial = new GeneralStats()
        {
            CurrentLevel = 1,
            CurrentXP = 0,
            MaxHP = 10,
            MaxWalkRange = 3.5f,
            MaxRunRange = 8f,
            MaxAttack = 5,
            MaxDefense = 5,
            MaxInitiative = 5,
            MaxSkill = 5,
            MaxActionPoints = 2
        };

        BattleActions battleActionsInitial = new BattleActions()
        {
            Melee = new List<string>(),
            Ranged = new List<string>(),
            Skills = new List<string>()
        };

        BattleStates BattleStatesInitial = new BattleStates()
        {
            States = new List<string>()
        };

        SetInitialStatsAndAbilities(charStatsInitial, generalStatsInitial, battleActionsInitial);

        //El CurrentStats va despu�s del c�lculo del GeneralStats segun el tipo.
        CurrentStats currentStatsInitial = new CurrentStats()
        {
            CurrentHP = generalStatsInitial.MaxHP,
            CurrentWalkRange = generalStatsInitial.MaxWalkRange,
            CurrentRunRange = generalStatsInitial.MaxRunRange,
            CurrentAttack = generalStatsInitial.MaxAttack,
            CurrentDefense = generalStatsInitial.MaxDefense,
            CurrentInitiative = generalStatsInitial.MaxInitiative,
            CurrentSkill = generalStatsInitial.MaxSkill,
            CurrentActionPoints = generalStatsInitial.MaxActionPoints
        };

        CharacterData character = new CharacterData()
        {
            CharacterStats = charStatsInitial,
            GeneralStats = generalStatsInitial,
            CurrentStats = currentStatsInitial,
            BattleActions = battleActionsInitial,
            BattleStates = BattleStatesInitial
        };

        return character;
    
    }

    private void SetInitialStatsAndAbilities(CharacterStats charStatsInitial, GeneralStats generalStatsInitial, BattleActions battleActionsInitial)
    {
        // Definici�n Global
        generalStatsInitial.CurrentLevel = 1;
        generalStatsInitial.CurrentXP = 0;

        //Se revisa cada personaje para establecer sus datos por default.
        switch (charStatsInitial.CharacterSubType)
        {
            case "PolygonDog": // PolygonDog
               //GeneralStats

                Debug.Log("Inicializando el " + charStatsInitial.CharacterSubType + " " + charStatsInitial.Name);

                generalStatsInitial.MaxHP = 10;
                generalStatsInitial.MaxWalkRange = 3.5f;
                generalStatsInitial.MaxRunRange = 8f;
                generalStatsInitial.MaxAttack = 5;
                generalStatsInitial.MaxDefense = 5;
                generalStatsInitial.MaxInitiative = 5;
                generalStatsInitial.MaxSkill = 5;
                generalStatsInitial.MaxActionPoints = 2;
                //BattleActions
                battleActionsInitial.Melee.Add("Bite");
                battleActionsInitial.Melee.Add("Claw");
                battleActionsInitial.Ranged.Add("Howl");
                break;
            case "SimpleDog": // SimpleDog
                //GeneralStats
                Debug.Log("Inicializando el " + charStatsInitial.CharacterSubType + " " + charStatsInitial.Name);
                generalStatsInitial.MaxHP = 7;
                generalStatsInitial.MaxWalkRange = 2f;
                generalStatsInitial.MaxRunRange = 6f;
                generalStatsInitial.MaxAttack = 3;
                generalStatsInitial.MaxDefense = 3;
                generalStatsInitial.MaxInitiative = 3;
                generalStatsInitial.MaxSkill = 3;
                generalStatsInitial.MaxActionPoints = 2;
                //BattleActions
                battleActionsInitial.Melee.Add("Bite");
                break;
            default: 
                break;
        }
    }

}

public class CharacterStats
{
    public string Name { get; set; }
    public string CharacterType { get; set; }
    public string CharacterSubType { get; set; }
    public int Color { get; set; }
}

public class GeneralStats
{
    public int CurrentLevel { get; set; }
    public int CurrentXP { get; set; }
    public int MaxHP { get; set; }
    public float MaxWalkRange { get; set; }
    public float MaxRunRange { get; set; }
    public int MaxAttack { get; set; }
    public int MaxDefense { get; set; }
    public int MaxInitiative { get; set; }
    public int MaxSkill { get; set; }
    public int MaxActionPoints { get; set; }
}

public class CurrentStats
{
    public int CurrentHP { get; set; }
    public float CurrentWalkRange { get; set; }
    public float CurrentRunRange { get; set; }
    public int CurrentAttack { get; set; }
    public int CurrentDefense { get; set; }
    public int CurrentInitiative { get; set; }
    public int CurrentSkill { get; set; }
    public int CurrentActionPoints { get; set; }
}

public class BattleActions
{
    public List<string> Melee { get; set; }
    public List<string> Ranged { get; set; }
    public List<string> Skills { get; set; }
}

public class BattleStates
{
    public List<string> States { get; set; }
}

/*
public class Program
{
    public static Dictionary<string, object> DisplayCharacterDataContent(CharacterData characterData)
    {
        var characterDataContent = new Dictionary<string, object>();

        characterDataContent.Add("Name", characterData.CharacterStats.Name);
        characterDataContent.Add("Character Type", characterData.CharacterStats.CharacterType);
        characterDataContent.Add("Character Sub Type", characterData.CharacterStats.CharacterSubType);
        characterDataContent.Add("Color", characterData.CharacterStats.Color);

        characterDataContent.Add("Current Level", characterData.GeneralStats.CurrentLevel);
        characterDataContent.Add("Current XP", characterData.GeneralStats.CurrentXP);
        characterDataContent.Add("Max HP", characterData.GeneralStats.MaxHP);
        characterDataContent.Add("Max Walk Range", characterData.GeneralStats.MaxWalkRange);
        characterDataContent.Add("Max Run Range", characterData.GeneralStats.MaxRunRange);
        characterDataContent.Add("Max Attack", characterData.GeneralStats.MaxAttack);
        characterDataContent.Add("Max Defense", characterData.GeneralStats.MaxDefense);
        characterDataContent.Add("Max Initiative", characterData.GeneralStats.MaxInitiative);
        characterDataContent.Add("Max Skill", characterData.GeneralStats.MaxSkill);
        characterDataContent.Add("Max Action Points", characterData.GeneralStats.MaxActionPoints);

        characterDataContent.Add("Current HP", characterData.CurrentStats.CurrentHP);
        characterDataContent.Add("Current Walk Range", characterData.CurrentStats.CurrentWalkRange);
        characterDataContent.Add("Current Run Range", characterData.CurrentStats.CurrentRunRange);
        characterDataContent.Add("Current Attack", characterData.CurrentStats.CurrentAttack);
        characterDataContent.Add("Current Defense", characterData.CurrentStats.CurrentDefense);
        characterDataContent.Add("Current Initiative", characterData.CurrentStats.CurrentInitiative);
        characterDataContent.Add("Current Skill", characterData.CurrentStats.CurrentSkill);
        characterDataContent.Add("Current Action Points", characterData.CurrentStats.CurrentActionPoints);

        characterDataContent.Add("Melee", characterData.BattleActions.Melee);
        characterDataContent.Add("Ranged", characterData.BattleActions.Ranged);
        characterDataContent.Add("Skills", characterData.BattleActions.Skills);

        characterDataContent.Add("States", characterData.BattleStates.States);

        return characterDataContent;
    }

}
*/



