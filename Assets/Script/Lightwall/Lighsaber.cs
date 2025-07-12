using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Lighsaber : MonoBehaviour
{
    //#pragma strict
    //#pragma implicit
    //#pragma downcast

    public float trailWidth;
    public float trailLength;
    public bool generate = false;
    public bool devTest = false;

    private Mesh mesh;
    private Vector3 vertex0;
    private Vector3 vertex1;
    private List<Vector3> vertices;
    private List<Vector3> verticesWorld;
    private Vector2[] uvs;
    private int[] triangles;

    // private List<Vector3> normals;
    // private Vector3 vector1;
    // private Vector3 vector2;
    void Start()
    {
        vertex0 = new Vector3(trailWidth / 2, 0, 0);
        vertex1 = new Vector3(-trailWidth / 2, 0, 0);
        mesh = GetComponent<MeshFilter>().mesh;
        vertices = new List<Vector3>();
        verticesWorld = new List<Vector3>();
    }

    void FixedUpdate()
    {
        // clear old mesh before calculating a new one
        mesh.Clear();

        if (generate)
        {
            // set local position of the first 2 vertices
            vertex0 = new Vector3(trailWidth / 2, 0, 0);
            vertex1 = new Vector3(-trailWidth / 2, 0, 0);

            // add these vertices at the top of the vertex array
            vertices.Insert(0, vertex0); // Uhhhh
            vertices.Insert(0, vertex1); // Uhhhh

            // if max length has been reached discard the last two vertices
            if (vertices.Count > trailLength * 2 + 2)
            {
                vertices.RemoveAt(vertices.Count - 1);
                vertices.RemoveAt(vertices.Count - 1);
            }
        }
        else
        {
            // if trail has been disabled and there are still vertices discard the last two vertices
            if (vertices.Count > 0)
            {
                vertices.RemoveAt(vertices.Count - 1);
                vertices.RemoveAt(vertices.Count - 1);
            }
        }

        int vertexCount = vertices.Count;

        // if we have at least 4 vertices
        if (vertexCount >= 4)
        {

            // read saved world positions shifted by 2 verts as local positions
            for (int i = 0; i < vertexCount - 2; i++)
            {
                vertices[i + 2] = transform.InverseTransformPoint(verticesWorld[i]);
            }

            // add vertices to mesh
            mesh.vertices = vertices.ToArray();
        }

        // save world positions of all vertices for reuse in next cycle
        verticesWorld = new List<Vector3>();

        for (int i = 0; i < vertexCount; i++)
        {
            verticesWorld.Add(transform.TransformPoint(vertices[i]));
        }

        if (vertexCount >= 4)
        {
            uvs = new Vector2[vertexCount];

            float vertexCountFloat = (float)vertexCount;
            for (int i = 1; i < vertexCount; i++)
            {
                if (i % 2 == 0)
                    uvs[i] = new Vector2(i / (vertexCountFloat - 2), 0);
                else
                    uvs[i] = new Vector2((i - 1) / (vertexCountFloat - 2), 1);
            }

            mesh.uv = uvs;

            // calculate triangles
            triangles = new int[(vertexCount - 2) * 3];

            // first triangle
            triangles[0] = 0;
            triangles[1] = 2;
            triangles[2] = 1;
            int tri = 3;

            // subsequent triangles
            for (int i = 1; i < vertexCount - 2; i++) // Triangle count will always be (vertex.count-2)*3
            {
                if (i % 2 == 0)
                {
                    triangles[tri] = i;
                    triangles[tri + 1] = i + 1;
                    triangles[tri + 2] = i + 2;
                }
                else
                {
                    triangles[tri] = i;
                    triangles[tri + 1] = i + 2;
                    triangles[tri + 2] = i + 1;
                }
                tri += 3;
            }

            mesh.triangles = triangles;

            // // calculate normals
            //
            // normals = new Vector3[vertices.length];
            //
            // // normal for vertex 0
            // vector1 = verticesWorld[1] - verticesWorld[0];
            // vector2 = verticesWorld[2] - verticesWorld[0];
            //
            // if (Vector3.Angle(vector1, vector2) < 180) {
            //     normals[0] = Vector3.Cross(vector1, vector2);
            //     normals[0] = transform.InverseTransformDirection(normals[0].normalized);
            // } else {
            //     normals[0] = Vector3.Cross(vector2, vector1);
            //     normals[0] = transform.InverseTransformDirection(normals[0].normalized);
            // }
            //
            // // normals for subsequent vertices
            // for (i = 1; i < vertices.length - 1; i++) {
            //     vector1 = verticesWorld[i-1] - verticesWorld[i];
            //     vector2 = verticesWorld[i+1] - verticesWorld[i];
            //
            //     if (Vector3.Angle(vector1, vector2) < 180) {
            //         normals[i] = Vector3.Cross(vector1, vector2);
            //         normals[i] = transform.InverseTransformDirection(normals[i].normalized);
            //     } else {
            //         normals[i] = Vector3.Cross(vector2, vector1);
            //         normals[i] = transform.InverseTransformDirection(normals[i].normalized);
            //     }
            // }
            //
            // // normal for last vertex
            // vector1 = verticesWorld[vertices.length - 2] - verticesWorld[vertices.length - 1];
            // vector2 = verticesWorld[vertices.length - 3] - verticesWorld[vertices.length - 1];
            //
            // if (Vector3.Angle(vector1, vector2) < 180) {
            //     normals[vertices.length - 1] = Vector3.Cross(vector1, vector2);
            //     normals[vertices.length - 1] = transform.InverseTransformDirection(normals[vertices.length - 1].normalized);
            // } else {
            //     normals[vertices.length - 1] = Vector3.Cross(vector2, vector1);
            //     normals[vertices.length - 1] = transform.InverseTransformDirection(normals[vertices.length - 1].normalized);
            // }
            //
            // mesh.normals = normals;

        }

    }

}