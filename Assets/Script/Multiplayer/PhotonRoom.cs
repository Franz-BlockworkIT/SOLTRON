using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using System;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PhotonRoom : MonoBehaviourPunCallbacks, IInRoomCallbacks
{

    //Room Info
    public static PhotonRoom room;
    //private PhotonView PV;

    public bool isGameLoaded;
    public int currentScene;

    public int multiplayerScene;


    // Player Info
    Photon.Realtime.Player[] photonPlayer;
    public int playersInRoom;
    public int myNumberInRoom;

    public int playersInGame;

    public static bool noPlayerFound;

    //Delayed start

    public float startingTime;

    [SerializeField] CountDown searchPlayers;
    public bool startGame = true;
    public int randomWait;
    public int maxPlayers;

    public bool PlayerDetected;

    string roomName;


    [Header("Lobby")]


    public Transform playerPanel;
    public TextMeshProUGUI GameModeTitleTxt;
    public TextMeshProUGUI limitedPlayers;



    public GameObject playerListingPrefab;


    public GameObject startButton;
    //public GameObject cancelButton;

    public bool masterCheck;

    public bool SpawnPlayer;


    [Header("LoadingScreen")]
    public Image progressBar;
    public Image loadingScreen;


    [Header("Tournament")]
    public GameObject roomListingPrefab;


    private void Awake()
    {
        if (PhotonRoom.room == null)
        {
            PhotonRoom.room = this;
        }
        else
        {
            if (PhotonRoom.room != this)
            {
                Destroy(PhotonRoom.room.gameObject);
                PhotonRoom.room = this;
            }
        }

        DontDestroyOnLoad(this.gameObject);
        randomWait = UnityEngine.Random.Range(50, 70);
        
    }


    public override void OnEnable()
    {
        // subscribe to functions
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    public override void OnDisable()
    {
        // subscribe to functions
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
        SceneManager.sceneLoaded -= OnSceneFinishedLoading;
    }
    private void Start()
    {

        if (PhotonRoom.room == null)
        {
            PhotonRoom.room = this;
        }
        else
        {
            if (PhotonRoom.room != this)
            {
                Destroy(PhotonRoom.room.gameObject);
                PhotonRoom.room = this;
            }
        }

        DontDestroyOnLoad(this.gameObject);
        randomWait = UnityEngine.Random.Range(10, 40);


        startGame = true;
        //PV = GetComponent<PhotonView>();




    }

    private ExitGames.Client.Photon.Hashtable _myCustomPropertiesForTour = new ExitGames.Client.Photon.Hashtable();

    private void Update()
    {
        


        Debug.Log(" Client State : " + PhotonNetwork.NetworkClientState);
        Debug.Log(" Master Client State : " + PhotonNetwork.IsMasterClient);
        Debug.Log(" Players : " + PhotonNetwork.PlayerList.Length);
        PhotonNetwork.SetPlayerCustomProperties(_myCustomPropertiesForTour);

        if (startButton != null)
        {
            if (PhotonNetwork.IsMasterClient)
            {

                startButton.SetActive(true);
                //cancelButton.SetActive(false);

            }
            else
            {
                startButton.SetActive(false);
                //cancelButton.SetActive(true);


            }
        }


        if (GameModeTitleTxt != null)
        {
            if (MainMenu.teamDeathmatch)
            {
                maxPlayers = 4;

                GameModeTitleTxt.text = "Team Deathmatch";
                limitedPlayers.text = playersInRoom + "/" + maxPlayers;


            }
            else
            {
                if (MainMenu.quickMatch)
                {
                    GameModeTitleTxt.text = "Quick Match";
                    maxPlayers = 2;
                    limitedPlayers.text = playersInRoom + "/" + maxPlayers;
                }

                if (MainMenu.tournament)
                {
                    GameModeTitleTxt.text = "Tournament";
                    maxPlayers = 4;
                    limitedPlayers.text = playersInRoom + "/" + maxPlayers;
                    
                }
            }
        }
        if (PhotonNetwork.NetworkClientState == ClientState.Disconnected)
            PhotonNetwork.ConnectUsingSettings();

        if (PhotonNetwork.NetworkClientState == ClientState.ConnectedToMasterServer)
        {
            if (PhotonLobbyNew.lobby.battleButton != null)
                PhotonLobbyNew.lobby.battleButton.SetActive(true);
        }
        if (PhotonNetwork.NetworkClientState != ClientState.Joined)
            return;


        if (startButton != null)
        {
            if (PhotonNetwork.PlayerList.Length != maxPlayers)
            {
                startButton.GetComponent<Button>().interactable = false;
            }
            else
            {
                startButton.GetComponent<Button>().interactable = true;

            }
        }
        // for delay start only, count down to start

        if (MainMenu.training)
        {

           
            if (PhotonNetwork.IsConnected)
            {
                if (startGame)
                {

                    StartGame();
                    startGame = false;
                }
            }
        }




    }


    //void CreatePlayer()
    //{
    //    PhotonNetwork.InstantiateRoomObject(System.IO.Path.Combine("PhotonPrefabs", "PhotonNetworkPlayer"), transform.position, Quaternion.identity, 0);

    //}
    void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        currentScene = scene.buildIndex;

       
        
    }


    private ExitGames.Client.Photon.Hashtable _teamProperty = new ExitGames.Client.Photon.Hashtable();
    public void SetTeams(int team)
    {

        
        int playerTeam = team;

        _teamProperty["Team"] = playerTeam;

        PhotonNetwork.LocalPlayer.CustomProperties = _teamProperty;
    }
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("Joined Room");


        if (!MainMenu.training)
        {
            if (startButton != null)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    startButton.SetActive(true);
                    //cancelButton.SetActive(false);

                }
                else
                {
                    startButton.SetActive(false);
                    //cancelButton.SetActive(true);


                }
            }

            
            if (currentScene != multiplayerScene)
            {
                ClearPlayerListings();
                PlayerListings();
            }
        }
        



        //if (PV.IsMine)
        //{
        //    PV.RPC("RPC_GetTeam", RpcTarget.MasterClient);
        //}
        photonPlayer = PhotonNetwork.PlayerList;
        playersInRoom = photonPlayer.Length;
        myNumberInRoom = playersInRoom;


        roomName = PhotonNetwork.CurrentRoom.Name;

    }

    public void ClearPlayerListings()
    {

        if (currentScene == 0)
        {
            for (int i = playerPanel.childCount - 1; i >= 0; i--)
            {

                Destroy(playerPanel.GetChild(i).gameObject);
            }
        }
        


    }

    public void PlayerListings()
    {
        if (currentScene == 0)
        {
            if (PhotonNetwork.InRoom)
            {

                for(int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
                {
                    if(i % 2 == 0)
                    {
                        if (PhotonNetwork.PlayerList[i].IsLocal)
                        {
                            SetTeams(1);
                        }
                    }
                    else
                    {
                        if (PhotonNetwork.PlayerList[i].IsLocal)
                        {
                            SetTeams(2);
                        }
                    }
                }

                foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
                {

                    
                    GameObject tempListing = Instantiate(playerListingPrefab, playerPanel);
                    Text tempText = tempListing.transform.GetChild(0).GetComponent<Text>();
                    tempText.text = player.NickName;


                }

            }


        }
    }


    private PlayerListing _playerListing;
    private List<PlayerListing> _listings = new List<PlayerListing>();
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);


        

        
        Debug.Log("A New Player Has Joined the room");


        //PV.RPC("RPC_GetTeam", RpcTarget.MasterClient);
        if (currentScene != multiplayerScene)
        {
                ClearPlayerListings();
                PlayerListings();
            
        }
        photonPlayer = PhotonNetwork.PlayerList;
        playersInRoom++;
        
    }

    
    public void StartGame()
    {

        isGameLoaded = true;
        // Loads the multiplayer scene for all Players

        loadingScreen.gameObject.SetActive(true);


        if (startGame)
        {
            //PhotonNetwork.LoadLevel(1);
            StartCoroutine(LoadLevelAsync());
            
            startGame = false;
        }
            
        
    }

    IEnumerator LoadLevelAsync()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.IsMessageQueueRunning = false;
            PhotonNetwork.LoadLevel(1);
        }

        while (PhotonNetwork.LevelLoadingProgress < 1)
        {
            //loadAmountText.text = "Loading: %" + (int)(PhotonNetwork.LevelLoadingProgress * 100);
            
            progressBar.fillAmount = Mathf.Clamp01(PhotonNetwork.LevelLoadingProgress/.9f);
            yield return null;
        }
    }

    
    


    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        Debug.Log(otherPlayer.NickName + " has left the game");
        playersInRoom = photonPlayer.Length;
        
        
        //    if (currentScene == 1)
        //    {
        //        int opponentTeam = (int)otherPlayer.CustomProperties["Team"];
        //        int playerTeam = (int)PhotonNetwork.LocalPlayer.CustomProperties["Team"];
        //        if (MainMenu.quickMatch || MainMenu.tournament)
        //        {

        //            if (opponentTeam != playerTeam)
        //            {
        //                if (!RoundSystem.RS.team1Won && !RoundSystem.RS.team2Won)
        //                {
        //                    if (playerTeam == 1)
        //                    {

        //                        RoundSystem.RS.team1Won = true;
        //                    }
        //                    else
        //                    {
        //                        RoundSystem.RS.team2Won = true;
        //                    }
        //                }
        //            }
        //        }
        //        if (MainMenu.teamDeathmatch)
        //        {
        //            if (opponentTeam != playerTeam)
        //            {
        //                if (opponentTeam == 1)
        //                {
        //                    int playerLeft = (int)PhotonNetwork.CurrentRoom.CustomProperties["Team1"] - 1;

        //                    PhotonNetwork.CurrentRoom.CustomProperties["Team1"] = playerLeft;

        //                }
        //                else
        //                {
        //                    int playerLeft = (int)PhotonNetwork.CurrentRoom.CustomProperties["Team2"] - 1;

        //                    PhotonNetwork.CurrentRoom.CustomProperties["Team2"] = playerLeft;
        //                }

        //                if (!RoundSystem.RS.team1Won && !RoundSystem.RS.team2Won)
        //                {
        //                    if (playerTeam == 1 && (int)PhotonNetwork.CurrentRoom.CustomProperties["Team2"] == 0)
        //                    {

        //                        RoundSystem.RS.team1Won = true;
        //                    }
        //                    else if (playerTeam == 2 && (int)PhotonNetwork.CurrentRoom.CustomProperties["Team1"] == 0)
        //                    {
        //                        RoundSystem.RS.team2Won = true;
        //                    }
        //                }

        //            }

        //        }
        //    PhotonNetwork.DestroyPlayerObjects(otherPlayer);

        //}

        

        if (currentScene != multiplayerScene)
            {

                ClearPlayerListings();
                PlayerListings();

            }






            playersInGame--;
        
        
    }

    public void DisconnectPlayer()
    {

        

        Destroy(PhotonRoom.room.gameObject);



        StartCoroutine(DisconnectAndLoad());
    }

    IEnumerator DisconnectAndLoad()
    {
        //PhotonNetwork.Disconnect();

        PhotonNetwork.LeaveRoom();


        //while (PhotonNetwork.IsConnected)
        while (PhotonNetwork.InRoom)
            yield return null;

        if (PhotonNetwork.NetworkClientState != ClientState.Leaving)
        {
            SceneManager.LoadScene(MultilplayerSettings.multiplayerSettings.menuScene);
        }
        
    }

    
    public override void OnDisconnected(DisconnectCause cause)
    {
        StartCoroutine(MainReconnect());
        ClearPlayerListings();
        PlayerListings();
    }
    private IEnumerator MainReconnect()
    {
        while (PhotonNetwork.NetworkingClient.LoadBalancingPeer.PeerState != ExitGames.Client.Photon.PeerStateValue.Disconnected)
        {
            Debug.Log("Waiting for client to be fully disconnected..", this);

            yield return new WaitForSeconds(0.2f);
        }

        
        
        Debug.Log("Client is disconnected!", this);
        //PhotonNetwork.ConnectUsingSettings();
        //if (PhotonNetwork.InRoom)
        //{
            PhotonNetwork.ReconnectAndRejoin();
        //}
        if (!PhotonNetwork.ReconnectAndRejoin())
        {
            

            if (PhotonNetwork.Reconnect())
            {
                Debug.Log("Successful reconnected!", this);
                
            }
        }
        else
        {
            Debug.Log("Successful reconnected and joined!", this);

            PhotonNetwork.RejoinRoom(roomName);




        }
    }

    
    

}
