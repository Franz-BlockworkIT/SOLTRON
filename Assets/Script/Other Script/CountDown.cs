using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class CountDown : MonoBehaviour
{
    public static CountDown waitForPlayers;

    public float  Countdown = 0;

    public Text countDownText;


    private void OnEnable()
    {
        Countdown = 0;

    }

    private void Start()
    {
        Countdown = 0;
        countDownText = GetComponent<Text>();
    }
    void Update()
    {
        Countdown += Time.deltaTime;


        countDownText.text = Countdown.ToString("0");
        if (Countdown <= 0)
        {
            Countdown = 0;
            
        }
    }
}
