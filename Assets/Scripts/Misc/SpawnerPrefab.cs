using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerPrefab : MonoBehaviour
{
    /*public static SpawnerPrefab instance;

    private void Awake()
    {
        instance = this;
    }
    */

    public Character vChar;
    private bool alreadySpawned = false;

    //Se manda llamar función para Instanciar prefabs de carpeta Asets/Resources (solamente se usarán los que se tengan que instanciar durante Runtime)
    public void SpawnPrefabInResources(string pathOfPrefab, Transform transformOfPrefab)
    {
        GameObject myPrefab = Resources.Load<GameObject>(pathOfPrefab);

        GameObject g = GameObject.Instantiate(
          myPrefab,
          transformOfPrefab.position, // new Vector3(-1, 1, 0),
          transformOfPrefab.rotation); // Quaternion.Euler(0, 45, 0)

        CameraController.instance.SnapBackToPlayerUI(g); //La camara enfoca al jugador activo. Se manda true para que se active.
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
                Destroy(cc.gameObject); //Se usa remove gameObject para quitar todo el Prefab, si se pone solamente "cc" quita el script CharacterController.
                SFXManager.instance.UICancel.Play();
                return;
            }
        }

        if (characterFound == false)
        {
            {
                SFXManager.instance.UICancel.Play();
                PlayerInputMenu.instance.ShowErrorText("No character to Retreat");
                Debug.Log("No hay personaje que se deba DeSpawnear");
            }
        }
    }


    public void SpawnDogCharactersInPlay()
    {
        string msg = "";
        if (vChar is not null)
        {
            if (alreadySpawned == false)
            {
                bool charFound = false;
                foreach (Character cc in GameManager.instance.dogPlayers)
                {
                    int spawnPointsCount = GameManager.instance.playerSpawnPoints.Count;
                    if (spawnPointsCount > 0) // Se valida que aun queden spawn points en el mapa.
                    {
                        // Mientras aun existan Players pendientes de asignar spawn
                        if (cc.charID == vChar.charID) // Mientras aun existan Players pendientes de asignar spawn
                        {
                            charFound = true;
                            int pos = Random.Range(0, spawnPointsCount);
                            SpawnPrefabInResources(cc.charPrefabName, GameManager.instance.playerSpawnPoints[pos]);
                            alreadySpawned = true; //Se valida que el personaje ya se spawneo.
                            GameManager.instance.playerSpawnPoints.RemoveAt(pos);

                            SFXManager.instance.UISelect.Play();
                            msg = "Se spawneo el personaje: " + cc.charName;
                            return;
                        }
                    }
                    else
                    {
                        msg = "No se puede deploy más personajes en el campo | spawnPointsCount="+ spawnPointsCount;
                        PlayerInputMenu.instance.ShowErrorText(msg);
                        SFXManager.instance.UICancel.Play();
                        return;
                    }
                }
                if (charFound == false)
                {
                    msg = "No se encuentra el personaje guardado.";
                    PlayerInputMenu.instance.ShowErrorText(msg);
                }
            }
            else
            {
                msg = "Este personaje ya se encuentra en el campo!";
                PlayerInputMenu.instance.ShowErrorText(msg);
                SFXManager.instance.UICancel.Play();
            }
        }
        else
        {
            msg = "vChar es Null, no se asignó desde UnitContainerSpawner";
            PlayerInputMenu.instance.ShowErrorText(msg);
        }
        Debug.Log(msg);
    }

}
