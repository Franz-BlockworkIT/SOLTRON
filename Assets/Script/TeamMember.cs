using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TeamMember : MonoBehaviour
{
    public int _teamID = 0;


    public int teamID
    {
        get { return _teamID; }
    }

    [PunRPC]
    void SetTeamID(int id)
    {
        _teamID = id;

        if (id == 1)
        {
            gameObject.tag = "Team1";
            gameObject.layer = 8;
        }
        else
        {
            gameObject.tag = "Team2";
            gameObject.layer = 9;
        }
    }
}
