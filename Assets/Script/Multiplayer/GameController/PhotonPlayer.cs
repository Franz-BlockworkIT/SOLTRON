using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using System;
using Photon.Realtime;

public class PhotonPlayer : MonoBehaviourPunCallbacks, IInRoomCallbacks
{

    public static PhotonPlayer _instance;

    public PhotonView PV;
    public GameObject myAvatar;
    public int myTeam;
    public int team;
    
    public static bool TeamChange;
    private LevelSystem levelsystem;


    public RoundSystem RS;
    

    

    bool spawned;


    Transform[] Team1Pos;
    Transform[] Team2Pos;

    bool Added = false;


   

    [Header("Level System")]
    [SerializeField] private LevelWindow levelWindowWin;
    [SerializeField] private LevelWindow levelWindowLose;
    [SerializeField] private LevelWindow levelWindowTour;


    private void Awake()
    {
        _instance = this;

        PV = GetComponent<PhotonView>();
        RS = GameObject.Find("Game Manager").GetComponent<RoundSystem>();


        Team1Pos = GameManager.GM.spawn_pointsTeam1;
        Team2Pos = GameManager.GM.spawn_pointsTeam2;

        if (PV.IsMine)
        {
            levelWindowWin = GameObject.Find("Canvas").transform.Find("Level Win Panel").transform.Find("Level Window").GetComponent<LevelWindow>();
            levelWindowLose = GameObject.Find("Canvas").transform.Find("Level lose Panel").transform.Find("Level Window").GetComponent<LevelWindow>();
            levelWindowTour = GameObject.Find("Canvas").transform.Find("Tournament").transform.Find("TournamentWinPanel").transform.Find("Level Win Panel").transform.Find("Level Window").GetComponent<LevelWindow>();
        }
    }
    private void Start()
    {
        
        if (PV.IsMine)
        {
            if (!TeamChange)
            {
                //PV.RPC("RPC_GetTeam", RpcTarget.MasterClient);
                TeamChange = true;
            }
            myTeam = (int)PhotonNetwork.LocalPlayer.CustomProperties["Team"];
        }
        else
        {
            return;
        }
        LevelSystem levelSystem = new LevelSystem();


        levelWindowWin.SetLevelSystem(levelSystem);
        levelWindowLose.SetLevelSystem(levelSystem);
        levelWindowTour.SetLevelSystem(levelSystem);
        SetLevelSystem(levelSystem);





    }

    private void Update()
    {
        
        

        SpawnPlayer();

        if (RoundSystem.RS.team1Won)
        {

            if (!MainMenu.tournament)
            {
                if (PV.IsMine && myTeam == 1)
                {



                    if (MainMenu.freeToPlay || MainMenu.training)
                    {
                        RoundSystem.RS.xpWinPanel.SetActive(true);
                    }
                    else
                    {
                        RoundSystem.RS.WinPanel.SetActive(true);
                        GameManager.GM.canMove = false;
                    }
                }
                if (PV.IsMine && myTeam != 1)
                {
                    if (MainMenu.freeToPlay || MainMenu.training)
                    {
                        RoundSystem.RS.xpLosePanel.SetActive(true);
                    }
                    else
                    {
                        RoundSystem.RS.LosePanel.SetActive(true);
                        GameManager.GM.canMove = false;
                    }

                }







                
            }
            else
            {
                if (PV.IsMine && myTeam == 1)
                {

                    if (!TournamentData._TD.semiFinalist)
                    {
                        TournamentData._TD.semiFinalist = true;
                    }
                    else if (!TournamentData._TD.finalist)
                    {
                        TournamentData._TD.finalist = true;
                    }
                    else
                    {
                        // Tournament Won
                    }
                }
            }

        }
        else if (RoundSystem.RS.team2Won)
        {
            Debug.Log("Team2 Won : " + RoundSystem.RS.team2Won);

            if (!MainMenu.tournament)
            {
                if (PV.IsMine && myTeam == 2)
                {

                    Debug.Log("playerTeam : " + myTeam);

                    if (MainMenu.freeToPlay || MainMenu.training)
                    {
                        RoundSystem.RS.xpWinPanel.SetActive(true);
                    }
                    else
                    {
                        RoundSystem.RS.WinPanel.SetActive(true);
                        GameManager.GM.canMove = false;
                    }
                }

                if (PV.IsMine && myTeam != 2)
                {
                    if (MainMenu.freeToPlay || MainMenu.training)
                    {
                        RoundSystem.RS.xpLosePanel.SetActive(true);
                    }
                    else
                    {
                        RoundSystem.RS.LosePanel.SetActive(true);
                        GameManager.GM.canMove = false;
                    }

                }


            }
            else
            {
                if (PV.IsMine && myTeam == 2)
                {

                    if (!TournamentData._TD.semiFinalist)
                    {
                        TournamentData._TD.semiFinalist = true;
                    }
                    else if (!TournamentData._TD.finalist)
                    {
                        TournamentData._TD.finalist = true;
                    }
                    else
                    {
                        // Tournament Won
                    }

                }

            }
        }

            if (PV.IsMine)
        {
            if (!Added)
            {

                            Debug.Log("Outside freeplay");

                if (MainMenu.freeToPlay)
                {
                            Debug.Log("Inside freeplay 1");

                    if (RoundSystem.RS.team1Won || RoundSystem.RS.team2Won)
                        {
                            

                            GetExpAfterWin();
                        }
                    
                }
                if (MainMenu.playToEarn)
                {
                    EarnAfterWin();
                }
                
            }

           

        }

    }

