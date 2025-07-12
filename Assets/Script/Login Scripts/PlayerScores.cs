using System.Collections;
using System.Collections.Generic;
using FullSerializer;
using Proyecto26;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;
using Photon.Pun;



public class PlayerScores : MonoBehaviour
{


    //public TMP_InputField getScoreText;


    public static PlayerScores _PS;
    public Text BattleWon;

    

    [Header("UI")]
    public Text menuUsernameTxt;

    public Text scoreText;
    public Text WarningText;
    public GameObject WarningPanel;
    public GameObject LoginPanel;
    public GameObject LoginScreen;
    public GameObject RegisterPanel;
    public GameObject AfterSignIn;

    public GameObject content;
    public GameObject prefab;


    public InputField getScoreText;

    public TMP_InputField emailText;
    public TMP_InputField usernameText;
    public TMP_InputField passwordText;
    public TMP_InputField VerifypasswordText;

    public TMP_InputField emailTextSignIn;
    public TMP_InputField passwordTextSignIn;

    [Header("Others")]

    [SerializeField] public GameObject Bike;

    private System.Random random = new System.Random();

    User user = new User();

    private string databaseURL = "https://soltron-c922b-default-rtdb.asia-southeast1.firebasedatabase.app";
    private string AuthKey = "AIzaSyAtPEuMzonQDCWOUMq2wGFsGDQWFG1py9M";

    public static fsSerializer serializer = new fsSerializer();


    public static int playerScore;
    public static string playerName;
    
    public static int playerLevel = 1;
    public static int playerXp = 1;
    public static int playerSolBal;
    public static int playerTronBal;
    public static int playerBikeColor;
    public static int playerBattleWon;
    public static int playerBikeLevel;


    private string idToken;

    public static string localId;

    private string getLocalId;

    public static bool notLoggedIn = true;


    public bool GuestUser, WalletUser;

    private void Awake()
    {
        _PS = this;
    }
    private void Start()
    {
        

        LoginPanel.SetActive(notLoggedIn);
        Bike.SetActive(!notLoggedIn);
        AfterSignIn.SetActive(!notLoggedIn);

    }
    private void Update()
    {
        Debug.Log("Player NAme : " + playerName);
        Debug.Log("Player Level : " + playerLevel);

        if(CryptoReceiver.CR.walletAddress == "")
        {
            ConnectToWallet.isWalletUser = false;
        }
        

        menuUsernameTxt.text = playerName;


    }
    public void OnSubmit()
    {
        PostToDatabase();
    }

    public void OnGetScore()
    {

        for (int child = 0; child <= content.transform.childCount - 1; child++)
        {
            if (content.transform.childCount != 0)
                Destroy(content.transform.GetChild(child).gameObject);
        }

        GetLocalId();
    }

    private void UpdateScore()
    {
        scoreText.text = PlayerPrefs.GetInt(playerName + "BattleWon", 0).ToString();

    }

    private void PostToDatabase(bool emptyScore = false, string idTokenTemp = "")
    {

        if (idTokenTemp == "")
        {
            idTokenTemp = idToken;
        }
        User user = new User();

        if (emptyScore)
        {
            user.userScore = 0;
            user.Level = 1;
            user.xp = 0;
            user.userSolBal = 0;
            user.userTronBal = 0;
        }

        RestClient.Put(databaseURL + "/" + localId + ".json?auth=" + idTokenTemp, user);
    }

    private void RetrieveFromDatabase()
    {

        RestClient.Get<User>(databaseURL + "/" + getLocalId + ".json?auth=" + idToken).Then(response =>
        {
            user = response;

        });
    }

    public void SignUpUserButton()
    {
        if (usernameText.text == "")
        {
            WarningText.text = "Please Enter Username";
            WarningPanel.SetActive(true);
            return;

        }
        if (VerifypasswordText.text != passwordText.text)
        {
            WarningText.text = "Password Does Not Match !";
            WarningPanel.SetActive(true);
            return;
        }

        SignUpUser(emailText.text, usernameText.text, passwordText.text);

    }

    public void SignInUserButton()
    {


        SignInUser(emailTextSignIn.text, passwordTextSignIn.text);


    }

