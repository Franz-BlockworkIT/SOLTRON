// PhotonLobbyNew.cs
// Author: Hafiz Saad Khawar
// Created: 2024-02-04
// Description: Handles matchmaking and lobby functionality using Photon PUN for Soltron multiplayer game.
//              Manages joining rooms, creating rooms, cancelling matchmaking, and syncing game modes.
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PhotonLobbyNew : MonoBehaviourPunCallbacks, ILobbyCallbacks
{
    public static PhotonLobbyNew lobby;


    public GameObject battleButton;
    public GameObject cancelButton;
    public GameObject offlineUI;

    public bool dontDestroyOnLeave = true;

    [HideInInspector] float disconnectTimeOutMS = 60.0f * 1000.0f;
    

    public int group = 1;

    public PhotonView PV;



    private void Awake()
    {
        lobby = this;
        PV = GetComponent<PhotonView>();
    }

    private void Start()
    {

        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }

        

        MainMenu.quickMatch = false;
        MainMenu.teamDeathmatch = false;
        MainMenu.training = false;
        MainMenu.tournament = false;

    }

    #region Funtions

    public override void OnConnectedToMaster()
    {
        Debug.Log("Player Has Connected to the Master!");
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.NickName = PlayerScores.playerName;
        
        battleButton.SetActive(true);
    }


    public void OnBattleButtonClicked()
    {
        Debug.Log("is Master Client : " + PhotonNetwork.IsConnected);
        battleButton.SetActive(false);
        offlineUI.SetActive(false);
        //cancelButton.SetActive(true);
        


        if (!MainMenu.training)
        {
            if (MainMenu.quickMatch)
            {
                if (PhotonNetwork.NetworkClientState == ClientState.ConnectedToMasterServer)
                {
                    PhotonNetwork.JoinRandomRoom(null, 2);
                }
            }

            if (MainMenu.teamDeathmatch)
            {
                if (PhotonNetwork.NetworkClientState == ClientState.ConnectedToMasterServer)
                {
                    
                        PhotonNetwork.JoinRandomRoom(null, 8);
                }
            }
            //if (MainMenu.tournament)
            //{
            //    //Turn On Tournament

            //    if (PhotonNetwork.NetworkClientState == ClientState.ConnectedToMasterServer)
            //    {

            //        PhotonNetwork.JoinRandomRoom(null, 4);
            //    }
            //}


        }

        else
        {

            if (PhotonNetwork.NetworkClientState == ClientState.ConnectedToMasterServer)
            {
                PhotonNetwork.JoinRandomRoom(null, 1);
                
            }

        }

        if (MainMenu.training)
            return;
        StartCoroutine(JoiningRoom());

    }


    IEnumerator JoiningRoom()
    {

        while (!PhotonNetwork.InRoom/*PhotonNetwork.NetworkClientState != ClientState.ConnectedToMasterServer || PhotonNetwork.NetworkClientState != ClientState.Joined*/)
        {
            yield return null;
        }
        if (PhotonNetwork.NetworkClientState == ClientState.Joined)
        {
            cancelButton.SetActive(true);
        }
           
        
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Tried to Join A room and Failed");

        if (!MainMenu.tournament)
        {
            CreateRoom();
        }

    }

    private ExitGames.Client.Photon.Hashtable _team1Prop = new ExitGames.Client.Photon.Hashtable();
    private ExitGames.Client.Photon.Hashtable _team2Prop= new ExitGames.Client.Photon.Hashtable();

    void CreateRoom()
    {
        Debug.Log("Trying to create room...");
        int randomRoomName = Random.Range(0, 10000);

            
        if (!MainMenu.training)
        {
            
            if (MainMenu.quickMatch)
            {
                RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)2};
                roomOps.CleanupCacheOnLeave = false;
                roomOps.PlayerTtl = -1;
                roomOps.EmptyRoomTtl = 60000;


                PhotonNetwork.KeepAliveInBackground = disconnectTimeOutMS;
                

                PhotonNetwork.CreateRoom("Room" + randomRoomName, roomOps);
            }
            else if (MainMenu.teamDeathmatch )
            {
                RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)8 };
                roomOps.CleanupCacheOnLeave = false;
                roomOps.PlayerTtl = -1;
                roomOps.EmptyRoomTtl = 60000;


                PhotonNetwork.KeepAliveInBackground = disconnectTimeOutMS;


                PhotonNetwork.CreateRoom("Room" + randomRoomName, roomOps);
            }
            else if (MainMenu.tournament)
            {

                RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)4 };
                roomOps.CleanupCacheOnLeave = false;
                roomOps.PlayerTtl = -1;
                roomOps.EmptyRoomTtl = 60000;


                _team1Prop["Team1"] = 4;
                _team2Prop["Team2"] = 4;


                roomOps.CustomRoomProperties = _team1Prop;
                roomOps.CustomRoomProperties = _team2Prop;


                PhotonNetwork.KeepAliveInBackground = disconnectTimeOutMS;


                PhotonNetwork.CreateRoom("Room" + randomRoomName, roomOps);
            }
        }

        else
        {

                RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)1 };
            roomOps.CleanupCacheOnLeave = false;
            roomOps.PlayerTtl = -1;
            roomOps.EmptyRoomTtl = 60000;
            
            

            PhotonNetwork.KeepAliveInBackground = disconnectTimeOutMS;

            PhotonNetwork.CreateRoom("Room" + randomRoomName, roomOps);
            
        }

    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Tried To Create a new room but failed, there must be already a room with the same name");

        if(!MainMenu.tournament)
            CreateRoom();
        
    }


    public void OnCancelButtonClicked()
    {
        Debug.Log("Cancel Btn clicked!");



        StartCoroutine(CancelRoom());
    }

   IEnumerator CancelRoom()
    {

        while(!PhotonNetwork.InRoom/*PhotonNetwork.NetworkClientState != ClientState.ConnectedToMasterServer || PhotonNetwork.NetworkClientState != ClientState.Joined*/)
        {
            yield return null;
        }
        if (PhotonNetwork.NetworkClientState != ClientState.Leaving)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;

            PhotonNetwork.LeaveRoom();

            


            cancelButton.SetActive(false);
            battleButton.SetActive(true);
            offlineUI.SetActive(true);
        }
    }

    #endregion

    public void JoinLobbyOnClick()
    {
        cancelButton.SetActive(true);
        PhotonNetwork.JoinLobby();
    }
    public void MatchMakingCancel()
    {
        cancelButton.SetActive(false);

        PhotonNetwork.LeaveLobby();
    }

   
    

    
}
