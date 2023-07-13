using UnityEngine;
using System.Collections;
[System.Serializable]
public class Character
{
    
    public enum DogTypeList
    {
        Coyote,
        Dalmatian,
        DalmatianCollar,
        Doberman,
        DobermanCollar,
        Fox,
        GermanShepherd,
        GermanShepherdCollar,
        GoldenRetriever,
        GoldenRetrieverCollar,
        DogGreyhound,
        GreyhoundCollar,
        HellHound,
        Husky,
        HuskyCollar,
        Labrador,
        LabradorCollar,
        Pointer,
        PointerCollar,
        Ridgeback,
        RidgebackCollar,
        Robot,
        Scifi,
        Shiba,
        Shiba_Collar,
        Wolf,
        ZombieDoberman,
        ZombieGermanShepherd
    };


    public int charID;
    public string charPrefabName;
    public string charName;
    public string uiName;
    //Stat definition
    /*
    public CharacterData characterData = new CharacterData();
    public CharacterStats characterStats = new CharacterStats();
    public string charStatName;
    public string charStatType;
    public string charStatSubType;
    public int charStatColor;
    */
    public Character()
    {
        this.charID = 0;
        this.charPrefabName = "";
        this.charName = "";
        this.uiName = "";
    }
    public Character(int id, string prefabName, string name, string uiname)
    {
        this.charID = id;
        this.charPrefabName = prefabName;
        this.charName = name;
        this.uiName = uiname;
    }
}