using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UIElements;

public class cd_Fire : MonoBehaviour
{
    public Vector3 GridSize = Vector3.one;
    public float CubeScale = 1;
    public float StepTime = 0.3f;
    public float RenderTime = 0.3f;
    public int chase = 50;
    [SerializeField] private int width = 30;
    [SerializeField] private int height = 10;

    [SerializeField] float resolution = 1;
    [SerializeField] private float heightTresshold = 0.5f;

    [SerializeField] bool visualizeNoise;

    public List<Vector3> Vertices = new List<Vector3>();
    public List<Vector3> StartOn = new List<Vector3>();

    private List<int> triangles = new List<int>();
    private float[,,] heights;

    private Mesh m;
    private MeshFilter mf;
    private SkinnedMeshRenderer Smr;

    private float dt;

    void Start()
    {
        mf = GetComponent<MeshFilter>();
        Smr = GetComponent<SkinnedMeshRenderer>();
        m = new Mesh();
        Smr.sharedMesh = m;

        StartCoroutine(TestAll());

        //Vector3 startPos = transform.position + new Vector3(GridSize.x * - CubeScale, GridSize.y * CubeScale, GridSize.z * - CubeScale);

        //for (int i = 0; i <= GridSize.x; i++)
        //{
        //    for (int j = 0; j <= GridSize.y; j++)
        //    {
        //        for (int k = 0; k <= GridSize.z; k++)
        //        {
        //            Vertex v = new Vertex();
        //            v.position = startPos + new Vector3(i * CubeScale, j * -CubeScale, k * CubeScale);
        //            v.tempresture = 0;
        //            Vertices.Add(v);
        //        }
        //    }
        //}
    }

    void Update() //managed to crash unity for the first time
    {
        dt += Time.deltaTime;
        if (dt > StepTime)
        {
            for (int i = 0; i < StartOn.Count; i++)
            {
                StartOn[i] = (StartOn[i] + new Vector3(0, 1, 0));
                if (StartOn[i].y > height)
                {
                    StartOn.RemoveAt(i);
                }
            }
            List<Vector3> ToAddOn = new List<Vector3>();
            for (int i = 1; i < width; i++)
            {
                for (int j = 1; j < width; j++)
                {
                    if (UnityEngine.Random.Range(0, 100) > chase)
                    {
                        ToAddOn.Add(new Vector3(i, 0, j));
                    }
                }
            }
            StartOn.AddRange(ToAddOn);
            dt = 0;
        }
    }

    private IEnumerator TestAll()
    {
        while (true)
        {
            SetHeights();
            MarchCubes();
            SetMesh();
            yield return new WaitForSeconds(RenderTime);
        }
    }

    private void SetMesh()
    {
        Mesh mesh = new Mesh();

        mesh.vertices = Vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        Smr.sharedMesh = mesh;
    }

    private void SetHeights()
    {
        heights = new float[width + 1, height + 1, width + 1];

        for (int x = 0; x < width + 1; x++)
        {
            for (int y = 0; y < height + 1; y++)
            {
                for (int z = 0; z < width + 1; z++)
                {
                    if (StartOn.Contains(new Vector3(x,y,z)))
                    {
                        heights[x, y, z] = 1.0f;
                    }
                    else
                    {
                        heights[x,y,z] = 0.0f;
                    }
                }
            }
        }
    }

    private int GetConfigIndex(float[] cubeCorners)
    {
        int configIndex = 0;

        for (int i = 0; i < 8; i++)
        {
            if (cubeCorners[i] > heightTresshold)
            {
                configIndex |= 1 << i;
            }
        }

        return configIndex;
    }

    private void MarchCubes()
    {
        Vertices.Clear();
        triangles.Clear();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < width; z++)
                {
                    float[] cubeCorners = new float[8];

                    for (int i = 0; i < 8; i++)
                    {
                        Vector3Int corner = new Vector3Int(x, y, z) + sc_MarchingTable.Corners[i];
                        cubeCorners[i] = heights[corner.x, corner.y, corner.z];
                    }

                    MarchCube(new Vector3(x, y, z), cubeCorners);
                }
            }
        }
    }

    private void MarchCube(Vector3 position, float[] cubeCorners)
    {
        int configIndex = GetConfigIndex(cubeCorners);

        if (configIndex == 0 || configIndex == 255)
        {
            return;
        }

        int edgeIndex = 0;

        for (int i = 0; i < 5; i++)
        {
            bool added = true;
            for (int j = 0; j < 3; j++)
            {
                int triTableValue = sc_MarchingTable.Triangles[configIndex, edgeIndex];

                if (triTableValue == -1)
                {
                    added = false;
                    return;
                }

                Vector3 edgeStart = position + sc_MarchingTable.Edges[triTableValue, 0];
                Vector3 edgeEnd = position + sc_MarchingTable.Edges[triTableValue, 1];

                Vector3 vertex = (edgeStart + edgeEnd) / 2;

                Vertices.Add(vertex);
                triangles.Add(Vertices.Count - 1);

                edgeIndex++;
            }
            //if (added)
            //{
            //    Vector3 temp = Vertices[Vertices.Count - 1];
            //    Vertices[Vertices.Count - 1] = Vertices[Vertices.Count];
            //    Vertices[Vertices.Count] = temp;

            //}
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!visualizeNoise)// || !Application.isPlaying)
        {
            return;
        }

        for (int x = 0; x < width + 1; x++)
        {
            for (int y = 0; y < height + 1; y++)
            {
                for (int z = 0; z < width + 1; z++)
                {
                    Gizmos.color = new Color(heights[x, y, z], heights[x, y, z], heights[x, y, z], 1);
                    Gizmos.DrawSphere(new Vector3(x * resolution, y * resolution, z * resolution), 0.2f * resolution);
                }
            }
        }
    }
}
