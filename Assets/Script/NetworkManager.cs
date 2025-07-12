using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        Connect();
    }




    public void Connect()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public void Play()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Tried to Join A room and Failed");

        // Most Likely No Room
        PhotonNetwork.CreateRoom(null,new RoomOptions { MaxPlayers = 4});
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined a room!!");

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(0);
        }
    }
}
