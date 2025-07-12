using Proyecto26;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FullSerializer;
using TMPro;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class TournamentMatchMaker : MonoBehaviourPunCallbacks, ILobbyCallbacks, IInRoomCallbacks
{


    private string databaseURL = "https://soltron-c922b-default-rtdb.asia-southeast1.firebasedatabase.app";


    public static bool tournamentFull;

    

    public bool startTournament;

    public static fsSerializer serializer = new fsSerializer();

    public List<TournamentUsers> tournamentUsers;

    public string tournamentMatch;

    public bool opponentLeft;
    public bool inSemiFinal;

    [HideInInspector] float disconnectTimeOutMS = 60.0f * 1000.0f;

    bool changeScene;


    private ExitGames.Client.Photon.Hashtable _localIdProperty = new ExitGames.Client.Photon.Hashtable();

    private void Start()
    {


        GetData();

        PhotonNetwork.LeaveRoom();

        SetLocalIdToCustomProperties();

 
    }

    private void Update()
    {
        if (!startTournament)
        {
            if (PhotonNetwork.InRoom)
            {

                string tournamentRoom = "Room" + tournamentMatch;
                if (PhotonNetwork.CurrentRoom.Name == tournamentRoom)
                {
                    if (PhotonNetwork.PlayerList.Length == 2 && PhotonNetwork.IsMasterClient)
                    {
                        PhotonNetwork.LoadLevel(1);
                        startTournament = true;
                    }
                }
            }
        }

        if (opponentLeft)
        {
            inSemiFinal = true;

        }
        
    }

    private void SetLocalIdToCustomProperties()
    {
        string playerLocalId = PlayerScores.localId;

        _localIdProperty["playerId"] = playerLocalId;

        PhotonNetwork.LocalPlayer.CustomProperties = _localIdProperty;
    }


    private void GetData()
    {
        string pathOfUser = "/Tournaments/Users/Tournament" + TournamentData.getLocalId + "Users";
        RestClient.Get(databaseURL + pathOfUser + "/.json").Then(response =>
        {
            fsData userData = fsJsonParser.Parse(response.Text);
            Dictionary<string, TournamentUsers> users = null;
            serializer.TryDeserialize(userData, ref users);

            Debug.Log("Get Data : " + response.Text);

            Debug.Log("Users Count : " + users.Values);
            foreach (var player in users.Values)
            {

                Debug.Log("Get Players : " + player);

                Debug.Log(users.Count);

                if (!player.userLeft)
                {
                    if (tournamentUsers.Count < users.Count)
                    {
                        tournamentUsers.Add(player);
                        //GameObject clone = Instantiate(prefab, transform.position, transform.rotation);
                        //clone.transform.parent = content.transform;
                        //clone.transform.localScale = new Vector3(1, 1, 1);
                        //clone.transform.GetChild(0).GetComponent<Text>().text = player.userName;
                    }
                }

            }

            StartCoroutine(CreateOrJoinRoom());


        }).Catch(error => {

            Debug.Log("Get User Data Failed : " + error);
            Debug.Log("Path Of User : " + pathOfUser);
        });
    }

    IEnumerator CreateOrJoinRoom()
    {
        while (PhotonNetwork.NetworkClientState != ClientState.ConnectedToMasterServer)
        {
            yield return null;
        }

        while (tournamentUsers.Count <= 1)
        {
            yield return null;
        }



        
        for (int i = 0; i < tournamentUsers.Count; i++)
        {
            Debug.Log("Tournament Users : " + i);
            if (tournamentUsers[i].localId == TournamentData.localId && i % 2 == 0)
            {
                Debug.Log("Tournament Odd Users : " + i);
                
                tournamentMatch = tournamentUsers[i].localId + tournamentUsers[i + 1].localId;
                CreateRoom();
            }
            else if (tournamentUsers[i].localId == TournamentData.localId && i % 2 != 0)
            {
                Debug.Log("Tournament Even Users : " + i);

                tournamentMatch = tournamentUsers[i - 1].localId + tournamentUsers[i].localId;

                JoinRoom();
            }
        }


    }
    void CreateRoom()
    {
        Debug.Log("Trying to create room...");
        int randomRoomName = Random.Range(0, 10000);

        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)2 };
        roomOps.CleanupCacheOnLeave = false;
        roomOps.PlayerTtl = -1;
        roomOps.EmptyRoomTtl = 60000;
        roomOps.PublishUserId = true;

        PhotonNetwork.KeepAliveInBackground = disconnectTimeOutMS;
        PhotonNetwork.CreateRoom("Room" + tournamentMatch, roomOps);
    }


    void JoinRoom()
    {
        PhotonNetwork.JoinRoom("Room" + tournamentMatch);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);

        PhotonNetwork.JoinRoom("Room" + tournamentMatch);



    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {

        Debug.Log("Tried To Create a new room but failed, there must be already a room with the same name");
        
        CreateRoom();
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        string playerId = (string)newPlayer.CustomProperties["playerId"];


        Debug.Log("New Player Entered Room : " + playerId);
    }
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);


        if (PhotonNetwork.CurrentRoom.Name == tournamentMatch)
        {
            if (otherPlayer.CustomProperties.ContainsKey("playerId"))
            {
                string playerId = (string)otherPlayer.CustomProperties["playerId"];

                if (playerId != TournamentData.localId)
                {
                    opponentLeft = true;
                }

                RestClient.Get<TournamentUsers>(databaseURL + "/Tournaments/Users/Tournament" + TournamentData.getLocalId + "Users/" + playerId + ".json").Then(response =>
                {
                    
                    response.userLeft = true;
                    RestClient.Put(databaseURL + "/Tournaments/Users/Tournament" + TournamentData.getLocalId + "Users/" + response.localId + ".json", response);


                });
            }
        }
    }

    

}
