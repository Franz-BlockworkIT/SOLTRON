

// Ai
using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

public class Ai : MonoBehaviour
{
	public bool canMove = true;

	[SerializeField]
	public float speed = 10f;

	[SerializeField]
	public float maxSpeed = 10f;

	public GameManager stop;

	public int BotRandomPositionRadius = 10;

	public Vector3 moveSpot;

	private float forwardAccel;

	public float rotationSpeed = 50f;

	public float wheelRotationSpeed = 10f;

	[SerializeField]
	private GameObject[] wheelsToRotate;

	public bool grounded;

	public LayerMask whatIsGround;

	public float groundRayLength = 0.5f;

	public Transform groundRayPoint;

	public float gravityForce = 10f;

	private RaycastHit hit2;

	private Vector3 collision = Vector3.zero;

	public MeshRenderer[] bikeMat;

	private Rigidbody rb;

	private LightWallsAI trails;

	public float force = 5f;

	public float radius;

	[Range(0f, 360f)]
	public float angle;

	public float detectionRayLength = 10f;

	public LayerMask targetMask;

	public LayerMask obstructionMask;

	private bool ObjDetected;

	public bool changeMoveSpot;

	private Ray ray;

	private float changePos = 4f;

	[SerializeField]
	private Transform bikeBody;

	[Header("Attack")]
	public float cooldownTime = 2f;

	private float nextFireTime;

	[SerializeField]
	private GameObject rocketPrefab;

	[SerializeField]
	private GameObject[] spawnPos;

	private int MaxRockets = 3;

	public int waitForNextEmp = 4;
	public int waitForNextRocket = 4;


	private int nextDropTime;
	

	[SerializeField]
	private GameObject grenade;

	[SerializeField]
	private GameObject grenadeSpawnPos;

	[SerializeField]
	private Collider[] findEnemy;

	private Transform _target;

	private Collider[] EnemyCheckForBomb;

	PhotonView PV;

	private void Awake()
	{

		stop = GameObject.Find("Game Manager").GetComponent<GameManager>();
		bikeMat = GetComponentsInChildren<MeshRenderer>();
		PV = GetComponent<PhotonView>();
	}

	private void Start()
	{
		if (!PV.IsMine)
			return;
		moveSpot = GetNewPosition();
		rb = GetComponent<Rigidbody>();
		((MonoBehaviour)this).StartCoroutine(FOVRoutine());
		speed = maxSpeed;
		waitForNextEmp = UnityEngine.Random.Range(2, 5);
	}

	private void Update()
	{

		if (!PV.IsMine)
			return;
		if (!stop.canMove)
		{
			return;
		}
		if (canMove)
		{
			WatchYouStep();
			GetToStopping();
			changePos -= Time.deltaTime;
			if (changePos <= 0f)
			{
				moveSpot = GetNewPosition();
				changePos = 4f;
			}
			if (changeMoveSpot)
			{
				((MonoBehaviour)this).StartCoroutine(ChangeMoveSpot());
			}
			Quaternion rotation = transform.rotation;
			Vector3 velocity = rb.velocity;
			//Quaternion bodyRotation = rotation * Quaternion.Euler(0f, 0f,  30f);
			//bikeBody.rotation = Quaternion.Lerp(bikeBody.rotation, bodyRotation, 0.1f);
			WheelSpinning();
			Attack();
		}
		else if (trails != null)
		{
			trails.enabled = false;
		}
		HomingMissile();
	}

	private void FixedUpdate()
	{
		if (!PV.IsMine)
			return;
		if (!stop.canMove || !canMove)
		{
			return;
		}
		grounded = false;
		RaycastHit hit = default(RaycastHit);
		if (Physics.Raycast(groundRayPoint.position, -transform.up, out hit, groundRayLength, whatIsGround))
		{
			grounded = true;
			transform.rotation = (Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation);
		}
		if (grounded)
		{
			BotMove();
			rb.drag = 1f;
		}
		else
		{
			rb.drag = 0.1f;
			rb.AddForce(Vector3.up * (0f - gravityForce) * 100f);
		}
		ray = new Ray(groundRayPoint.transform.position, transform.forward);
		Debug.DrawLine(transform.position,hit2.point, Color.white);
		if (!Physics.Raycast(ray, out hit2, 80f))
		{
			return;
		}
		collision = hit2.point;
		Debug.DrawRay(transform.position, collision, Color.blue);
		if (hit2.collider.gameObject.tag == "Wall" && changeMoveSpot)
		{
			moveSpot = transform.forward * (0f - hit2.point.z);
		}
		if (gameObject.tag == "Team2")
		{
			if (hit2.collider.gameObject.layer == LayerMask.NameToLayer("Player Trail"))
			{
				moveSpot = transform.forward * (0f - hit2.point.z);
			}
		}
		else if (gameObject.tag == "Team1" && hit2.collider.gameObject.layer == LayerMask.NameToLayer("Enemy Trail"))
		{
			moveSpot = transform.forward * (0f - hit2.point.z);
		}
	}

	public void BotMove()
	{
		
		float curspeed = speed;
		forwardAccel = curspeed * Time.smoothDeltaTime;
		
		rb.velocity = transform.forward * curspeed;
	}

