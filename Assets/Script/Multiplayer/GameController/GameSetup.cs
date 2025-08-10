// GameSetup.cs
// Author: Hafiz Saad Khawar
// Created: 2024-02-04
// Description: Singleton class that holds spawn points for player instantiation in the Soltron game.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetup : MonoBehaviour
{
    public static GameSetup GS;

    public Transform[] SpawnPoints;

    private void OnEnable()
    {
        if(GameSetup.GS == null)
        {
            GameSetup.GS = this;
        }
    }
}
