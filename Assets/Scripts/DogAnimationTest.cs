using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogAnimationTest : CharacterController
{
    public enum DogTypeList
    {
        Coyote,
        Dalmatian,
        DalmatianCollar,
        Doberman,
        DobermanCollar,
        Fox,
        GermanShepherd,
        GermanShepherdCollar,
        GoldenRetriever,
        GoldenRetrieverCollar,
        DogGreyhound,
        GreyhoundCollar,
        HellHound,
        Husky,
        HuskyCollar,
        Labrador,
        LabradorCollar,
        Pointer,
        PointerCollar,
        Ridgeback,
        RidgebackCollar,
        Robot,
        Scifi,
        Shiba,
        Shiba_Collar,
        Wolf,
        ZombieDoberman,
        ZombieGermanShepherd
    };
    Transform[] children;
    static private string[] dogLabels;
    static private string[] DogNewTypes; // Dog Types
    Animator dogAnim;// Animator for the assigned dog

    /// < Dog controller variables>
    [Header("Animation Control")]
        bool attackMode;
        int  attackType;
        bool walkPressed;
        bool turnBack;
        bool leftTurn;
        bool rightTurn;
        bool jumpPressed;
        bool runPressed;
        bool sitPressed;
        bool sleepPressed;
        bool deathPressed;
        bool resetPressed;
        bool a1Pressed;
        bool a2Pressed;
        bool a3Pressed;
        bool a4Pressed;
        bool a5Pressed;
        bool a6Pressed;
        bool a7Pressed;
        bool a8Pressed;
        bool a9Pressed;
        bool a10Pressed;
        bool a11Pressed;
        bool a12Pressed;
        bool a13Pressed;
    /// </Dog controller variables>

    bool dogActionEnabled;
    public float timeRemaining = 1.0f;
    private int countDown = 1;
    //bool Movement_f;
    //bool death_b = false;
    bool Sleep_b = false;
    bool Sit_b = false;
    [Header("Movement Control")]
    private float w_movement = 0.0f; // Run value
    public float acceleration = 1.0f;
    public float decelleration = 1.0f;
    private float maxWalk = 0.5f;
    private float maxRun = 1.0f;
    private float currentSpeed;
    private Transform getDogName;
    [Header("Particle FX")]
    public ParticleSystem poopFX;
    public ParticleSystem dirtFX;
    public ParticleSystem peeFX;
    public ParticleSystem waterFX;
    private Vector3 newSpawn = new Vector3();
    public Transform fxTransform;
    public Transform fxTail;
    void Start() // On start store dogKeyCodes
    {
        InitDogCharacterController();

        InitBoolControllerVariables();
        InitLabelValueVariables();

        dogAnim = GetComponent<Animator>(); // Get the animation component
     }

    void Update()
    {
        if (GameManager.instance.isPaused == false)
        {
            CharacterMovement();
            //ShotLineController();
            DogAnimationController();
        }
    }

    private void DogAnimationController()
    {
        if (attackMode)
        {
            dogAnim.SetBool("AttackReady_b", true);
            dogAnim.SetInteger("AttackType_int", attackType);
        }
        else
        {
            dogAnim.SetBool("AttackReady_b", false);
            dogAnim.SetInteger("AttackType_int", 0);
        }

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

        
        if (leftTurn)
        {
            if (w_movement > 0.25 && w_movement < 0.75)
            {
                transform.Rotate(Vector3.up * Time.deltaTime * -45, Space.Self);
            }
            if (w_movement > 0.75)
            {
                transform.Rotate(Vector3.up * Time.deltaTime * -65, Space.Self);
            }
            if (w_movement < 0.25)
            {
                dogAnim.SetInteger("TurnAngle_int", -90);
            }
        }
        else if (rightTurn)
        {
            if (w_movement > 0.25 && w_movement < 0.75)
            {
                transform.Rotate(-Vector3.down * Time.deltaTime * 45, Space.Self);
            }
            if (w_movement > 0.75)
            {
                transform.Rotate(-Vector3.down * Time.deltaTime * 65, Space.Self);
            }
            if (w_movement < 0.25)
            {
                dogAnim.SetInteger("TurnAngle_int", 90);
            }
        }
        else if (turnBack)
        {
            dogAnim.SetInteger("TurnAngle_int", 180);
        }
        else
        {
            dogAnim.SetInteger("TurnAngle_int", 0);
        }
        if (jumpPressed)
        {
            dogAnim.SetTrigger("Jump_tr");
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
        if (sleepPressed) // Sleep
        {
            if (Sleep_b == false)
            {
                Sleep_b = true;
            }
            else if (Sleep_b == true)
            {
                Sleep_b = false;
            }
            dogAnim.SetBool("Sleep_b", Sleep_b); // Set sleep animation
        }
        if (deathPressed)
        {
            dogAnim.SetBool("Death_b", true);  // Kill the dog 
        }
        else
        {
            dogAnim.SetBool("Death_b", false);  // Revive the dog 
        }
        if (resetPressed)
        {
            dogAnim.Rebind();
            dogAnim.Update(0f);
        }
        if (a1Pressed && !dogActionEnabled)
        {
            StartCoroutine(DogActions(1));
        }
        if (a2Pressed && !dogActionEnabled)
        {
            StartCoroutine(DogActions(2));
        }
        if (a3Pressed && !dogActionEnabled)
        {
            StartCoroutine(DogActions(3));
        }
        if (a4Pressed && !dogActionEnabled)
        {
            StartCoroutine(DogActions(4));
            if (!Sit_b)
            {
                ParticleSystem go = Instantiate(dirtFX, new Vector3(this.transform.position.x, fxTransform.transform.position.y, fxTransform.transform.position.z), this.transform.rotation);
                go.transform.SetParent(fxTransform);
                go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, go.transform.localPosition.z + 0.3f);
            }
        }
        if (a5Pressed && !dogActionEnabled)
        {
            StartCoroutine(DogActions(5));
        }
        if (a6Pressed && !dogActionEnabled)
        {
            StartCoroutine(DogActions(6));
        }
        if (a7Pressed && !dogActionEnabled)
        {
            StartCoroutine(DogActions(7));
        }
        if (a8Pressed && !dogActionEnabled)
        {
            StartCoroutine(DogActions(8));
            if (!Sit_b)
            {
                ParticleSystem go = Instantiate(peeFX, new Vector3(this.transform.position.x, fxTransform.transform.position.y + 0.5f, fxTransform.transform.position.z - 0f), this.transform.rotation);
                go.transform.SetParent(fxTransform);
                go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, go.transform.localPosition.z - 0.2f);
                go.transform.localRotation = Quaternion.Euler(0, -45, 0);
            }
        }
        if (a9Pressed && !dogActionEnabled)
        {
            StartCoroutine(DogActions(9));
            if (!Sit_b)
            {
                ParticleSystem go = Instantiate(poopFX, new Vector3(this.transform.position.x, fxTransform.transform.position.y + 0.5f, fxTransform.transform.position.z - 0f), this.transform.rotation);
                go.transform.SetParent(fxTransform);
                go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, go.transform.localPosition.z - 0.35f);
            }
        }
        if (a10Pressed && !dogActionEnabled)
        {
            StartCoroutine(DogActions(10));
            if (!Sit_b)
            {
                ParticleSystem go = Instantiate(waterFX, new Vector3(this.transform.position.x, fxTransform.transform.position.y + 0.5f, fxTransform.transform.position.z - 0f), this.transform.rotation);
                go.transform.SetParent(fxTransform);
                go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y - 0.0f, go.transform.localPosition.z);
                go.gameObject.transform.GetChild(0).transform.position = new Vector3(fxTail.transform.position.x, fxTail.transform.position.y, fxTail.transform.position.z);
            }
        }
        if (a11Pressed && !dogActionEnabled)
        {
            StartCoroutine(DogActions(11));
        }
        if (a12Pressed && !dogActionEnabled)
        {
            StartCoroutine(DogActions(12));
        }
        if (a13Pressed && !dogActionEnabled)
        {
            StartCoroutine(DogActions(13));
        }
        dogAnim.SetTrigger("Blink_tr"); // Blink will continue unless asleep or dead
        dogAnim.SetFloat("Movement_f", w_movement); // Set movement speed for all required parameters
        //navAgent.speed = w_movement;

    }

    IEnumerator DogActions(int actionType) // Dog action coroutine
    {
        dogActionEnabled = true; // Enable the dog animation flag
        dogAnim.SetInteger("ActionType_int", actionType); // Enable Animation
        yield return new WaitForSeconds(countDown); // Countdown
        dogAnim.SetInteger("ActionType_int", 0); // Disable animation
        dogActionEnabled = false; // Disable the dog animation flag

        InitBoolControllerVariables();
    }
    private void InitBoolControllerVariables()
    {
        attackMode = false;
        attackType = 0;
        walkPressed = false;
        turnBack = false;
        leftTurn = false;
        rightTurn = false;
        jumpPressed = false;
        runPressed = false;
        sitPressed = false;
        sleepPressed = false;
        deathPressed = false;
        resetPressed = false;
        a1Pressed = false;
        a2Pressed = false;
        a3Pressed = false;
        a4Pressed = false;
        a5Pressed = false;
        a6Pressed = false;
        a7Pressed = false;
        a8Pressed = false;
        a9Pressed = false;
        a10Pressed = false;
        a11Pressed = false;
        a12Pressed = false;
        a13Pressed = false;
    }
    private void InitLabelValueVariables()
    {
        currentSpeed = 0.0f;
        DogNewTypes = new string[]
        {
        "Coyote",
        "Dalmatian",
        "DalmatianCollar",
        "Doberman",
        "DobermanCollar",
        "Fox",
        "GermanShepherd",
        "GermanShepherdCollar",
        "GoldenRetriever",
        "GoldenRetrieverCollar",
        "DogGreyhound",
        "GreyhoundCollar",
        "HellHound",
        "Husky",
        "HuskyCollar",
        "Labrador",
        "LabradorCollar",
        "Pointer",
        "PointerCollar",
        "Ridgeback",
        "RidgebackCollar",
        "Robot",
        "Scifi",
        "Shiba",
        "Shiba_Collar",
        "Wolf",
        "ZombieDoberman",
        "ZombieGermanShepherd"
        };
        dogLabels = new string[] // Labels for UI
        {
        "Attack: ",
        "Second Attack: ",
        "Forward: ",
        "Backward: ",
        "Left: ",
        "Right: ",
        "Action: ",
        "Jump: ",
        "Run: ",
        "Sit: ",
        "Sleep: ",
        "Exit Playmode: ",
        "Death: ",
        "Reset: ",
        "Action 1: ",
        "Action 2: ",
        "Action 3: ",
        "Action 4: ",
        "Action 5: ",
        "Action 6: ",
        "Action 7: ",
        "Action 8: ",
        "Action 9: ",
        "Action 10: ",
        "Action 11: ",
        "Action 12: ",
        "Action 13: "
        };
        children = GetComponentsInChildren<Transform>();
        for (int x = 0; x < children.Length; x++) // Search for dog names
        {
            if (children[x].name.Contains("Dogs"))
            {
                getDogName = children[x];
            }
        }
        newSpawn = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
    }

    private void InitDogCharacterController()
    {
        isDogger = true;
        moveRange = 3.5f; runRange = 8f;
        isMoving = false; isMeleeing = false; isRunning = false; isShooting = false; isDefending = false; isDamaged = false; isKnockedOut = false;
        meleeRange = 2f; meleeDamage = 5f;
        shootRange = 10f; shootDamage = 3f;
        currentMeleeTarget = 0; currentShootTarget = 0; //Default no hay enemigos en rango.
        UpdateHealthDisplay(); //Inicializamos el UI de Health.
        shootLine.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        shootLine.transform.SetParent(null);
        shotRemainTime = 0.5f;
        rotateSpeed = 45f;
        isRotating = false;

        w_movement = 0.0f; // Run value
        acceleration = 1.0f;
        decelleration = 1.0f;
        maxWalk = 0.5f;
        maxRun = 1.0f;
    }

    public void FunctionDogKnockedOut()
    {
        deathPressed = true;
    }
    public void FunctionDogWalking(bool isWalking, bool isRunning)
    {
        walkPressed = isWalking;
        runPressed = isRunning;
    }



    /// <FunctionDogAttackType>
    /// <param name="atkMode"></param>
    //Set this parameter to make the Dog get into an Attack ready position
    /// <param name="atkType"></param>
    //Set this parameter greater than zero to make the Dog Attack (While in a Ready position)
    //  0 - 	No Attack
    //  1 - 	Bite
    //  2 - 	Claw
    //  3 - 	Hit
    //  4 - 	Stun
    /// </summary>
    public void FunctionDogAttackType(bool atkMode, int atkType)
    {
        attackMode = atkMode;
        attackType = atkType;
        StartCoroutine(ResetDogAttackType());
    }

    //Se encarga de devolver la animación de attackType a 0 cuando termine los frames de animación
    private IEnumerator ResetDogAttackType()
    {
        int timeToWaitPerAnimation;
        switch (attackType)
        {
            case 0:
                timeToWaitPerAnimation = 30;
                break;
            case 1:
                timeToWaitPerAnimation = 36;
                break;
            case 2:
                timeToWaitPerAnimation = 37;
                break;
            case 3:
                timeToWaitPerAnimation = 15;
                break;
            case 4:
                timeToWaitPerAnimation = 65;
                break;
            default:
                timeToWaitPerAnimation = 1;
                Debug.Log("Hubo error en el ResetDogAttackType()");
                break;
        }

        yield return StartCoroutine(WaitFor.Frames(timeToWaitPerAnimation));
        if(attackType == 0) //Si ya terminó de atacar, se quita la animacion de ready.
        {
            attackMode = false;
            isDamaged = false;
            isMeleeing = false;
        }
        else // Si uso otra animacion que no es terminar de atacar, se manda llama quitando la animación.
        {
            //Quita la animacion.
            FunctionDogAttackType(true,0);
        }
    }
    public IEnumerator FunctionDogAction(bool isStanding, int actionType)
    {
        int timeToWaitPerAnimation;

        switch (actionType)
        {
            case 0: //No Action
                timeToWaitPerAnimation = 3;
                Debug.Log("Action: 0");
                break;
            case 1:  //Bark
                timeToWaitPerAnimation = 87;
                a1Pressed = true;
                break;
            case 2: // Beg
                timeToWaitPerAnimation = 147;
                a2Pressed = true;
                break;
            case 3: // Cower
                timeToWaitPerAnimation = 180;
                a3Pressed = true;
                break;
            case 4: // Dig
                timeToWaitPerAnimation = 170;
                a4Pressed = true;
                break;
            case 5: // Eat
                timeToWaitPerAnimation = 170;
                a5Pressed = true;
                break;
            case 6: // Howl
                timeToWaitPerAnimation = 200;
                a6Pressed = true;
                break;
            case 7: // Drink
                timeToWaitPerAnimation = 150;
                a7Pressed = true;
                break;
            case 8: // Pee
                timeToWaitPerAnimation = 108;
                a8Pressed = true;
                break;
            case 9: // Poo
                timeToWaitPerAnimation = 230;
                a9Pressed = true;
                break;
            case 10: // Shake
                timeToWaitPerAnimation = 195;
                a10Pressed = true;
                break;
            case 11: // Sniff
                timeToWaitPerAnimation = 170;
                a11Pressed = true;
                break;
            case 12:  //Yawn
                timeToWaitPerAnimation = 160;
                a12Pressed = true;
                break;
            case 13: // ShakeToy
                timeToWaitPerAnimation = 135;
                a13Pressed = true;
                break;
            default:
                timeToWaitPerAnimation = 3;
                Debug.Log("Default Action: null");
                break;
        }
        yield return StartCoroutine(WaitFor.Frames(timeToWaitPerAnimation));

    }


}
