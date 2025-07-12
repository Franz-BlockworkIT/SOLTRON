using UnityEngine;
using Cinemachine;
using System.Collections;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;

public class Player : MonoBehaviourPunCallbacks, IPunObservable
{

    public static Player _playerInstance;
    
    public int maxHealth = 100;
    public int currentHealth;

    

    public Health healthbar;
    TeamMember team;

    public GameObject LoseVfx;

    [SerializeField] CinemachineVirtualCamera loseCamera;
    [SerializeField] CinemachineVirtualCamera FPCamera;
    private CinemachineBasicMultiChannelPerlin virtualCameraNoise;


    public float ShakeDuration = 0.3f;                  // Time the Camera Shake effect will last
    public float ShakeAmplitude = 1.2f;
    public float ShakeFrequency = 2.0f;


    private float ShakeElapsedTime = 0f;

    public bool explode;



    BikeController playerBike;
    Animator anim;

    

    public Material trailsMat;

    [SerializeField] Camera mainCamera;

    public PhotonView PV;

    public bool defeated;

    float waitForAnimation = 3;

    Vector3 rayHit;

    public int force = 10;

    public bool Team1, Team2;

    public GameObject trails;


    public Slider lightWallBar;
    private int maxLightWallCharge = 100;
    public float currentLightWallCharge;

    public float lightwallsDecreaseRate = .5f;

    public float chargeTimer = 3f;

    public static int death;
    public bool dead;

    

    



    private void Awake()
    {
        _playerInstance = this;

        playerBike = GetComponent<BikeController>();
        anim = GetComponent<Animator>();

        PV = GetComponent<PhotonView>();

        if (PV.IsMine)
        {
            
            healthbar = GameObject.Find("Canvas").transform.Find("Player Info").transform.Find("Health").GetComponent<Health>();
            
        }

        team = GetComponent<TeamMember>();



    }
    private void Start()
    {
        


        if (PV.IsMine)
        {
            Tournament.team = team._teamID;

            lightWallBar = GameObject.Find("Canvas").transform.Find("Player Info").transform.Find("LightWalls").GetComponent<Slider>();


            maxHealth = PlayerPrefs.GetInt(PlayerScores.playerName + "Health",100);

            currentHealth = maxHealth;
            healthbar.SetMaxHealth(maxHealth);
            
            currentLightWallCharge = maxLightWallCharge;
            lightWallBar.maxValue = maxLightWallCharge;
            lightWallBar.value = maxLightWallCharge;


        }

        if (team._teamID == 1)  
        {
            Team1 = true;


        }
        else
        {
            Team2 = true;

        }
        

        if (loseCamera != null)
        {
            virtualCameraNoise = loseCamera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
        }



       
    }

    private void Update()
    {

        PV.RPC("RPC_AnimateEnable", RpcTarget.AllBuffered,anim.enabled);


        if (RoundSystem.RS.RoundStarted)
        {        
            defeated = true;

            PhotonNetwork.Destroy(gameObject);


        }

        //if (RoundSystem.RS.enemiesLength == 0 || RoundSystem.RS.teamPlayersAlive == 0)
        //{
        //    defeated = true;

        //    PhotonNetwork.Destroy(gameObject);
        //}
        if (!PV.IsMine)
        {
            mainCamera.gameObject.SetActive(false);
            loseCamera.gameObject.SetActive(false);

            
            return;
        }

        
        if (currentHealth <= 0)
        {

            defeated = true;
            anim.enabled = true;
            currentHealth = 0;

            
            if (healthbar != null)
            {
                healthbar.SetHealth(currentHealth);
            }
            playerBike.canMove = false;
            loseCamera.m_Lens.FieldOfView = Mathf.Lerp(loseCamera.m_Lens.FieldOfView, 70, 20 * Time.deltaTime);

            if (!dead)
            {
                death++;
                dead = true;
            }

            //PhotonNetwork.Destroy(gameObject, 3);
            waitForAnimation -= Time.deltaTime;

            if (waitForAnimation <= 0)
            {
                PhotonNetwork.Destroy(gameObject);
                waitForAnimation = 3;
            }



            

            
        }
        else
        {

            if (healthbar != null)
            {
                healthbar.SetHealth(currentHealth);
            }
                
            loseCamera.m_Lens.FieldOfView = 40;


            if (Input.GetKeyDown(KeyCode.Space))
            {


                PV.RPC("SpawningTrailRend", RpcTarget.AllBuffered);


            }

            if (Input.GetKey(KeyCode.Space))
            {
                PV.RPC("UseLightwall", RpcTarget.AllBuffered, lightwallsDecreaseRate);
                //UseLightwall(lightwallsDecreaseRate);
            }
            else
            {
                //StartCoroutine(RegenStamina());
               
                PV.RPC("ChangeTrailParent", RpcTarget.AllBuffered);
            }
            
        }

        // TODO: Replace with your trigger
        if (explode)
        {
            explode = false;
            ShakeElapsedTime = ShakeDuration;
        }
        // If the Cinemachine componet is not set, avoid update
        if (loseCamera != null || virtualCameraNoise != null)
        {
            // If Camera Shake effect is still playing
            if (ShakeElapsedTime > 0)
            {
                // Set Cinemachine Camera Noise parameters
                virtualCameraNoise.m_AmplitudeGain = ShakeAmplitude;
                virtualCameraNoise.m_FrequencyGain = ShakeFrequency;

                // Update Shake Timer
                ShakeElapsedTime -= Time.deltaTime;
            }
            else
            {
                // If Camera Shake effect is over, reset variables
                virtualCameraNoise.m_AmplitudeGain = 0f;
                ShakeElapsedTime = 0f;

            }


        }


        SwitchCamera();
    }



