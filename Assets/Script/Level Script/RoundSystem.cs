using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Photon.Pun;
using System.Collections.Generic;

public class RoundSystem : MonoBehaviour
{


    public static RoundSystem RS;
    
    public enum SpawnState { Spawning, Waiting, Counting }
    [System.Serializable]
    public class Wave
    {
        public string Round;
        public Transform enemy;
        public Transform teammate;
        public int count;
        public float rate;
    }

    public Wave[] waves;
    [SerializeField]private int nextWave = 0;

    public float timeBetweenWaves = 10f;
    public float waveCountdown;

    private float searchCountdown = 3f;

    private SpawnState state = SpawnState.Counting;

    public Transform[] spawnPointsTeam1;
    public Transform[] spawnPointsTeam2;

    public int team1WinScore;
    public int team2WinScore;
    public bool team1Won;
    public bool team2Won;

    public GameObject xpWinPanel;
    public GameObject xpLosePanel;


    public bool nextRound = true ;
    public bool RoundStarted;

    
    [Header("UI")]
    public Text CountDownUI;
    public Text enemiesCountUI;
    public Text teammateCountUI;
    public Text RoundsUI;
    public Text team1WinPointTxtUI;
    public Text team2WinPointTxtUI;

    public GameObject WinPanel;
    public GameObject LosePanel;

    GameManager stop;

    PhotonView PV;

    public bool AiBotClrChanged;
    

    public List<GameObject> posCheck;

    public int team1Players;
    public int team2Players;

    public int team1Clr;
    public int team2Clr;

    public bool masterChanged;

    private bool PlayersAlive;

    public PhotonPlayer player;

    private void Awake()
    {
        RS = this;
        stop = GetComponent<GameManager>();
        PV = GetComponent<PhotonView>();
        
    }
    private void Start()
    {
        foreach (PhotonPlayer obj in FindObjectsOfType<PhotonPlayer>())
        {
            if (obj.PV.IsMine)
            {
                player = obj;
            }
        }

        if (MainMenu.training)
        {
            timeBetweenWaves = 7;
        }
        waveCountdown = timeBetweenWaves;
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        
        

        if (spawnPointsTeam1.Length == 0)
        {
            Debug.Log("No Spawn Points Team 1");
        }
        if (spawnPointsTeam2.Length == 0)
        {
            Debug.Log("No Spawn Points Team 2");
        }

        Time.timeScale = 1;

        
        StartCoroutine(RPC_timer());
        

    }

    GameObject[] totalTeam1;
    private void Update()
    {

        //PV.RPC("RPC_RoundStarted", RpcTarget.AllBuffered, RoundStarted);
        Debug.Log("Round started : " + RoundStarted);
        //PV.RPC("RPC_ChangeState", RpcTarget.AllBuffered, state);






        if (team1Won)
        {            
                Team1WinPoints();
                Team2WinPoints();

                return;
        }
        else if (team2Won)
        {
            Debug.Log("Team2 Won : " + team2Won);

           
            Team1WinPoints();
            Team2WinPoints();


            return;
        }



        if (waveCountdown <= 0)
        {
            stop.canMove = true;

        }
        else
        {
            stop.canMove = false;

        }

        //EnemiesCount();
        //TeammateCount();
        Team1WinPoints();
        Team2WinPoints();

        if (waveCountdown > 5 || waveCountdown < 1)
        {
            CountDownUI.gameObject.SetActive(false);
        }
        else
        {
            CountDownUI.gameObject.SetActive(true);
        }


        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        if (PhotonRoom.room.playersInRoom != PhotonNetwork.PlayerList.Length)
        {
            Debug.Log("Not all players Are in Room! ");
            return;
        }

        totalEnemies = GameObject.FindGameObjectsWithTag("Team2");
        int totalTeam2alive = totalEnemies.Length;
        PV.RPC("EnemiesCount", RpcTarget.AllBuffered,totalTeam2alive);

        totalTeam1 = GameObject.FindGameObjectsWithTag("Team1");
        int totalTeam1Alive = totalTeam1.Length;
        PV.RPC("TeammateCount", RpcTarget.AllBuffered, totalTeam1Alive);


        PV.RPC("RPC_RoundStarted", RpcTarget.AllBuffered, RoundStarted);
        PV.RPC("RPC_SpawnPlayer", RpcTarget.AllBuffered, spawnPlayers);
        PV.RPC("RPC_TeamScore", RpcTarget.AllBuffered, team1WinScore,team2WinScore);


        if (nextRound)
        {

            PV.RPC("CountDownTimerUI", RpcTarget.AllBuffered);
            RoundTextUI();

        }


        //EnemiesCount();
        //TeammateCount();
        Team1WinPoints();
        Team2WinPoints();

        //if (RoundStarted)
        //{

        //    //PV.RPC("RPC_RoundStarted", RpcTarget.AllBuffered, false);
        //    RoundStarted = false;

        //}
        if (spawnPlayers && FindObjectsOfType<Player>().Length == PhotonNetwork.PlayerList.Length)
        {
            spawnPlayers = false;
        }

        //-----------------------------//




        if (state == SpawnState.Waiting)
            {


            if (FindObjectsOfType<PhotonPlayer>().Length == PhotonNetwork.PlayerList.Length)
            {
                
                if (!EnemyIsAlive() && !PlayersAlive)
                {
                    print("State : " + state);

                    PV.RPC("BeginNewRound", RpcTarget.AllBuffered);

                    return;
                }
                else
                {
                    PlayersAlive = false;
                    return;
                }
            }
            }


            if (state != SpawnState.Spawning)
            {


            if (FindObjectsOfType<PhotonPlayer>().Length == PhotonRoom.room.playersInRoom)
            {
                StartCoroutine(SpawnWave(waves[nextWave]));
            }
                
            }

        

    }


