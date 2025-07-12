using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Training : MonoBehaviour
{
    GameObject Bot;
    public Transform Botpos;

    
    void Update()
    {
        if(Bot == null)
        {
            Bot = Instantiate(Resources.Load("AI Enemy"), Botpos.position, Botpos.rotation) as GameObject;
        }
    }
}
