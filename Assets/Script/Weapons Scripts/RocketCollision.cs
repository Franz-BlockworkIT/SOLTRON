using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class RocketCollision : MonoBehaviour, IPunObservable
{

    public Rocket rocket;

    public float radius;
    public float power;

    [SerializeField] GameObject explosionVFX;

    Player playerHealth;
    AiHealth enemyHealth;

    PhotonView PV;
    private void Start()
    {
        rocket = GetComponentInParent<Rocket>();

        PV = GetComponent<PhotonView>();


       

        



    }
    private void OnTriggerEnter(Collider other)
    {


        if (gameObject.layer == LayerMask.NameToLayer("Team2") && other.tag == "Team1")
        {

            if (other.GetComponent<AiHealth>() != null)
            {
                enemyHealth = other.GetComponent<AiHealth>();

                //enemyHealth.TakeDamage(PlayerPrefs.GetInt(PlayerScores.playerName + "Rocket"));
                enemyHealth.PV.RPC("TakeDamageAi", RpcTarget.AllBuffered, PlayerPrefs.GetInt(PlayerScores.playerName + "Rocket"));


                Explode();


            }

            


            if (other.GetComponent<Player>() != null)
            {
                playerHealth = other.GetComponent<Player>();

                //PV.RPC("RPC_Damage", RpcTarget.All, playerHealth);
                //playerHealth.TakeDamage(40);
                playerHealth.PV.RPC("TakeDamage", RpcTarget.AllBuffered, PlayerPrefs.GetInt(PlayerScores.playerName + "Rocket"));

                playerHealth.explode = true;
                Explode();



            }



        }


        else if (other.tag == "Team2" && gameObject.layer == LayerMask.NameToLayer("Team1"))
        {
            if (other.GetComponent<AiHealth>() != null)
            {
                enemyHealth = other.GetComponent<AiHealth>();
                //PV.RPC("Ai_RPC_Damage", RpcTarget.All, enemyHealth);
                //enemyHealth.TakeDamage(PlayerPrefs.GetInt(PlayerScores.playerName + "Rocket"));
                enemyHealth.PV.RPC("TakeDamageAi", RpcTarget.AllBuffered, PlayerPrefs.GetInt(PlayerScores.playerName + "Rocket"));


                Explode();

            }




            if (other.GetComponent<Player>() != null)
            {
                playerHealth = other.GetComponent<Player>();

                
                playerHealth.PV.RPC("TakeDamage", RpcTarget.AllBuffered, PlayerPrefs.GetInt(PlayerScores.playerName + "Rocket"));

                playerHealth.explode = true;
                Explode();



            }

            //PV.RPC("RPC_Damage", RpcTarget.All, other);


        }
    }
        public void Explode()
    {

        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);

        //GameObject explosion = PhotonNetwork.Instantiate("Weapons/Rocket Explode", transform.position, Quaternion.identity);
        //explosion.SetActive(true);
        GameObject explosion = Instantiate(explosionVFX, transform.position, Quaternion.identity);

        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null)
                rb.AddExplosionForce(power, explosionPos, radius, 3.0f);
        }
        PV.RPC("RPC_Destroy", RpcTarget.AllBuffered);







    }

    [PunRPC]
    void RPC_Damage(Player other)
    {

        other.TakeDamage(PlayerPrefs.GetInt(PlayerScores.playerName + "Rocket"));
        other.explode = true;
        Explode();

    }
    
   
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
        }
        else
        {
            transform.position = (Vector3)stream.ReceiveNext();
        }
    }

    [PunRPC]
    void RPC_Destroy()
    {

        Destroy(gameObject);
    }

}
