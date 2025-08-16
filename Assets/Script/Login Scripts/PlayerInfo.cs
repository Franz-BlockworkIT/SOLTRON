/*
Author: Hafiz Saad Khawar
Script: PlayerInfo.cs

Description:
This script manages the player's selected character and ensures that
only one instance of PlayerInfo exists at any time (Singleton pattern). 
It also saves and loads the player's selected character using PlayerPrefs.

Key Features:
1. Singleton: The static variable 'PI' allows global access to the player's info.
2. Character Selection: Stores the index of the currently selected character.
3. Persistence: Saves the selected character to PlayerPrefs so it persists between sessions.
4. Scene Safety: Ensures duplicates are destroyed if multiple PlayerInfo objects exist.

Notes:
- 'allCharacters' array is prepared for future use (activating/deactivating characters).
- Singleton initialization happens in OnEnable, but Awake could also be used.
- Remember to call PlayerPrefs.Save() if you want immediate persistence.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public static PlayerInfo PI;

    public int mySelectedCharacter;

    public GameObject[] allCharacters;

    public void OnEnable()
    {
        if(PlayerInfo.PI == null)
        {
            PlayerInfo.PI = this;
        }

        else
        {
            if(PlayerInfo.PI != this)
            {
                Destroy(PlayerInfo.PI.gameObject);
                PlayerInfo.PI = this;
            }
        }
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("MyCharacter"))
        {
            mySelectedCharacter = PlayerPrefs.GetInt("MyCharacter");
        }
        else
        {
            mySelectedCharacter = 0;
            PlayerPrefs.SetInt("MyCharacter", mySelectedCharacter); 
        }
    }
}
