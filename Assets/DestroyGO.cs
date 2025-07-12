using UnityEngine;
using Photon.Pun;
using System.Collections;

public class DestroyGO : MonoBehaviour
{
    
    void Start()
    {
        StartCoroutine(DestroyDelay());
    }

    
    IEnumerator DestroyDelay()
    {
        yield return new WaitForSecondsRealtime(2);
        PhotonNetwork.Destroy(gameObject);

    }

}
