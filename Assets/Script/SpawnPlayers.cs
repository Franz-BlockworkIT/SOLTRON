using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;

public class SpawnPlayers : MonoBehaviour
{


    public int spawnGroup;

    public GameObject networkObject;

    void Update()
    {

        
        
            if (networkObject == null)
            {
                StartCoroutine(SpawnPlayer());
            }
        


            
       


    }

    IEnumerator SpawnPlayer()
    {
        while(PhotonNetwork.NetworkClientState != ClientState.Joined)
        {
            yield return null;
        }
        if (networkObject == null)
        {
            networkObject = PhotonNetwork.Instantiate(System.IO.Path.Combine("PhotonPrefabs", "PhotonNetworkPlayer"), transform.position, Quaternion.identity, 0);
        }
    }
}