    private void SignUpUser(string email, string username, string password)
    {
        string userData = "{\"email\":\"" + email + "\",\"password\":\"" + password + "\",\"returnSecureToken\":true}";
        RestClient.Post<SignResponse>("https://www.googleapis.com/identitytoolkit/v3/relyingparty/signupNewUser?key=" + AuthKey, userData).Then(
            response =>
            {
                string emailVerification = "{\"requestType\":\"VERIFY_EMAIL\",\"idToken\":\"" + response.idToken + "\"}";
                RestClient.Post("https://identitytoolkit.googleapis.com/v1/accounts:sendOobCode?key=" + AuthKey, emailVerification);
                localId = response.localId;
                playerName = username;
                PostToDatabase(true, response.idToken);
                RegisterPanel.SetActive(false);
                LoginScreen.SetActive(true);
                WarningText.text = "Verification Email sent to your Email AdDress";
                WarningPanel.SetActive(true);

            }).Catch(error =>
            {
                WarningText.text = "Oh no! Something went Wrong...";
                WarningPanel.SetActive(true);
                Debug.Log("Something went Wrong - " + error);
            });
    }

    private void SignInUser(string email, string password)
    {
        string userData = "{\"email\":\"" + email + "\",\"password\":\"" + password + "\",\"returnSecureToken\":true}";
        RestClient.Post<SignResponse>("https://www.googleapis.com/identitytoolkit/v3/relyingparty/verifyPassword?key=" + AuthKey, userData).Then(
            response =>
            {
                string emailVerification = "{\"idToken\":\"" + response.idToken + "\"}";
                RestClient.Post("https://identitytoolkit.googleapis.com/v1/accounts:lookup?key=" + AuthKey, emailVerification).Then(emailResponse =>
                {

                    fsData emailVerificationData = fsJsonParser.Parse(emailResponse.Text);
                    EmailConfirmationInfo emailConfirmationInfo = new EmailConfirmationInfo();
                    serializer.TryDeserialize(emailVerificationData, ref emailConfirmationInfo).AssertSuccessWithoutWarnings();


                    if (emailConfirmationInfo.users[0].emailVerified)
                    {
                        idToken = response.idToken;
                        localId = response.localId;
                        GetUsername();



                        //emailTextSignIn.text = "";
                        //passwordTextSignIn.text = "";

                    }
                    else
                    {
                        WarningText.text = "Please Verify your Email First";
                        WarningPanel.SetActive(true);
                    }

                });

            }).Catch(error =>
            {
                WarningText.text = "Make sure your Email and Password are correct.";
                WarningPanel.SetActive(true);
            });
       
    }

    private void GetUsername()
    {
        RestClient.Get<User>(databaseURL + "/" + localId + ".json?auth=" + idToken).Then(response =>
        {
            playerName = response.userName;
            playerScore = response.userScore;
            playerLevel = response.Level;
            playerSolBal = response.userSolBal;
            playerTronBal = response.userTronBal;
            playerBikeColor = response.userBikeColor;
            menuUsernameTxt.text = playerName;
            

            AfterSignIn.SetActive(true);
            LoginPanel.SetActive(false);

        });
    }

    private void GetLocalId()
    {
        RestClient.Get(databaseURL + "/" + ".json?auth=" + idToken).Then(response =>
        {
            //var username = getScoreText.text;

            fsData userData = fsJsonParser.Parse(response.Text);
            Dictionary<string, User> users = null;
            serializer.TryDeserialize(userData, ref users);

            foreach (var user in users.Values)
            {
                GameObject clone = Instantiate(prefab, transform.position, transform.rotation);
                clone.transform.parent = content.transform;
                clone.transform.localScale = new Vector3(1, 1, 1);
                clone.transform.GetChild(0).GetComponent<Text>().text = user.userName;
                clone.transform.GetChild(1).GetComponent<Text>().text = user.userScore.ToString();


            }
        }).Catch(error =>
        {
            Debug.Log(error);
        });
    }



    public void PlayAsGuest()
    {
        Debug.Log(System.DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
        localId = System.DateTime.UtcNow.ToString("yyyyMMddHHmmss");

        
        AuthenticationValues authValue = new AuthenticationValues(localId);
        PhotonNetwork.AuthValues = authValue;
        playerName = "Guest";
        PhotonNetwork.NickName = playerName;
        AfterSignIn.SetActive(true);
        Bike.SetActive(true);

    }



    public void InsideGame()
    {

        notLoggedIn = false;
        Bike.SetActive(true);
    }
}

