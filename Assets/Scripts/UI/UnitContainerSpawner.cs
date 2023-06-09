using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitContainerSpawner : MonoBehaviour
{
    private int posY = 0;

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
        string nameOfDog = cChar.uiName;
        childObject.GetComponent<TextMeshProUGUI>().text = nameOfDog;
        //// Editando raw image del hijo 2////
        childObject = gObj.transform.GetChild(1).gameObject;
        string path = "Character Portraits/"+ cChar.charName + "_portrait";
        childObject.GetComponent<RawImage>().texture = Resources.Load<Texture2D>(path);

        //// Editando boton del hijo 3 y 4. Se le agrega personajes a una lista para luego darle deploy////
        gObj.transform.GetComponent<SpawnerPrefab>().vChar = cChar;
    }
}
