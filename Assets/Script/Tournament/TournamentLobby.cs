using Photon.Realtime;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TournamentLobby : MonoBehaviourPunCallbacks, ILobbyCallbacks
{

    public static TournamentLobby _TL;


    [HideInInspector] float disconnectTimeOutMS = 60.0f * 1000.0f;


    private void Awake()
    {
        _TL = this;
    }




}
