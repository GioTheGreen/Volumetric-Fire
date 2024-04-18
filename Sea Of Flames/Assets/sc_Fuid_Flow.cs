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
    public struct Flow
    {
        public Vector3 axis;
        public float power;
        public bool Display;
    }
    private Flow[] flows;
    private float dt;
    public enum EDiscplyType 
    {
        eNone,
        e6Flows,
        eMainFlow,
        eCellTypes
    }
    public EDiscplyType displayType = EDiscplyType.eNone;

    void Start()
    {
        cells = new cd_BaseCell[weidth+2,height+2,depth+2];
        flows = new Flow[(((weidth+3) * (height + 2) * (depth + 2)) + ((weidth + 2) * (height+3) * (depth + 2)) + ((weidth + 2) * (height + 2) * (depth+3))) / 2];
        Debug.Log("calc: " + flows.Length);
        for (int i = 0; i < weidth + 2; i++)
        {
            for (int j = 0; j < height + 2; j++)
            {
                for (int k = 0; k < depth + 2; k++)
                {
                    if (jets.Contains<Vector3>(new Vector3(i - 1, j - 1, k - 1)))
                    {
                        cd_JetCell c = new cd_JetCell();
                        c.inisate();
                        cells[i, j, k] = c;
                    }
                    else if (i == 0 || j == 0 || k == 0 || i > weidth + 1 || j > height + 1 || k > depth + 1 || walls.Contains<Vector3>(new Vector3(i - 1, j - 1, k - 1)))
                    {
                        cd_WallCell c = new cd_WallCell();
                        c.inisate();
                        cells[i, j, k] = c;
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

        int count = 0;
        for (int i = 0; i < weidth + 2; i++) // decided to loop twice just to provent future problems
        {
            for (int j = 0; j < height + 2; j++)
            {
                for (int k = 0; k < depth + 2; k++)
                {
                    // stitching the cells and the flows
                    if (cells[i, j, k].addedFlow.Contains<bool>(false)) //order: left right down up front back
                    {
                        if (!cells[i, j, k].addedFlow[0] && i > 0)
                        {
                            if (cells[i - 1, j, k].addedFlow[1])
                            {
                                cells[i, j, k].flowIndex[0] = cells[i - 1, j, k].flowIndex[1];
                            }
                            else
                            {
                                cells[i, j, k].flowIndex[0] = count;
                                flows[count].axis = Vector3.right;
                                count++;
                            }
                            cells[i, j, k].addedFlow[0] = true;
                        }
                        if (!cells[i, j, k].addedFlow[1] && i < weidth + 1)
                        {
                            if (cells[i + 1, j, k].addedFlow[0])
                            {
                                cells[i, j, k].flowIndex[1] = cells[i + 1, j, k].flowIndex[0];
                            }
                            else
                            {
                                cells[i, j, k].flowIndex[1] = count;
                                flows[count].axis = Vector3.right;
                                count++;
                            }
                            cells[i, j, k].addedFlow[1] = true;
                        }
                        if (!cells[i, j, k].addedFlow[2] && j > 0)
                        {
                            if (cells[i, j - 1, k].addedFlow[3])
                            {
                                cells[i, j, k].flowIndex[2] = cells[i, j -1 , k].flowIndex[3];
                            }
                            else
                            {
                                cells[i, j, k].flowIndex[2] = count;
                                flows[count].axis = Vector3.up;
                                count++;
                            }
                            cells[i, j, k].addedFlow[2] = true;
                        }
                        if (!cells[i, j, k].addedFlow[3] && j < height + 1)
                        {
                            if (cells[i, j + 1, k].addedFlow[2])
                            {
                                cells[i, j, k].flowIndex[3] = cells[i, j + 1, k].flowIndex[2];
                            }
                            else
                            {
                                cells[i, j, k].flowIndex[3] = count;
                                flows[count].axis = Vector3.up;
                                count++;
                            }
                            cells[i, j, k].addedFlow[3] = true;
                        }
                        if (!cells[i, j, k].addedFlow[4] && k > 0)
                        {
                            if (cells[i, j , k - 1].addedFlow[5])
                            {
                                cells[i, j, k].flowIndex[4] = cells[i, j , k-1].flowIndex[5];
                            }
                            else
                            {
                                cells[i, j, k].flowIndex[4] = count;
                                flows[count].axis = Vector3.forward;
                                count++;
                            }
                            cells[i, j, k].addedFlow[4] = true;
                        }
                        if (!cells[i, j, k].addedFlow[5] && k < depth + 1)
                        {
                            if (cells[i, j, k + 1].addedFlow[4])
                            {
                                cells[i, j, k].flowIndex[5] = cells[i, j, k + 1].flowIndex[4];
                            }
                            else
                            {
                                cells[i, j, k].flowIndex[5] = count;
                                flows[count].axis = Vector3.forward;
                                count++;
                            }
                            cells[i, j, k].addedFlow[5] = true;
                        }
                    }
                }
            }
        } //conacting the cells and flows
        Debug.Log("acutal: " + count);
    }

    void Update()
    {
        dt = Time.deltaTime;

        ModifyVelocity();
        MakeIncompressable();
        MoveVelocityFeild();

        Vector3 startPos = transform.position + new Vector3(-weidth / 2, -height / 2, -depth / 2);
        switch (displayType)
        {
            case EDiscplyType.eNone:
                //nothing... obviously
                break;
            case EDiscplyType.e6Flows:
                for (int i = 0; i < flows.Length; i++)
                {
                    flows[i].Display = false;
                }
                for (int i = 1; i < weidth+1; i++)//left-blue, right-red, front-green, back-yellow, up-black, down white
                {
                    for (int j = 1; j < height+1; j++) 
                    {
                        for (int k = 1; k < depth+1; k++)
                        {
                            for (int p = 0; p < 6; p++)
                            {
                                if (cells[i, j, k].addedFlow[p] && !flows[cells[i, j, k].flowIndex[p]].Display)
                                {
                                    Debug.DrawRay(startPos + new Vector3(i, j, k), flows[cells[i, j, k].flowIndex[p]].axis * .3f, Color.green);
                                    
                                    flows[cells[i, j, k].flowIndex[p]].Display = true;
                                }
                            }
                        }
                    }
                } //isnt approprate yet
                break;
            case EDiscplyType.eMainFlow:
                for (int i = 1; i < weidth + 1; i++)
                {
                    for (int j = 1; j < height + 1; j++)
                    {
                        for (int k = 1; k < depth + 1; k++)
                        {
                            Vector3 line = Vector3.zero;
                            int count = 0;
                            for (int p = 0; p < 6; p++)
                            {
                                if (cells[i, j, k].addedFlow[p])
                                {
                                    line += flows[cells[i, j, k].flowIndex[p]].axis * flows[cells[i, j, k].flowIndex[p]].power;
                                    count++;
                                }
                            }
                            line /= count;
                            Debug.DrawRay(startPos + new Vector3(i, j, k), line, Color.blue);
                        }
                    }
                }
                break;
            case EDiscplyType.eCellTypes:
                for (int i = 1; i < weidth + 1; i++)
                {
                    for (int j = 1; j < height + 1; j++)
                    {
                        for (int k = 1; k < depth + 1; k++)
                        {
                            switch (cells[i,j,k].type)
                            {
                                case cd_BaseCell.EType.eNone:
                                    Gizmos.color = Color.white;
                                    Gizmos.DrawCube(startPos + new Vector3(i, j, k), new Vector3(.3f,.3f,.3f));
                                    break;
                                case cd_BaseCell.EType.eFluid:
                                    Gizmos.color = Color.blue;
                                    Gizmos.DrawCube(startPos + new Vector3(i, j, k), new Vector3(.3f, .3f, .3f));
                                    break;
                                case cd_BaseCell.EType.eWall:
                                    Gizmos.color = Color.black;
                                    Gizmos.DrawCube(startPos + new Vector3(i, j, k), new Vector3(.3f, .3f, .3f));
                                    break;
                                case cd_BaseCell.EType.eJet:
                                    Gizmos.color = Color.cyan;
                                    Gizmos.DrawCube(startPos + new Vector3(i, j, k), new Vector3(.3f, .3f, .3f));
                                    break;
                                case cd_BaseCell.EType.eDeletor:
                                    Gizmos.color = Color.red;
                                    Gizmos.DrawCube(startPos + new Vector3(i, j, k), new Vector3(.3f, .3f, .3f));
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                } // draw cubes of diifern colour
                            break;
            default:
                break;
        }
        
        
    }

    public void ModifyVelocity()
    {
        //foreach (cd_BaseCell cell in cells)
        //{
        //    //g = -9.81
        //    //v = v + dt* g
        //    //temp calc
        //}
    }

    public void MakeIncompressable()
    {
        //foreach(cd_BaseCell cell in cells)
        //{
        //    // get divergence
        //}
        //for (int i = 0; i < itterations; i++)
        //{
        //    //fix divergense
        //}
    }

    public void MoveVelocityFeild()
    {

    }
}
