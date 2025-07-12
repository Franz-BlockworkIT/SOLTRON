using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Rocket : MonoBehaviourPunCallbacks
{
    [HideInInspector]public Transform _target = null;

    //public AudioSource audioSource;


    private void Awake()
    {
        //audioSource = GetComponent<AudioSource>();
    }
    private void Start()
    {

        
        //audioSource.Play();
    }
    private void Update()
    {

        if (_target == null)
            transform.position += transform.forward * rocketSpeed * Time.deltaTime;
        else
        {


            
                transform.LookAt(_target.transform);
            

                try
                {
                    StartCoroutine(SendRocket(gameObject));
                }
                catch (UnassignedReferenceException error)
                {
                    print($"Ignore this Error {error} ");
                }
            
        }

        if(gameObject.activeSelf)
            StartCoroutine(DestroyAfterSec(6));

        return;
    }
    

    public float rocketSpeed = 1f;

    public IEnumerator SendRocket(GameObject rocket)
    {


        if (rocket != null && _target != null)
        {
            while (Vector3.Distance(_target.transform.position, rocket.transform.position) > 0.3f)
            {
                if (rocket != null && _target != null)
                {
                    rocket.transform.position += (_target.transform.position - rocket.transform.position).normalized * rocketSpeed * Time.deltaTime;
                    rocket.transform.LookAt(_target.transform);
                }
                yield return null;
            }
        }



        //photonView.RPC("RPC_Destroy", RpcTarget.AllBuffered);




    }


    IEnumerator DestroyAfterSec(int time)
    {

        yield return new WaitForSeconds(time);

        photonView.RPC("RPC_Destroy",RpcTarget.AllBuffered);
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if(gameObject.tag == "Team1")
        {
            if(other.gameObject.tag == "Team2")
            {
                if(other != null)
                    _target = other.transform;
            }
        }
        
        if(gameObject.tag == "Team2")
        {
            if(other.gameObject.tag == "Team1")
            {
                if (other != null)
                    _target = other.transform;
            }
        }
    }


    [PunRPC]
    void RPC_Destroy()
    {
        
        Destroy(gameObject);
    }
    public override void OnDisconnected(Photon.Realtime.DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        PhotonNetwork.Destroy(gameObject);
    }
}



