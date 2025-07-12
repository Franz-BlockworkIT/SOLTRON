using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LightWallsDestroy : MonoBehaviourPun
{
    


    private void Start()
    {
        PhotonNetwork.AllocateViewID(photonView);
        
    }
    private void Update()
    {

        

        if (RoundSystem.RS.RoundStarted)
        {

            PhotonNetwork.Destroy(gameObject);


        }
    }

    

}
