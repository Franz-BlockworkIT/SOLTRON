using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Testing : MonoBehaviourPunCallbacks
{
    [SerializeField] private LevelWindow levelWindow;
    [SerializeField] private PhotonPlayer player;

    private void Awake()
    {
        if (!photonView.IsMine)
            return;


        foreach (PhotonPlayer obj in FindObjectsOfType<PhotonPlayer>())
        {
            if (obj.PV.IsMine)
            {
                player = obj;
            }
        }


        if (player != null)
        {

            LevelSystem levelSystem = new LevelSystem();

            levelWindow.SetLevelSystem(levelSystem);
            player.SetLevelSystem(levelSystem);
            
        }
        
        
        

    }
  
}
