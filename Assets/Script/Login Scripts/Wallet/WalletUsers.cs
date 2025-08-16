/*
Author: Hafiz Saad Khawar
Script: WalletUsers.cs

Description:
This class represents a player’s wallet and game-related stats.
It is marked as [System.Serializable] so it can be easily saved, 
loaded, or sent over the network as JSON or other serializable formats.

Key Features:
1. Player Stats: Stores player's name, level, bike level, experience points (XP), and battle wins.
2. In-Game Currency: Holds balances for SOL and TRON tokens.
3. Bike Customization: Tracks bike color and bike upgrade level.
4. Wallet Integration: Stores the player's connected wallet address.
5. Constructor Initialization: Automatically populates fields from PlayerScores and ConnectToWallet classes.

Notes:
- This class assumes PlayerScores and ConnectToWallet are already populated.
- Can be extended to include other in-game stats or blockchain-related info.
*/

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
