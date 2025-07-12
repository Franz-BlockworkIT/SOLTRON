using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class WalletUsers
{


    public string userName;
    public int Level;
    public int userBikeLevel;
    public int xp;
    public int userSolBal;
    public int userTronBal;
    public int userBikeColor;
    public int userBattleWon;

    public string userWalletAddress;


    public WalletUsers()
    {


        userName = PlayerScores.playerName;
        Level = PlayerScores.playerLevel;
        userBikeLevel = PlayerScores.playerBikeLevel;
        xp = PlayerScores.playerXp;
        userSolBal = PlayerScores.playerSolBal;
        userTronBal = PlayerScores.playerTronBal;
        userBikeColor = PlayerScores.playerBikeColor;
        userBattleWon = PlayerScores.playerBattleWon; 
        userWalletAddress = ConnectToWallet.playerWalletAddress;


    }


}
