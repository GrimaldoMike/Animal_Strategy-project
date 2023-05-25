using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;

    private void Awake()
    {
        instance = this;
    }

    public AudioSource deathHuman, deathRobot, impact, meleeHit, takeDamage, UICancel, UISelect;
    public AudioSource[] shootSounds;

    //Función para que aleatoriamente elija un sonido de disparo.
    public void PlayShoot() 
    {
        shootSounds[Random.Range(0, shootSounds.Length)].Play();
    }

}
