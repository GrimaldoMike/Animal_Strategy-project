using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;

public class DogController : CharacterController
{

    /// <summary>
    /// Dog variables
    public float w_movement; // Run value
    public float acceleration;
    public float decelleration;
    public float maxWalk;
    public float maxRun;
    public float currentSpeed;
    public bool  isReadyToAttack;

    /// </summary>


    // Start is called before the first frame update
    void Start()
    {
        InitDogCharacterController();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.isPaused == false)
        {
            AnimationController();
        }
    }

    private void AnimationController()
    {
        /*
        if (isRunning)
        {
            currentSpeed = maxRun;
        }
        if (!isRunning)
        {
            currentSpeed = maxWalk;
        }
        if (isMoving && (w_movement < currentSpeed)) // If walking
        {
            w_movement += Time.deltaTime * acceleration;
        }
        if (isMoving && !isRunning && w_movement > currentSpeed) // Slow down
        {
            w_movement -= Time.deltaTime * decelleration;
        }
        if (!isMoving && w_movement > 0.0f) // If no longer walking
        {
            w_movement -= Time.deltaTime * decelleration;
        }
        */
        if (isRunning)
        {
            w_movement = maxRun;
        }
        else if (isMoving)
        {
            w_movement = maxWalk;
        }
        else
        {
            w_movement = 0f;
        }

        if (isReadyToAttack == true)
        {
            anim.SetBool("AttackReady_b", true);
            anim.SetInteger("AttackType_int", 0);
        }
        else
        {
            anim.SetInteger("AttackType_int", 0);
            anim.SetBool("AttackReady_b", false);
        }
        if (isMeleeing)
        {
            anim.SetInteger("AttackType_int", 1);
        }
        else
        {
            anim.SetInteger("AttackType_int", 0);
        }
        if (isDamaged == true)
        {
            isDamaged = false;
            anim.SetBool("AttackReady_b", true);
            anim.SetInteger("AttackType_int", 3);
        }
        else
        {
            anim.SetInteger("AttackType_int", 0);
            anim.SetBool("AttackReady_b", false);
        }
        if (isKnockedOut == true)
        {
            isKnockedOut = false;
            anim.SetBool("Death_b", true);
        }

        anim.SetFloat("Movement_f", w_movement); // Set movement speed for all required parameters
        //navAgent.speed = w_movement;

        if (w_movement > 0)
        {
            anim.SetInteger("AttackType_int", 0);
            anim.SetBool("AttackReady_b", false);
        }
        else if (!isReadyToAttack && !isMeleeing && !isKnockedOut && !isMoving && !isRunning && !isDamaged)
        {
            anim.SetInteger("ActionType_int", 0);
        }
    }

    //Revisa si hay contrincantes a la distancia de "meleeRange". Si es así, los coloca en la lista de objetivos meleeTargets.
    public void GetMeleeTargetsDogs()
    {
        meleeTargets.Clear();

        if (isKnockedOut == false) // Se valida si un personaje en el suelo es un objetivo para melee.
        {
            if (isEnemy == false)
            {
                foreach (CharacterController cc in GameManager.instance.enemyTeam) //Revisa objetivos del jugador.
                {
                    if (Vector3.Distance(transform.position, cc.transform.position) < meleeRange)
                    {
                        meleeTargets.Add(cc);
                    }
                }
            }
            else
            {
                foreach (CharacterController cc in GameManager.instance.playerTeam) //Revivsa objetivos del enemigo.
                {
                    if (Vector3.Distance(transform.position, cc.transform.position) < meleeRange)
                    {
                        meleeTargets.Add(cc);
                    }
                }
            }
        }

        //Revisamos si el numero de objetivos cambio al navegar en los menus.
        if (currentMeleeTarget >= meleeTargets.Count)
        {
            currentMeleeTarget = 0;
        }
    }

    public void SetReadyPosition()
    {
        GetMeleeTargetsDogs();
        //Revisamos si el numero de objetivos cambio al navegar en los menus.
        if (meleeTargets.Count >= 1)
        {
            isReadyToAttack = true;
            meleeTargets.Clear();
        }
    }


    private void InitDogCharacterController()
    {
        moveRange = 3.5f; runRange = 8f;
        isMoving = false; isMeleeing = false; isRunning = false; isShooting = false; isDefending = false; isDamaged = false; isKnockedOut = false;
        meleeRange = 2f; meleeDamage = 5f;
        shootRange = 10f; shootDamage = 3f;
        currentMeleeTarget = 0; currentShootTarget = 0; //Default no hay enemigos en rango.
        UpdateHealthDisplay(); //Inicializamos el UI de Health.
        shootLine.transform.position = Vector3.zero;
        shootLine.transform.rotation = Quaternion.identity;
        shootLine.transform.SetParent(null);
        shotRemainTime = 0.5f;
        rotateSpeed = 45f;
        isRotating = false;

        w_movement = 0.0f; // Run value
        acceleration = 1.0f;
        decelleration = 1.0f;
        maxWalk = 0.5f;
        maxRun = 1.0f;
        isReadyToAttack = false;
    }

}