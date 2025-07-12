using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class NewPhotonRoom : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    //Room Info
    public static NewPhotonRoom room;
    private PhotonView PV;

    public int currentScene;

    private void Awake()
    {
        if (NewPhotonRoom.room == null)
        {
            NewPhotonRoom.room = this;
        }
        else
        {
            if (NewPhotonRoom.room != this)
            {
                Destroy(PhotonRoom.room.gameObject);
                NewPhotonRoom.room = this;
            }
        }

        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        PV = GetComponent<PhotonView>();
    }

    public override void OnEnable()
    {
        // subscribe to functions
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    public override void OnDisable()
    {
        // subscribe to functions
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
        SceneManager.sceneLoaded -= OnSceneFinishedLoading;
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        Debug.Log("We are in a room");

        StartGame();
    }



    void StartGame()
    {

        
        // Loads the multiplayer scene for all Players
        Debug.Log("inside StarGame is Master Client : " + PhotonNetwork.IsMasterClient);
        if (!PhotonNetwork.IsMasterClient)
            return;

        
            PhotonNetwork.LoadLevel(MultilplayerSettings.multiplayerSettings.multiplayerScene);
    }


    void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {

        // Called when multiplayer scene is loaded
        currentScene = scene.buildIndex;

        if (currentScene == MultilplayerSettings.multiplayerSettings.multiplayerScene)
        {
            PV.RPC("CreatePlayer", RpcTarget.All);
        }
    }


    private void CreatePlayer()
    {
        Debug.Log("player Created");

        // Creates player network Controller but not player Character
        PhotonNetwork.InstantiateSceneObject(Path.Combine("PhotonPrefabs", "PhotonNetworkPlayer"), transform.position, Quaternion.identity, 0);
    }
}