	private void WheelSpinning()
	{
		GameObject[] array = wheelsToRotate;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].transform.Rotate(Time.deltaTime * speed * wheelRotationSpeed, 0f, 0f, (Space)1);
		}
	}

	private Vector3 GetNewPosition()
	{
		
		return new Vector3(UnityEngine.Random.insideUnitSphere.x * radius, transform.position.y, UnityEngine.Random.insideUnitSphere.z * radius);
	}

	private void GetToStopping()
	{
		rb.AddForce(transform.forward * force * Time.deltaTime);
		Vector3 velocity = rb.velocity;
		if (velocity.magnitude > speed)
		{
			rb.velocity = Vector3.ClampMagnitude(rb.velocity, speed);
		}
		if (Vector3.Distance(transform.position, moveSpot) <= speed)
		{
			moveSpot = GetNewPosition();
		}
	}

	private void WatchYouStep()
	{
		
		Quaternion newDirection = Quaternion.LookRotation(moveSpot - transform.position);
		transform.rotation = (Quaternion.Lerp(transform.rotation, newDirection, rotationSpeed * Time.deltaTime));
	}

	private IEnumerator FOVRoutine()
	{
		WaitForSeconds wait = new WaitForSeconds(0.3f);
		while (true)
		{
			yield return wait;
			FOVCheck();
		}
	}

	private IEnumerator ChangeMoveSpot()
	{
		moveSpot = transform.forward * (0f - hit2.point.z);
		changeMoveSpot = false;
		yield return (object)new WaitForSeconds(3f);
	}

	private IEnumerator WaitForSpeed()
	{
		yield return (object)new WaitForSeconds(1f);
		speed = maxSpeed;
	}

	private void FOVCheck()
	{
		
		Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);
		if (rangeChecks.Length == 0)
		{
			return;
		}
		Transform target = ((Component)rangeChecks[0]).transform;
		Vector3 val = target.position - transform.position;
		Vector3 directionToTarget = val.normalized;
		if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2f)
		{
			Vector3.Distance(transform.position, target.position);
			ObjDetected = Physics.Raycast(transform.position, directionToTarget, detectionRayLength);
			if (ObjDetected)
			{
				moveSpot = GetNewPosition();
			}
		}
	}

	public void Attack()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (Physics.Raycast(ray, out hit2, 100f))
		{
			if (gameObject.tag == "Team1")
			{
				if (hit2.collider.tag == "Team2" && Time.time > nextFireTime && MaxRockets > 0)
				{
					FireRockets();
				}
			}
			else if (gameObject.tag == "Team2" && hit2.collider.tag == "Team1" && Time.time > nextFireTime && MaxRockets > 0)
			{

				FireRockets();
			}
		}
		DeployBomb();
	}

	private void FireRockets()
	{
	
		GameObject obj = PhotonNetwork.Instantiate("Weapons/EnemyRocket", spawnPos[UnityEngine.Random.Range(0, spawnPos.Length)].transform.position, spawnPos[UnityEngine.Random.Range(0, spawnPos.Length)].transform.rotation, 0);
		obj.GetComponent<Rocket>().rocketSpeed = speed * 1.5f;
		obj.layer = gameObject.layer;
		obj.GetComponent<Rocket>()._target = _target;
		nextFireTime = Time.deltaTime + cooldownTime;
		MaxRockets--;
	}

	private void DeployBomb()
	{
		
		if (gameObject.tag == "Team1")
		{
			EnemyCheckForBomb = Physics.OverlapSphere(transform.position, 30f, LayerMask.GetMask(new string[1] { "Team2" }));
			if (EnemyCheckForBomb.Length != 0 && Time.deltaTime > (float)nextDropTime)
			{
				PhotonNetwork.Instantiate("Weapons/Player Emp Gernade", grenadeSpawnPos.transform.position, transform.rotation, 0);
				nextDropTime = (int)Time.deltaTime + waitForNextEmp;
			}
		}
		else if (gameObject.tag == "Team2")
		{
			EnemyCheckForBomb = Physics.OverlapSphere(transform.position, 30f, LayerMask.GetMask(new string[1] { "Team1" }));
			if (EnemyCheckForBomb.Length != 0 && Time.deltaTime > (float)nextDropTime)
			{
				PhotonNetwork.Instantiate("Weapons/Emp GernadeEnemy", grenadeSpawnPos.transform.position, transform.rotation, 0);
				nextDropTime = (int)Time.deltaTime + waitForNextEmp;
			}
		}
	}

	private void HomingMissile()
	{
		
		if (findEnemy.Length == 0)
		{
			_target = null;
		}
		if (gameObject.tag == "Team1")
		{
			findEnemy = Physics.OverlapSphere(transform.position + transform.forward * 30f, 30f, LayerMask.GetMask(new string[1] { "Team2" }));
			if (findEnemy.Length != 0)
			{
				_target = ((Component)findEnemy[0]).transform;
			}
			try
			{
				if (((Component)findEnemy[0]).tag != "Team2")
				{
					_target = null;
				}
				return;
			}
			catch (IndexOutOfRangeException)
			{
				MonoBehaviour.print((object)"Searching Target...");
				return;
			}
		}
		if (!(gameObject.tag == "Team2"))
		{
			return;
		}
		findEnemy = Physics.OverlapSphere(transform.position + transform.forward * 30f, 30f, LayerMask.GetMask(new string[1] { "Team1" }));
		if (findEnemy.Length != 0)
		{
			_target = findEnemy[0].transform;
		}
		try
		{
			if (findEnemy[0].tag != "Team1")
			{
				_target = null;
			}
		}
		catch (IndexOutOfRangeException)
		{
			MonoBehaviour.print((object)"Searching Target...");
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag != "Ground")
		{
			changeMoveSpot = true;
			speed = 0f;
			if (changeMoveSpot)
			{
				StartCoroutine(ChangeMoveSpot());
			}
			StartCoroutine("WaitForSpeed");
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		speed = maxSpeed;
		changeMoveSpot = false;
	}

	private void OnDrawGizmos()
	{
		
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireSphere(collision, 10f);
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere(moveSpot, 5f);
	}

	

	
}
