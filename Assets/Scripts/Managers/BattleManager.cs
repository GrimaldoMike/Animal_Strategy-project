using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;
    public void Awake()
    {
        instance = this;
        InitBattleManagerValues();
    }

    /// <BattleState>
    /// Se creó estados de combate para controlar los encuentros.
    ///     PROLOGUE: Se utiliza para contenido Inicial previo a un Combate: Dialogo entre personajes, animaciones, cinematicos, etc.
    ///     START: Es el seteo de un Combate: Deploy units, moverse a spawn points, acomodar items, buffs, equipo (no implementado)
    ///     PLAYERTURN: Se cambia a PLAYERTURN cuando el personaje anterior y el activePlayer son diferentes en su isEnemy. Indica que es el turno del Jugador
    ///     ENEMYTURN: Se cambia a ENEMYTURN cuando el personaje anterior y el activePlayer son diferentes en su isEnemy. Indica que es el turno del Enemigo
    ///     WIN: Estado que se logra cuando se revisa CheckForVictory() desde el GameManager y TODOS los jugadores del EnemyTeam[] tienen isKnockedOut igual a verdadero   
    ///     LOSE: Estado que se logra cuando se revisa CheckForVictory() desde el GameManager y TODOS los jugadores del PlayerTeam[] tienen isKnockedOut igual a verdadero   
    ///     END: Es el cleanup del Combate: Se muestran items, equipo, unidades, stats, etc. (no implementado) obtenido/perdido por haber ganado o perdido el Combate
    ///     EPILOGUE: Se utiliza para contenido Final posterior al Combate: Dialogo entre personajes, animaciones, cinematicos, etc.
    /// </BattleState>
    public enum BattleState { PROLOGUE, START, PLAYERTURN, ENEMYTURN, WIN, LOST, END, EPILOGUE }

    //Se guardan cuales fueron los battle states actuales y el anterior a este.
    public BattleState currentBattleState, previousBattleState;

    // <>
    //Función que se encarga de llevar el control del cambio de battle states
    public void UpdateBattleState()
    {
        string textMessage;
        bool enterOnce = false;
        switch (currentBattleState)
        {
            case BattleState.PROLOGUE:
                if(previousBattleState == currentBattleState && enterOnce == false) //Se revisa que el prologo sea el primer estado de una batalla.
                {
                    currentBattleState = BattleState.START;
                    enterOnce = true;
                    textMessage = "|| previousBattleState=" + previousBattleState +" || currentBattleState=" + currentBattleState;
                }
                else
                {
                    textMessage = "ERROR0 en:  BattleState= " + BattleState.PROLOGUE + "|| previousBattleState=" + previousBattleState + " || currentBattleState=" + currentBattleState;
                }
                break;
            case BattleState.START: //Start solo comienza cuando haya terminado el prologo. Solo puede existir 1 por escena.
                if (previousBattleState == BattleState.PROLOGUE && currentBattleState == BattleState.START && enterOnce == false)
                {
                    if (PlayerInputMenu.instance.isCutsceneOver == true)
                    {
                        previousBattleState = currentBattleState;
                        if (GameManager.instance.allChars[0].isEnemy == true)
                        {
                            currentBattleState = BattleState.ENEMYTURN;
                        }
                        else
                        {
                            currentBattleState = BattleState.PLAYERTURN;
                        }
                        enterOnce = true;
                        textMessage = "|| previousBattleState=" + previousBattleState + " || currentBattleState=" + currentBattleState;
                    }
                    else
                    {
                        textMessage = "ERROR1 en:  BattleState= " + BattleState.START + "|| previousBattleState=" + previousBattleState + " || currentBattleState=" + currentBattleState;
                    }
                }
                else
                {
                    textMessage = "ERROR2 en:  BattleState= " + BattleState.START + "|| previousBattleState=" + previousBattleState + " || currentBattleState=" + currentBattleState;
                }
                break;
            case BattleState.PLAYERTURN: // Solamente estamos en PLAYERTURN si estuve en START o ENEMYTURN
                if (previousBattleState == BattleState.START && enterOnce == false)
                {
                    previousBattleState = currentBattleState;
                    currentBattleState = BattleState.ENEMYTURN;
                    enterOnce = true;
                    textMessage = "|| previousBattleState=" + previousBattleState + " || currentBattleState=" + currentBattleState;
                }else if(previousBattleState == BattleState.ENEMYTURN && enterOnce == false)
                {
                    previousBattleState = currentBattleState;
                    if (GameManager.instance.matchEnded == true)
                    {
                        currentBattleState = BattleState.END;
                    }
                    else
                    {
                        currentBattleState = BattleState.ENEMYTURN;
                    }
                    enterOnce = true;
                    textMessage = "|| previousBattleState=" + previousBattleState + " || currentBattleState=" + currentBattleState;
                }
                else
                {
                    textMessage = "ERROR3 en:  BattleState= " + BattleState.PLAYERTURN + "|| previousBattleState=" + previousBattleState + " || currentBattleState=" + currentBattleState;
                }
                break;
            case BattleState.ENEMYTURN: // Solamente estamos en PLAYERTURN si estuve en START o PLAYERTURN
                if (previousBattleState == BattleState.START && enterOnce == false)
                {
                    previousBattleState = currentBattleState;
                    currentBattleState = BattleState.PLAYERTURN;
                    enterOnce = true;
                    textMessage = "|| previousBattleState=" + previousBattleState + " || currentBattleState=" + currentBattleState;
                }
                else if (previousBattleState == BattleState.PLAYERTURN && enterOnce == false)
                {
                    previousBattleState = currentBattleState;
                    if (GameManager.instance.matchEnded == true)
                    {
                        currentBattleState = BattleState.END;
                    }
                    else
                    {
                        currentBattleState = BattleState.PLAYERTURN;
                    }
                    enterOnce = true;
                    textMessage = "|| previousBattleState=" + previousBattleState + " || currentBattleState=" + currentBattleState;
                }
                else
                {
                    textMessage = "ERROR4 en:  BattleState= " + BattleState.ENEMYTURN + "|| previousBattleState=" + previousBattleState + " || currentBattleState=" + currentBattleState;
                }
                break;
            case BattleState.END:
                if (previousBattleState == BattleState.PLAYERTURN && enterOnce == false)
                {
                    previousBattleState = currentBattleState;
                    currentBattleState = BattleState.WIN;
                    enterOnce = false;
                    textMessage = "|| previousBattleState=" + previousBattleState + " || currentBattleState=" + currentBattleState;
                }
                else if (previousBattleState == BattleState.ENEMYTURN && enterOnce == false)
                {
                    previousBattleState = currentBattleState;
                    currentBattleState = BattleState.LOST;
                    enterOnce = true;
                    textMessage = "|| previousBattleState=" + previousBattleState + " || currentBattleState=" + currentBattleState;
                }
                else
                {
                    textMessage = "ERROR5 en:  BattleState= " + BattleState.ENEMYTURN + "|| previousBattleState=" + previousBattleState + " || currentBattleState=" + currentBattleState;
                }
                break;
            case BattleState.WIN:
                if (previousBattleState == BattleState.END && enterOnce == false)
                {
                    previousBattleState = currentBattleState;
                    currentBattleState = BattleState.EPILOGUE;
                    enterOnce = true;
                    textMessage = "|| previousBattleState=" + previousBattleState + " || currentBattleState=" + currentBattleState;
                }
                else
                {
                    textMessage = "ERROR6 en:  BattleState= " + BattleState.WIN + "|| previousBattleState=" + previousBattleState + " || currentBattleState=" + currentBattleState;
                }
                break;
            case BattleState.LOST:
                if (previousBattleState == BattleState.END && enterOnce == false)
                {
                    previousBattleState = currentBattleState;
                    currentBattleState = BattleState.EPILOGUE;
                    enterOnce = true;
                    textMessage = "|| previousBattleState=" + previousBattleState + " || currentBattleState=" + currentBattleState;
                }
                else
                {
                    textMessage = "ERROR7 en:  BattleState= " + BattleState.LOST + "|| previousBattleState=" + previousBattleState + " || currentBattleState=" + currentBattleState;
                }
                break;
            case BattleState.EPILOGUE:
                if ((previousBattleState == BattleState.WIN || previousBattleState == BattleState.LOST) && enterOnce == false)
                {
                    previousBattleState = currentBattleState;
                    currentBattleState = BattleState.EPILOGUE;
                    enterOnce = true;
                    textMessage = "Comienza el combate:  BattleState= " + BattleState.START + "|| previousBattleState=" + previousBattleState + " || currentBattleState=" + currentBattleState;
                }
                else
                {
                    textMessage = "ERROR8 en:  BattleState= " + BattleState.START + "|| previousBattleState=" + previousBattleState + " || currentBattleState=" + currentBattleState;
                }
                break;
            default:
                textMessage = "ERROR GENERAL:  BattleState= N/A || previousBattleState=" + previousBattleState + " || currentBattleState=" + currentBattleState;
                break;
        }
        Debug.Log(textMessage);
    }

    private void InitBattleManagerValues()
    {
        currentBattleState = BattleState.PROLOGUE;
        previousBattleState = BattleState.PROLOGUE;
    }
}
