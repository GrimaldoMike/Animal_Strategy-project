using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerPrefab : MonoBehaviour
{
    public static SpawnerPrefab instance;

    private void Awake()
    {
        instance = this;
    }

    //Se manda llamar función para Instanciar prefabs de carpeta Asets/Resources (solamente se usarán los que se tengan que instanciar durante Runtime)
    public void SpawnPrefabInResources(string nameOfPrefab, Transform transformOfPrefab)
    {
        GameObject myPrefab = Resources.Load<GameObject>("Prefabs/"+ nameOfPrefab);

        GameObject g = GameObject.Instantiate(
          myPrefab,
          transformOfPrefab.position, // new Vector3(-1, 1, 0),
          transformOfPrefab.rotation); // Quaternion.Euler(0, 45, 0)
    }

    // Por medio del boton UI se spawnea un personaje en el campo
    public void SpawnCharactersInPlay()
    {

        List<CharacterController> tempList = new List<CharacterController>();
        tempList.AddRange(FindObjectsOfType<CharacterController>()); //Se guarda cada personaje con script de CharacterController(los que se mueven en el plano).

        int playerCount = 0, enemyCount = 0;

        if (tempList.Count > 0)
        {
            foreach (CharacterController cc in tempList)
            {
                if (cc.isEnemy == false)
                {
                    playerCount++;
                }
                else
                {
                    enemyCount++;
                }
            }

            if (playerCount < enemyCount)
            {
                int spawnPointsCount = GameManager.instance.playerSpawnPoints.Count;
                if (spawnPointsCount > 0) // Mientras aun existan Players pendientes de asignar spawn
                {
                    int pos = Random.Range(0, spawnPointsCount);
                    SpawnerPrefab.instance.SpawnPrefabInResources("Player", GameManager.instance.playerSpawnPoints[pos]);
                }
            }
            else
            {
                {
                    PlayerInputMenu.instance.ShowErrorText("No more units to Deploy");
                    Debug.Log("No se spawnearan mas unidadess.");
                }
            }
        }
        else
        {
            PlayerInputMenu.instance.ShowErrorText("No Enemies in this Level");
            Debug.Log("No hay Enemigos en el nivel, agrega enemigos antes de comenzar.");
        }


    }
    public void DeSpawnCharactersInPlay()
    {
        List<CharacterController> tempList = new List<CharacterController>();
        tempList.AddRange(FindObjectsOfType<CharacterController>()); //Se guarda cada personaje con script de CharacterController(los que se mueven en el plano).

        bool characterFound = false;
        foreach (CharacterController cc in tempList)
        {
            if (cc.isEnemy == false)
            {
                characterFound = true;
                tempList.Remove(cc);
                Destroy(cc);
                return;
            }
        }

        if (characterFound == false)
        {
            {
                PlayerInputMenu.instance.ShowErrorText("No character to Retreat");
                Debug.Log("No hay personaje que se deba DeSpawnear");
            }
        }
    }
}
