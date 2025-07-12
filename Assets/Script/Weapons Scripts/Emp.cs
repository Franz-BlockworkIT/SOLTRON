using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ShopSystem;

public class Emp : MonoBehaviourPunCallbacks
{
    public float moveSpeed;
    public float explosionSpeed;
    public float radius;
    public float power;

    [SerializeField] GameObject explosionVFX;

    public Vector3 targetPos;

    Player playerHealth;
    AiHealth enemyHealth;

    PhotonView PV;

    public AudioSource audioSource;

    void Start()
    {
        PV = GetComponent<PhotonView>();
        // Travel
        this.GetComponent<Rigidbody>().AddForce(-transform.forward * moveSpeed);

        audioSource = GetComponent<AudioSource>();

        


        StartCoroutine("BombTimer");
        

    }
    IEnumerator BombTimer()
    {
        yield return new WaitForSeconds(3f);

        //Explode();

        PV.RPC("Explode", RpcTarget.AllBuffered);

        
            PV.RPC("RPC_Destroy", RpcTarget.AllBuffered);
            
        

        
    }

    
    [PunRPC]
    public void Explode()
    {
        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);

        GameObject explosion = Instantiate(explosionVFX, transform.position, Quaternion.identity);
        //GameObject explosion = PhotonNetwork.Instantiate("Weapons/EmpExplode", transform.position, Quaternion.identity);
        //explosion.SetActive(true);

        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null)
                rb.AddExplosionForce(power, explosionPos, radius, 3.0f);

            if (hit.tag == "Team1" && gameObject.layer == LayerMask.NameToLayer("Team2"))
            {

                if (hit.GetComponent<AiHealth>() != null)
                {
                    enemyHealth = hit.GetComponent<AiHealth>();
                    //enemyHealth.TakeDamage(PlayerPrefs.GetInt(PlayerScores.playerName + "EMP"));
                    enemyHealth.PV.RPC("TakeDamageAi", RpcTarget.AllBuffered, PlayerPrefs.GetInt(PlayerScores.playerName + "EMP"));

                }

                if (hit.GetComponent<Player>()!= null)
                {
                playerHealth = hit.GetComponent<Player>();

                    playerHealth.PV.RPC("TakeDamage", RpcTarget.AllBuffered, PlayerPrefs.GetInt(PlayerScores.playerName + "EMP"));

                    hit.GetComponent<Player>().explode = true;
                    PV.RPC("RPC_Destroy", RpcTarget.AllBuffered);

                }


            }
            if (hit.tag == "Team2" && gameObject.layer == LayerMask.NameToLayer("Team1"))
            {

                if (hit.GetComponent<AiHealth>() != null)
                {
                    enemyHealth = hit.GetComponent<AiHealth>();
                    //enemyHealth.TakeDamage(PlayerPrefs.GetInt(PlayerScores.playerName + "EMP"));
                    enemyHealth.PV.RPC("TakeDamageAi", RpcTarget.AllBuffered, PlayerPrefs.GetInt(PlayerScores.playerName + "EMP"));


                }


                if (hit.GetComponent<Player>() != null)
                    {
                    playerHealth = hit.GetComponent<Player>();

                        
                        playerHealth.PV.RPC("TakeDamage", RpcTarget.AllBuffered, PlayerPrefs.GetInt(PlayerScores.playerName + "EMP"));


                        hit.GetComponent<Player>().explode = true;
                        PV.RPC("RPC_Destroy", RpcTarget.AllBuffered);

                    }


                

            }
        }


        if (PV != null)
        {
            PV.RPC("RPC_Destroy", RpcTarget.AllBuffered);
        }
        

    }

    public override void OnDisconnected(Photon.Realtime.DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        PhotonNetwork.Destroy(gameObject);
    }

    [PunRPC]
    void RPC_Destroy()
    {
        audioSource.Play();
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
        gameObject.transform.GetChild(1).gameObject.SetActive(false);
        Destroy(gameObject,3);
    }
}
