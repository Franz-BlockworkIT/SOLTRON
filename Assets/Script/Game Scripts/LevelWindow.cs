using System;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Collections;

public class LevelWindow : MonoBehaviour
{

    [SerializeField]private Text levelTxt;
    [SerializeField]private Text expTxt;
    [SerializeField]private Text wonTxt;
    [SerializeField]private Text loseTxt;
    [SerializeField]private Text deathTxt;
    [SerializeField] private Image expBarImage;


    public static int team1WonTour;
    public static int team2WonTour;

    public float normalizedValue;
    public float speed = 1;
    private LevelSystem levelSystem;

    private PhotonPlayer player;

    

    private void Awake()
    {

        
       
        levelTxt = transform.Find("Experience").transform.Find("levelText").GetComponent<Text>();
        expTxt = transform.Find("Experience").transform.Find("NextLevel").GetComponent<Text>();
        expBarImage = transform.Find("Experience").transform.Find("experienceBar").Find("bar").GetComponent<Image>();
        
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
        if (player.team == 1)
        {
            if (!MainMenu.tournament)
            {
                wonTxt.text = "Won   : " + RoundSystem.RS.team1WinScore;
                loseTxt.text = "Lose  : " + RoundSystem.RS.team2WinScore;
            }
            else
            {
                team1WonTour += RoundSystem.RS.team1WinScore;
                team2WonTour += RoundSystem.RS.team2WinScore;

                wonTxt.text = "Won   : " + team1WonTour;
                loseTxt.text = "Lose  : " + team2WonTour;
            }
            deathTxt.text = "Death : " + Player.death;

        }
        else
        {
            if (!MainMenu.tournament)
            {
                wonTxt.text = "Won   : " + RoundSystem.RS.team2WinScore;
                loseTxt.text = "Lose  : " + RoundSystem.RS.team1WinScore;
            }
            else
            {
                team1WonTour += RoundSystem.RS.team1WinScore;
                team2WonTour += RoundSystem.RS.team2WinScore;

                wonTxt.text = "Won   : " + team2WonTour;
                loseTxt.text = "Lose  : " + team1WonTour;
            }
            deathTxt.text = "Death : " + Player.death;
        }

        if (player.PV.IsMine)
        {
            float expBarFill = PlayerPrefs.GetFloat(PlayerScores.playerName + "ExpBarFill", 0);
            expBarImage.fillAmount = expBarFill;
        }

        GameManager.clrChanged = false;
    }

    private void Update()
    {
       

        if (!player.PV.IsMine)
            return;

        StartCoroutine(DelayExp());
        

        if (levelSystem.GetExpNumber() < 0)
        {
            PlayerPrefs.SetInt(PlayerScores.playerName + "Xp", 0);
            
        }

        

    }

    IEnumerator DelayExp()
    {
        SetLevelNumber(levelSystem.GetLevelNumber(), levelSystem.GetExpNumber(), levelSystem.GetNextLevelNumber());

        yield return new WaitForSecondsRealtime(5);
        SetExpBarSize(levelSystem.GetExpNumber(), levelSystem.GetNextLevelNumber());

    }
    private void SetExpBarSize(int expNumber, int nextLevelNumber)
    {
        float expBarFill = PlayerPrefs.GetFloat(PlayerScores.playerName + "ExpBarFill", 0);
        expBarImage.fillAmount = Mathf.Lerp(expBarFill, (float)expNumber/nextLevelNumber, speed * Time.deltaTime);
        PlayerPrefs.SetFloat(PlayerScores.playerName + "ExpBarFill", expBarImage.fillAmount);
    }

    private void SetLevelNumber(int levelNumber,int expNumber,int nextLevelNumber)
    {
        levelTxt.text = "Level : " + levelNumber;
        expTxt.text = expNumber + "/" + nextLevelNumber;
    }

    public void SetLevelSystem(LevelSystem levelSystem)
    {
        this.levelSystem = levelSystem;

        normalizedValue = levelSystem.GetExpNormalized();

        levelSystem.onExpChanged += LevelSystem_OnExperienceChanged;
        levelSystem.onLevelChanged += LevelSystem_OnLevelChanged;
    }

    private void LevelSystem_OnLevelChanged(object sender, EventArgs e)
    {
        // Level Changed, Update Text
        SetLevelNumber(levelSystem.GetLevelNumber(), levelSystem.GetExpNumber(), levelSystem.GetNextLevelNumber());

    }

    private void LevelSystem_OnExperienceChanged(object sender, EventArgs e)
    {
        // Experience Changed, update bar Size
        SetExpBarSize(levelSystem.GetExpNumber(), levelSystem.GetNextLevelNumber());
    }

   
}
