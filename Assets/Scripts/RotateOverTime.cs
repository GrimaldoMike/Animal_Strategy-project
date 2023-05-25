using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateOverTime : MonoBehaviour
{
    // <>

    public float rotateSpeed;
    public float movementSpeed, movementRangeLimit;

    private void Start()
    {
        InitializeRotateOverTimeValues();
    }

    // Update is called once per frame
    void Update()
    {
        TargetDisplayRotation();
        TargetDisplayMovement();
    }

    //El target rota sobre el eje Y encima del objetivo.
    private void TargetDisplayRotation()
    {
        transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y + (rotateSpeed * Time.deltaTime), 0f); //Rotacion constante de un objeto
    }

    //El target display sube y baja encima del objetivo.
    private void TargetDisplayMovement()
    {
        transform.position = new Vector3(transform.position.x, (transform.position.y + (movementSpeed * Time.deltaTime)), transform.position.z);

        if (transform.position.y > movementRangeLimit || transform.position.y < -movementRangeLimit)
        {
            movementSpeed = -movementSpeed;
        }
    }

    private void InitializeRotateOverTimeValues()
    {
        rotateSpeed = 45f;
        movementSpeed = 0.75f;
        movementRangeLimit = 0.35f;
    }


}
