using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{



    public GameObject wallPrefab;


    public List<GameObject> walls;

    public int wallsRange = 8;

    public float speed;


    private void Start()
    {
        //InvokeRepeating("spawnWalls", 0);
    }

    private void Update()
    {
        transform.position += transform.forward * Input.GetAxis("Vertical") * speed;

        spawnWalls();
    }
    void spawnWalls()
    {
        if (Input.GetAxis("Vertical") >= 1)
        {
            GameObject wall = Instantiate(wallPrefab, transform.position + transform.forward * -1.5f, transform.rotation);
            walls.Add(wall);

        }

        if(walls.Count > wallsRange)
        {
            Destroy(walls[0].gameObject);
            walls.Remove(walls[0]);

        }

    }
}