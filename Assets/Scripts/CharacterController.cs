using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;

public class CharacterController : MonoBehaviour
{
    // <>
    //Movement variables
    public float moveSpeed;
    private Vector3 moveTarget;
    public float moveRange, runRange;
    public bool finishedMovement;
    public Vector3 originalPosition;

    public NavMeshAgent navAgent;

    /// <summary>
    /// Dog variables
    /*public float w_movement; // Run value
    public float acceleration;
    public float decelleration;
    public float maxWalk;
    public float maxRun;
    public float currentSpeed;
    public bool isReadyToAttack;
    */
    /// </summary>

    //Control and animation variables
    public bool isMoving, isMeleeing, isRunning;
    public bool isShooting;
    public bool isDamaged, isKnockedOut;
    public bool isEnemy, isDogger;

    //Melee variables
    public float meleeRange;
    [HideInInspector]
    public List<CharacterController> meleeTargets = new List<CharacterController>();
    [HideInInspector]
    public int currentMeleeTarget;
    public float meleeDamage;

    //Fire variables
    public float shootRange, shootDamage;
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
    public float maxHealth;
    [HideInInspector]
    public float currentHealth;

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

    private void Awake()
    {
        //Se movieron al Awake para que el juego no empieze con la match y los jugadors en HP =0, o terminaría la partida al empezar
        moveTarget = transform.position;
        navAgent.speed = moveSpeed;
        maxHealth = 10f;
        currentHealth = maxHealth;
        isKnockedOut = false;
        //Debug.Log("Llegue AWAKE de CharacterController");
    }

