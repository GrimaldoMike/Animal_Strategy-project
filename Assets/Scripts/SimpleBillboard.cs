using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBillboard : MonoBehaviour
{


    // Update is called once per frame
    void LateUpdate()
    {
        //La rotacion de este objeta voltea ver a la camara siempre.
        transform.rotation = CameraController.instance.transform.rotation;
    }
}
