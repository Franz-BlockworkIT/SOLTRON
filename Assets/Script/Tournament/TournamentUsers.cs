using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TournamentUsers
{
    public string userName;
    public string localId;
    public bool userLeft;
    
    
    public TournamentUsers()
    {
        userName = TournamentData.playerName;
        localId = TournamentData.localId;
        userLeft = TournamentData.playerLeft;
        
    }
}