    // Start is called before the first frame update
    void Start()
    {
        InitCharacterControllerValues();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.isPaused == false)
        {
            CharacterMovement();
            ShotLineController();
            AnimationController();
            PhaseThroughObjectsController();
        }
    }

    private void LateUpdate()
    {
        if (GameManager.instance.isPaused == false)
        {
            RotationController();//Se cambio al Update ya que queremos rotar un personaje con Time.deltaTime.
        }
    }

    private void CharacterMovement()
    {
        if (isMoving == true)
        {
            //Moving to a point with mouse.
            if (GameManager.instance.activePlayer == this) //Solamente se valida el jugador activo.
            {
                CameraController.instance.SetMoveTarget(transform.position); //Mueve la cámara hacia la posición del personaje.

                if (Vector3.Distance(transform.position, moveTarget) < 0.2f) //Si el jugador llegó a su destino (o a una distancia muy corta)
                {
                    isMoving = false;
                    isRunning = false;
                    if(isEnemy == false)
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
    }

    //Funcion que espera a que se acepte o no el movimiento. Pudiese poner un timer en el futuro para q
    private void MovementDecided()
    {
        if (PlayerInputMenu.instance.moveReturnMenu.activeSelf == false)
        {
            PlayerInputMenu.instance.ShowMoveReturnMenu();
        }
    }

    private void AnimationController()
    {
        if(isDogger== false)
        {
            if (isMoving == true)
            {
                anim.SetBool("a_isWalking", true);
            }
            else
            {
                anim.SetBool("a_isWalking", false);
            }
            if (isMeleeing == true)
            {
                isMeleeing = false;
                anim.SetTrigger("a_doMelee");
            }
            if (isDamaged == true)
            {
                isDamaged = false;
                anim.SetTrigger("a_damaged");
            }
            if (isKnockedOut == true)
            {
                isKnockedOut = false;
                anim.SetTrigger("a_die");
            }
        }
        /*else
        {
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
            navAgent.speed = w_movement;

            if(w_movement > 0){
                anim.SetInteger("AttackType_int", 0);
                anim.SetBool("AttackReady_b", false);
            }
            else if(!isReadyToAttack && !isMeleeing && !isKnockedOut && !isMoving && !isRunning && !isDamaged)
            {
                anim.SetInteger("ActionType_int", 0);
            }
        }*/


    }

    public void MoveToPoint(Vector3 pointToMoveTo)
    {
        //Guardamos la posición actual.
        originalPosition = transform.position;

        //Se moverá por parametro hacia el punto enviado.
        moveTarget = pointToMoveTo;

        navAgent.SetDestination(moveTarget);
        
        //Este if solo funciona porque sabemos que "currentActionCost" vale 1 cuando camina y 2 cuando corre. Se deberá modificar si existen más puntos de accion cuando hay movimiento.
        if (GameManager.instance.currentActionCost >= 2)
        {
            isRunning = true;
        }
        isMoving = true;
    }

    //Revisa si hay contrincantes a la distancia de "meleeRange". Si es así, los coloca en la lista de objetivos meleeTargets.
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
        //meleeTargets[currentMeleeTarget].gameObject.SetActive(false); //Se agregó currentMeleeTarget para saber quién es el objetivo a golpear.
        isMeleeing = true;
        SFXManager.instance.meleeHit.Play();
        meleeTargets[currentMeleeTarget].TakeDamage(meleeDamage);
    }

    public void TakeDamage(float damage)
    {
        if(isDefending == true) // Se revisa si estuvo en defensa, reduce el daño.
        {
            damage *= .5f;
        }

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0f;
            CharacterDefeatedByHealth();
        }
        else
        {
            isDamaged = true;
            SFXManager.instance.takeDamage.Play();
        }
        UpdateHealthDisplay();
    }


    public void CharacterDefeatedByHealth()
    {
        navAgent.enabled = false; //Desactiva script de navegacion.
        isKnockedOut = true; //Se activa la bandera de jugador inhabilitado para acciones.

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
        healthText.text = "HP: " + currentHealth + "/" + maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }

    //Misma revision de objetivos para el disparo, copiado como el de melee.
    public void GetShootTargets()
    {
        shootTargets.Clear();

        if (isKnockedOut == false) // Se valida si un personaje en el suelo es un objetivo para shoot.
        {
            if (isEnemy == false)
            {
                foreach (CharacterController cc in GameManager.instance.enemyTeam)
                {
                    if (Vector3.Distance(transform.position, cc.transform.position) < shootRange)
                    {
                        shootTargets.Add(cc);
                    }
                }
            }
            else
            {
                foreach (CharacterController cc in GameManager.instance.playerTeam)
                {
                    if (Vector3.Distance(transform.position, cc.transform.position) < shootRange)
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
            //Se obtiene la posicion del targetpoint basado en el shootpoint del oponente.
            Vector3 targetPoint = new Vector3(shootTargets[currentShootTarget].transform.position.x, shootTargets[currentShootTarget].shootPoint.position.y, shootTargets[currentShootTarget].transform.position.z);

            targetPoint.y = Random.Range(targetPoint.y, shootTargets[currentShootTarget].transform.position.y + 0.25f); //Este código ajusta un rango de propabilidad aleatoria en eje "y" (altura). Puede disparar entre los pies +.25 hasta el shootPoint.

            //El offset es una formula que toma el shotMissedRange al azar y calcula, dependiendo de la distancia del punto de disparo y el objetivo, una cantidad de variación.
            //Entre más lejos, mas alto el valor deprobabilidad de fallo.
            //Si la distancia entre el punto de disparo y objetivo es más pequeña, la variación de fallo es mucho más pequeña.
            Vector3 targetOffset = new Vector3(Random.Range(-shotMissedRange.x, shotMissedRange.x),
                                               Random.Range(-shotMissedRange.y, shotMissedRange.y),
                                               Random.Range(-shotMissedRange.z, shotMissedRange.z));
            //targetOffset = targetOffset * (Vector3.Distance(targetPoint, shootPoint.position) / shootRange);
            targetOffset = targetOffset * (Vector3.Distance(shootTargets[currentShootTarget].transform.position, transform.position) / shootRange);

            targetPoint += targetOffset; //Se agrega la variacion de probabilidad de fallo a la distancia.

            Vector3 shootDirection = (targetPoint - shootPoint.position).normalized;

            Debug.DrawRay(shootPoint.position, shootDirection * shootDamage, Color.green, 1f);

            //Se calcula dónde el raycast golpea al objetivo y si le dio o no.
            RaycastHit hit;
            if (Physics.Raycast(shootPoint.position, shootDirection, out hit, shootRange))
            {
                if (hit.collider.gameObject == shootTargets[currentShootTarget].gameObject)
                {
                    //Debug.Log(name + " Shot Target " + shootTargets[currentShootTarget].name);
                    shootTargets[currentShootTarget].TakeDamage(shootDamage);

                    Instantiate(shotHitFX, hit.point, Quaternion.identity); // Instanciamos el FX de daño.

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
                shootLine.SetPosition(1, shootPoint.position + (shootDirection * shootRange));
            }

            shootLine.gameObject.SetActive(true);
            shotRemainCounter = shotRemainTime;

            SFXManager.instance.PlayShoot();

        }
    }

    private void ShotLineController()
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

    //Esta funcion es la que calculará el porcentaje de probabilidad para golpear un jugador evaluando el mundo y su alrededor. Se convertirá en texto para desplegar.
    public float CheckShotChance()
    {
        float shotChance = 0f;

        RaycastHit hit;

        //Se revisa el rayo en HIGH: Se envia un Raycast a la altura del Shootpoint del enemigo. Si nada bloquea su trayectoria, se suma 50% de accuracy.
        Vector3 targetPoint = new Vector3(shootTargets[currentShootTarget].transform.position.x, shootTargets[currentShootTarget].shootPoint.position.y, shootTargets[currentShootTarget].transform.position.z);
        Vector3 shootDirection = (targetPoint - shootPoint.position).normalized;
        Debug.DrawRay(shootPoint.position, shootDirection * shootRange, Color.red, 1f);
        if (Physics.Raycast(shootPoint.position, shootDirection, out hit, shootRange))  //Se obtiene el gameobject con que colisionó el Raycast.
        {
            if (hit.collider.gameObject == shootTargets[currentShootTarget].gameObject) // Si el objeto con el que colisionó el Raycast es el objetivo a disparar.
            {
                shotChance += 50f;
            }
        }

        //Se revisa el rayo en LOW: Se envia un Raycast a la altura de las piernas +0.25f del enemigo. Si nada bloquea su trayectoria, se suma otro 50% de accuracy.
        targetPoint.y = shootTargets[currentShootTarget].transform.position.y + 0.25f;
        shootDirection = (targetPoint - shootPoint.position).normalized;
        Debug.DrawRay(shootPoint.position, shootDirection * shootRange, Color.red, 1f);
        if (Physics.Raycast(shootPoint.position, shootDirection, out hit, shootRange))  //Se obtiene el gameobject con que colisionó el Raycast.
        {
            if (hit.collider.gameObject == shootTargets[currentShootTarget].gameObject) // Si el objeto con el que colisionó el Raycast es el objetivo a disparar.
            {
                shotChance += 50f;
            }
        }

        shotChance *= 0.95f; // Visualmente ponemos que no es seguro el tiro.
        shotChance *= 1f - (Vector3.Distance(shootTargets[currentShootTarget].transform.position, transform.position) / shootRange); // Calculamos el accuracy tomando en cuenta el Offset por distancia

        return shotChance;
    }

    public void LookAtTarget(Transform target)
    {
        transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z), Vector3.up);
    }    
    
    //Funcion para rota un jugador hacia un objetivo. Se guarda el jugador objetivo en variable y se activa el booleano isRotating. Entonces RotationController() en Update se encargará de rotarlo.
    public void LookAtShootTarget(Transform target)
    {
        endRotationTarget = target;

        lookOnLook = Quaternion.LookRotation(target.transform.position - transform.position); //Funcion similar a LookAt(), esta sí devuelve un Quaternion.
        isRotating = true;
    }

    // Se debe activar el booleano cuando presionen el botón de Disparar. Se desactiva cuando termine de rotar.
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

    //Funcion que activa y desactiva el gameobject de defensa. El booleano hará las modificaciones en el Update.
    public void SetDefending(bool defend)
    {
        isDefending = defend;
        defendObject.SetActive(isDefending);
    }

    private void PhaseThroughObjectsController()
    {
        /*
        if (isMoving)
        {
            if(isEnemy == false)
            {
                GetComponent<Collider>().enabled = false; 
            }
        }
        else
        {
            GetComponent<Collider>().enabled = true;
        }
        */
    }

    //Revisa si hay contrincantes a la distancia de "meleeRange". Si es así, los coloca en la lista de objetivos meleeTargets.
    /*public void GetMeleeTargetsDogs()
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
    /*
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
    */


    private void InitCharacterControllerValues()
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
        shotRemainCounter = shotRemainTime;
        rotateSpeed = 45f;
        isRotating = false;
        endRotationTarget = this.transform;
    }

}
