using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class User
{
    public string userName;
    public int userScore;
    public int Level;
    public int userBikeLevel;
    public int xp;
    public int userSolBal;
    public int userTronBal;
    public int userBikeColor;
    public int userBattleWon;
    public string localId;
    public string userWalletAddress;

    public User()
    {
        userName = PlayerScores.playerName;
        userScore = PlayerScores.playerScore;
        Level = PlayerScores.playerLevel;
        userBikeLevel = PlayerScores.playerBikeLevel;
        xp = PlayerScores.playerXp;
        userSolBal = PlayerScores.playerSolBal;
        userTronBal = PlayerScores.playerTronBal;
        userBikeColor = PlayerScores.playerBikeColor;
        localId = PlayerScores.localId;
        userBattleWon = PlayerScores.playerBattleWon;


    }
}
    
