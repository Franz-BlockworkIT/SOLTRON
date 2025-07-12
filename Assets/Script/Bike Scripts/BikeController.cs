   using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BikeController : MonoBehaviourPunCallbacks
{

    GameManager stop;
    [SerializeField] private Transform bikeBody;
    
    

    public bool canMove = true;

    public float speedInput, turnInput;
   

    private float currentSpeed;    
    public float minSpeed,speed, maxSpeed = 20f;
    public float rotationSpeed = 50;
    public float wheelRotationSpeed = 10;

    [SerializeField] GameObject[] wheelsToRotate;

    public bool grounded;

    public LayerMask whatIsGround;

    public float groundRayLength = .5f;
    public Transform groundRayPoint;

    public float gravityForce = 10f;

    Rigidbody rb;

    public Material bikeMat;
    public SkinnedMeshRenderer[] Character;

    [SerializeField]
    Vector3 movement;

    PhotonView PV;

    public GameObject trails;

    public ShopSystem.ShopData shopData;

    private int currentIndex = 0;
    private int selectedIndex = 0;

    public float debugSpeed = 1;
    public float timeZeroToMax = 2.5f;
    float accelRatePerSec;
    float forwardVelocity;

    [SerializeField]
    private float smoothMovementTime = .2f;

    private Vector3 smoothInputVelocity;

    [Header("Audio")]

    AudioSource BikeAudio;

    

    public float minPitch;
    public float maxPitch;
    private float pitchFromBike;

    public GameObject rearView;

    
    

    private void Awake()
    {
        stop = GameObject.Find("Game Manager").GetComponent<GameManager>();
        bikeMat = GetComponent<MeshRenderer>().material;
        PV = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody>();
        Character = transform.Find("Body").transform.Find("Char").GetComponentsInChildren<SkinnedMeshRenderer>();
        BikeAudio = GetComponent<AudioSource>();

        
        
    }


    void Start()
    {
        BikeAudio.PlayOneShot((AudioClip)Resources.Load("/Sounds/WAV 1/bikeStarting"));

        selectedIndex = shopData.selectedIndex;
        currentIndex = selectedIndex;

        accelRatePerSec = speed / timeZeroToMax;
        forwardVelocity = 0f;

        //BikeAudio.pitch = Audiopitch;

        maxSpeed = PlayerPrefs.GetInt(PlayerScores.playerName + "Engine");
        rotationSpeed = PlayerPrefs.GetInt(PlayerScores.playerName + "Tyres");

        speed = maxSpeed;
        foreach (SkinnedMeshRenderer mat in Character)
        {
            mat.material.SetColor("_EmissionColor", bikeMat.GetColor("_EmissionColor"));
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (!PV.IsMine) return;



        EngineSound();
        if (stop.canMove)
        {
            
            // For Turning motion Of a Body
            Quaternion bodyRotation = transform.rotation * Quaternion.Euler(0f, 0f, -turnInput * 55 * Input.GetAxis("Vertical"));
            bikeBody.rotation = Quaternion.Lerp(bikeBody.rotation, bodyRotation, 0.1f);

            //-------------------------//

            WheelSpinning();
        }

        if (Input.GetKey(KeyCode.V))
        {
            rearView.SetActive(true);
        }
        else
        {
            rearView.SetActive(false);

        }



    }

    private void FixedUpdate()
    {
        if (!PV.IsMine) return;

        else
        {

            if (stop.canMove)
            {
                if (canMove)
                {
                    trails.SetActive(true);
                    grounded = false;
                    RaycastHit hit;

                    if (Physics.Raycast(groundRayPoint.position, -transform.up, out hit, groundRayLength, whatIsGround))
                    {
                        grounded = true;

                        transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
                    }


                    if (grounded)
                    {

                        Move();
                        rb.drag = 1;
                    }
                    else
                    {
                        rb.drag = 0.1f;
                        rb.AddForce(Vector3.up * -gravityForce * 100f);

                    }
                }
                
            }
            else
            {
                trails.SetActive(false);
                return;
            }

        }

    }

    
    public void Move()
    {
        turnInput = Input.GetAxis("Horizontal");
        speedInput = Input.GetAxis("Vertical");
        float curspeed = speed;

        forwardVelocity += accelRatePerSec * Time.deltaTime;
        forwardVelocity = Mathf.Min(forwardVelocity, speed);




        //movement = new Vector3(Input.GetAxis("Horizontal") * curspeed , 0, Input.GetAxis("Vertical") * curspeed );

        
        Movement(transform.forward * speedInput * curspeed);
        


        if (Input.GetAxis("Horizontal") != 0)
        {

            transform.Rotate(Vector3.up * turnInput * rotationSpeed * Time.deltaTime * speedInput);
        }


    }

    private void WheelSpinning()
    {

        float verticalAxis = Input.GetAxisRaw("Vertical");

        foreach (GameObject wheel in wheelsToRotate)
        {
            wheel.transform.Rotate(Time.deltaTime * speedInput * speed * wheelRotationSpeed, 0, 0, Space.Self);
        }
    }


    void Movement(Vector3 direction) 
    {
        rb.velocity = direction;

    }

    public void BikeExplosioSound()
    {
        
        BikeAudio.PlayOneShot((AudioClip)Resources.Load("Sounds/WAV 1/Explosion_Destroyed_Bike"));
    }
    
    void EngineSound()
    {

        currentSpeed = rb.velocity.magnitude;
        pitchFromBike = rb.velocity.magnitude / maxSpeed;

        if(currentSpeed < minSpeed)
        {
            BikeAudio.pitch = minPitch;
        }

        if(currentSpeed > minSpeed && currentSpeed < maxSpeed)
        {
            BikeAudio.pitch = minPitch + pitchFromBike;
        }
        if(currentSpeed > maxSpeed)
        {
            BikeAudio.pitch = maxPitch;
        }
    }
}
