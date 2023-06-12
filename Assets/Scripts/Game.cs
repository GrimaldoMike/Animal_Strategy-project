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
        Character germanShepherd = new Character(0, "Prefabs/" + Character.DogTypeList.GermanShepherd.ToString(), Character.DogTypeList.GermanShepherd.ToString());
        Character goldenRetriever = new Character(1, "Prefabs/" + Character.DogTypeList.GoldenRetriever.ToString(), Character.DogTypeList.GoldenRetriever.ToString());

        dogCharacters.Add(germanShepherd);
        dogCharacters.Add(goldenRetriever);
    }

}