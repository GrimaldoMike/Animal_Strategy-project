using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;
using System;

public class CharacterController : MonoBehaviour
{
    // <>
    //Movement variables
    [Header("Movement variables")]
    public float moveSpeed, runningSpeed;
    private Vector3 moveTarget;
    //public float moveRange, runRange;
    public bool finishedMovement;
    public Vector3 originalPosition;
    public NavMeshAgent navAgent;
    public Transform centerPoint;

    //Control and animation variables
    [Header("Control variables")]
    public bool isMeleeing, isMoving, isRunning, isShooting;
    public bool isDamaged, isKnockedOut;
    public bool isEnemy, isDogger;

    //Melee variables
    [Header("Melee variables")]
    public float meleeRange;
    [HideInInspector]
    public List<CharacterController> meleeTargets = new List<CharacterController>();
    [HideInInspector]
    public int currentMeleeTarget;
    //public float meleeDamage;

    //Fire variables
    [Header("Fire variables")]
    //public float shootRange;
    //public float shootDamage;
    [HideInInspector]
    public List<CharacterController> shootTargets = new List<CharacterController>();
    [HideInInspector]
    public int currentShootTarget;
    public Transform shootPoint;
    public Vector3 shotMissedRange;
    public LineRenderer shootLine;
    public float shotRemainTime;
    private float shotRemainCounter;
    public GameObject shotHitFX, shotMissFX;

    //Defend Variables
    public GameObject defendObject;
    public bool isDefending;

    //Health variables
    /*public float maxHealth;
    [HideInInspector]
    public float currentHealth;
    */
    [Header("Health variables")]
    public TMP_Text healthText;
    public Slider healthSlider;

    //Rotation Character variables
    public float rotateSpeed;
    public bool isRotating;
    private Transform endRotationTarget;
    Quaternion lookOnLook;

    public AIBrain brain;

    //Animation
    public Animator anim;

    //Stat definition
    [Header("Stat definition")]
    public CharacterData characterData = new CharacterData();
    public string charStatName;
    public string charStatType;
    public string charStatSubType;
    public int charStatColor;


    private void Awake()
    {
        //Se movieron al Awake para que el juego no empieze con la match y los jugadors en HP =0, o terminar�a la partida al empezar
        moveTarget = transform.position;
        navAgent.speed = moveSpeed;
        /*maxHealth = 10f;
        currentHealth = maxHealth;*/
        isKnockedOut = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        //InitCharacterControllerValues();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.isPaused == false)
        {
            ShotLineController();
        }
    }

    private void LateUpdate()
    {
        if (GameManager.instance.isPaused == false)
        {
            RotationController();//Se cambio al Update ya que queremos rotar un personaje con Time.deltaTime.
        }
    }

    public void CharacterMovement()
    {
        if (isMoving == true)
        {
            //Moving to a point with mouse.
            if (GameManager.instance.activePlayer == this) //Solamente se valida el jugador activo.
            {
                CameraController.instance.SetMoveTarget(transform.position); //Mueve la c�mara hacia la posici�n del personaje.

                //if (Vector3.Distance(transform.position, moveTarget) < 0.2f) //Si el jugador lleg� a su destino (o a una distancia muy corta)
                if (Vector3.Distance(centerPoint.position, moveTarget) < 0.2f) //Si el jugador lleg� a su destino (o a una distancia muy corta)
                {
                    isMoving = false;
                    isRunning = false;
                    CheckForCharacterInteractionController("Movement"); //Funcion que validar� las acciones seg�n el script de cada personaje.

                    if (isEnemy == false)
                    {
                        MovementDecided();
                    }
                    else
                    {
                        GameManager.instance.FinishedMovement();
                    }
                }
            }
        }
        else
        {
            navAgent.speed = 0;
        }
    }

    //Funcion que espera a que se acepte o no el movimiento. Pudiese poner un timer en el futuro para que no sea infinito el tiempo.
    private void MovementDecided()
    {
        if (PlayerInputMenu.instance.moveReturnMenu.activeSelf == false)
        {
            PlayerInputMenu.instance.ShowMoveReturnMenu();
        }
    }

    public void MoveToPoint(Vector3 pointToMoveTo)
    {
        //Guardamos la posicion actual.
        originalPosition = transform.position;

        //Se mover� por parametro hacia el punto enviado.
        moveTarget = pointToMoveTo;

        navAgent.SetDestination(moveTarget);

        navAgent.speed = moveSpeed;

        //Este if solo funciona porque sabemos que "currentActionCost" vale 1 cuando camina y 2 cuando corre. Se debera modificar si existen mas puntos de accion cuando hay movimiento.
        //if (GameManager.instance.currentActionCost >= 2)
        if (GameManager.instance.currentActionCost >= 2)
        {
            isRunning = true;
            navAgent.speed = runningSpeed;
        }
        isMoving = true;

        CheckForCharacterInteractionController("Movement"); //Funcion que validar� las acciones seg�n el script de cada personaje.
    }

