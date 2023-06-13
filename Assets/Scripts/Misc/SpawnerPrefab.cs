using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerPrefab : MonoBehaviour
{

    public Character vChar;
    private string msg = "";
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
        bool charMatch = CheckForCharacterMatch(); // Revisa que sea un personaje existente en el SaveLoad
        if (charMatch == true)
        {
            bool characterFound = false;
            List<CharacterController> tempList = new List<CharacterController>();
            tempList.AddRange(FindObjectsOfType<CharacterController>()); //Se guarda cada personaje con script de CharacterController(los que se mueven en el plano).

            foreach (CharacterController cc in tempList)
            {
                if (cc.isEnemy == false && cc.name.Contains(vChar.charName)) // Debe ser un Player y llamarse como el personaje guardado en SaveLoad
                {
                    int pos = GameManager.instance.AddRemoveFromSpawnPointsList(cc.transform); // Manda llamar la funcion con sus variables de remover spawn.
                    if (pos >= 0)
                    {
                        characterFound = true;
                        alreadySpawned = false; //Se valida que el personaje ya se DESspawneo.
                        tempList.Remove(cc);
                        Destroy(cc.gameObject); //Se usa remove gameObject para quitar todo el Prefab, si se pone solamente "cc" quita el script CharacterController.
                        msg = "Se Despawneo el personaje: " + vChar.charName;
                        SFXManager.instance.UICancel.Play();
                        return;
                    }
                    else
                    {
                        msg = "No hay más Desspawn points disponibles.";
                        PlayerInputMenu.instance.ShowErrorText(msg);
                    }
                }
            }
            if (characterFound == false)
            {
                {
                    msg = "No character to Retreat";
                    PlayerInputMenu.instance.ShowErrorText(msg);
                    SFXManager.instance.UICancel.Play();
                }
            }
        }

        Debug.Log(msg);

    }


    public void SpawnCharactersInPlay()
    {
        msg = "";
        if (alreadySpawned == false)
        {
            bool charMatch = CheckForCharacterMatch();
            bool spawnPointsFound = CheckForSpawnPointMatch();
            if (charMatch == true && spawnPointsFound == true)
            {
                int pos = GameManager.instance.AddRemoveFromSpawnPointsList(null); // Manda llamar la funcion con sus variables de remover spawn.
                if(pos >= 0)
                {
                    SpawnPrefabInResources(vChar.charPrefabName, GameManager.instance.playerSpawnPoints[pos]);
                    alreadySpawned = true; //Se valida que el personaje ya se spawneo.

                    msg = "Se spawneo el personaje: " + vChar.charName;
                    SFXManager.instance.UISelect.Play();
                }
                else
                {
                    msg = "No hay más spawn points disponibles. var pos = "+pos ;
                    PlayerInputMenu.instance.ShowErrorText(msg);
                }
            }
        }
        else
        {
            msg = "Este personaje ya se encuentra en el campo!";
            PlayerInputMenu.instance.ShowErrorText(msg);
            SFXManager.instance.UICancel.Play();
        }

        Debug.Log(msg);
    }

    private bool CheckForCharacterMatch()
    {
        bool charExist = false;

        if (vChar is not null)
        {
            msg = "NO hay match de personajes en SpawnerPrefab con GameManager.instance.dogPlayers" + "\n";
            foreach (Character cc in GameManager.instance.dogPlayers)
            {
                // Mientras aun existan Players pendientes de asignar spawn
                if (cc.charID == vChar.charID) // Mientras aun existan Players pendientes de asignar spawn
                {
                    charExist = true;
                    msg = "Sí ENCONTRE el mtach de Char.";
                    return charExist;
                }
            }
            if(charExist == false)
            {
                PlayerInputMenu.instance.ShowErrorText(msg);
            }
        }
        else
        {
            msg = "vChar es Null, no se asignó desde UnitContainerSpawner";
            PlayerInputMenu.instance.ShowErrorText(msg);
        }
        return charExist;
    }

    private bool CheckForSpawnPointMatch()
    {
        bool spawnPointsExist = false;

        if (alreadySpawned == false) // Se encarga de evaluar si estas Spawneando o Despawneando.
        {
            int spawnPointsCount = GameManager.instance.playerSpawnPoints.Count;

            if (spawnPointsCount > 0) // Se valida que aun queden spawn points en el mapa.
            {
                Debug.Log("Sí hay suficientes spawn points.");
                spawnPointsExist = true;
                return spawnPointsExist;
            }
            else
            {
                msg += "No se puede deploy más personajes en el campo | spawnPointsCount=" + spawnPointsCount;
                PlayerInputMenu.instance.ShowErrorText(msg);
                SFXManager.instance.UICancel.Play();
            }
        }
        return spawnPointsExist;
    }

}
