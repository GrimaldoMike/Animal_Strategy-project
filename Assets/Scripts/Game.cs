using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class Game
{

    public static Game current;

    public List<Character> dogCharacters = new List<Character>();

    // Se inicializa el guardado de los personajes principales tipo perro.
    public Game()
    {
        Character germanShepherd = new Character(0, "Prefabs/Characters(Main)/" + Character.DogTypeList.GermanShepherd.ToString(), Character.DogTypeList.GermanShepherd.ToString());
        Character goldenRetriever = new Character(1, "Prefabs/Characters(Main)/" + Character.DogTypeList.GoldenRetriever.ToString(), Character.DogTypeList.GoldenRetriever.ToString());

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
}