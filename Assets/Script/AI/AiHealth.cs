// AiHealth
using System.Collections;
using Photon.Pun;
using UnityEngine;

public class AiHealth : MonoBehaviour
{
	public int maxHealth = 100;

	public int currentHealth;

	[SerializeField]
	private Animator anim;

	private Ai aiCont;

	public bool defeated;

	public PhotonView PV;

	private void Awake()
	{
		anim = GetComponent<Animator>();
		aiCont = GetComponent<Ai>();
		PV = GetComponent<PhotonView>();
	}

	private void Start()
	{
		currentHealth = maxHealth;
	}

	private void Update()
	{
		if (!PV.IsMine)
			return;
		if (RoundSystem.RS.RoundStarted)
		{
			defeated = true;
			PV.RPC("RPC_Destroy", RpcTarget.AllBuffered);
			PhotonNetwork.Destroy(base.gameObject);
		}
		if (currentHealth <= 0)
		{
			defeated = true;
			currentHealth = 0;
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Player Trail") && base.gameObject.tag == "Team2")
		{
			StartCoroutine("WaitForSpeed");
			//TakeDamage(maxHealth / 4);
			PV.RPC("TakeDamageAi", RpcTarget.AllBuffered, maxHealth / 4);

		}
		if (collision.gameObject.tag == "Team2" || collision.gameObject.tag == "Team1")
		{
			//TakeDamage(maxHealth / 10);
			PV.RPC("TakeDamageAi", RpcTarget.AllBuffered, maxHealth / 10);
		}
		
		if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Enemy Trail") && base.gameObject.tag == "Team1")
		{
			StartCoroutine("WaitForSpeed");
			//TakeDamage(maxHealth / 4);
			PV.RPC("TakeDamageAi", RpcTarget.AllBuffered, maxHealth / 4);

		}
		if (collision.gameObject.tag == "Team1" || collision.gameObject.tag == "Team2")
		{
			//TakeDamage(maxHealth / 10);
			PV.RPC("TakeDamageAi", RpcTarget.AllBuffered, maxHealth / 10);
		}
	}

	[PunRPC]
	public void TakeDamageAi(int damage)
	{
		currentHealth -= damage;
		if (currentHealth < 0)
		{
			currentHealth = 0;
			aiCont.canMove = false;
			anim.enabled = true;
			Object.Destroy(base.gameObject, 4f);
		}
	}

	private IEnumerator WaitForSpeed()
	{
		yield return new WaitForSeconds(1f);
		aiCont.speed = aiCont.maxSpeed;
	}

	[PunRPC]
	private void RPC_Destroy()
	{
		Destroy(gameObject);
	}

	public void PlayDestroySound()
	{
		PhotonNetwork.Instantiate("AfterDestroySound", transform.position, transform.rotation);
	}
}