    [PunRPC]
    void BeginNewRound()
    {
        

        state = SpawnState.Counting;
        

        // After 3 Rounds Check
        if (nextWave + 1 > waves.Length - 1) {

            if (waveCountdown < 5)
            {
                
                    if (totalEnemies.Length == 0 && teamPlayersAlive > 0)
                    {
                    //PV.RPC("RPC_TeamScore", RpcTarget.AllBuffered, 1);
                    team1WinScore++;


                    }
                    else if (totalEnemies.Length > 0 && teamPlayersAlive == 0)
                    {
                    //PV.RPC("RPC_TeamScore", RpcTarget.AllBuffered, 2);
                    team2WinScore++;
                    }
                

            }
            //if (!MainMenu.training)
            //{
                if ((team1WinScore >= 3) && team1WinScore != team2WinScore)
                {

                    PV.RPC("RPC_TeamWonCheck", RpcTarget.AllBuffered, 1, true);

                    PV.RPC("RPC_nextRound", RpcTarget.AllBuffered, false);


                }
                else if ((team2WinScore >= 3) && team2WinScore != team1WinScore)
                {

                    PV.RPC("RPC_TeamWonCheck", RpcTarget.AllBuffered, 2, true);


                    PV.RPC("RPC_nextRound", RpcTarget.AllBuffered, false);

                }
                else
                {

                    nextWave = 2;
                
                RoundStarted = true;

                RoundCount++;

                PV.RPC("RPC_nextRound", RpcTarget.AllBuffered, true);

                PlayersAlive = true;


                //waveCountdown = timeBetweenWaves;




            }


        }
        else
        {
            if (waveCountdown < 5)
            {
               
                    if (totalEnemies.Length == 0 && teamPlayersAlive > 0)
                    {

                    //PV.RPC("RPC_TeamScore", RpcTarget.AllBuffered, 1);
                    team1WinScore++;
                    PV.RPC("RPC_RoundStarted", RpcTarget.AllBuffered, true);
                    //RoundStarted = true;
                    RoundCount++;
                    nextWave++;

                    //waveCountdown = timeBetweenWaves;

                }
                    else if (totalEnemies.Length > 0 && teamPlayersAlive == 0)
                    {
                    //team2WinScore++;
                    //PV.RPC("RPC_TeamScore", RpcTarget.AllBuffered, 2);
                    team2WinScore++;
                    PV.RPC("RPC_RoundStarted", RpcTarget.AllBuffered, true);
                    //RoundStarted = true;
                    RoundCount++;
                    nextWave++;
                    PlayersAlive = true;

                    //waveCountdown = timeBetweenWaves;

                }



            }
            //RoundStarted = true;


            //RoundCount++;
            //nextWave++;

            //waveCountdown = timeBetweenWaves;


        }

    }
    

    bool EnemyIsAlive()
    {


        searchCountdown -= Time.deltaTime;
        if (searchCountdown <= 0f)
        {
            searchCountdown = 3f;
            if (GameObject.FindGameObjectsWithTag("Team2").Length == 0 || GameObject.FindGameObjectsWithTag("Team1").Length == 0)
            {
                

                return false;
            }
        }
        Debug.Log("Player Alive");
        return true;
    }



    public bool spawnPlayers;
    
    IEnumerator SpawnWave(Wave _wave)
    {

        PV.RPC("RPC_RoundStarted", RpcTarget.AllBuffered, false);
        PV.RPC("RPC_SpawnPlayer", RpcTarget.AllBuffered, true);

        Debug.Log(_wave.Round);

        spawnPlayers = true;
        PV.RPC("ResetTime", RpcTarget.AllBuffered);

        if (MainMenu.quickMatch || MainMenu.tournament)
        {
            if (!PhotonRoom.room.PlayerDetected) { _wave.count = 1;}

            else { _wave.count = 0; }
        }
        else
        {
               _wave.count = 4;
        }
        state = SpawnState.Spawning;



        if (/*(team1Players + team2Players) == PhotonRoom.room.playersInRoom */ MainMenu.training)
        {


        
            if (MainMenu.teamDeathmatch)
            {
                for (int i = 0; i < _wave.count; i++)
                {

                    StartCoroutine(SpawnEnemy(_wave.enemy, _wave.teammate, 1));
                    
                }
            }

            for (int i = 0; i < _wave.count; i++)
            {
                if (MainMenu.quickMatch)
                {
                    if (GameObject.FindGameObjectsWithTag("Team2").Length < 1)
                    {
                        StartCoroutine(SpawnEnemy(_wave.enemy,_wave.teammate, 2));
                    }
                }
                else
                {
                    StartCoroutine(SpawnEnemy(_wave.enemy, _wave.teammate, 2));

                }

            }

        }
        
        state = SpawnState.Waiting;



        yield break;
    }