    [PunRPC]
    void RPC_GetTeam()
    {
        myTeam = GameManager.GM.nextPlayersTeam;
        GameManager.GM.UpdateTeam();
        PV.RPC("RPC_SentTeam", RpcTarget.OthersBuffered, myTeam);
    }

    [PunRPC]
    void RPC_SentTeam(int whichTeam)
    {
        myTeam = whichTeam;
    }
    public void AddExp(int exp)
    {


        if (levelsystem != null)
        {
            levelsystem.AddExperience(exp);

        }

    }
    
    public void LoseExp(int exp)
    {
        
        if(levelsystem != null)
            levelsystem.LoseExperience(exp);

    }

    #region Spawn Players
    void SpawnPlayer()
    {
        if (/*RS.waveCountdown > 5*/ RoundSystem.RS.spawnPlayers)
        {
            if (myAvatar == null)
            {
                if (myTeam == 1)
                {
                    int spawnPicker = UnityEngine.Random.Range(0, Team1Pos.Length);

                    if (PV.IsMine)
                    {

                        if (!GameManager.GM.spawn_pointsTeam1[spawnPicker].GetComponent<PostitionCheck>().occupied)
                        {


                            myAvatar = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"), GameManager.GM.spawn_pointsTeam1[spawnPicker].position, GameManager.GM.spawn_pointsTeam1[spawnPicker].rotation, 0);
                            if (myAvatar != null)
                            { myAvatar.GetComponent<PhotonView>().RPC("SetTeamID", RpcTarget.AllBuffered, myTeam); }


                            if (!spawned)
                            {
                                spawned = true;


                                    team = 1;


                            }
                        }


                    }

                }
                else
                {
                    int spawnPicker = UnityEngine.Random.Range(0, Team2Pos.Length);
                    if (PV.IsMine)
                    {
                        if (!GameManager.GM.spawn_pointsTeam2[spawnPicker].GetComponent<PostitionCheck>().occupied)
                        {
                            myAvatar = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"), GameManager.GM.spawn_pointsTeam2[spawnPicker].position, GameManager.GM.spawn_pointsTeam2[spawnPicker].rotation, 0);
                            if (myAvatar != null)
                            {
                                myAvatar.GetComponent<PhotonView>().RPC("SetTeamID", RpcTarget.AllBuffered, myTeam);
                            }
                            if (!spawned)
                            {
                                spawned = true;
                                
                                    PV.RPC("teamPlayers", RpcTarget.AllBuffered, 2);
                                team = 2;



                            }
                        }

                    }

                }
            }
        }

    }

    #endregion


