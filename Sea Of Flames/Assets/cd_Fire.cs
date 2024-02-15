using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class cd_Fire : MonoBehaviour
{
    public Vector3 GridSize = Vector3.one;
    public float CubeScale = 1;


    public List<Vertex> Vertices = new List<Vertex>();
    private Mesh m;
    private MeshFilter mf;
    private SkinnedMeshRenderer Smr;

    void Start()
    {
        mf = GetComponent<MeshFilter>();
        Smr = GetComponent<SkinnedMeshRenderer>();
        m = new Mesh();
        Smr.sharedMesh = m;

        Vector3 startPos = transform.position + new Vector3(GridSize.x * - CubeScale, GridSize.y * CubeScale, GridSize.z * - CubeScale);

        for (int i = 0; i <= GridSize.x; i++)
        {
            for (int j = 0; j <= GridSize.y; j++)
            {
                for (int k = 0; k <= GridSize.z; k++)
                {
                    Vertex v = new Vertex();
                    v.position = startPos + new Vector3(i * CubeScale, j * -CubeScale, k * CubeScale);
                    v.tempresture = 0;
                    Vertices.Add(v);
                }
            }
        }
    }

    void Update()
    {
        //drawTriangle();
    }






    void drawTriangle()
    {
        //Vector3[] posArray = new Vector3[Vertices.Count];
        //int[] trianglesArray = new int[(int)((GridSize.x) * (GridSize.y) * (GridSize.z) * 2 * 3)];

        ////all points here
        //for (int i = 0; i < posArray.Length; i++)
        //{
        //    posArray[i] = Vertices[i].position;
        //}

        ////order
        //for (int i = 0; i < GridSize.x; i++)
        //{
        //    for (int j = 0; j < GridSize.y; j++)
        //    {
        //        for (int k = 0; k < GridSize.z; k++)
        //        {

        //        }
        //        int s = (i * (int)GridSize.y) + j;
        //        int p = ((i * ((int)GridSize.y - 1)) + j) * 2 * 3;

        //        trianglesArray[p] = s;
        //        trianglesArray[p + 1] = s + (int)GridSize.y;
        //        trianglesArray[p + 2] = s + 1;

        //        trianglesArray[p + 3] = s + 1;
        //        trianglesArray[p + 4] = s + (int)GridSize.y;
        //        trianglesArray[p + 5] = s + (int)GridSize.y + 1;

        //    }
        //}

        Vector3[] posArray = new Vector3[Vertices.Count];
        int[] trianglesArray = new int[6 * 2 * 3];

        for (int i = 0;i < posArray.Length; i++)
        {
            posArray[i] = Vertices[i].position;
        }

        for (int i = 0; i < GridSize.x; i++)
        {
            for (int j = 0; j < GridSize.y; j++)
            {
                for (int k = 0; k < GridSize.z; k++)
                {




                }
            }
        }
        
        //set
        m.vertices = posArray;
        m.triangles = trianglesArray;
    }
}
