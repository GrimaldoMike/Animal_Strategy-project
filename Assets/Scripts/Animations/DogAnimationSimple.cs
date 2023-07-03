using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogAnimationSimple : CharacterController
{
    static private string[] DogNewSimpleTypes; // Dog Types
    Animator dogAnim;// Animator for the assigned dog
    public AudioSource walkingSFX;
    public AudioSource runningSFX;

    bool deathPressed;
    bool attackMode;
    bool walkPressed;
    bool runPressed;
    bool sitPressed;
    bool Sit_b = false;

    [Header("Movement Control")]
    private float w_movement = 0.0f; // Run value
    public float acceleration = 1.0f;
    public float decelleration = 1.0f;
    private float maxWalk = 0.5f;
    private float maxRun = 1.0f;
    private float currentSpeed;

    public List<GameObject> dogSimpleTypeAndColorList;

    void Start() // On start store dogKeyCodes
    {
        InitDogCharacterController();
        InitLabelValueVariables();

        dogAnim = GetComponent<Animator>(); // Get the animation component
    }

    void Update()
    {
        if (GameManager.instance.isPaused == false)
        {
            CharacterMovement();
            ShotLineController();
            DogSimpleAnimationController();
        }
    }

    private void DogSimpleAnimationController()
    {
        if (isDogger == false)
        {
            if (runPressed)
            {
                currentSpeed = maxRun;
            }
            if (!runPressed)
            {
                currentSpeed = maxWalk;
            }
            if (walkPressed && (w_movement < currentSpeed)) // If walking
            {
                w_movement += Time.deltaTime * acceleration;
            }
            if (walkPressed && !runPressed && w_movement > currentSpeed) // Slow down
            {
                w_movement -= Time.deltaTime * decelleration;

            }
            if (!walkPressed && w_movement > 0.0f) // If no longer walking
            {
                w_movement -= Time.deltaTime * decelleration;
            }


            if (sitPressed) // Sit
            {
                if (Sit_b == false)
                {
                    Sit_b = true;
                }
                else if (Sit_b == true)
                {
                    Sit_b = false;
                }
                dogAnim.SetBool("Sit_b", Sit_b); // Set sit animation
            }
            if (isMeleeing == true)
            {
                isMeleeing = false;
                anim.SetTrigger("AttackReady_t");
            }
            if (isDamaged == true)
            {
                isDamaged = false;
                anim.SetTrigger("a_damaged");
            }
            if (deathPressed)
            {
                dogAnim.SetBool("Death_b", true);  // Kill the dog 
            }
            else
            {
                dogAnim.SetBool("Death_b", false);  // Revive the dog 
            }

            dogAnim.SetFloat("Speed_f", w_movement); // Set movement speed for all required parameters
                                                        //navAgent.speed = w_movement;
            if (w_movement > 0.5f)
            {
                runningSFX.gameObject.SetActive(true);
            }
            else if (w_movement > 0f)
            {
                walkingSFX.gameObject.SetActive(true);
            }
            else
            {
                runningSFX.Stop();
                runningSFX.gameObject.SetActive(false);
                walkingSFX.Stop();
                walkingSFX.gameObject.SetActive(false);
            }
        }
    }
    public void FunctionDogKnockedOut()
    {
        deathPressed = true;
    }

    private void InitLabelValueVariables()
    {
        DogNewSimpleTypes = new string[]
        {
        "Beagle",
        "BorderCollie",
        "Bulldog",
        "BullTerrier",
        "Chihuahua",
        "Doberman",
        "GermanShepherd",
        "Labrador",
        "Poodle",
        "Pug",
        "RhodesianRidgeback",
        "SaintBernard"
        };

    }

    private void InitDogCharacterController()
    {
        currentSpeed = 0.0f;
        deathPressed = false;
        attackMode = false;
        walkPressed = false;
        runPressed = false;
        sitPressed = false;
        isDogger = true;
        moveRange = 2f; runRange = 6f;
        isMoving = false; isMeleeing = false; isRunning = false; isShooting = false; isDefending = false; isDamaged = false; isKnockedOut = false;
        meleeRange = 2f; meleeDamage = 3f;
        shootRange = 6f; shootDamage = 1.5f;
        currentMeleeTarget = 0; currentShootTarget = 0; //Default no hay enemigos en rango.
        UpdateHealthDisplay(); //Inicializamos el UI de Health.
        shootLine.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        shootLine.transform.SetParent(gameObject.transform);
        shotRemainTime = 0.5f;
        rotateSpeed = 45f;
        isRotating = false;

        w_movement = 0.0f; // Run value
        acceleration = 1.0f;
        decelleration = 1.0f;
        maxWalk = 0.5f;
        maxRun = 1.0f;
    }

}
