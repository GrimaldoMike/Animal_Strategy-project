using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class UnitContainerSpawner : MonoBehaviour
{
    private int posY = 0;
    private Character cChar;

    // Start is called before the first frame update
    void Start()
    {
        InitCharacterSelectScreen();
    }

    private void InitCharacterSelectScreen()
    {
        int count = 0;
        foreach (Character cc in GameManager.instance.dogPlayers)
        {
            if (GameManager.instance.dogPlayers.Count > 0) // Mientras aun existan Players pendientes de asignar spawn
            {
                SpawnerUI("Unit Container", GameManager.instance.dogPlayers[count]); //"Unit Container" es el nombre del GameObject UI que muestra el portrait texto y botones del Spawn character individual.
                count++;
            }
        }
    }

    private void SpawnerUI(string nameOfPrefab, Character cChar)
    {
        GameObject myPrefab = Resources.Load<GameObject>("Prefabs/" + nameOfPrefab);

        GameObject g = GameObject.Instantiate(
          myPrefab,
          new Vector3(transform.position.x-345, transform.position.y + posY, transform.position.z), transform.rotation); // Quaternion.Euler(0, 45, 0)
        posY += 200;
        g.transform.SetParent(gameObject.transform);

        SpawnerEditContainerUnit(g, cChar);
    }

    // Comienza a editarse el Unit container con el contenido de los personajes principales.
    private void SpawnerEditContainerUnit(GameObject gObj, Character cChar)
    {
        GameObject childObject;

        //// Editando texto del hijo 1////
        childObject = gObj.transform.GetChild(0).gameObject;
        string nameOfDog = cChar.charName;
        childObject.GetComponent<TextMeshProUGUI>().text = nameOfDog;
        //// Editando raw image del hijo 2////
        childObject = gObj.transform.GetChild(1).gameObject;
        string path = "Assets/Images/Character Portraits/" + cChar.charName + "_portrait.png";
        childObject.GetComponent<RawImage>().texture = (Texture2D)AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D));

        //// Editando boton del hijo 3 y 4. Se le agrega personajes a una lista para luego darle deploy////
        gObj.transform.GetComponent<SpawnerPrefab>().vChar = cChar;
    }
}
