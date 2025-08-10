// MultilplayerSettings.cs
// Author: Hafiz Saad Khawar
// Created: 2024-02-04
// Description: Singleton class to hold multiplayer game settings such as delay start, 
//              max players, and scene indexes for menu and multiplayer gameplay. 
//              Ensures persistence across scene loads.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultilplayerSettings : MonoBehaviour
{
    public static MultilplayerSettings multiplayerSettings;

    public bool delayStart;
    public int maxPlayers;

    public int menuScene;
    public int multiplayerScene;
    

    ///-------------------///
    private void Awake()
    {
        if(MultilplayerSettings.multiplayerSettings == null)
        {
            MultilplayerSettings.multiplayerSettings = this;
        }
        else
        {
            if(MultilplayerSettings.multiplayerSettings != this)
            {
                Destroy(this.gameObject);
            }

        }
        DontDestroyOnLoad(this.gameObject);
    }


    
}
