using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePointCube : MonoBehaviour
{
    public Material movePointMat, movePointMatSelected;
    public MovePoint mp;

    public float movementSpeed, movementRangeLimit;
    private Vector3 initialPosition;

    private void Start()
    {
        InitializeRotateOverTimeValues();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.isPaused == false)
        {
            TargetDisplayMovement();
        }
    }

    //El target display sube y baja encima del objetivo.
    private void TargetDisplayMovement()
    {
        if (mp.ReturnMouseOver() == true)
        {
            transform.position = new Vector3(transform.position.x, (transform.position.y + (movementSpeed * Time.deltaTime)), transform.position.z);

            if (transform.position.y > movementRangeLimit || transform.position.y < 0f)
            {
                movementSpeed = -movementSpeed;
            }

            this.GetComponent<MeshRenderer>().material = movePointMatSelected;
        }
        else
        {
            transform.position = initialPosition;
            this.GetComponent<MeshRenderer>().material = movePointMat;
        }
    }

    private void InitializeRotateOverTimeValues()
    {
        movementSpeed = 0.5f;
        movementRangeLimit = 0.35f;
        initialPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }
}
