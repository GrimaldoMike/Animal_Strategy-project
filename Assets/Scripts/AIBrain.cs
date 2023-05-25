using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBrain : MonoBehaviour
{

    // <>

    //Solamente decide qué acciones hará el charactercontroller
    public CharacterController charaCon;

    public float waitBeforeActing = 1f, waitAfterActing = 1f, waitBeforeShooting = 0.5f;

    public float moveChance = 60f, defendChance = 30f, skipChance = 10f;

    [Range(0f, 100f)]
    public float ignoreShootChance = 20f, moveRandomChance = 25f;

    public void ChooseAction()
    {
        StartCoroutine(ChooseCo());
    }

    public IEnumerator ChooseCo()
    {
        Debug.Log(name + " choosing an action");
        yield return new WaitForSeconds(waitBeforeActing);

        bool actionTaken = false;

        ///Toma decisión de qué accion tomar:
        ///
        actionTaken = AIChecksForMelee(actionTaken);

        actionTaken = AIChecksForShoot(actionTaken);


        if (actionTaken == false) //Si no hay acciones más que hacer...
        {
            float actionDecision = Random.Range(0f, moveChance + defendChance + skipChance);
            // <>

            if (actionDecision < moveChance) // Move
            {
                AIChecksForMovement();
            } else if (actionDecision < moveChance + defendChance) // Defend
            {
                AIChecksForDefend();
            }
            else
            {
                //Skip turn
                Debug.Log(name + " skipped turn");
                GameManager.instance.EndTurn();
            }
        }
    }

    IEnumerator WaitToEndAction(float timeToWait)
    {
        Debug.Log("Waiting to end an action");
        yield return new WaitForSeconds(timeToWait);
        GameManager.instance.SpendTurnPoints();
    }

    IEnumerator WaitToShoot()
    {
        yield return new WaitForSeconds(waitBeforeShooting);

        GameManager.instance.activePlayer.isShooting = true;
        charaCon.FireShot();
        GameManager.instance.currentActionCost = 1; // El disparo causa 1 acción por turno.

        StartCoroutine(WaitToEndAction(waitAfterActing));

    }

    private bool AIChecksForMelee(bool actionTaken)
    {
        charaCon.GetMeleeTargets();

        if (charaCon.meleeTargets.Count > 0) //Si encontró enemigos en rango para hacer melee.
        {
            Debug.Log("Is meleeing");

            //Opcion 1: Selecciona un enemigo a atacar al asar
            //charaCon.currentMeleeTarget = Random.Range(0, charaCon.meleeTargets.Count); 

            //Opcion 2: Se selecciona atacar al de menor hp.
            float minHp = 9999;
            int i = 0;
            foreach (CharacterController cc in charaCon.meleeTargets)
            {
                if (cc.currentHealth < minHp && cc.currentHealth > 0)
                {
                    charaCon.currentMeleeTarget = i;
                    minHp = cc.currentHealth;
                }
                i++;
            }

            GameManager.instance.currentActionCost = 1; //El melee cuesta 1 punto de acción.

            StartCoroutine(WaitToEndAction(waitAfterActing));
            Debug.Log(name + " melee at " + charaCon.meleeTargets[charaCon.currentShootTarget].name);

            charaCon.DoMelee();
            actionTaken = true;
        }

        return actionTaken;
    }

    private bool AIChecksForShoot(bool actionTaken)
    {
        charaCon.GetShootTargets();

        if (actionTaken == false && charaCon.shootTargets.Count > 0) //Si no se tomó una acción de melee y sí encontré enemigos en rango
        {
            if(Random.Range(0f, 100f) > ignoreShootChance) // Se agregó una validación para saltarse la fase de disparo.
            { 
                //Se decidirá quien será disparado por porcentaje de accuracy.
                List<float> hitChances = new List<float>();

                for (int i = 0; i< charaCon.shootTargets.Count; i++) //Loopea los objetivos en rango y guardo
                {
                        charaCon.currentShootTarget = i;
                        charaCon.LookAtTarget(charaCon.shootTargets[i].transform); //Hacemos que lo vea directamente para que su rotación no afecte su accuracy.
                        hitChances.Add(charaCon.CheckShotChance()); //Almacenamos el porcentaje
                }

                float highestChance = 0f;
                for (int i = 0; i < hitChances.Count; i++ ) //Se almacena el porcentaje más alto
                {
                    if (hitChances[i] > highestChance)
                    {
                        highestChance = hitChances[i];
                        charaCon.currentShootTarget = i;
                    } else if (hitChances[i] == highestChance){ //Si 2 objetivos tienen el mismo porcentaje
                        if (Random.value > .5f) //Elije uno de ellos al azar.
                        {
                            charaCon.currentShootTarget = i; 
                        }
                    }
                }

                if(highestChance > 0f) // Mientras encntramos un enemigo en la distancia de disparo...
                {
                    charaCon.LookAtTarget(charaCon.shootTargets[charaCon.currentShootTarget].transform); //Hacemos que el AI rote hacia su enemigo para mirarlo.
                    CameraController.instance.SetFireView(); //Se cambia la camara al modo disparo.

                    actionTaken = true;

                    Debug.Log(name + " shot at " + charaCon.shootTargets[charaCon.currentShootTarget].name);
                    StartCoroutine(WaitToShoot());//Funcion que espera medio segundo antes de disparar.
                }
            }
        }
        return actionTaken;
    }

    //Se va encargar de moverse en el mapa hacia algún jugador.
    private void AIChecksForMovement()
    {
        float moveRandom = Random.Range(0f, 100f);
        List<MovePoint> potentialMovePoints = new List<MovePoint>();
        int selectedPoint = 0;


        if (moveRandom > moveRandomChance) //Se valida una probabilidad al random de moverse al jugador reposicionarse en otro lugar arbitrario.
        {
            int nearestPlayer = 0;
            for(int i = 1; i < GameManager.instance.playerTeam.Count; i++)
            {
                //Revisa la istancia de los jugadores enemigos consigo mismo. Va comparando distancias de 2 jugadore y elige moverse hacia el más cercano.
                if(Vector3.Distance(transform.position, GameManager.instance.playerTeam[nearestPlayer].transform.position)
                    > Vector3.Distance(transform.position, GameManager.instance.playerTeam[i].transform.position)) 
                {
                    nearestPlayer = i;
                }
            }

            if (Vector3.Distance(transform.position, GameManager.instance.playerTeam[nearestPlayer].transform.position) > charaCon.moveRange && //Se revisa si el AI usara RUN o MOVE dependiendo si su moveRange alcanza.
               GameManager.instance.turnPointsRemaining >= 2) //Si aun me quedan los turnPoints suficientes para poder ejecutar correr.
            {
                potentialMovePoints = MoveGrid.instance.GetMovePointsInRange(charaCon.runRange, transform.position);

                float closestDistance = 1000f;

                for (int i = 0; i < potentialMovePoints.Count; i++)
                {
                    // Elige el punto más cercano al jugador objetivo y lo actualiza por cada pasado del for, se guarda para siempre tener el más cercano para después ejecutar el movimiento.
                    if (Vector3.Distance(GameManager.instance.playerTeam[nearestPlayer].transform.position, potentialMovePoints[i].transform.position) < closestDistance)
                    {
                        closestDistance = Vector3.Distance(GameManager.instance.playerTeam[nearestPlayer].transform.position, potentialMovePoints[i].transform.position);
                        selectedPoint = i;
                    }
                }

                GameManager.instance.currentActionCost = 2;
                Debug.Log(name + "is RUNNING to " + GameManager.instance.playerTeam[nearestPlayer].name);
            }
            else //Se ejecuta el mover al jugador.
            {
                potentialMovePoints = MoveGrid.instance.GetMovePointsInRange(charaCon.moveRange, transform.position);

                float closestDistance = 1000f;

                for (int i = 0; i < potentialMovePoints.Count; i++)
                {
                    // Elige el punto más cercano al jugador objetivo y lo actualiza por cada pasado del for, se guarda para siempre tener el más cercano para después ejecutar el movimiento.
                    if (Vector3.Distance(GameManager.instance.playerTeam[nearestPlayer].transform.position, potentialMovePoints[i].transform.position) < closestDistance)
                    {
                        closestDistance = Vector3.Distance(GameManager.instance.playerTeam[nearestPlayer].transform.position, potentialMovePoints[i].transform.position);
                        selectedPoint = i;
                    }
                }

                GameManager.instance.currentActionCost = 1;
                Debug.Log(name + "is MOVING to " + GameManager.instance.playerTeam[nearestPlayer].name);

            }

        }
        else
        {
            potentialMovePoints = MoveGrid.instance.GetMovePointsInRange(charaCon.moveRange, transform.position); //Se carga el listado de movimientos desde donde el AI esta parado.
            selectedPoint = Random.Range(0, potentialMovePoints.Count); //Se selecciona uno al azar.
            GameManager.instance.currentActionCost = 1; //Solo se moverá, no correrá gastando 2 movimientos.

            Debug.Log(name + "is MOVING to random spot");
        }

        charaCon.MoveToPoint(potentialMovePoints[selectedPoint].transform.position); //Ejecutamos el movimiento del AI al punto seleccionado anteriormente.
    }


    private void AIChecksForDefend()
    {
        Debug.Log(name + "is DEFENDING");

        charaCon.SetDefending(true);
        GameManager.instance.currentActionCost = GameManager.instance.turnPointsRemaining; //Significa que gastará todos sus puntos restantes en defender.

        StartCoroutine(WaitToEndAction(waitAfterActing)); // Hace una pausa demostrando que defendió.
    }

}