    [PunRPC]
    void RPC_AnimateEnable(bool animate)
    {
        anim.enabled = animate;
    }

    public float rayCastRange = 8;

    
    [PunRPC]
    public void TakeDamage(int damage)
    {
            
        currentHealth -= damage;

    }


    IEnumerator WaitForSpeed()
    {
        yield return new WaitForSeconds(1);
        playerBike.speed = playerBike.maxSpeed;

    }

    public int count;
    // Start is called before the first frame update
    

    





    private void OnCollisionEnter(Collision collision)
    {


        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Enemy Trail") || collision.collider.gameObject.layer == LayerMask.NameToLayer("Player Trail"))
        {
            StartCoroutine("WaitForSpeed");

            PV.RPC("TakeDamage", RpcTarget.AllBuffered, UnityEngine.Random.Range(80, 120));
        }

        if (collision.gameObject.GetComponent<Player>() != null)
        {
            if (collision.gameObject.GetComponent<Player>().Team2 && Team1)
            {
                PV.RPC("TakeDamage", RpcTarget.AllBuffered, UnityEngine.Random.Range(10, 20));
            }


            if (collision.gameObject.GetComponent<Player>().Team1 && Team2)
            {

                PV.RPC("TakeDamage", RpcTarget.AllBuffered, UnityEngine.Random.Range(10, 20));

            }
        }
    }
    
    

    private void OnCollisionExit(Collision collision)
    {
            playerBike.speed = playerBike.maxSpeed;
    }

    



    

    

