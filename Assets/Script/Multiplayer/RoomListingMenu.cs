using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class RoomListingMenu : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Transform _content;
    [SerializeField]
    private GameObject _roomListing;


    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        
    }
}
