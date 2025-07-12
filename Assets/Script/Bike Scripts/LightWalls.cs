using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class LightWalls : MonoBehaviour
{

    public static LightWalls _LW;

    public Transform followPos;
    [SerializeField] float height = 1f;
    [SerializeField] float width = 0.001f;
    [SerializeField] float destroyTrailAfterSecond = 4f;
    [SerializeField] Material trailMaterial;
    [SerializeField] float spawnDelay = 0.01f;
    [SerializeField] float maxSpawnDelay = 99999f;

    
    public BikeController playerCont;


    bool firstTime = true;
    GameObject trail;
    [SerializeField] List<GameObject> trails;
    MeshFilter meshFilter;
    MeshCollider meshCollider;
    List<Vector3> verticesDef;
    List<int> triangleDef;
    List<Vector2> UVsDef;
    bool isEven = true;
    int x;
    float currentSpawnTrailTime = 0;
    
    PhotonView PV;

    const string addMeshstrName = "AddMesh";

    public Player playerChck;

    bool spawnTrails;

    [SerializeField] Material lightWallsMat;
    [SerializeField] Material lightWallsMat2;

    public TrailRenderer trailRend;
    public TrailRenderer followPosTrailRend;

    public bool team1, team2;
     
    public void SetFollowPos(Transform pos)
    {
        followPos = pos;
    }

    private void Awake()
    {
        _LW = this;


        //playerChck = GetComponent<Player>();
        PV = GetComponent<PhotonView>();



    }


    void Start()
    {



        InvokeRepeating(addMeshstrName, 0, spawnDelay);

        followPosTrailRend = followPos.GetComponent<TrailRenderer>();
       

    }


    
    private void AddMesh()
    {
        if (RoundSystem.RS.RoundStarted)
        {
            
            PV.RPC("RPC_DestroyTrail", RpcTarget.AllBuffered);

        }

        //if (!PV.IsMine)
        //    return;
        if (playerChck != null)
        {
            if (playerChck.defeated)
            {
                PV.RPC("RPC_DestroyTrail", RpcTarget.AllBuffered);

                return;

            }
        }
        
        if(followPos == null )
        {

            return;
        }
        MonoLine();
        //PV.RPC("MonoLine", RpcTarget.All);
        if (currentSpawnTrailTime > destroyTrailAfterSecond)
        {
            //PV.RPC("RemoveMesh", RpcTarget.All);
            RemoveMesh();
        }








    }
    
    
    void Update()
    {
      


        currentSpawnTrailTime += Time.deltaTime;
        if (!PV.IsMine)
        {
            return;
        }
        

        if (followPos == null || followPosTrailRend.time <= 0.1f)  
        {
            PV.RPC("RPC_DestroyTrail", RpcTarget.AllBuffered);
        }
        


        if (playerChck != null)
        {
            if (playerChck.gameObject.tag == "Team1")
            {
                lightWallsMat = Resources.Load("Materials/LightWalls/LightWalls") as Material;
                lightWallsMat2 = Resources.Load("Materials/LightWalls/LightWalls2") as Material;
            }
            else
            {
                lightWallsMat = Resources.Load("Materials/LightWalls/LightWalls1_1") as Material;
                lightWallsMat2 = Resources.Load("Materials/LightWalls/LightWalls2_1") as Material;
            }

            lightWallsMat.SetColor("_EmissionColor", playerCont.bikeMat.GetColor("_EmissionColor"));
            lightWallsMat.color = playerCont.bikeMat.GetColor("_EmissionColor");
            lightWallsMat2.SetColor("_EmissionColor", playerCont.bikeMat.GetColor("_EmissionColor"));
            lightWallsMat2.color = playerCont.bikeMat.GetColor("_EmissionColor");


            Color LWmatColor = lightWallsMat.color;
            LWmatColor.a = .6f;
            lightWallsMat.color = LWmatColor;



            Color LWmatColor2 = lightWallsMat2.color;
            LWmatColor2.a = .3f;
            lightWallsMat2.color = LWmatColor2;
        }
        //if (playerChck != null)
        //{
        //    if (playerChck.defeated)
        //    {
        //        //if (PV.IsMine)
        //        //{
        //        PV.RPC("RPC_DestroyTrail", RpcTarget.AllBuffered);
        //        //}    
        //    }
        //}


       
    }



    [PunRPC]
    private void MonoLine()
    {

        
        Vector3[] vertices = null;
        int[] triangles = null;
        

        var backward = followPos.transform.position - (followPos.transform.forward);

        if (firstTime)
        {
            vertices = new Vector3[]
            {
            backward + (followPos.transform.right * -width),
            backward - (followPos.transform.right * -width),
            backward - (followPos.transform.right * -width) + followPos.transform.up * height,
            backward + (followPos.transform.right * -width) + followPos.transform.up * height,
            };
                

            triangles = new int[]
            {
            0, 2, 1,
            0, 3, 2,
            };


            trail = new GameObject();



            if (playerChck.gameObject.tag == "Team1")
            {
                trail.name = "Player Trails";
                trail.layer = LayerMask.NameToLayer("Player Trail");

            
  
            }
            if(playerChck.gameObject.tag == "Team2")
            {
                trail.name = "Enemy Trails";
                trail.layer = LayerMask.NameToLayer("Enemy Trail");

            


            }




            if (trail.GetComponent<MeshFilter>() == null)
                meshFilter = trail.AddComponent<MeshFilter>();
            //if (trail.GetComponent<MeshRenderer>() == null)
            //    trail.AddComponent<MeshRenderer>();
            if (trail.GetComponent<PhotonView>() == null)
                trail.AddComponent<PhotonView>();
            

            DestroyTrails();
            




            meshCollider = trail.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = meshFilter.mesh;

            verticesDef = new List<Vector3>();
            triangleDef = new List<int>();
            

            foreach (var v in vertices)
                verticesDef.Add(v);
            foreach (var t in triangles)
                triangleDef.Add(t);
            

            meshFilter.mesh.vertices = vertices;
            meshFilter.mesh.triangles = triangles;


            trails.Add(trail);

            isEven = false;
            firstTime = false;

            

            x = 4;
            return;

        }

        if (isEven)
        {
            verticesDef.Add(backward + (followPos.transform.right * -width));
            verticesDef.Add(backward - (followPos.transform.right * -width));
            verticesDef.Add(backward - (followPos.transform.right * -width) + followPos.transform.up * height);
            verticesDef.Add(backward + (followPos.transform.right * -width) + followPos.transform.up * height);



            //left face
            triangleDef.Add(x - 4); //0
            triangleDef.Add(x - 1); //3
            triangleDef.Add(x);     //4

            triangleDef.Add(x - 4); //0
            triangleDef.Add(x);     //4
            triangleDef.Add(x + 3); //7

            //top face
            triangleDef.Add(x - 4); //0
            triangleDef.Add(x + 3); //7
            triangleDef.Add(x + 2); //6

            triangleDef.Add(x - 4); //0
            triangleDef.Add(x + 2); //6
            triangleDef.Add(x - 3); //1

            //right face
            triangleDef.Add(x - 3); //5
            triangleDef.Add(x + 2); //10
            triangleDef.Add(x + 1); //9

            triangleDef.Add(x - 3); //5
            triangleDef.Add(x + 1); //9
            triangleDef.Add(x - 2); //6



            isEven = false;

        }
        else
        {
            verticesDef.Add(backward + (followPos.transform.right * -width) + followPos.transform.up * height);
            verticesDef.Add(backward - (followPos.transform.right * -width) + followPos.transform.up * height);
            verticesDef.Add(backward - (followPos.transform.right * -width));
            verticesDef.Add(backward + (followPos.transform.right * -width));


            //left face
            triangleDef.Add(x - 4); //0
            triangleDef.Add(x + 3); //7
            triangleDef.Add(x);     //4

            triangleDef.Add(x - 4); //0
            triangleDef.Add(x);     //4
            triangleDef.Add(x - 1); //3

            //top face
            triangleDef.Add(x - 2); //2
            triangleDef.Add(x - 1); //3
            triangleDef.Add(x);     //4

            triangleDef.Add(x - 2); //2
            triangleDef.Add(x);     //4
            triangleDef.Add(x + 1); //5

            //right face
            triangleDef.Add(x - 3); //5
            triangleDef.Add(x - 2); //6
            triangleDef.Add(x + 1); //9

            triangleDef.Add(x - 3); //5
            triangleDef.Add(x + 1); //9
            triangleDef.Add(x + 2); //10

            


            isEven = true;
        }
        x += 4;


        if (verticesDef != null)
        {
            meshFilter.mesh.vertices = verticesDef.ToArray();
            meshFilter.mesh.triangles = triangleDef.ToArray();
            

        }
        meshCollider.sharedMesh = meshFilter.mesh;


    }

    [PunRPC]
    private void RemoveMesh()
    {


        verticesDef.RemoveAt(0);
        verticesDef.RemoveAt(0);
        verticesDef.RemoveAt(0);
        verticesDef.RemoveAt(0);


        triangleDef.RemoveAt(triangleDef.Count - 1);
        triangleDef.RemoveAt(triangleDef.Count - 1);
        triangleDef.RemoveAt(triangleDef.Count - 1);

        triangleDef.RemoveAt(triangleDef.Count - 1);
        triangleDef.RemoveAt(triangleDef.Count - 1);
        triangleDef.RemoveAt(triangleDef.Count - 1);

        triangleDef.RemoveAt(triangleDef.Count - 1);
        triangleDef.RemoveAt(triangleDef.Count - 1);
        triangleDef.RemoveAt(triangleDef.Count - 1);

        triangleDef.RemoveAt(triangleDef.Count - 1);
        triangleDef.RemoveAt(triangleDef.Count - 1);
        triangleDef.RemoveAt(triangleDef.Count - 1);

        triangleDef.RemoveAt(triangleDef.Count - 1);
        triangleDef.RemoveAt(triangleDef.Count - 1);
        triangleDef.RemoveAt(triangleDef.Count - 1);

        triangleDef.RemoveAt(triangleDef.Count - 1);
        triangleDef.RemoveAt(triangleDef.Count - 1);
        triangleDef.RemoveAt(triangleDef.Count - 1);

        




        meshFilter.mesh.Clear();

        


        meshFilter.mesh.vertices = verticesDef.ToArray();
        meshFilter.mesh.triangles = triangleDef.ToArray();
        



        x -= 4;


        
        

    }

    void DestroyTrails()
    {
        if (trail.GetComponent<LightWallsDestroy>() == null)
        {
             trail.AddComponent<LightWallsDestroy>();
        }

    }

    [PunRPC]

    void RPC_DestroyTrail()
    {
       Destroy(trail);
        

    }

    



    void OnDestroy()
    {
        Debug.Log("After Destroy");
        Destroy(trail);
    }
}
