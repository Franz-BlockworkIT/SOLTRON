using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class GameManager: MonoBehaviourPunCallbacks
{
    [Header("UI")]
    public GameObject pauseCanvas;
    [SerializeField] GameObject LostInternet;

    [Header("Instances")]
    public static GameManager GM;
    public RoundSystem RS;
    public PhotonView PV;

    [Header("Other")]

    public bool canMove = true;

    public int nextPlayersTeam;
    public Transform[] spawn_pointsTeam1;
    public Transform[] spawn_pointsTeam2;
    public GameObject Tournament;


    public bool paused;


    [SerializeField] int solBalEarned;


    [Header("TeamDeathMatch Team Color")]
    public Color[] randomMaterials;
    public static int RandomColorTeam1;
    public static int RandomColorTeam2;
    public static bool clrChanged;
    public bool LWclrChanged;

    public PhotonPlayer player;
    private void Awake()
    {
        PhotonPlayer.TeamChange = false;

        ColorPicker.colorChange = false;

        PV = GetComponent<PhotonView>();
        clrChanged = false;
        ColorChange.clrChangedOne = false;
        if (!PhotonNetwork.IsMasterClient)
            return;

        RandomColorTeam1 = Random.Range(0, randomMaterials.Length - 1);
        RandomColorTeam2 = Random.Range(0, randomMaterials.Length - 1);
        if (!clrChanged)
        {
            PV.RPC("ChangeTeamColor", RpcTarget.AllBuffered, RandomColorTeam1, RandomColorTeam2);
        }
    }
    private void OnEnable()
    {

        RS = GetComponent<RoundSystem>();
        if (GameManager.GM == null)
            GameManager.GM = this;
    }
    private void Start()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            paused = !paused;
            setPaused();
        }

        

        if (MainMenu.tournament)
        {
            

            Tournament.SetActive(true);
        }
        else
        {
            Tournament.SetActive(false);
        }

        if(PhotonNetwork.NetworkClientState == Photon.Realtime.ClientState.ConnectingToGameServer || PhotonNetwork.NetworkClientState == Photon.Realtime.ClientState.ConnectingToMasterServer)
        {
            LostInternet.SetActive(true);
        }
        else
        {
            LostInternet.SetActive(false);
        }
    }

    public void setPaused()
    {
        pauseCanvas.SetActive(paused);

        Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;

        Cursor.visible = paused;

    }

    public bool disconnected;
    public void DisconnectPlayer()
    {
        disconnected = true;


        foreach (PhotonPlayer obj in FindObjectsOfType<PhotonPlayer>())
        {
            if (obj.PV.IsMine)
            {
                player = obj;
                
                    PhotonNetwork.Destroy(player.gameObject);

                    if (player.myAvatar != null)
                    {
                        PhotonNetwork.Destroy(player.myAvatar.gameObject);
                    }
                

            }
        }

        
            StartCoroutine(DisconnectAndLoad());
        
    }

   

    

    IEnumerator DisconnectAndLoad()
    {

       
            if (PhotonRoom.room.currentScene == 1)
            {

                int playerTeam = (int)PhotonNetwork.LocalPlayer.CustomProperties["Team"];
                if (MainMenu.quickMatch || MainMenu.tournament)
                {


                    if (!RoundSystem.RS.team1Won && !RoundSystem.RS.team2Won)
                    {
                        if (playerTeam == 1)
                        {

                            PV.RPC("TeamWon", RpcTarget.AllBuffered, 2, true);
                        }
                        else
                        {
                            PV.RPC("TeamWon", RpcTarget.AllBuffered, 1, true);
                        }
                    }
                }

                if (MainMenu.teamDeathmatch)
                {

                    if (playerTeam == 1)
                    {
                        int playerLeft = (int)PhotonNetwork.CurrentRoom.CustomProperties["Team1"] - 1;

                        PhotonNetwork.CurrentRoom.CustomProperties["Team1"] = playerLeft;

                    }
                    else
                    {
                        int playerLeft = (int)PhotonNetwork.CurrentRoom.CustomProperties["Team2"] - 1;

                        PhotonNetwork.CurrentRoom.CustomProperties["Team2"] = playerLeft;
                    }

                    if (!RoundSystem.RS.team1Won && !RoundSystem.RS.team2Won)
                    {
                        if (playerTeam == 1 && (int)PhotonNetwork.CurrentRoom.CustomProperties["Team1"] == 1)
                        {

                            PV.RPC("TeamWon", RpcTarget.AllBuffered, 2, true);
                        }
                        else if (playerTeam == 2 && (int)PhotonNetwork.CurrentRoom.CustomProperties["Team2"] == 1)
                        {
                            PV.RPC("TeamWon", RpcTarget.AllBuffered, 1, true);

                            
                        }
                    }





                }
                PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer);
            }

        

        PhotonNetwork.LeaveRoom();



        if (MainMenu.tournament)
        {
            TournamentData._TD.tourUser.userLeft = true;

            Proyecto26.RestClient.Put("https://soltron-c922b-default-rtdb.asia-southeast1.firebasedatabase.app" + "/Tournaments/Users/Tournament" + TournamentData.getLocalId + "Users/" + TournamentData.localId + ".json", TournamentData._TD.tourUser);
        }

        while (PhotonNetwork.InRoom)
            yield return null;
        
        SceneManager.LoadScene(MultilplayerSettings.multiplayerSettings.menuScene); 
    }

    public void ScreenShot()
    {
        ScreenCapture.CaptureScreenshot("Screenshot.png");
    }

    [PunRPC]
    void TeamWon(int teamP, bool teamWon)
    {
        if (teamP == 1)
        {
            RoundSystem.RS.team1Won = teamWon;
        }
        else
        {
            RoundSystem.RS.team2Won = teamWon;
        }
    }
    public void UpdateTeam()
    {
        if(nextPlayersTeam == 1)
        {
            nextPlayersTeam = 2;
        }
        else
        {
            nextPlayersTeam = 1;
        }
    }

    [PunRPC]

    void ChangeTeamColor(int team1,int team2)
    {
        RandomColorTeam1 = team1;
        RandomColorTeam2 = team2;
    }

    
}
