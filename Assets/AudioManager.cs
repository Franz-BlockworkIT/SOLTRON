using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip btnSound;
    
    AudioSource audioSource;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        //btnSound = Resources.Load("Sounds/Main Menu Click Button") as AudioClip;
    }


    public void ButtonSound()
    {
        audioSource.PlayOneShot(btnSound);
        
    }

    public void BikeExplosioSound()
    {
        audioSource.PlayOneShot((AudioClip)Resources.Load("Sounds/WAV 1/Explosion_Destroyed_Bike"));
    }
}
