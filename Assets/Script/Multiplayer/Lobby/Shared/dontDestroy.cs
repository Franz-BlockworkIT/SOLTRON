using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dontDestroy : MonoBehaviour
{
    public bool dontDestroyOnLeave = true;

    public static dontDestroy _instance;

    private void Awake()
    {
        if (dontDestroy._instance == null)
        {
            dontDestroy._instance = this;
        }
        else
        {
            if (dontDestroy._instance != this)
            {
                Destroy(this.gameObject);
            }

        }
        DontDestroyOnLoad(this.gameObject);
    }
}
