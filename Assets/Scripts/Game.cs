using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

[System.Serializable]
public class Game
{

    public static Game current;

    public List<Character> dogCharacters = new List<Character>();

    // Se inicializa el guardado de los personajes principales tipo perro.
    public Game()
    {
        string result1 = ConvertCamelCaseToSpaced(Character.DogTypeList.GermanShepherd.ToString());
        string result2 = ConvertCamelCaseToSpaced(Character.DogTypeList.GoldenRetriever.ToString());

        Character germanShepherd = new Character(0, "Prefabs/Characters(Main)/" + Character.DogTypeList.GermanShepherd.ToString(), Character.DogTypeList.GermanShepherd.ToString(), result1);
        Character goldenRetriever = new Character(1, "Prefabs/Characters(Main)/" + Character.DogTypeList.GoldenRetriever.ToString(), Character.DogTypeList.GoldenRetriever.ToString(), result2);

        dogCharacters.Add(germanShepherd);
        dogCharacters.Add(goldenRetriever);
    }


    //Se guardan los personajes obtenidos en el save file
    public void SaveNewCompanion(string newCompanionName)
    {

        //Character newCompanion = new Character(0, "Prefabs/Characters(Main)/" + Character.DogTypeList.newCompanionName.ToString(), Character.DogTypeList.GermanShepherd.ToString());
        Character newCompanion = new Character();

        dogCharacters.Add(newCompanion);
    }

    public static string ConvertCamelCaseToSpaced(string camelCase)
    {
        string spaced = Regex.Replace(camelCase, @"(\p{Lu})", " $1");
        return spaced.Trim();
    }
}