    int totalSpawns1 = 0;
    int totalSpawns2 = 0;

    
   
    IEnumerator SpawnEnemy(Transform _enemy,Transform teammate,int team)
    {


        yield return new WaitForSecondsRealtime(0);
        Debug.Log("Spawning Enemy :" + _enemy.name);
            Transform _sp1 = spawnPointsTeam1[totalSpawns1];
            Transform _sp2 = spawnPointsTeam2[totalSpawns2];


        if (team == 1)
        {
            if (MainMenu.teamDeathmatch)
            {
                
                if (!_sp1.GetComponent<PostitionCheck>().occupied)
                {
                   PhotonNetwork.Instantiate(teammate.name, _sp1.transform.position, _sp1.transform.rotation);
                    
                    
                }
        if (totalSpawns1 != spawnPointsTeam1.Length - 1)
            totalSpawns1++;
        else
            totalSpawns1 = 0;
            }

        }
        else
        {


            if (!_sp2.GetComponent<PostitionCheck>().occupied)
            {
                PhotonNetwork.Instantiate(_enemy.name, _sp2.transform.position, _sp2.transform.rotation);

            }


        if (totalSpawns2 != spawnPointsTeam2.Length - 1)
            totalSpawns2++;
        else
            totalSpawns2 = 0;
        }
        

        
    }


    GameObject[] totalEnemies;
    public int enemiesLength;

    [PunRPC]
    void EnemiesCount(int team2Alive)
    {
        enemiesLength = team2Alive;
        enemiesCountUI.text = enemiesLength.ToString();
    }
    
    public int teamPlayersAlive;

    [PunRPC]
    void TeammateCount(int team1Alive)
    {
        
        teamPlayersAlive = team1Alive;
        teammateCountUI.text = teamPlayersAlive.ToString();
    }


    [PunRPC]
    void CountDownTimerUI()
    {
        CountDownUI.text = waveCountdown.ToString("0");
        if (waveCountdown <= 0)
            CountDownUI.gameObject.SetActive(false);
        else if(waveCountdown <= 5)
            CountDownUI.gameObject.SetActive(true);

    }

    int RoundCount = 1;

    void RoundTextUI()
    {
        RoundsUI.text = $"Round {RoundCount}";
        if (waveCountdown >= 5)
            RoundsUI.gameObject.SetActive(true);
        else
            RoundsUI.gameObject.SetActive(false);


    }

    void Team1WinPoints()
    {
        team1WinPointTxtUI.text = team1WinScore.ToString();
    }
    void Team2WinPoints()
    {
        team2WinPointTxtUI.text = team2WinScore.ToString();
    }

    

    public int count;

    IEnumerator RPC_timer()
    {
        while(FindObjectsOfType<Player>().Length != PhotonRoom.room.playersInRoom)
        {
            yield return null;
        }
            PV.RPC("timerAsync", RpcTarget.AllBuffered, waveCountdown);
        
        yield return new WaitForSeconds(1);
        StartCoroutine(RPC_timer());

    }

    [PunRPC]
    public void timerAsync(float timerCount)
    {
                waveCountdown--;
    }
    [PunRPC]
    public void ResetTime()
    {
        waveCountdown = timeBetweenWaves;
        
    }

    [PunRPC]
    void RPC_TeamScore(int team1,int team2)
    {
        team1WinScore = team1;
        team2WinScore = team2;

        //if(team == 1)
        //{
        //    team1WinScore++;
        //}
        //else
        //{
        //    team2WinScore++;
        //}
    }
   
    


    [PunRPC]
    void RPC_RoundStarted(bool started)
    {
        RoundStarted = started;
    }
    [PunRPC]
    void RPC_SpawnPlayer(bool spawn)
    {
        spawnPlayers = spawn;
    }

    [PunRPC]
    void RPC_ChangeState(SpawnState ChangeState)
    {
        state = ChangeState;
    }

    [PunRPC]
    void RPC_nextRound(bool next)
    {
        nextRound = next;

    }

    [PunRPC]

    void RPC_TeamWonCheck(int team , bool check)
    {
        if (team == 1)
        {
            team1Won = check;
        }
        else
        {
            team2Won = check;
        }
    }



    
}
