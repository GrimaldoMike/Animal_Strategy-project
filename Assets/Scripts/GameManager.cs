using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // <>

    public static GameManager instance;

    public void Awake()
    {
        instance = this;
    }
    public CharacterController activePlayer;

    public List<CharacterController> allChars = new List<CharacterController>();
    public List<CharacterController> playerTeam = new List<CharacterController>(), enemyTeam = new List<CharacterController>();

    private int currentChar;

    //Acciones por turno.
    public int totalTurnPoints;
    [HideInInspector]
    public int turnPointsRemaining;
    public int currentActionCost;

    public GameObject targetDisplay;

    public bool shouldSpawnAtRandomPoint;
    public List<Transform> playerSpawnPoints = new List<Transform>();
    public List<Transform> enemySpawnPoints = new List<Transform>();

    public bool matchEnded;
    public string levelToLoad;

    public bool isPaused;
    public bool lastTurnBelongsToEnemy;


    // Start is called before the first frame update
    void Start()
    {
        BattleManager.instance.UpdateBattleState(); // Primer update a battle START.
        InitGameManagerValues();
        //SetUpCharactersInPlay();
    }

    public void SetUpCharactersInPlay()
    {
        SettingUpTurnOrder();

        activePlayer = allChars[0]; // Se agrega el primer personaje de la lista como el activo.
        lastTurnBelongsToEnemy = activePlayer.isEnemy;

        SettingUpSpawnPointsOnMap();

        CameraController.instance.SnapBackToPlayer(true); //La camara enfoca al jugador activo. Se manda true para que se active.

        //Se inicializa con -1 para que cuando EndTurn() suceda, inicialize el activo con el 0.
        currentChar = -1;
        //EndTurn();
    }

    //
    private void SettingUpTurnOrder()
    {
        List<CharacterController> tempList = new List<CharacterController>();
        tempList.AddRange(FindObjectsOfType<CharacterController>()); //Se guarda cada personaje con script de CharacterController(los que se mueven en el plano).

        //////////////Codigo que ordena el orden de movimiento completamente al azar. //////////////
        while (tempList.Count > 0)
        {
            int randomPick = Random.Range(0, tempList.Count);
            allChars.Add(tempList[randomPick]);

            tempList.RemoveAt(randomPick);
        }

        foreach (CharacterController cc in allChars)
        {
            // Se divide la lista personajes controlados por jugador y enemigos, así es más fácil asignar prioridad en los turnos.
            if (cc.isEnemy == false)
            {
                playerTeam.Add(cc);
            }
            else
            {
                enemyTeam.Add(cc);
            }
        }

        //Se borra la lista primero. Despues se define un orden es al azar. Puede ir primero el jugador o el enemigo.
        allChars.Clear();

        if (Random.value >= 0.5)
        {
            allChars.AddRange(playerTeam);
            allChars.AddRange(enemyTeam);
        }
        else
        {
            allChars.AddRange(enemyTeam);
            allChars.AddRange(playerTeam);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////
    }

    //Funcion que decide, en base al gameobject de SpawnPoint, spawn aleatorio de player y enemy.
    private void SettingUpSpawnPointsOnMap()
    {
        if(shouldSpawnAtRandomPoint == true) // Mientras en el GameManager se elija ser spawn point aleatorio. (los que no son aleatorios seran decidos por Unity)
        {
            foreach (CharacterController cc in playerTeam)
            {
                if (playerSpawnPoints.Count > 0) // Mientras aun existan Players pendientes de asignar spawn
                {
                    int pos = Random.Range(0, playerSpawnPoints.Count);
                    cc.transform.position = playerSpawnPoints[pos].position;
                    playerSpawnPoints.RemoveAt(pos);
                }
            }
            foreach (CharacterController cc in enemyTeam)
            {
                if (enemySpawnPoints.Count > 0) // Mientras aun existan Players pendientes de asignar spawn
                {
                    int pos = Random.Range(0, enemySpawnPoints.Count);
                    cc.transform.position = enemySpawnPoints[pos].position;
                    enemySpawnPoints.RemoveAt(pos);
                }
            }
        }
    }

    public void ReturnToPoint()
    {
        activePlayer.transform.position = activePlayer.originalPosition;
        activePlayer.navAgent.SetDestination(activePlayer.originalPosition);
        activePlayer.isMoving = false;
    }

    //Funcion de referencia para scripts de fuera.
    public void FinishedMovement()
    {
        //EndTurn();
        /*
        if (GameManager.instance.activePlayer.isDogger)
        {
            GameManager.instance.activePlayer.SetReadyPosition();
        }
        */
        SpendTurnPoints();
    }

    //Funcion que controla las acciones por turno.
    public void SpendTurnPoints()
    {
        turnPointsRemaining -= currentActionCost; //Se cambia -1 a currentActionPoints. Esto maneja diferentes acciones causan diferentes costos.

        CheckForVictory();
        
        if(matchEnded == false)
        {
            if (turnPointsRemaining <= 0)
            {
                EndTurn();
            }
            else
            {
                if (activePlayer.isEnemy == false)
                {
                    //MoveGrid.instance.ShowPointsInRange(activePlayer.moveRange, activePlayer.transform.position); //Mientras sea el turno del jugador, muestra los move points asignados a ese personaje.
                    PlayerInputMenu.instance.ShowInputMenu();//Se muestran los botones del jugador porque es turno del jugador.
                }
                else
                {
                    PlayerInputMenu.instance.HideMenus(); //Se ocultan los botones del jugador porque es turno del enemigo.
                    activePlayer.brain.ChooseAction(); // Se manda llamar las decisiones de AI.
                }
            }
        }
        PlayerInputMenu.instance.UpdateTurnPointsText(turnPointsRemaining);
    }

    public void EndTurn()
    {
        CheckForVictory();

        if (matchEnded == false)
        {
            //Se lleva el control de turno
            currentChar++;
            if (currentChar >= allChars.Count)
            {
                currentChar = 0;
            }


            activePlayer = allChars[currentChar];

            if(CheckForTurnChange() == true)
            {
                return;
            }

            CameraController.instance.SnapBackToPlayer(true); //La camara enfoca al jugador nuevo activo. Se manda true para que se active.

            turnPointsRemaining = totalTurnPoints; // Se resetean las acciones por turno.

            //Se revisa quien tiene el turno.
            //Si es enemigo, Nos saltamos el turno de los enemigos, no serán controlados por el jugador.
            if (activePlayer.isEnemy == false)
            {
                //MoveGrid.instance.ShowPointsInRange(activePlayer.moveRange, activePlayer.transform.position); //Mientras sea el turno del jugador, muestra los move points asignados a ese personaje.
                PlayerInputMenu.instance.ShowInputMenu();//Se muestran los botones del jugador porque es turno del jugador.
                PlayerInputMenu.instance.turnPointsText.gameObject.SetActive(true);
            }
            else
            {
                PlayerInputMenu.instance.HideMenus(); //Se ocultan los botones del jugador porque es turno del enemigo.
                PlayerInputMenu.instance.turnPointsText.gameObject.SetActive(false);

                //StartCoroutine(AISkipCo()); //TEMPORAL.
                activePlayer.brain.ChooseAction(); // Se manda llamar las decisiones de AI.

            }

            currentActionCost = 1; //reseteamos el costo de una acción a 1. 
            PlayerInputMenu.instance.UpdateTurnPointsText(turnPointsRemaining);

            //Se quita el estado de Defensa al principio del turno de cada ActivePlayer
            activePlayer.SetDefending(false);
        }
    }

    private bool CheckForTurnChange()
    {
        bool varBool = false ;
        if (activePlayer.isEnemy != lastTurnBelongsToEnemy)
        {
            BattleManager.instance.UpdateBattleState(); // Tercer update a battle PLAYERTURN o ENEMYTURN.
            lastTurnBelongsToEnemy = activePlayer.isEnemy;
            currentChar--;

            PlayerInputMenu.instance.ShowTurnPhases(true);
            varBool = true;
        }
        return varBool;
    }

    //Corutina para pasar el turno de los enemigos. TEMPORAL.
    public IEnumerator AISkipCo()
    {
        yield return new WaitForSeconds(1f);
        EndTurn();
    }

    public void CheckForVictory()
    {
        bool allDead = true;

        foreach(CharacterController cc in playerTeam) //Se recorre si playerteam queda alguien con HP mayor a 0
        {
            if (cc.isKnockedOut == false)
            {
                allDead = false;
            }
        }

        if (allDead == true)
        {
            BattleEnded(2); //Se activa el final de la batalla. PlayerLooses();
        }
        else
        {
            allDead = true;
            foreach (CharacterController cc in enemyTeam) //Se recorre si enemyteam queda alguien con HP mayor a 0
            {
                if (cc.isKnockedOut == false)
                {
                    allDead = false;
                }
            }
            if (allDead == true)
            {
                BattleEnded(1); //Se activa el final de la batalla. PlayerWins();
            }
        }
    }

    /// Se crea una funcion BattleEnded que recibe un estado de la batalla en entero. 
    ///     battleState=1: Gano Player.  battleState=2: Gano Enemigo,  battleState=3: Empate
    ///     battleState=0: No definido (puede usarse para cinematicas que mantengan el estado de la batalla)
    private void BattleEnded(int battleState)
    {
        BattleManager.instance.UpdateBattleState(); // Cuarto update a battle WIN o LOSE.
        matchEnded = true;

        PlayerInputMenu.instance.turnPointsText.gameObject.SetActive(false);
        string resultText;

        switch (battleState)
        {
            case 1:
                resultText = "Player Wins!";
                break;
            case 2:
                resultText = "Player Loses!";
                break;
            case 3:
                resultText = "Draw!";
                break;
            default:
                resultText = "Undefined result";
                break;
        }

        PlayerInputMenu.instance.resultText.gameObject.SetActive(true);
        PlayerInputMenu.instance.resultText.text = resultText;

        PlayerInputMenu.instance.endBattleButton.SetActive(true);
    }

    public void LeaveBattle()
    {
        BattleManager.instance.UpdateBattleState(); // Quinto update a battle EPILOGUE.
        SceneManager.LoadScene(levelToLoad);
    }

    //Sirve para detener el Timescale controlado por 
    public void TimeScalingController(float times)
    {
        Time.timeScale = times;
    }

    //Se manejará remover personajes en el GameManager.
    public void RemoveCharacterFromPlay(CharacterController chara)
    {
        allChars.Remove(chara);
        if (playerTeam.Contains(chara))
        {
           playerTeam.Remove(chara);
        }
        if (enemyTeam.Contains(chara))
        {
            enemyTeam.Remove(chara);
        }
    }
    
    private void InitGameManagerValues()
    {
        shouldSpawnAtRandomPoint = true;
        matchEnded = false;
        totalTurnPoints = 2;
        currentActionCost = 1;
        isPaused = false;
        lastTurnBelongsToEnemy = false;
    }
}
