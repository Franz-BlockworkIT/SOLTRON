using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSdisplay : MonoBehaviour
{
    public float timer, refresh, avgFramerate;
    public string display = "Fps : {0}";
    public Text m_Text;

    private void Start()
    {
        
    }

    private void Update()
    {
        float timeLapse = Time.smoothDeltaTime;
        timer = timer <= 0 ? refresh : timer -= timeLapse;

        if (timer <= 0) avgFramerate = (int)(1f / timeLapse);
        m_Text.text = string.Format(display, avgFramerate.ToString());

    }
}
