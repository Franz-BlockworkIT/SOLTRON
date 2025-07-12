using UnityEngine;
using Photon.Pun;

public class ColorChange : MonoBehaviour
{
    public Color[] randomMaterials;
    public SkinnedMeshRenderer[] Character;
    Ai ai;

    static int RandomColor;
    public static bool clrChangedOne;
    

    PhotonView PV;
    private void Awake()
    {
        Character = transform.Find("Body").transform.Find("Char").GetComponentsInChildren<SkinnedMeshRenderer>();
        ai = GetComponent<Ai>();
        PV = GetComponent<PhotonView>();
        


    }
    private void Start()
    {
        
        if (PhotonNetwork.IsMasterClient)
        {
            
            if (MainMenu.teamDeathmatch)
            {
                PV.RPC("ChangeClr", RpcTarget.AllBuffered);
            }
            else
            {

                PV.RPC("ChangeClrOneVsOne", RpcTarget.AllBuffered);
            }
        }

    }

   
    [PunRPC]
    void ChangeClr()
    {

        if (!GameManager.clrChanged)
        {
            if (gameObject.tag == "Team1")
            {

                if (GameManager.RandomColorTeam1 == GameManager.RandomColorTeam2)
                {
                    GameManager.RandomColorTeam1 = Random.Range(0, randomMaterials.Length);

                }


            }
            if (gameObject.tag == "Team2")
            {

                if (GameManager.RandomColorTeam2 == GameManager.RandomColorTeam1)
                {
                    GameManager.RandomColorTeam2 = Random.Range(0, randomMaterials.Length);
                }
            }
            GameManager.clrChanged = true;
        }

        foreach (MeshRenderer mesh in ai.bikeMat)
        {
            if (gameObject.layer == LayerMask.NameToLayer("Team1"))
            {

                mesh.material.color = randomMaterials[GameManager.RandomColorTeam1];
                mesh.material.SetColor("_EmissionColor", randomMaterials[GameManager.RandomColorTeam1]);
            }
            if (gameObject.layer == LayerMask.NameToLayer("Team2"))
            {
                mesh.material.color = randomMaterials[GameManager.RandomColorTeam2];
                mesh.material.SetColor("_EmissionColor", randomMaterials[GameManager.RandomColorTeam2]);
            }
            foreach (SkinnedMeshRenderer mat in Character)
            {
                mat.material.SetColor("_EmissionColor", mesh.material.GetColor("_EmissionColor"));
                mat.material.color = mesh.material.GetColor("_EmissionColor");
            }
        }
    } 
    [PunRPC]
    void ChangeClrOneVsOne()
    {

        if (!clrChangedOne)
        {



            RandomColor = Random.Range(0, randomMaterials.Length);






            clrChangedOne = true;
        }

        foreach (MeshRenderer mesh in ai.bikeMat)
        {
            

                mesh.material.color = randomMaterials[RandomColor];
                mesh.material.SetColor("_EmissionColor", randomMaterials[RandomColor]);
            
            foreach (SkinnedMeshRenderer mat in Character)
            {
                mat.material.SetColor("_EmissionColor", mesh.material.GetColor("_EmissionColor"));
                mat.material.color = mesh.material.GetColor("_EmissionColor");
            }
        }
    }

    

    
}