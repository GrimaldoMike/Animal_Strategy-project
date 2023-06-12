using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public Toggle fullscreenTog, vsyncTog;

    public ResolutionItem[] resolutions;
    private int selectedResolution;

    public Text resolutionLabel;


    // Start is called before the first frame update
    void Start()
    {
        InitOptionsMenuValues();
    }

    //Cambia el texto de la caja de resolucion una vez que se defina lo que se aplicará.
    public void UpdateResolutionLabel()
    {
        resolutionLabel.text= resolutions[selectedResolution].horizontal.ToString() + " X " + resolutions[selectedResolution].vertical.ToString();
    }

    //Bottón de resolucion izquierdo
    public void ResolutionLeft()
    {
        selectedResolution--;
        if (selectedResolution < 0)
        {
            selectedResolution = 0;
        }
        UpdateResolutionLabel();
    }

    //Bottón de resolucion derecho
    public void ResolutionRight()
    {
        selectedResolution++;
        if (selectedResolution > resolutions.Length - 1)
        {
            selectedResolution = resolutions.Length - 1;
        }
        UpdateResolutionLabel();
    }

    // Función que el botón de aplicar graficos usa para definir los cambios. Si va estar fullscreen, aplicar vsync o cambiar resolucion.
    public void ApplyGraphics()
    {
        //Apply Vsync
        if(vsyncTog.isOn == true)
        {
            QualitySettings.vSyncCount = 1;
        }
        else
        {
            QualitySettings.vSyncCount = 0;
        }
        //Apply resolution
        Screen.SetResolution(resolutions[selectedResolution].horizontal, resolutions[selectedResolution].vertical, fullscreenTog.isOn);
    }

    //Valores iniciales al abrir el juego. Se detecta lo que se tiene al abrir el juego y se aplica según lo encontrado.
    private void InitOptionsMenuValues()
    {
        fullscreenTog.isOn = Screen.fullScreen; 
        if(QualitySettings.vSyncCount == 0)
        {
            vsyncTog.isOn = false;
        }
        else
        {
            vsyncTog.isOn = true;
        }

        //Search for resolution
        bool foundResolution = false;
        for(int i = 0; i < resolutions.Length; i++)
        {
            if(Screen.width == resolutions[i].horizontal &&  Screen.height == resolutions[i].vertical)
            {
                foundResolution = true; 
                selectedResolution = i;
                UpdateResolutionLabel();
            }
        }
        if(foundResolution == false)
        {
            resolutionLabel.text = Screen.width.ToString() + " + " + Screen.height.ToString();
        }
    }


    [System.Serializable]
    public class ResolutionItem
    {
        public int horizontal, vertical;
    }
}