    void SwitchCamera()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (loseCamera.gameObject.activeSelf)
            {
                loseCamera.gameObject.SetActive(false);
                FPCamera.gameObject.SetActive(true);
            }
            else
            {
                FPCamera.gameObject.SetActive(false);
                loseCamera.gameObject.SetActive(true);
            }
        }
    }


    


    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(rayHit,10f);
        

    }


    
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(currentHealth);
        }
        else
        {
            currentHealth = (int)stream.ReceiveNext();
        }
    }


    GameObject lightwalls;

    GameObject trail1;
    GameObject trail2;
    GameObject trail3;
    GameObject trail4;

    [SerializeField] Transform[] trailsRendPos;

    

    [PunRPC]
    void ChangeTrailParent()
    {
        chargeTimer -= Time.deltaTime;
        if (chargeTimer <= 0)
        {
            if (currentLightWallCharge < maxLightWallCharge)
            {
                currentLightWallCharge += .1f;
                if(lightWallBar != null)
                    lightWallBar.value = currentLightWallCharge;
            }
        }
        if (trail1 != null)
        {
            trail1.transform.parent = null;
            trail1 = null;
        }
        if (trail2 != null)
        {
            trail2.transform.parent = null;
            trail2 = null;
        }
        if (trail3 != null)
        {
            trail3.transform.parent = null;
            trail3 = null;
        }
        if (trail4 != null)
        {
            trail4.transform.parent = null;
            trail4 = null;
        }

        if (lightwalls != null)
        {
            lightwalls.transform.parent = null;
            lightwalls = null;
        }
    }


    [PunRPC]
    void SpawningTrailRend()
    {
        if (currentLightWallCharge - lightwallsDecreaseRate >= 0)
        {
            if (gameObject.tag == "Team1")
            {
                trail1 = PhotonNetwork.Instantiate("PhotonPrefabs/LightWalls1", trailsRendPos[0].position, trailsRendPos[0].rotation);
                trail2 = PhotonNetwork.Instantiate("PhotonPrefabs/LightWalls2", trailsRendPos[1].position, trailsRendPos[1].rotation);
                trail3 = PhotonNetwork.Instantiate("PhotonPrefabs/LightWalls2", trailsRendPos[2].position, trailsRendPos[2].rotation);
                trail4 = PhotonNetwork.Instantiate("PhotonPrefabs/LightWalls1", trailsRendPos[3].position, trailsRendPos[3].rotation);




            }
            else
            {

                trail1 = PhotonNetwork.Instantiate("PhotonPrefabs/LightWalls1_1", trailsRendPos[0].position, trailsRendPos[0].rotation);
                trail2 = PhotonNetwork.Instantiate("PhotonPrefabs/LightWalls2_1", trailsRendPos[1].position, trailsRendPos[1].rotation);
                trail3 = PhotonNetwork.Instantiate("PhotonPrefabs/LightWalls2_1", trailsRendPos[2].position, trailsRendPos[2].rotation);
                trail4 = PhotonNetwork.Instantiate("PhotonPrefabs/LightWalls1_1", trailsRendPos[3].position, trailsRendPos[3].rotation);


            }

           
            trail1.transform.parent = trailsRendPos[0];
            trail2.transform.parent = trailsRendPos[1];
            trail3.transform.parent = trailsRendPos[2];
            trail4.transform.parent = trailsRendPos[3];


            //PV.RPC("LightWallsTrails", RpcTarget.AllBuffered);
            LightWallsTrails();

        }
    }

    [PunRPC]
    void LightWallsTrails()
    {

            if(lightWallBar != null)
                lightWallBar.value = currentLightWallCharge;
        lightwalls = PhotonNetwork.Instantiate("PhotonPrefabs/LightWalls", trail1.transform.position, trail1.transform.rotation);

        //if (gameObject.tag == "Team1")
        //    lightwalls.tag = "Player Trail";

        //if (gameObject.tag == "Team2")
        //    lightwalls.tag = "Enemy Trail";
       
        

        if (lightwalls.GetComponent<LightWalls>() != null)
        {
            if (lightwalls.GetComponent<LightWalls>().playerCont == null)
                lightwalls.GetComponent<LightWalls>().playerCont = GetComponent<BikeController>();
            if (lightwalls.GetComponent<LightWalls>().playerChck == null)
                lightwalls.GetComponent<LightWalls>().playerChck = GetComponent<Player>();
            if (lightwalls.GetComponent<LightWalls>().followPos == null)
                lightwalls.GetComponent<LightWalls>().followPos = trail1.transform;

            if (lightwalls.GetComponent<LightWalls>().trailRend == null)
                lightwalls.GetComponent<LightWalls>().trailRend = trail1.GetComponent<TrailRenderer>();

            if (gameObject.tag == "Team1")
            {
                lightwalls.GetComponent<LightWalls>().team1 = true;
            }
            else
            {
                lightwalls.GetComponent<LightWalls>().team2 = true;
            }
        }

        lightwalls.transform.parent = trail1.transform;
        
       
    }


    [PunRPC]
    void UseLightwall(float amount)
    {
        if(currentLightWallCharge - amount >= 0)
        {
            currentLightWallCharge -= amount;

            if(lightWallBar != null)
                lightWallBar.value = currentLightWallCharge;
        }
        else
        {
            Debug.Log("Not Enough Charge !!!");

            if (trail1 != null)
            {
                trail1.transform.parent = null;
                trail1 = null;
            }
            if (trail2 != null)
            {
                trail2.transform.parent = null;
                trail2 = null;
            }
            if (trail3 != null)
            {
                trail3.transform.parent = null;
                trail3 = null;
            }
            if (trail4 != null)
            {
                trail4.transform.parent = null;
                trail4 = null;
            }

            if (lightwalls != null)
            {
                lightwalls.transform.parent = null;
                lightwalls = null;
            }
        }

        chargeTimer = 3;

    }


    [PunRPC]
    void ResetPlayer()
    {
        PhotonNetwork.Destroy(gameObject);
    }


    public void PlayDestroySound()
    {
        PhotonNetwork.Instantiate("AfterDestroySound", transform.position, transform.rotation);
    }
    
}