    #region After Winning Functions
    void GetExpAfterWin() {

        if (myTeam == 1)
        {


            Debug.Log("Team 1 : " + team);
            if (RoundSystem.RS.team1Won)
            {
            Debug.Log("MY Team 1 : " + myTeam);

                PlayerScores.playerScore += 1;
                //PlayerScores._PS.OnSubmit();
                PlayerPrefs.SetInt(PlayerScores.playerName + "BattleWon", PlayerPrefs.GetInt(PlayerScores.playerName + "BattleWon", PlayerScores.playerBattleWon) + 1);
                PlayerScores.playerBattleWon = PlayerPrefs.GetInt(PlayerScores.playerName + "BattleWon", PlayerScores.playerBattleWon);
                if (MainMenu.quickMatch)
                {
                    

                    AddExp(100 * PlayerPrefs.GetInt(PlayerScores.playerName + "Level", PlayerScores.playerLevel));
                    Added = true;
                }
                if (MainMenu.training)
                {
                    Debug.Log("Training : " + MainMenu.training);

                    AddExp(50 * PlayerPrefs.GetInt(PlayerScores.playerName + "Level", PlayerScores.playerLevel));
                    Added = true;
                }
                if (MainMenu.teamDeathmatch)
                {
                    AddExp(200 * PlayerPrefs.GetInt(PlayerScores.playerName + "Level", PlayerScores.playerLevel));
                    Added = true;
                }
                if (MainMenu.tournament && Tournament.Match == 8)
                {
                    AddExp(500 * PlayerPrefs.GetInt(PlayerScores.playerName + "Level", PlayerScores.playerLevel));
                    Added = true;
                }
                
            }
            else
            {
                if (levelsystem != null)
                {
                    if (levelsystem.GetExpNumber() > 0)
                    {
                        if (MainMenu.quickMatch)
                        {
                            LoseExp(100 * PlayerPrefs.GetInt(PlayerScores.playerName + "Level", PlayerScores.playerLevel) / 3);
                            Added = true;
                        }
                        if (MainMenu.training)
                        {
                            Debug.Log("Training Lose : " + MainMenu.training);

                            LoseExp(50 * PlayerPrefs.GetInt(PlayerScores.playerName + "Level", PlayerScores.playerLevel) / 3);
                            Added = true;

                        }
                        if (MainMenu.teamDeathmatch)
                        {
                            LoseExp(200 * PlayerPrefs.GetInt(PlayerScores.playerName + "Level", PlayerScores.playerLevel) / 3);
                            Added = true;
                        }
                        
                    }
                }
            }
        } 
        
        else if (myTeam == 2)
        {
            Debug.Log("Team 2 : " + team);

            if (RoundSystem.RS.team2Won)
            {
                Debug.Log("Team 1 Won : " + RoundSystem.RS.team1Won);
                Debug.Log("Team 2 Won : " + RoundSystem.RS.team2Won);


                PlayerScores.playerScore += 1;
                //PlayerScores._PS.OnSubmit();
                PlayerPrefs.SetInt(PlayerScores.playerName + "BattleWon", PlayerPrefs.GetInt(PlayerScores.playerName + "BattleWon") + 1);
                PlayerScores.playerBattleWon = PlayerPrefs.GetInt(PlayerScores.playerName + "BattleWon",PlayerScores.playerBattleWon);
                
                

                if (MainMenu.training)
                {
                    AddExp(50 * PlayerPrefs.GetInt(PlayerScores.playerName + "Level", PlayerScores.playerLevel));
                    Added = true;
                }
                if (MainMenu.quickMatch)
                {
                    AddExp(100 * PlayerPrefs.GetInt(PlayerScores.playerName + "Level", PlayerScores.playerLevel));
                    Added = true;
                }
                if (MainMenu.teamDeathmatch)
                {
                    AddExp(200 * PlayerPrefs.GetInt(PlayerScores.playerName + "Level", PlayerScores.playerLevel));
                    Added = true;
                }
                if (MainMenu.tournament && Tournament.Match == 8)
                {
                    AddExp(500 * PlayerPrefs.GetInt(PlayerScores.playerName + "Level", PlayerScores.playerLevel));
                    Added = true;
                }
                

                if (ConnectToWallet.isWalletUser)
                {
                    ConnectToWallet._CW.SaveData();
                }

            }
            else
            {

                if (levelsystem != null)
                {
                    if (levelsystem.GetExpNumber() > 0)
                    {
                        if (MainMenu.training)
                        {
                            LoseExp(50 * PlayerPrefs.GetInt(PlayerScores.playerName + "Level", PlayerScores.playerLevel)/ 3);
                            Added = true;
                        }
                        if (MainMenu.quickMatch)
                        {
                            LoseExp(100 * PlayerPrefs.GetInt(PlayerScores.playerName + "Level", PlayerScores.playerLevel) / 3);
                            Added = true;
                        }
                        if (MainMenu.teamDeathmatch)
                        {
                            LoseExp(200 * PlayerPrefs.GetInt(PlayerScores.playerName + "Level", PlayerScores.playerLevel)/ 3);
                            Added = true;
                        }
                    }
                    
                }
            }
        }

        if (ConnectToWallet.isWalletUser)
        {
            ConnectToWallet._CW.SaveData();
        }


    }

