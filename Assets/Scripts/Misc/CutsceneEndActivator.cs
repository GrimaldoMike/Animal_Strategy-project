using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneEndActivator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Significa que la cutscene terminó y debe pasar al siguiente game state.
        PlayerInputMenu.instance.CutsceneController();

    }
}


