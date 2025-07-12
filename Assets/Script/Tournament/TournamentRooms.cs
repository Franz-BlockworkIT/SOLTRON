using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TournamentRooms
{
    public bool tournament;
    public string tournamentRoomID;

    public TournamentRooms()
    {
        tournament = TournamentData.tournamentFull;
        tournamentRoomID = TournamentData.tournamentRoomId;
    }
}
