using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // <>
    public static CameraController instance;
    public void Awake()
    {
        instance = this;
        moveTarget = transform.position; //Inicializa en la posición objetivo inicial por default.
    }

    public float moveSpeed, playerCamMoveSpeed;
    private Vector3 moveTarget;
    private Vector2 moveInput;

    //Rotation cameras
    private float targetRotation;
    public float rotateSpeed;
    private int currentAngle;

    public bool freeCameraIsReturning;

    public Transform theCam;
    public float fireCamViewAngle;
    private float targetCamViewAngle;
    private bool isFireView;

    // Start is called before the first frame update
    void Start()
    {
        InitCameraControllerValues();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.isPaused == false)
        {
            DollyFollowsCamControl();
            PlayerCamControl();
            SnapBackToPlayer(false);
            RotationCamControl();
        }
    }

    //El movimiento de un jugador activo mueve la camara hacia el objetivo.
    private void DollyFollowsCamControl()
    {
        // Si la camara aun no llega a la posicion objetivo, desplazar hacia la posición objetivo usando MoveTowards.
        if (moveTarget != transform.position)
        {
            transform.position = Vector3.MoveTowards(transform.position, moveTarget, moveSpeed * Time.deltaTime);
        }
        else
        {
            //Cuando entra al else, es que ya terminó de llegar a la posición. Se revisa si freeCameraIsReturning es verdadero, ya que significa que se movio la camara con wasd y debe volverse falso.
            if (freeCameraIsReturning)
            {
                freeCameraIsReturning = false;
            }
        }
    }

    //Se controla el movimiento de camera por medio del jugador.
    private void PlayerCamControl()
    {
        if (isFireView == false)
        {
            if (!freeCameraIsReturning) //Solamente se podrá controlar la cámara cuando no este regresando de presionar Tab.
            {
                moveInput.x = Input.GetAxis("Horizontal");
                moveInput.y = Input.GetAxis("Vertical");
                moveInput.Normalize();

                if (moveInput != Vector2.zero)
                {
                    //moveInput obtiene el boton izquierda o derecha del teclado (tambien A o D del control WASD)
                    //La posición cambia con el movimiento hacia donde la cámara esta viendo, se mutiplica por la velocidad manual y Delta time.
                    //transform.position += new Vector3(moveInput.x + playerCamMoveSpeed, 0f, moveInput.y + playerCamMoveSpeed) * Time.deltaTime;
                    // transform.position += ((transform.forward * (moveInput.y + playerCamMoveSpeed)) + (transform.right * (moveInput.x + playerCamMoveSpeed))) * Time.deltaTime;
                    transform.position += ((transform.forward * (moveInput.y * playerCamMoveSpeed)) + (transform.right * (moveInput.x * playerCamMoveSpeed))) * Time.deltaTime;
                    moveTarget = transform.position;
                }
            }
        }
    }

    //Botón que regresa la camara al jugador activo sin necesidad de usar un movimiento.
    public void SnapBackToPlayer(bool snapBackTriggered)
    {
        if (isFireView == false)
        {
            if (Input.GetKeyDown(KeyCode.Tab) || snapBackTriggered)
            {
                if (!freeCameraIsReturning) //No valida varios Tab a la vez hasta que  alla vuelto.
                {
                    freeCameraIsReturning = true;
                    if (GameManager.instance.activePlayer is not null)
                    {
                        SetMoveTarget(GameManager.instance.activePlayer.transform.position);
                    }
                }
            }
        }
    }
    public void SnapBackToPlayerUI(GameObject gObj)
    {
        if (gObj is not null)
        {
            SetMoveTarget(gObj.transform.position);
        }
    }

    //Se encarga de mover la cámara en 4 diferentes ángulos alrededor del jugador activo. 
    //Se utiliza Q y E para rotar.
    private void RotationCamControl()
    {
        if (Input.GetKeyDown(KeyCode.E)) //Si se presiona la tecla E, se incrementa el ángulo. 90 grados cada valor.
        {
            currentAngle++;
            if (currentAngle >= 4)
            {
                currentAngle = 0;
            }
        }
        if (Input.GetKeyDown(KeyCode.Q)) //Si se presiona la tecla Q, se decrementa el ángulo.
        {
            currentAngle--;
            if (currentAngle < 0)
            {
                currentAngle = 3;
            }
        }

       if(isFireView == false)
        {
            targetRotation = (90f * currentAngle) + 45f;  //Aplicamos el algulo de rotación al eje de la cámara.
        }
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, targetRotation, 0f), rotateSpeed * Time.deltaTime); //Formula que modifica la rotación de la camara, solo cambia el eje Y. "Slerp" hace que se mueva rápido al principio pero lento al final.

        theCam.localRotation = Quaternion.Slerp(theCam.localRotation, Quaternion.Euler(targetCamViewAngle, 0f, 0f), rotateSpeed * Time.deltaTime);

    }

    //Función que cambia los objetivos de la camara. Recibe el objetivo (por clic de boton, por evento, etc,)
    public void SetMoveTarget(Vector3 newTarget)
    {
        moveTarget = newTarget;

        targetCamViewAngle = 45f;
        isFireView = false;
    }

    public void SetFireView()
    {
        moveTarget = GameManager.instance.activePlayer.transform.position;

        targetRotation = GameManager.instance.activePlayer.transform.rotation.eulerAngles.y;

        targetCamViewAngle = fireCamViewAngle;
        isFireView = true;
    }

    private void InitCameraControllerValues()
    {
        playerCamMoveSpeed = 5f;
        freeCameraIsReturning = false;
        //SnapBackToPlayer(true);
        fireCamViewAngle = 30f;
        targetCamViewAngle = 45f;
    }
}
