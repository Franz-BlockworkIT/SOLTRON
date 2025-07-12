using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Photon.Pun;


public class LightWallsAI : MonoBehaviour
{

    [SerializeField] Transform followPos;
    [SerializeField] float height = 1f;
    [SerializeField] float width = 0.001f;
    [SerializeField] float destroyTrailAfterSecond = 4f;
    [SerializeField] Material trailMaterial;
    [SerializeField] float spawnDelay = 0.01f;

    Ai aiController;
    AiHealth aiDefeat;

    ColorChange clrChange;


    bool firstTime = true;
    GameObject trail;
    MeshFilter meshFilter;
    MeshCollider meshCollider;
    //BoxCollider boxCol;

    List<Vector3> verticesDef;
    List<int> triangleDef;
    List<Vector2> UVsDef;
    bool isEven = true;
    int x;
    float currentSpawnTrailTime = 0;



    const string addMeshstrName = "AddMesh";

    PhotonView PV;

    [SerializeField] Material lightWallsMat;
    [SerializeField] Material lightWallsMat2;

    public void SetFollowPos(Transform pos)
    {
        followPos = pos;
    }

    private void Awake()
    {
        aiController = GetComponent<Ai>();
        //playerCont = GetComponent<BikeController>();
        aiDefeat = GetComponent<AiHealth>();

        clrChange = GetComponent<ColorChange>();
        
        PV = GetComponent<PhotonView>();
    }
    void Start()
    {
        InvokeRepeating(addMeshstrName, 0, spawnDelay);

        


    }

    

    private void AddMesh()
    {

        if (aiDefeat.defeated)
        {
            PV.RPC("RPC_DestroyTrail", RpcTarget.All);
            return;
            
        }
        
            MonoLine();
            if (currentSpawnTrailTime > destroyTrailAfterSecond)
            {
                RemoveMesh();
            }
        

    }

    void Update()
    {
        //if (!clrChange.LWclrChanged)
        //{
            foreach (MeshRenderer mesh in aiController.bikeMat)
            {
                lightWallsMat.SetColor("_EmissionColor", mesh.material.GetColor("_EmissionColor"));
                lightWallsMat.color = mesh.material.GetColor("_EmissionColor");


                lightWallsMat2.SetColor("_EmissionColor", mesh.material.GetColor("_EmissionColor"));
                lightWallsMat2.color = mesh.material.GetColor("_EmissionColor");
            }

            Color LWmatColor = lightWallsMat.color;
            LWmatColor.a = .6f;
            lightWallsMat.color = LWmatColor;

            Color LWmatColor2 = lightWallsMat2.color;
            LWmatColor2.a = .3f;
            lightWallsMat2.color = LWmatColor2;
            GameManager.GM.LWclrChanged = true;
        

        currentSpawnTrailTime += Time.deltaTime;

    }

    private void MonoLine()
    {

        if (aiDefeat.defeated)
        {
            return;
        }
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


            if (gameObject.tag == "Team1")
            {
                trail.name = "Player Trails";
                trail.layer = LayerMask.NameToLayer("Player Trail");
            }


            if (gameObject.tag == "Team2")
            {
                trail.name = "Enemy Trails";
                trail.layer = LayerMask.NameToLayer("Enemy Trail");
            }

                







            if (trail.GetComponent<MeshFilter>() == null)
                meshFilter = trail.AddComponent<MeshFilter>();
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


        if (verticesDef.Count != 0)
        {
            meshFilter.mesh.vertices = verticesDef.ToArray();
            meshFilter.mesh.triangles = triangleDef.ToArray();
            
        }
        meshCollider.sharedMesh = meshFilter.mesh;


    }

    private void RemoveMesh()
    {
        if (aiDefeat.defeated)
        {
            return;
        }

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
        if (trail != null)
            if (trail.GetComponent<LightWallsDestroy>() == null)
                trail.AddComponent<LightWallsDestroy>();

    }


    [PunRPC]
    void RPC_DestroyTrail()
    {
        
        
        Destroy(trail);
    }

   
    void OnDestroy()
    {
        
        
        Destroy(trail);
    }
}
