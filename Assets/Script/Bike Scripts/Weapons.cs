using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Collections;

public class Weapons : MonoBehaviourPunCallbacks
{
    public float cooldownTime = 2;
    public float bombChargeTime = 3;

    private float nextFireTime;
    private float nextDropTime;

    BikeController bikeCont;
    GameManager GM;
    PhotonView PV;

    public Image Aim;

    AudioSource audioS;

    // Rockets //

    [SerializeField] GameObject rocketPrefab;
    [SerializeField] GameObject[] spawnPos;

    

    public Transform _target;
    

    public Player player;

    // ------------------------ //

    // Grenade //

    [SerializeField] GameObject grenade;
    [SerializeField] GameObject grenadeSpawnPos;

    // -------------------------//

    private void Awake()
    {


        bikeCont = GetComponent<BikeController>();
        PV = GetComponent<PhotonView>();
        player = GetComponent<Player>();
        GM = GameObject.Find("Game Manager").GetComponent<GameManager>();

        audioS = GetComponent<AudioSource>();

        if (!PV.IsMine)
        {
            Aim.transform.parent.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!PV.IsMine) return;

        bool pause = Input.GetKeyDown(KeyCode.Escape);

        if (pause)
        {
            GameObject.Find("Pause").GetComponent<Pause>().TogglePause();
        }


        if (GM.canMove)
        {
            if (bikeCont.canMove)
            {
                FireInput();
            }
        }
        else
        {
            return;
        }




        HomingMissile();
            
    }


    public void FireInput()
    {
        if (Time.time > nextFireTime)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                FireRockets();
                
            }
        }


        if (player.Team1)
        {
            if (Time.time > nextDropTime)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    PhotonNetwork.Instantiate("Weapons/Player Emp Gernade", grenadeSpawnPos.transform.position, transform.rotation);
                    nextDropTime = Time.time + bombChargeTime;

                }
            }
        }
        if (player.Team2)
        {
            if (Time.time > nextDropTime)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    PhotonNetwork.Instantiate("Weapons/Emp GernadeEnemy", grenadeSpawnPos.transform.position, transform.rotation);
                    nextDropTime = Time.time + bombChargeTime;


                }
            }
        }
    }
    void FireRockets() {

        if (player.Team1)
        {
            GameObject rocket = PhotonNetwork.Instantiate("Weapons/Rocket", spawnPos[Random.Range(0, spawnPos.Length)].transform.position, spawnPos[Random.Range(0, spawnPos.Length)].transform.rotation);

            

            rocket.GetComponent<Rocket>().rocketSpeed = bikeCont.speed * 2;

            rocket.GetComponent<Rocket>()._target = _target;


        }
        if (player.Team2)
        {
            GameObject rocket = PhotonNetwork.Instantiate("Weapons/EnemyRocket", spawnPos[Random.Range(0, spawnPos.Length)].transform.position, spawnPos[Random.Range(0, spawnPos.Length)].transform.rotation);

            rocket.GetComponent<Rocket>().rocketSpeed = bikeCont.speed * 2;

            rocket.GetComponent<Rocket>()._target = _target;


        }

        nextFireTime = Time.time + cooldownTime;


    }
    [SerializeField] Collider[] findEnemy;


    int waitForLoop = 1;
    
    void HomingMissile()
    {

        if(findEnemy.Length == 0)
        {
            Aim.gameObject.SetActive(false);
            _target = null;
        }
        
        


        // Team 1 //
            if (gameObject.tag == "Team1")
        {
            findEnemy = Physics.OverlapSphere(transform.position + (transform.forward * 30), 30, LayerMask.GetMask("Team2"));

            if (findEnemy.Length != 0)
            {
                
                //if (findEnemy[0].gameObject.tag == "Team2")
                //{
                //    _target = findEnemy[0].transform;
                //}
                //else if(findEnemy[1].gameObject.tag == "Team2")
                //{
                //    _target = findEnemy[1].transform;
                //}

                StartCoroutine(FindingEnemyTeam2());

                if (_target != null)
                {
                    Aim.gameObject.SetActive(true);

                    Aim.transform.position = _target.position;

                    if (_target.gameObject.GetComponent<Player>() != null)
                    {
                        if (_target.gameObject.GetComponent<Player>().defeated)
                        {
                            Aim.gameObject.SetActive(false);
                        }
                    }
                    if (_target.gameObject.GetComponent<AiHealth>() != null)
                    {
                        if (_target.gameObject.GetComponent<AiHealth>().defeated)
                        {
                            Aim.gameObject.SetActive(false);
                        }
                    }
                }
            }

            try
            {
                if (_target != null)
                {
                    if (_target.gameObject.tag != "Team2")
                    {
                        _target = null;
                        Aim.gameObject.SetActive(false);

                    }
                }

            }
            catch (System.IndexOutOfRangeException)
            {
                print("Searching Target...");
            }
        }

            // Team 2 //
        else if (gameObject.tag == "Team2")
        {
            findEnemy = Physics.OverlapSphere(transform.position + (transform.forward * 30), 30, LayerMask.GetMask("Team1"));

            if (findEnemy.Length != 0)
            {
                Aim.gameObject.SetActive(true);

                //if (findEnemy[0].gameObject.tag != "Team1")
                //{
                //    _target = findEnemy[1].transform;
                //}
                //else
                //{
                //    _target = findEnemy[0].transform;
                //}

                StartCoroutine(FindingEnemyTeam1());
                Aim.transform.position = _target.position;

                if (_target.gameObject.GetComponent<Player>() != null)
                {
                    if (_target.gameObject.GetComponent<Player>().defeated)
                    {
                        Aim.gameObject.SetActive(false);
                    }
                }
                if (_target.gameObject.GetComponent<AiHealth>() != null)
                {
                    if (_target.gameObject.GetComponent<AiHealth>().defeated)
                    {
                        Aim.gameObject.SetActive(false);
                    }
                }

            }
            try
            {
                if (_target.gameObject.tag != "Team1")
                {
                    _target = null;
                    Aim.gameObject.SetActive(false);

                }

            }
            catch (System.IndexOutOfRangeException)
            {
                print("Searching Target...");
            }
        }
    }

    IEnumerator FindingEnemyTeam1()
    {
        foreach(Collider enemy in findEnemy)
        {
            if(enemy.tag == "Team1")
            {
                _target = enemy.transform;
            }
            yield return null;

        }
    }
    IEnumerator FindingEnemyTeam2()
    {
        foreach(Collider enemy in findEnemy)
        {
            if(enemy.tag == "Team2")
            {
                _target = enemy.transform;
            }
            yield return null;

        }
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position + transform.forward * 30, 30);
        Gizmos.color = Color.red;

    }
    






}
