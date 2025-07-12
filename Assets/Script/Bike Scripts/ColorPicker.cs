using UnityEngine;
using Photon.Pun;

public class ColorPicker : MonoBehaviour
{
    public Material[] BodyColorMat;
    Material currMat;
    Renderer[] rends;
    
    PhotonView PV;

    TeamMember team;

    static int colorIndex;

    public static bool colorChange;

    private void Awake()
    {
        team = GetComponent<TeamMember>();

    }
    private void Start()
    {

        
        

       
        
        
        if (GetComponent<PhotonView>() != null) {
            PV = GetComponent<PhotonView>();

            if (PV.IsMine)
            {
                if (!colorChange)
                {
                    colorIndex = PlayerPrefs.GetInt(PlayerScores.playerName + "BikeColor",PlayerScores.playerBikeColor);
                    PlayerScores.playerBikeColor = colorIndex;
                    colorChange = true;
                }

                    if (MainMenu.teamDeathmatch)
                {
                    
                        if (gameObject.tag == "Team1")
                        {

                            PV.RPC("RPC_ChangeBikeClr", RpcTarget.AllBuffered, GameManager.RandomColorTeam1);

                        }

                        else if(gameObject.tag == "Team2")
                        {

                            PV.RPC("RPC_ChangeBikeClr", RpcTarget.AllBuffered, GameManager.RandomColorTeam2);

                        }
                    
                }
                else
                {
                    PV.RPC("RPC_ChangeBikeClr", RpcTarget.AllBuffered, colorIndex);
                }
            }
            
        }
        if (gameObject.layer == LayerMask.NameToLayer("Default"))
        {
            rends = GetComponentsInChildren<Renderer>();
            RPC_ChangeBikeClr(PlayerPrefs.GetInt(PlayerScores.playerName + "BikeColor"));
        }
    }


    [PunRPC]
    void RPC_ChangeBikeClr(int clr)
    {

        rends = GetComponentsInChildren<Renderer>();

        if (rends != null)
        {
            foreach (Renderer rend in rends)
            {
                rend.material.SetColor("_EmissionColor", BodyColorMat[clr].color);
                currMat = rend.material;
            }
        }
        
    }
    public void BlueClr()
    {

        if (rends != null)
        {
            foreach (Renderer rend in rends)
            {
                rend.material.SetColor("_EmissionColor", BodyColorMat[0].color);
                currMat = rend.material;

                PlayerPrefs.SetInt(PlayerScores.playerName + "BikeColor", 0);
                PlayerScores.playerBikeColor = 0;

            }
        }
        
    }
    public void RedClr()
    {
        if (rends != null)
        {
        foreach (Renderer rend in rends)
        {
            rend.material.SetColor("_EmissionColor", BodyColorMat[1].color);
            currMat = rend.material;
            PlayerPrefs.SetInt(PlayerScores.playerName + "BikeColor", 1);
                PlayerScores.playerBikeColor = 1;

            }
        }

    }
    public void SkyblueClr()
    {

        if (rends != null)
        {
        foreach (Renderer rend in rends)
        {
            rend.material.SetColor("_EmissionColor", BodyColorMat[2].color);
            currMat = rend.material;
            PlayerPrefs.SetInt(PlayerScores.playerName + "BikeColor", 2);
                PlayerScores.playerBikeColor = 2;

            }
        }
    }
    public void WhiteClr()
    {

        if (rends != null)
        {
        foreach (Renderer rend in rends)
        {
            rend.material.SetColor("_EmissionColor", BodyColorMat[3].color);
            currMat = rend.material;
            PlayerPrefs.SetInt(PlayerScores.playerName + "BikeColor", 3);
                PlayerScores.playerBikeColor = 3;

            }
        }
    }
    public void PurpleClr()
    {

        if (rends != null)
        {
        foreach (Renderer rend in rends)
        {
            rend.material.SetColor("_EmissionColor", BodyColorMat[4].color);
            currMat = rend.material;
            PlayerPrefs.SetInt(PlayerScores.playerName + "BikeColor", 4);
                PlayerScores.playerBikeColor = 4;

            }
        }
    }
    public void YellowClr()
    {

        if (rends != null)
        {
        foreach (Renderer rend in rends)
        {
            rend.material.SetColor("_EmissionColor", BodyColorMat[5].color);
            currMat = rend.material;
            PlayerPrefs.SetInt(PlayerScores.playerName + "BikeColor", 5);
                PlayerScores.playerBikeColor = 5;

            }
        }
    }
    public void GreenClr()
    {

        if (rends != null)
        {
        foreach (Renderer rend in rends)
        {
            rend.material.SetColor("_EmissionColor", BodyColorMat[6].color);
            currMat = rend.material;
            PlayerPrefs.SetInt(PlayerScores.playerName + "BikeColor", 6);
                PlayerScores.playerBikeColor = 6;

            }
        }
    }
}