    void EarnAfterWin()
    {
        if (myTeam == 1)
        {
            if (RoundSystem.RS.team1Won)
            {
                PlayerPrefs.SetInt(PlayerScores.playerName + "BattleWon", PlayerPrefs.GetInt(PlayerScores.playerName + "BattleWon") + 1);

                if (MainMenu.quickMatch)
                {

                    PlayerPrefs.SetInt(PlayerScores.playerName + "SolBal" ,PlayerPrefs.GetInt(PlayerScores.playerName + "SolBal", PlayerScores.playerSolBal) + 100);
                    Added = true;

                }
                if (MainMenu.teamDeathmatch)
                {
                    PlayerPrefs.SetInt(PlayerScores.playerName + "SolBal", PlayerPrefs.GetInt(PlayerScores.playerName + "SolBal", PlayerScores.playerSolBal) + 400);
                    Added = true;

                }
                if (MainMenu.tournament && Tournament.Match == 8)
                {
                    PlayerPrefs.SetInt(PlayerScores.playerName + "SolBal", PlayerPrefs.GetInt(PlayerScores.playerName + "SolBal", PlayerScores.playerSolBal) + 1000);
                    Added = true;

                }



            }

        }

        if (myTeam == 2)
        {


            if (RoundSystem.RS.team2Won)
            {
                if (MainMenu.quickMatch)
                {
                    PlayerPrefs.SetInt(PlayerScores.playerName + "SolBal", PlayerPrefs.GetInt(PlayerScores.playerName + "SolBal", PlayerScores.playerSolBal) + 100);
                    Added = true;

                }
                if (MainMenu.teamDeathmatch)
                {
                    PlayerPrefs.SetInt(PlayerScores.playerName + "SolBal", PlayerPrefs.GetInt(PlayerScores.playerName + "SolBal", PlayerScores.playerSolBal) + 400);
                    Added = true;


                }
                if (MainMenu.tournament && Tournament.Match == 8)
                {
                    PlayerPrefs.SetInt(PlayerScores.playerName + "SolBal", PlayerPrefs.GetInt(PlayerScores.playerName + "SolBal", PlayerScores.playerSolBal) + 1000);
                    Added = true;


                }
                

            }

        }
        PlayerScores.playerSolBal = PlayerPrefs.GetInt(PlayerScores.playerName + "SolBal", PlayerScores.playerSolBal);

        if (ConnectToWallet.isWalletUser)
        {
            ConnectToWallet._CW.SaveData();
        }
    }


    [PunRPC]

    void teamPlayers(int teamP)
    {
        if(teamP == 1)
        {
            RoundSystem.RS.team1Players++;

        }
        else
        {
            RoundSystem.RS.team2Players++;

        }
    }

    

    
    
    


    public void SetLevelSystem(LevelSystem lvlsystem)
    {

        this.levelsystem = lvlsystem;
        levelsystem.onLevelChanged += LevelSystem_onLevelChanged;

    }

    
    private void LevelSystem_onLevelChanged(object sender, EventArgs e)
    {
        Debug.Log("Level UP");
    }

    #endregion

}


