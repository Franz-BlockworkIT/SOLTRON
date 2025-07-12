 using Proyecto26;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FullSerializer;
using TMPro;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class TournamentData : MonoBehaviourPunCallbacks, ILobbyCallbacks
{
    public static TournamentData _TD;

    public static string playerName;

    public static bool tournamentFull;
    
    public static string tournamentRoomId;
    TournamentRooms troom = new TournamentRooms();

    public TournamentUsers tourUser = new TournamentUsers();


    private string databaseURL = "https://soltron-c922b-default-rtdb.asia-southeast1.firebasedatabase.app";
    

    
    public bool insideRoom;
    public static string localId;
    public static string getLocalId;
    public static bool playerLeft;
    

    public bool startTournament;

    

    public static fsSerializer serializer = new fsSerializer();

   


    public List<TournamentUsers> tournamentUsers;

    public string tournamentMatch;


    public bool semiFinalist;
    public bool finalist;

    bool changeScene;


    [Header("UI")]

    public GameObject content;
    public GameObject prefab;
    public Text readyText;
    
    public TextMeshProUGUI totalPlayersText;


    private void Start()
    {
        if (TournamentData._TD == null)
        {
            TournamentData._TD = this;
        }
        else
        {
            if (TournamentData._TD != this)
            {
                Destroy(TournamentData._TD.gameObject);
                TournamentData._TD = this;
            }
        }

        DontDestroyOnLoad(this.gameObject);


       

        
    }
    private void Update()
    {


        localId = PlayerScores.localId;

        if (MainMenu.tournament)
        {
            if (PhotonRoom.room.currentScene != 0)
                changeScene = false;
            if (PhotonNetwork.PlayerList.Length == 4)
            {
                if (!changeScene)
                {
                    PostToDatabase();
                    troom.tournament = true;
                    troom.tournamentRoomID = getLocalId;
                    RestClient.Put(databaseURL + "/Tournaments/Rooms/Tournament" + getLocalId + ".json", troom);


                    if (PhotonNetwork.IsMasterClient)
                    {
                        PhotonNetwork.LoadLevel(2);
                    }

                    changeScene = true;
                }
            }
        }

            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                semiFinalist = false;
                finalist = false;
            }

        
    }

    
    IEnumerator CreateRoomAfterDelay()
    {
        while(tournamentMatch == "")
        {
            yield return null;
        }
        Debug.Log("Creating Room After Delay");
        CreateRoom();
    }

    IEnumerator JoinRoomAfterDelay()
    {
        while (tournamentMatch == "")
        {
            yield return null;
        }
        Debug.Log("Joining Room After Delay");

        PhotonNetwork.JoinRoom("Room" + tournamentMatch);
    }

  
    public void OnSubmit()
    {
        StartCoroutine(InTournament());
    }

    IEnumerator InTournament()
    {
        while(PhotonNetwork.NetworkClientState != ClientState.ConnectedToMasterServer)
        {
            yield return null;
        }
        playerName = PlayerScores.playerName;
        
        tournamentRoomId = localId;
        

        EnteringTournament();


    }

    public void OnGetScore()
    {
        Debug.Log("On Get Score Function ");
        for (int child = 0; child <= content.transform.childCount - 1; child++)
        {
            if (content.transform.childCount != 0)
                Destroy(content.transform.GetChild(child).gameObject);
        }

            GetData();

    }

    

    public void PostToDatabase()
    {
        TournamentUsers tuser = new TournamentUsers();

        RestClient.Put(databaseURL + "/Tournaments/Users/Tournament" + getLocalId + "Users/" + localId + ".json", tuser);

    }

    private void CreateTournament()
    {
        TournamentRooms troom = new TournamentRooms();
        getLocalId = localId;
        RestClient.Put(databaseURL + "/Tournaments/Rooms/Tournament" + tournamentRoomId + ".json", troom);
        
        

    }

    void CreateTournamentDelay()
    {
        CreateTournament();
       

    }
    private void GetTournament()
    {
        Debug.Log("Get Tournament Local Id : " + getLocalId);
        RestClient.Get<TournamentRooms>(databaseURL + "/Tournaments/Rooms/Tournament" + getLocalId + ".json").Then(response =>
        {
            troom = response;

        }).Catch(error => {

            CreateTournament();

        });
    }


    IEnumerator GetTournamentDelay()
    {
        GetTournament();

        yield return new WaitForSeconds(2);

        insideRoom = true;
    }
    void EnteringTournament()
    {
        GetTournamentRoomLocalId();
    }

    public void RemoveFromDatabase()
    {
        RestClient.Delete(databaseURL + "/Tournaments/Users/Tournament" + getLocalId + "Users/" + localId + ".json");
        for (int child = 0; child <= content.transform.childCount - 1; child++)
        {
            if (content.transform.childCount != 0)
                Destroy(content.transform.GetChild(child).gameObject);
        }
        PhotonNetwork.LeaveLobby();

        insideRoom = false;
        

        
        
        
        
    }

    


    


    bool tournamentIsFull;

    private void GetData()
    {
        string pathOfUser = "/Tournaments/Users/Tournament" + getLocalId + "Users/";
        RestClient.Get(databaseURL + pathOfUser +".json").Then(response =>
        {
            Debug.Log("Get Data : " + response.Text);
            fsData userData = fsJsonParser.Parse(response.Text);
            Dictionary<string, TournamentUsers> users = null;
            serializer.TryDeserialize(userData, ref users);

            

            foreach (var player in users.Values)
            {
                //if (user.userName == username)
                //{
                //    getLocalId = user.localId;
                //    RetrieveFromDatabase();
                //    break;
                //}
                Debug.Log("Get Players : " + player);


                
                RestClient.Get<TournamentRooms>(databaseURL + "/Tournaments/Rooms/Tournament" + getLocalId + ".json").Then(response =>
                {
                    troom = response;
                    //tournamentRoomId = troom.tournamentRoomID;

                    if (users.Count >= 8)
                    {
                        Debug.Log("T room : " + troom.tournament);
                        troom.tournament = true;

                    }
                    else
                    {
                        troom.tournament = false;
                    }
                    Debug.Log("T Room ID : " + troom.tournamentRoomID);
                    RestClient.Put(databaseURL + "/Tournaments/Rooms/Tournament" + getLocalId + ".json", troom);


                    


                });

                
                   






                Debug.Log("T room outside : " + troom);
                
                Debug.Log(users.Count);

                if (tournamentUsers.Count < users.Count)
                {
                    tournamentUsers.Add(player);
                    GameObject clone = Instantiate(prefab, transform.position, transform.rotation);
                    clone.transform.parent = content.transform;
                    clone.transform.localScale = new Vector3(1, 1, 1);
                    clone.transform.GetChild(0).GetComponent<Text>().text = player.userName;
                }

            }




        }).Catch(error => {

            Debug.Log("Get User Data Failed : " + error);
            Debug.Log("Path Of User : " + pathOfUser);
        });
    }

    public void GetTournamentRoomLocalId()
    {
        RestClient.Get(databaseURL + "/Tournaments/Rooms.json").Then(response =>
        {

            fsData roomData = fsJsonParser.Parse(response.Text);
            Dictionary<string, TournamentRooms> rooms= null;
            serializer.TryDeserialize(roomData, ref rooms);
            foreach (var room in rooms.Values)
            {
                Debug.Log("Get Tournament rooms : " + room);

                
                    if (!room.tournament)
                    {

                        getLocalId = room.tournamentRoomID;
                        Debug.Log("Inside Tournament Room : " + getLocalId);
                        Debug.Log("Inside Tournament ID : " + room.tournamentRoomID);

                    
                        break;
                    }


                Debug.Log("Outside Tournament Room ! ");


            }
           

            PhotonNetwork.JoinRoom("Room" + getLocalId);


        }).Catch(error => {

            Debug.Log("GetLocal ID Failed : " + error);

            
            CreateTournament();

        });
    }

  



    #region In Room

  

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);


        if (PhotonRoom.room.currentScene == 0 && MainMenu.tournament)
        {
            
            CreateRoom();
        }

    }

    [HideInInspector] float disconnectTimeOutMS = 60.0f * 1000.0f;
    void CreateRoom()
    {
        Debug.Log("Trying to create room...");
        int randomRoomName = Random.Range(0, 10000);

        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)8 };
        roomOps.CleanupCacheOnLeave = false;
        roomOps.PlayerTtl = -1;
        roomOps.EmptyRoomTtl = 60000;
        roomOps.PublishUserId = true;

        PhotonNetwork.KeepAliveInBackground = disconnectTimeOutMS;
        PhotonNetwork.CreateRoom("Room" + getLocalId, roomOps);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {

        Debug.Log("Tried To Create a new room but failed, there must be already a room with the same name");
        //CreateRoom();
        if (PhotonRoom.room.currentScene == 0 && MainMenu.tournament)
        {
            
            CreateRoom();
        }
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        ClearPlayerListings();
        PlayerListings();

        Debug.Log("Joined Room, Player UserId : " + PhotonNetwork.LocalPlayer.UserId);
    }

    public void PlayerListings()
    {
        if (PhotonRoom.room.currentScene  == 0)
        {

            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                if (i % 2 == 0)
                {
                    if (PhotonNetwork.PlayerList[i].IsLocal)
                    {
                        PhotonRoom.room.SetTeams(1);
                    }
                }
                else
                {
                    if (PhotonNetwork.PlayerList[i].IsLocal)
                    {
                         PhotonRoom.room.SetTeams(2);
                    }
                }
            }


            if (PhotonNetwork.InRoom)
            {
                totalPlayersText.text = "Total Players : " + PhotonNetwork.PlayerList.Length + "/8";
                foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
                {
                    GameObject tempListing = Instantiate(prefab, content.transform);
                    Text tempText = tempListing.transform.GetChild(0).GetComponent<Text>();
                    tempText.text = player.NickName;


                }

            }


        }
    }


    public void ClearPlayerListings()
    {

        if (PhotonRoom.room.currentScene == 0)
        {
            for (int i = content.transform.childCount - 1; i >= 0; i--)
            {

                Destroy(content.transform.GetChild(i).gameObject);
            }
        }



    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        if(PhotonRoom.room.currentScene == 0 && MainMenu.tournament)
        {
            ClearPlayerListings();
            PlayerListings();
        }
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);

        if (PhotonRoom.room.currentScene == 0 && MainMenu.tournament)
        {
            ClearPlayerListings();
            PlayerListings();
        }
    }

    public void LeaveTournament()
    {
        PhotonNetwork.LeaveRoom();
    }
    #endregion
    
}


