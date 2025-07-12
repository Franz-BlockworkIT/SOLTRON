using System.Collections;
using System.Collections.Generic;
using FullSerializer;
using Proyecto26;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class ConnectToWallet : MonoBehaviour
{

    public static ConnectToWallet _CW;
    WalletUsers wuser = new WalletUsers();

    private string databaseURL = "https://soltron-c922b-default-rtdb.asia-southeast1.firebasedatabase.app";
    

    public static fsSerializer serializer = new fsSerializer();

    public static string playerWalletAddress;

    bool alreadyExist;

    public static bool isWalletUser;

    [Header("UI")]

    [SerializeField]GameObject UsernamePanel;
    [SerializeField] GameObject ErrorPanel;
    [SerializeField] Text ErrorText ;
    [SerializeField] GameObject MainScreenPanel;
    [SerializeField] GameObject LoginScreenPanel;
    [SerializeField] TMP_InputField usernameInput;



    private void Awake()
    {
        _CW = this;
    }

    private void Update()
    {
        playerWalletAddress = CryptoReceiver.CR.walletAddress;

        
        
    }


    public void CheckPlayer()
    {
        StartCoroutine(CheckWalletAddress());
    }
    IEnumerator CheckWalletAddress()
    {
        while(!CryptoReceiver.CR.isConnected && CryptoReceiver.CR.walletAddress == "")
        {
            yield return null;
        }
        
        GetPlayer();
    }


    public void CheckUser()
    {

        if (usernameInput.text.Length < 4)
        {

            ErrorText.text = "Username Not Long Enough !";
            ErrorPanel.SetActive(true);
           
            return;
        }

        string pathOfUser = "/Users/WalletUsers/";


        RestClient.Get(databaseURL + pathOfUser + ".json").Then(response =>
        {
            Debug.Log("Get Data : " + response.Text);
            fsData userData = fsJsonParser.Parse(response.Text);
            Dictionary<string, WalletUsers> users = null;
            serializer.TryDeserialize(userData, ref users);



            foreach (var player in users.Values)
            {
                if (usernameInput.text.Length < 4)
                {

                    ErrorText.text = "Username Not Long Enough !";
                    ErrorPanel.SetActive(true);
                    alreadyExist = true;
                    break;
                }
                if (usernameInput.text == player.userName)
                {
                    ErrorText.text = "Username Already Exist\nTry Different Username";

                    ErrorPanel.SetActive(true);
                    alreadyExist = true;
                    break;
                }
                

            }
            if (!alreadyExist)
            {
                CreatePlayer();
            }
            else
                alreadyExist = false;


        }).Catch(error => {

            CreatePlayer();
            MainScreenPanel.SetActive(true);
            LoginScreenPanel.SetActive(false);
            UsernamePanel.SetActive(false);

            
            Debug.Log("Get User Data Failed : " + error);
            Debug.Log("Path Of User : " + pathOfUser);
        });
    }



    public void GetPlayer()
    {

        string pathOfUser = "/Users/WalletUsers/";
        RestClient.Get<WalletUsers>(databaseURL + pathOfUser + playerWalletAddress + "/.json").Then(response =>
        {
             wuser = response;

            PlayerScores.localId = response.userWalletAddress;
            PlayerScores.playerName = response.userName;
            PlayerScores.playerLevel = response.Level;
            PlayerScores.playerBikeLevel = response.userBikeLevel;
            PlayerScores.playerSolBal = response.userSolBal;
            PlayerScores.playerTronBal = response.userTronBal;
            PlayerScores.playerXp = response.xp;
            PlayerScores.playerBikeColor = response.userBikeColor;
            PlayerScores.playerBattleWon = response.userBattleWon;
            PhotonNetwork.NickName = response.userName;

            PlayerScores._PS.Bike.SetActive(true);
            PlayerScores._PS.InsideGame();
            MainScreenPanel.SetActive(false);
            isWalletUser = true;
            LoginScreenPanel.SetActive(true);
            PlayerScores._PS.AfterSignIn.SetActive(true);
            

        }).Catch(error => {

            Debug.Log("Error : " + error);

            UsernamePanel.SetActive(true);
            LoginScreenPanel.SetActive(false);
       
        });
    }

    public void CreatePlayer()
    {
        string pathOfUser = "/Users/WalletUsers/";
        PlayerScores.playerName = usernameInput.text;
        PlayerScores.localId = playerWalletAddress;
        wuser.userName = usernameInput.text;
        wuser.userWalletAddress = playerWalletAddress;
        

        RestClient.Put(databaseURL + pathOfUser + playerWalletAddress +"/.json",wuser).Then(response => {


        PlayerScores._PS.menuUsernameTxt.text = PlayerScores.playerName;

            MainScreenPanel.SetActive(false);
            PlayerScores._PS.AfterSignIn.SetActive(true);
            PlayerScores._PS.Bike.SetActive(true);
            UsernamePanel.SetActive(false);
            LoginScreenPanel.SetActive(true);
            isWalletUser = true;
            PlayerScores._PS.InsideGame();


        });
    }

    public void SaveData()
    {
        if (isWalletUser)
        {
            string pathOfUser = "/Users/WalletUsers/";

            RestClient.Put(databaseURL + pathOfUser + playerWalletAddress + "/.json", wuser);
        }
    }
    
    public void ChangeScene_Debug()
    {
        SceneManager.LoadScene(2);
    }
}
