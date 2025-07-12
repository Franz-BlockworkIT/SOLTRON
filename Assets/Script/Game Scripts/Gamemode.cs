using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Gamemode : MonoBehaviourPunCallbacks
{



    private void Update()
    {

        if (!photonView.IsMine)
            return;
        if (MainMenu.quickMatch)
        {
            SolBalWon(200);
        }
        if (MainMenu.teamDeathmatch)
        {

        }
        if (MainMenu.tournament)
        {

        }
    }


    public void SolBalWon(int amount)
    {
        if (MainMenu.playToEarn)
        {


            PlayerPrefs.SetInt(PlayerScores.playerName + "Score", PlayerPrefs.GetInt(PlayerScores.playerName + "Score", 0) + amount);
        }
    }
}
