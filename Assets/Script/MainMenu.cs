using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    [Header("UI")]
    public Text battleWonText;
    public Text currentRankText;
    public GameObject LoginPanel;
    public GameObject afterSignInPanel;

    /// Game modes ///

    public static bool freeToPlay;
    public static bool playToEarn;

    // Freeplay
    public static bool quickMatch, teamDeathmatch, training;
    // Play to earn
    public static bool tournament;



    private void Start()
    {
        turnOff();

        Player.death = 0;

        LevelWindow.team1WonTour = 0;
        LevelWindow.team2WonTour = 0;

        LoadValues();
        PhotonPlayer.TeamChange = false;

    }

    private void Update()
    {


        battleWonText.text = "Battle Won : " + PlayerPrefs.GetInt(PlayerScores.playerName + "BattleWon", PlayerScores.playerBattleWon);



        currentRankText.text = "Current Rank : 0";

    }
    public void OneVsOne()
    {
        quickMatch = true;
    }

    public void FourVsFour()
    {
        teamDeathmatch = true;
    }

    public void Training()
    {
        training = true;
    }

    public void Tournament()
    {
        tournament = true;
    }

    public void turnOff()
    {
        quickMatch = false;
        teamDeathmatch = false;
        tournament = false;
        training = false;
        freeToPlay = false;
        playToEarn = false;

        

    }

    public void Logout()
    {


        LoginPanel.SetActive(true);

        if (ConnectToWallet.isWalletUser)
        {
            Debug.Log("LOG OUT ");
            CryptoReceiver.CR.ClearOnLogout();
            CryptoReceiver.CR.OnConnect(CryptoReceiver.CR.serviceName);

        }
        PlayerScores._PS.Bike.SetActive(false);
        ConnectToWallet.isWalletUser = false;
       

    }

    public void GameModes(int mode)
    {

        if (mode == 1)
        {
            freeToPlay = true;
        }
        else
        {
            playToEarn = true;
        }
    }

    [SerializeField] private Slider volumeSlider = null;
    
    public void SaveVolumeButton()
    {
        float volumeValue = volumeSlider.value;
        PlayerPrefs.SetFloat("VolumeValue", volumeValue);
    }

    void LoadValues()
    {
        float volumeValue = PlayerPrefs.GetFloat("VolumeValue",1);
        volumeSlider.value = volumeValue;
        AudioListener.volume = volumeValue;
    }

    

}
