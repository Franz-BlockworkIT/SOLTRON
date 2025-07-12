    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetworkPlayer : MonoBehaviourPun,IPunObservable 
{
    protected BikeController bikeCont;
    protected Vector3 RemotePlayerPosition;


    protected float RemoteLookX;
    protected float RemoteLookY;
    protected float LookXVel;
    protected float LookZVel;
    private void Awake()
    {
        bikeCont = GetComponent<BikeController>();
    }
    

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
            return;

        var LagDistance = RemotePlayerPosition - transform.position;

        if (LagDistance.magnitude > 5f)
        {
            transform.position = RemotePlayerPosition;
            LagDistance = Vector3.zero;
        }


        
        if (LagDistance.magnitude < 0.11f)
        {
            // Player is nearly at the point
            bikeCont.speedInput = 0;
            bikeCont.turnInput = 0;
        }
        else
        {
            //Player has to go to the point
            bikeCont.speedInput = LagDistance.normalized.z;
            bikeCont.turnInput = LagDistance.normalized.x;
        }

        // Look Smooth
        bikeCont.turnInput = Mathf.SmoothDamp(bikeCont.turnInput, RemoteLookX, ref LookXVel, 0.2f);
        bikeCont.speedInput = Mathf.SmoothDamp(bikeCont.speedInput, RemoteLookY, ref LookZVel, 0.2f);


    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(RemotePlayerPosition, 1f);
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(bikeCont.speedInput);
            stream.SendNext(bikeCont.turnInput);
        }
        else
        {
            RemotePlayerPosition = (Vector3)stream.ReceiveNext();
            RemoteLookX = (float)stream.ReceiveNext();
            RemoteLookY = (float)stream.ReceiveNext();
        }
    }

    
}
