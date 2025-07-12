using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine.SceneManagement;

public class Tournament : MonoBehaviourPunCallbacks
{

    [Header("UI")]
    [SerializeField] GameObject WinPanel;
    [SerializeField] GameObject LossPanel;
    [SerializeField] GameObject TournamentWinPanel;
    [SerializeField] GameObject LoadingScreen;

    [Header("Vfx")]
    [SerializeField] GameObject FireWork;

    [Header("Other")]

    public static int Match = 1;

    public static int team;

    public bool TournamentWon;
    bool leaving = true;
    

    
    public override void OnConnectedToMaster()
    {
        Debug.Log("Player Has Connected to the Master!");
        PhotonNetwork.AutomaticallySyncScene = true;
        
    }

    private void Update()
    {

        
        if(TournamentData._TD.finalist)
        {

            TournamentWon = true;
        }
        

        if (!TournamentWon)
        {
            if (RoundSystem.RS.team1Won)
            {
                if (team == 1)
                {
                    if (leaving)
                    {
                        PhotonNetwork.LeaveRoom();
                        leaving = false;
                    }
                    StartCoroutine("waitForRoomLeave");
                }
                else
                {
                    LossPanel.SetActive(true);
                }
            }

            if (RoundSystem.RS.team2Won)
            {
                if (team == 2)
                {
                    if (leaving)
                    {
                        PhotonNetwork.LeaveRoom();
                        leaving = false;
                    }
                    StartCoroutine("waitForRoomLeave");
                }
                else
                {
                    LossPanel.SetActive(true);
                }
            }
        }
        else
        {
            if (RoundSystem.RS.team1Won)
            {

                if (!WinPanel.activeSelf)
                {
                    if (team == 1)
                    {

                        TournamentWinPanel.SetActive(true);
                        FireWork.SetActive(true);

                    }
                    else
                    {
                        LossPanel.SetActive(true);
                    }
                }
            }

            if (RoundSystem.RS.team2Won)
            {
                if (!WinPanel.activeSelf)
                {
                    if (team == 2)
                    {
                        TournamentWinPanel.SetActive(true);
                        FireWork.SetActive(true);


                    }
                    else
                    {
                        LossPanel.SetActive(true);
                    }
                }
            }
        }
    }

  

    public void OnContinueButtonClicked()
    {
        Debug.Log("is Master Client : " + PhotonNetwork.IsConnected);

        SceneManager.LoadScene(2);
        
        
    }



   
    IEnumerator waitForRoomLeave()
    {
        yield return new WaitForSeconds(3);

        WinPanel.SetActive(true);


    }

}
