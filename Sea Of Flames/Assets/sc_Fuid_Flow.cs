using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class sc_Fuid_Flow : MonoBehaviour
{
    public int weidth = 10;
    public int height = 10;
    public int depth = 1;
    public Vector3[] jets;
    public float jet_temp = 100;
    public Vector3[] walls;
    public int itterations = 3;
    private cd_BaseCell[,,] cells;
    private struct Flow
    {
        public Vector3 axis;
        public float power;
    }
    private Flow[] flows;
    private float dt;



    void Start()
    {
        cells = new cd_BaseCell[weidth,height,depth];
        flows = new Flow[((weidth+1) * (height) * (depth)) + ((weidth) * (height+1) * (depth)) + ((weidth) * (height) * (depth+1))];
        for (int i = 0; i < weidth + 2; i++)
        {
            for (int j = 0; j < height + 2; j++)
            {
                for (int k = 0; k < depth + 2; k++)
                {
                    if (jets.Contains<Vector3>(new Vector3 (i-1,j-1,k-1)))
                    {
                        cd_JetCell c = new cd_JetCell();
                        c.inisate();
                        cells[i,j,k] = c;
                    }
                    else if (i == 0 || j == 0 || k == 0 || i > weidth + 1 || j > height + 1 || k > depth + 1 || walls.Contains<Vector3>(new Vector3(i,j,k)))
                    {
                        cd_WallCell c = new cd_WallCell();
                        c.inisate();
                        cells[i,j,k] = c;
                    }
                    else // add detetor cells here later
                    {
                        cd_FluidCell c = new cd_FluidCell();
                        c.inisate();
                        cells[i, j, k] = c;
                    }

                }
            }
        }
    }

    void Update()
    {
        dt = Time.deltaTime;
        ModifyVelocity();
        MakeIncompressable();
        MoveVelocityFeild();

        Vector3 startPos = transform.position + new Vector3(-weidth / 2, -height / 2, -depth / 2);
        for (int i = 1; i < weidth+1; i++)//left-blue, right-red, front-green, back-yellow, up-black, down white
        {
            for (int j = 1; j < height+1; j++) 
            {
                for (int k = 1; k < depth+1; k++)
                {
                    Debug.DrawRay(startPos + new Vector3(i, j, k) + (new Vector3(-.5f, 0, 0)), Vector3.left /4, Color.blue);
                    Debug.DrawRay(startPos + new Vector3(i, j, k) + (new Vector3(.5f, 0, 0)), Vector3.right / 4, Color.red);
                    Debug.DrawRay(startPos + new Vector3(i, j, k) + (new Vector3(0, 0, .5f)), Vector3.forward / 4, Color.green);
                    Debug.DrawRay(startPos + new Vector3(i, j, k) + (new Vector3(0, 0, -.5f)), Vector3.back / 4, Color.yellow);
                    Debug.DrawRay(startPos + new Vector3(i, j, k) + (new Vector3(0, .5f, 0)), Vector3.up / 4, Color.black);
                    Debug.DrawRay(startPos + new Vector3(i, j, k) + (new Vector3(0, -.5f, 0)), Vector3.down / 4, Color.white);
                }
            }
            
        }
        
    }

    public void ModifyVelocity()
    {
        foreach (cd_BaseCell cell in cells)
        {
            //g = -9.81
            //v = v + dt* g
            //temp calc
        }
    }

    public void MakeIncompressable()
    {
        foreach(cd_BaseCell cell in cells)
        {
            // get divergence
        }
        for (int i = 0; i < itterations; i++)
        {
            //fix divergense
        }
    }

    public void MoveVelocityFeild()
    {

    }
}