    //Revisa si hay contrincantes a la distancia de "meleeRange". Si es as�, los coloca en la lista de objetivos meleeTargets.
    public void GetMeleeTargets()
    {
        meleeTargets.Clear();

        if(isKnockedOut == false) // Se valida si un personaje en el suelo es un objetivo para melee.
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

    public void DoMelee()
    {
        isMeleeing = true;
        CheckForCharacterInteractionController("Bite");

        SFXManager.instance.meleeHit.Play();
        /*meleeTargets[currentMeleeTarget].TakeDamage(meleeDamage);
        */
        meleeTargets[currentMeleeTarget].TakeDamage(characterData.CurrentStats.CurrentAttack);
    }

    public void TakeDamage(int damage)
    {
        if(isDefending == true) // Se revisa si estuvo en defensa, reduce el da�o.
        {
            //Calculo de reduccion de daño (DEFENSA) (formula: reduced= (1 - (DEF * .1) )* ATK)
            float reducedNumber = damage * (1- (characterData.CurrentStats.CurrentDefense * 0.1f));
            damage = (int)Math.Round(reducedNumber); // Se usa formula para cambiar a int el daño reducido.
        }

        //characterData.CurrentStats.CurrentHP -= damage;
        /*currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0f;
            CharacterDefeatedByHealth();
        }
        */
        characterData.CurrentStats.CurrentHP -= damage;
        if (characterData.CurrentStats.CurrentHP <= 0)
        {
            characterData.CurrentStats.CurrentHP = 0;
            CharacterDefeatedByHealth();
        }
        else
        {
            isDamaged = true;
            SFXManager.instance.takeDamage.Play();
            CheckForCharacterInteractionController("Damaged");
        }
        UpdateHealthDisplay();
    }


    public void CharacterDefeatedByHealth()
    {
        navAgent.enabled = false; //Desactiva script de navegacion.
        isKnockedOut = true; //Se activa la bandera de jugador inhabilitado para acciones.

        CheckForCharacterInteractionController("KnockedOut");

        //Se remueve del juego para que no tenga acciones.
        GameManager.instance.RemoveCharacterFromPlay(this);

        if (isEnemy == false)
        {
            SFXManager.instance.deathHuman.Play();
        }
        else
        {
            SFXManager.instance.deathRobot.Play();
        }

        GetComponent<Collider>().enabled = false; //Se quita el collider para que no choque con los jugadores activos.
        SetDefending(false);
    }

    //Actualizamos el UI del HP
    public void UpdateHealthDisplay()
    {
        /*
        healthText.text = "HP: " + currentHealth + "/" + maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
        */
        healthText.text = "HP: " + characterData.CurrentStats.CurrentHP + "/" + characterData.GeneralStats.MaxHP;
        healthSlider.maxValue = characterData.GeneralStats.MaxHP;
        healthSlider.value = characterData.CurrentStats.CurrentHP;
    }

    //Misma revision de objetivos para el disparo, copiado como el de melee.
    public void GetShootTargets()
    {
        shootTargets.Clear();
        int rangeOfShooting = 10;

        if (isKnockedOut == false) // Se valida si un personaje en el suelo es un objetivo para shoot.
        {
            if (isEnemy == false)
            {
                foreach (CharacterController cc in GameManager.instance.enemyTeam)
                {
                    //if (Vector3.Distance(transform.position, cc.transform.position) < shootRange)
                    if (Vector3.Distance(transform.position, cc.transform.position) < rangeOfShooting)
                    {
                        shootTargets.Add(cc);
                    }
                }
            }
            else
            {
                foreach (CharacterController cc in GameManager.instance.playerTeam)
                {
                    //if (Vector3.Distance(transform.position, cc.transform.position) < shootRange)
                    if (Vector3.Distance(transform.position, cc.transform.position) < rangeOfShooting)
                    {
                        shootTargets.Add(cc);
                    }
                }
            }
        }

        //Revisamos si el numero de objetivos cambio al navegar en los menus.
        if (currentShootTarget >= shootTargets.Count)
        {
            currentShootTarget = 0;
        }
    }

    public void FireShot()
    {
        if(isShooting == true)
        {
            int rangeOfShooting = 10;
            //Se obtiene la posicion del targetpoint basado en el shootpoint del oponente.
            Vector3 targetPoint = new Vector3(shootTargets[currentShootTarget].transform.position.x, shootTargets[currentShootTarget].shootPoint.position.y, shootTargets[currentShootTarget].transform.position.z);

            targetPoint.y = UnityEngine.Random.Range(targetPoint.y, shootTargets[currentShootTarget].transform.position.y + 0.25f); //Este c�digo ajusta un rango de propabilidad aleatoria en eje "y" (altura). Puede disparar entre los pies +.25 hasta el shootPoint.

            //El offset es una formula que toma el shotMissedRange al azar y calcula, dependiendo de la distancia del punto de disparo y el objetivo, una cantidad de variaci�n.
            //Entre m�s lejos, mas alto el valor deprobabilidad de fallo.
            //Si la distancia entre el punto de disparo y objetivo es m�s peque�a, la variaci�n de fallo es mucho m�s peque�a.
            Vector3 targetOffset = new Vector3(UnityEngine.Random.Range(-shotMissedRange.x, shotMissedRange.x),
                                               UnityEngine.Random.Range(-shotMissedRange.y, shotMissedRange.y),
                                               UnityEngine.Random.Range(-shotMissedRange.z, shotMissedRange.z));
            //targetOffset = targetOffset * (Vector3.Distance(targetPoint, shootPoint.position) / shootRange);
            //targetOffset = targetOffset * (Vector3.Distance(shootTargets[currentShootTarget].transform.position, transform.position) / shootRange);
            targetOffset = targetOffset * (Vector3.Distance(shootTargets[currentShootTarget].transform.position, transform.position) / rangeOfShooting);

            targetPoint += targetOffset; //Se agrega la variacion de probabilidad de fallo a la distancia.

            Vector3 shootDirection = (targetPoint - shootPoint.position).normalized;

            //Debug.DrawRay(shootPoint.position, shootDirection * shootDamage, Color.green, 1f);
            //Debug.DrawRay(shootPoint.position, shootDirection * characterData.CurrentStats.CurrentSkill, Color.green, 1f);
            Debug.DrawRay(shootPoint.position, shootDirection * (characterData.CurrentStats.CurrentAttack * 2 / 3), Color.green, 1f); // Se usa una forula de dos tercios del ataque para el rango (temp).

            //Se calcula donde el raycast golpea al objetivo y si le dio o no.
            RaycastHit hit;

            //if (Physics.Raycast(shootPoint.position, shootDirection, out hit, shootRange))
            if (Physics.Raycast(shootPoint.position, shootDirection, out hit, rangeOfShooting))
            {
                if (hit.collider.gameObject == shootTargets[currentShootTarget].gameObject)
                {
                    //Debug.Log(name + " Shot Target " + shootTargets[currentShootTarget].name);
                    //shootTargets[currentShootTarget].TakeDamage(shootDamage);
                    //shootTargets[currentShootTarget].TakeDamage(characterData.CurrentStats.CurrentSkill); 
                    shootTargets[currentShootTarget].TakeDamage(characterData.CurrentStats.CurrentAttack * 2 / 3); // Se usa una forula de dos tercios del ataque para el rango (temp).

                    Instantiate(shotHitFX, hit.point, Quaternion.identity); // Instanciamos el FX de da�o.

                }
                else
                {
                    Debug.Log(name + " Missed " + shootTargets[currentShootTarget].name + "!");
                    PlayerInputMenu.instance.ShowErrorText("Shot Blocked!");

                    Instantiate(shotMissFX, hit.point, Quaternion.identity);    // Instanciamos el FX de fallo (golpea una pared).
                }

                //Aparecemos la linea visual que visualmente representa una bala. ACERTO.
                shootLine.SetPosition(0, shootPoint.position);
                shootLine.SetPosition(1, hit.point);

                SFXManager.instance.impact.Play();

            }
            else
            {
                Debug.Log(name + " Missed " + shootTargets[currentShootTarget].name + "!");
                PlayerInputMenu.instance.ShowErrorText("Shot Missed!");

                //Aparecemos la linea visual que visualmente representa una bala. FALLO.
                shootLine.SetPosition(0, shootPoint.position);
                //shootLine.SetPosition(1, shootPoint.position + (shootDirection * shootRange));
                shootLine.SetPosition(1, shootPoint.position + (shootDirection * rangeOfShooting));
            }

            shootLine.gameObject.SetActive(true);
            shotRemainCounter = shotRemainTime;

            CheckForCharacterInteractionController("Shoot");

            SFXManager.instance.PlayShoot();

        }
    }

    public void ShotLineController()
    {
        if(shotRemainCounter > 0)
        {
            shotRemainCounter -= Time.deltaTime;
            if (shotRemainCounter <= 0)
            {
                shootLine.gameObject.SetActive(false);
                isShooting = false;
            }
        }
    }

    //Esta funcion es la que calcular� el porcentaje de probabilidad para golpear un jugador evaluando el mundo y su alrededor. Se convertir� en texto para desplegar.
    public float CheckShotChance()
    {
        float shotChance = 0f;

        RaycastHit hit;
        int rangeOfShooting = 10;

        //Se revisa el rayo en HIGH: Se envia un Raycast a la altura del Shootpoint del enemigo. Si nada bloquea su trayectoria, se suma 50% de accuracy.
        Vector3 targetPoint = new Vector3(shootTargets[currentShootTarget].transform.position.x, shootTargets[currentShootTarget].shootPoint.position.y, shootTargets[currentShootTarget].transform.position.z);
        Vector3 shootDirection = (targetPoint - shootPoint.position).normalized;
        //Debug.DrawRay(shootPoint.position, shootDirection * shootRange, Color.red, 1f);
        Debug.DrawRay(shootPoint.position, shootDirection * rangeOfShooting, Color.red, 1f);
        //if (Physics.Raycast(shootPoint.position, shootDirection, out hit, shootRange))  //Se obtiene el gameobject con que colisiono el Raycast.
        if (Physics.Raycast(shootPoint.position, shootDirection, out hit, rangeOfShooting)) 
        {
            if (hit.collider.gameObject == shootTargets[currentShootTarget].gameObject) // Si el objeto con el que colision� el Raycast es el objetivo a disparar.
            {
                shotChance += 50f;
            }
        }

        //Se revisa el rayo en LOW: Se envia un Raycast a la altura de las piernas +0.25f del enemigo. Si nada bloquea su trayectoria, se suma otro 50% de accuracy.
        targetPoint.y = shootTargets[currentShootTarget].transform.position.y + 0.25f;
        shootDirection = (targetPoint - shootPoint.position).normalized;
        //Debug.DrawRay(shootPoint.position, shootDirection * shootRange, Color.red, 1f);
        Debug.DrawRay(shootPoint.position, shootDirection * rangeOfShooting, Color.red, 1f);
        //if (Physics.Raycast(shootPoint.position, shootDirection, out hit, shootRange))  //Se obtiene el gameobject con que colisiono el Raycast.
        if (Physics.Raycast(shootPoint.position, shootDirection, out hit, rangeOfShooting))  
        {
            if (hit.collider.gameObject == shootTargets[currentShootTarget].gameObject) // Si el objeto con el que colision� el Raycast es el objetivo a disparar.
            {
                shotChance += 50f;
            }
        }

        shotChance *= 0.95f; // Visualmente ponemos que no es seguro el tiro.
        //shotChance *= 1f - (Vector3.Distance(shootTargets[currentShootTarget].transform.position, transform.position) / shootRange); // Calculamos el accuracy tomando en cuenta el Offset por distancia
        shotChance *= 1f - (Vector3.Distance(shootTargets[currentShootTarget].transform.position, transform.position) / rangeOfShooting);

        return shotChance;
    }

    public void LookAtTarget(Transform target)
    {
        transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z), Vector3.up);
    }    
    
    //Funcion para rota un jugador hacia un objetivo. Se guarda el jugador objetivo en variable y se activa el booleano isRotating. Entonces RotationController() en Update se encargar� de rotarlo.
    public void LookAtShootTarget(Transform target)
    {
        endRotationTarget = target;

        lookOnLook = Quaternion.LookRotation(target.transform.position - transform.position); //Funcion similar a LookAt(), esta s� devuelve un Quaternion.
        isRotating = true;
    }

    // Se debe activar el booleano cuando presionen el bot�n de Disparar. Se desactiva cuando termine de rotar.
    private void RotationController()
    {
        if (isRotating)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, lookOnLook, rotateSpeed * Time.deltaTime).normalized;
            if ( (lookOnLook.eulerAngles.y - transform.eulerAngles.y) == 0f) //Si el angulo del tranform y el angulo objetivo ya son iguales.
            {
                isRotating = false;
            }
        }
    }

    //Funcion que activa y desactiva el gameobject de defensa. El booleano har� las modificaciones en el Update.
    public void SetDefending(bool defend)
    {
        isDefending = defend;
        defendObject.SetActive(isDefending);
        if(isDefending == true)
        {
            CheckForCharacterInteractionController("Defend");
        }
    }

    /// <summary>
    /// CheckForCharacterInteractionController
    /// Este controlador verifica qu� script tiene cada personaje y dependiendo de la interacci�n, obtiene el script correcto.
    /// </summary>
    /// <param name="interaction"></param>
    private void CheckForCharacterInteractionController(string interaction)
    {
        Debug.Log("interaction: " + interaction);
        switch (characterData.CharacterStats.CharacterSubType)
        {
            case "PolygonDog":
                DogAnimationTest dogPolygon = GetComponent<DogAnimationTest>(); // Get the animation component
                switch (interaction)
                {
                    case "Movement":
                        dogPolygon.FunctionDogWalking(isMoving, isRunning);
                        break;
                    case "Bite":
                        dogPolygon.FunctionDogAttackType(isMeleeing, 1);
                        break;
                    case "Shoot": //ActionType - 6 Howl
                        StartCoroutine(dogPolygon.FunctionDogAction(isShooting, 6)); //ActionType - 6 Howl
                        break;
                    case "Damaged"://ActionType - 0 Hurt
                        dogPolygon.FunctionDogAttackType(isDamaged, 0); 
                        break;
                    case "Defend"://ActionType - 3 AttackReady
                        dogPolygon.FunctionDogAttackType(isDefending, 3);
                        break;
                    case "KnockedOut":
                        dogPolygon.FunctionDogKnockedOut();
                        break;
                    default:
                        break;
                }
                return;
            case "SimpleDog":
                DogAnimationSimple dogSimple = GetComponent<DogAnimationSimple>(); // Get the animation component
                Debug.Log("Soy un " + characterData.CharacterStats.CharacterSubType + " y eleg�: "+ interaction);
                switch (interaction)
                {
                    case "Movement":
                        dogSimple.FunctionDogWalking(isMoving, isRunning);
                        return;
                    case "Bite":
                        dogSimple.FunctionDogAction(isMeleeing, 1);
                        return;
                    case "Shoot": //ActionType - 6 Howl
                        dogSimple.FunctionDogAction(isShooting, 6); //ActionType - 6 Howl
                        return;
                    case "Damaged":
                        dogSimple.FunctionDogAction(isDamaged, 0);
                        return;
                    case "Defend":
                        dogSimple.FunctionDogAction(isDefending, 3);
                        return;
                    case "KnockedOut":
                        dogSimple.FunctionDogKnockedOut();
                        return;
                    default:
                        Debug.Log("Me fui por el default en: " + characterData.CharacterStats.CharacterSubType + " y eleg�: " + interaction);
                        break;
                }

                return;
            case "SimpleFarm":
                return;
            case "SimpleWild":
                return;
            case "SimpleCat":
                return;
            case "Other":
                return;
            default:
                Debug.Log("Llegue aqui acaso?");
                return;
        }
    }
    /*
    private void InitCharacterControllerValues()
    {
        moveRange = 3.5f; runRange = 8f;
        isMoving = false; isMeleeing = false; isRunning = false; isShooting = false; isDefending = false; isDamaged = false; isKnockedOut = false;
        meleeRange = 2f; 
        //meleeDamage = 5f;
        shootRange = 10f; 
        //shootDamage = 3f;
        currentMeleeTarget = 0; currentShootTarget = 0; //Default no hay enemigos en rango.
        UpdateHealthDisplay(); //Inicializamos el UI de Health.
        shootLine.transform.position = Vector3.zero;
        shootLine.transform.rotation = Quaternion.identity;
        shootLine.transform.SetParent(gameObject.transform);
        shotRemainTime = 0.5f;
        shotRemainCounter = shotRemainTime;
        rotateSpeed = 45f;
        isRotating = false;
        endRotationTarget = this.transform;
    }
    */


    //Clase y funcion que transforma frames a un Yield en decimales de segundos.
    public static class WaitFor
    {
        public static IEnumerator Frames(int frameCount)
        {
            if (frameCount <= 0)
            {
                throw new System.ArgumentOutOfRangeException("frameCount", "Cannot wait for less that 1 frame");
            }

            while (frameCount > 0)
            {
                frameCount--;
                yield return null;
            }
        }
    }

    /*
    public void DespliegaElContenidoCharacterData()
    {
        // Mostrar el contenido del objeto CharacterData
        var characterDataContent = characterData.DisplayCharacterDataContent(characterData);

        Debug.Log("Character Data:");
        foreach (var item in characterDataContent)
        {
            Debug.Log($"{item.Key}: {item.Value}");
        }
    }
    **/
}
