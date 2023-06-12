using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveGrid : MonoBehaviour
{
    // <>
    public static MoveGrid instance;

    private void Awake()
    {
        instance = this;
        GenerateMoveGrid();  //Se crea el Grid de movimiento.
        HideMovePoints(); //Ocultamos visualmente.
    }

    public MovePoint startPoint;
    public Vector2Int spawnRange;

    public LayerMask whatIsGround, whatIsObstacle;

    public float obstacleRangeCheck;

    public bool mgIsGenerated = false;

    //Almaceamos los spawnPoints en una lista.
    public List<MovePoint> allMovePoints = new List<MovePoint>();

    //Se crea la plantilla de un grid de movimiento.
    public void GenerateMoveGrid()
    {
        //Se loopea en X y Y en una cuadrícula cada spawnPoint.
        for (int x = -spawnRange.x; x <= spawnRange.x; x++)
        {
            for (int y = -spawnRange.y; y <= spawnRange.y; y++)
            {
                //Antes de instanciar un point, se verifica raycast.

                RaycastHit hit;
                //Si este raycast dispara hacia el layer e Ground, entonces se permite crear un spawnPoint.
                if (Physics.Raycast(transform.position + new Vector3(x, 10f, y), Vector3.down, out hit, 20f, whatIsGround))
                {
                    //Vamos a buscar si hay Obstacles  en el spawnpoint.
                    //Se crea una esfera en el hit.point, checa si colisiona con obstaculos y almacena cada colisión en un arreglo.
                    //Si el tamaño del arreglo es 0, entonces crea el spawnpoint.
                    if(Physics.OverlapSphere(hit.point, obstacleRangeCheck,whatIsObstacle).Length == 0)
                    {
                        MovePoint newPoint = Instantiate(startPoint, hit.point, transform.rotation); //Se crea un gameObject spawnPoint.
                        newPoint.transform.SetParent(transform); //Se almacena en un newPoint en el Parent, que hace cada gameObject bajo jerarquía del Movement Grid.

                        allMovePoints.Add(newPoint); //Se almacena el nuevo spawnPoint.
                    }
                }
            }
        }

        startPoint.gameObject.SetActive(false);
        mgIsGenerated = true;
    }

    //Se desactivan todos los spawnpoints para no mostrarse en el mapa. Deben estar previamente creados.
    public void HideMovePoints()
    {
        foreach(MovePoint mp in allMovePoints)
        {
            mp.gameObject.SetActive(false);
        }
    }

    public void ShowPointsInRange(float moveRange, Vector3 centerPoint)
    {
        HideMovePoints();   //Ocultamos los puntos de movimiento antes de mostrar cualquiera otros puntos de movimientos.

        foreach (MovePoint mp in allMovePoints) //Recoremos el listado de los puntos del mapa.
        {
            //Se evalua cada punto del mapa disponible, con el centro (param). Si esa distancia es menor o igual en el rango de movimiento (param), entonces dibuja el punto e el mapa (se activa).
            if (Vector3.Distance(centerPoint, mp.transform.position) <= moveRange)
            {
                mp.gameObject.SetActive(true);

                //Se revisará que no pueda tocar ningun punto donde algun jugado esta parado, si hay alguien ahí, se desdibuja (desactiva).
                foreach (CharacterController cc in GameManager.instance.allChars)
                {
                    if(Vector3.Distance(cc.transform.position, mp.transform.position) < 0.5f)
                    {
                        mp.gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    //Se crea virtualmente un rango de movimiento para que los AI puedan seleccionar a dónde moverse sin mostrar Grid.
    public List<MovePoint> GetMovePointsInRange(float moveRange, Vector3 centerPoint)
    {
        List<MovePoint> foundPoint = new List<MovePoint>();

        foreach (MovePoint mp in allMovePoints) //Recoremos el listado de los puntos del mapa.
        {
            //Se evalua cada punto del mapa disponible, con el centro (param). Si esa distancia es menor o igual en el rango de movimiento (param), entonces dibuja el punto e el mapa (se activa).
            if (Vector3.Distance(centerPoint, mp.transform.position) <= moveRange)
            {
                bool shouldAdd = true; //Se crea una variable que menciona si el punto se debe agregar al listado creado para el AI.

                //Se revisará que no pueda tocar ningun punto donde algun jugado esta parado, si hay alguien ahí, se desdibuja (desactiva).
                foreach (CharacterController cc in GameManager.instance.allChars)
                {
                    if (Vector3.Distance(cc.transform.position, mp.transform.position) < 0.5f)
                    {
                        shouldAdd = false;
                    }
                }

                if(shouldAdd == true) //Si encontró un punto en la distancia que pueda recorrer el AI.
                {
                    foundPoint.Add(mp); //Agrega al listado.
                }
            }
        }
        return foundPoint;
    }
}

