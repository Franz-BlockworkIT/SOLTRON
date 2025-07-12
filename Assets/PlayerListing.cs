using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerListing : MonoBehaviour
{
    public Photon.Realtime.Player Player { get; private set; }
    public int PlayerGroups;

    public void SetPlayerInfo(Photon.Realtime.Player player)
    {
        Player = player;
        PlayerGroups = (int)player.CustomProperties["Grp"];
    }
}
