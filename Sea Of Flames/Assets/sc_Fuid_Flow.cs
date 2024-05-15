using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class sc_Fuid_Flow : MonoBehaviour
{
    public int weidth = 10;//move it move it
    public int height = 10;
    public int depth = 1;
    public Vector3[] jets;
    public float jet_temp = 100;
    public Vector3[] walls;
    public Vector3[] removers;
    public bool useItterations = true;
    public int itterations = 3;
    public bool overrelaxation = true;
    public float overrelaxationCount = 1.9f; // 1 < o < 2,    the bigger the better
    public cd_BaseCell[,,] cells;
    public struct Flow
    {
        public Vector3 axis;
        public float power;
        public bool Display;
    }
    public Flow[] flows; // staggered grid
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
        flows = new Flow[(((weidth+3) * (height + 2) * (depth + 2)) + ((weidth + 2) * (height+3) * (depth + 2)) + ((weidth + 2) * (height + 2) * (depth+3)))];
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
                    else if (removers.Contains<Vector3>(new Vector3(i-1,j-1,k-1)))
                    {
                        cd_DeletorCell c = new cd_DeletorCell();
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
        for (int i = 1; i < weidth + 1; i++) // decided to loop twice just to provent future problems
        {
            for (int j = 1; j < height + 1; j++)
            {
                for (int k = 1; k < depth + 1; k++)
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
        for (int i = 0; i < weidth + 2; i++)
        {
            for (int j = 0; j < height + 2; j++)
            {
                for (int k = 0; k < depth + 2; k++)
                {
                    if (cells[i,j,k].type == cd_BaseCell.EType.eJet)
                    {
                        flows[cells[i+1, j+1, k+1].flowIndex[3]].power = 1;
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
        //MoveVelocityFeild();

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
                            //int count = 0;
                            //for (int p = 0; p < 6; p++)
                            //{
                            //    if (cells[i, j, k].addedFlow[p])
                            //    {
                            //        line += flows[cells[i, j, k].flowIndex[p]].axis * flows[cells[i, j, k].flowIndex[p]].power;
                            //        count++;
                            //    }
                            //}
                            //line /= count;
                            line = new Vector3(flows[cells[i, j, k].flowIndex[1]].power - flows[cells[i, j, k].flowIndex[0]].power,
                        flows[cells[i, j, k].flowIndex[3]].power - flows[cells[i, j, k].flowIndex[2]].power,
                        flows[cells[i, j, k].flowIndex[5]].power - flows[cells[i, j, k].flowIndex[4]].power);
                            Debug.DrawRay(startPos + new Vector3(i, j, k), line, Color.blue);
                            //Gizmos.DrawSphere(startPos + new Vector3(i, j, k), 1);
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
        for (int i = 1; i < weidth + 1; i++)
        {
            for (int j = 1; j < height + 1; j++)
            {
                for (int k = 1; k < depth + 1; k++)//order: left right down up front back
                {
                    if (cells[i, j, k].s == 1)
                    {
                        if (cells[i,j,k].type == cd_BaseCell.EType.eJet)
                        {
                            float g = 5f;
                            if (cells[i, j - 1, k].s == 1)
                            {
                                flows[cells[i, j, k].flowIndex[2]].power += g * dt;
                            }
                            if (cells[i, j + 1, k].s == 1)
                            {
                                flows[cells[i, j, k].flowIndex[3]].power += g * dt;
                            }
                        }
                        else if (cells[i, j, k].type == cd_BaseCell.EType.eDeletor)
                        {
                            float g = 1f;
                            if (cells[i-1, j, k].s == 1)
                            {
                                flows[cells[i, j, k].flowIndex[0]].power -= g * dt;
                            }
                            if (cells[i+1, j, k].s == 1)
                            {
                                flows[cells[i, j, k].flowIndex[1]].power -= g * dt;
                            }
                            if (cells[i, j - 1, k].s == 1)
                            {
                                flows[cells[i, j, k].flowIndex[2]].power -= g * dt;
                            }
                            if (cells[i, j + 1, k].s == 1)
                            {
                                flows[cells[i, j, k].flowIndex[3]].power -= g * dt;
                            }
                            if (cells[i, j, k-1].s == 1)
                            {
                                flows[cells[i, j, k].flowIndex[4]].power -= g * dt;
                            }
                            if (cells[i, j, k+1].s == 1)
                            {
                                flows[cells[i, j, k].flowIndex[5]].power -= g * dt;
                            }
                        }
                        else
                        {
                            float g = -0.981f;
                            if (cells[i, j - 1, k].s == 1)
                            {
                                flows[cells[i, j, k].flowIndex[2]].power += g * dt;
                            }

                        }
                    }
                }
            }
        }
    }

    public void MakeIncompressable()
    {
        if (useItterations)
        {
            for (int t = 0; t < itterations; t++)
            {
                for (int i = 1; i < weidth + 1; i++)
                {
                    for (int j = 1; j < height + 1; j++)
                    {
                        for (int k = 1; k < depth + 1; k++)//order: left right down up front back
                        {
                            if (cells[i, j, k].s == 1)
                            {
                                cells[i, j, k].divCount = 0;
                                cells[i, j, k].d = 0;

                                cells[i, j, k].d -= flows[cells[i, j, k].flowIndex[0]].power;
                                cells[i, j, k].d += flows[cells[i, j, k].flowIndex[1]].power;
                                cells[i, j, k].d -= flows[cells[i, j, k].flowIndex[2]].power;
                                cells[i, j, k].d += flows[cells[i, j, k].flowIndex[3]].power;
                                cells[i, j, k].d -= flows[cells[i, j, k].flowIndex[4]].power;
                                cells[i, j, k].d += flows[cells[i, j, k].flowIndex[5]].power;

                                if (cells[i - 1, j, k].s == 1)
                                {
                                    cells[i, j, k].divCount++;
                                }
                                if (cells[i + 1, j, k].s == 1)
                                {
                                    cells[i, j, k].divCount++;
                                }
                                if (cells[i, j - 1, k].s == 1)
                                {
                                    cells[i, j, k].divCount++;
                                }
                                if (cells[i, j + 1, k].s == 1)
                                {
                                    cells[i, j, k].divCount++;
                                }
                                if (cells[i, j, k - 1].s == 1)
                                {
                                    cells[i, j, k].divCount++;
                                }
                                if (cells[i, j, k + 1].s == 1)
                                {
                                    cells[i, j, k].divCount++;
                                }

                                if (overrelaxation)
                                {
                                    cells[i, j, k].d *= overrelaxationCount;
                                }

                                if (cells[i, j, k].d != 0 || cells[i, j, k].divCount != 0)
                                {
                                    if (cells[i-1, j, k].s == 1)
                                    {
                                        flows[cells[i, j, k].flowIndex[0]].power += (cells[i, j, k].d * cells[i - 1, j, k].s / cells[i, j, k].divCount);
                                    }
                                    if (cells[i + 1, j, k].s == 1)
                                    {
                                        flows[cells[i, j, k].flowIndex[1]].power -= (cells[i, j, k].d * cells[i + 1, j, k].s / cells[i, j, k].divCount);
                                    }
                                    if (cells[i, j - 1, k].s == 1)
                                    {
                                        flows[cells[i, j, k].flowIndex[2]].power += (cells[i, j, k].d * cells[i, j - 1, k].s / cells[i, j, k].divCount);
                                    }
                                    if (cells[i, j + 1, k].s == 1)
                                    {
                                        flows[cells[i, j, k].flowIndex[3]].power -= (cells[i, j, k].d * cells[i, j + 1, k].s / cells[i, j, k].divCount);
                                    }
                                    if (cells[i, j, k - 1].s == 1)
                                    {
                                        flows[cells[i, j, k].flowIndex[4]].power += (cells[i, j, k].d * cells[i, j, k - 1].s / cells[i, j, k].divCount);
                                    }
                                    if (cells[i, j, k + 1].s == 1)
                                    {
                                        flows[cells[i, j, k].flowIndex[5]].power -= (cells[i, j, k].d * cells[i, j, k + 1].s / cells[i, j, k].divCount);
                                    }
                                }
                            }
                        }
                    }

                    //for (int i = 1; i < weidth + 1; i++)
                    //{
                    //    for (int j = 1; j < height + 1; j++)
                    //    {
                    //        for (int k = 1; k < depth + 1; k++)//order: left right down up front back
                    //        {
                    //            if (cells[i, j, k].s == 1)
                    //            {
                                    //if (cells[i, j, k].d != 0)
                                    //{
                                    //    flows[cells[i, j, k].flowIndex[0]].power -= (cells[i, j, k].d * cells[i - 1, j, k].s / cells[i, j, k].divCount);
                                    //    flows[cells[i, j, k].flowIndex[1]].power += (cells[i, j, k].d * cells[i + 1, j, k].s / cells[i, j, k].divCount);
                                    //    flows[cells[i, j, k].flowIndex[2]].power -= (cells[i, j, k].d * cells[i, j - 1, k].s / cells[i, j, k].divCount);
                                    //    flows[cells[i, j, k].flowIndex[3]].power += (cells[i, j, k].d * cells[i, j + 1, k].s / cells[i, j, k].divCount);
                                    //    flows[cells[i, j, k].flowIndex[4]].power -= (cells[i, j, k].d * cells[i, j, k - 1].s / cells[i, j, k].divCount);
                                    //    flows[cells[i, j, k].flowIndex[5]].power += (cells[i, j, k].d * cells[i, j, k + 1].s / cells[i, j, k].divCount);
                                    //}
                    //            }
                    //        }
                    //    }
                    //}
                }
            }
        }
        else
        {
            bool changed = true;
            while (changed)
            {
                for (int i = 1; i < weidth + 1; i++)
                {
                    for (int j = 1; j < height + 1; j++)
                    {
                        for (int k = 1; k < depth + 1; k++)//order: left right down up front back
                        {
                            if (cells[i, j, k].s == 1)
                            {
                                if (cells[i, j, k].d != 0)
                                {
                                    flows[cells[i, j, k].flowIndex[0]].power += (cells[i, j, k].d * cells[i - 1, j, k].s / cells[i, j, k].divCount);
                                    flows[cells[i, j, k].flowIndex[1]].power += (cells[i, j, k].d * cells[i + 1, j, k].s / cells[i, j, k].divCount);
                                    flows[cells[i, j, k].flowIndex[2]].power += (cells[i, j, k].d * cells[i, j - 1, k].s / cells[i, j, k].divCount);
                                    flows[cells[i, j, k].flowIndex[3]].power += (cells[i, j, k].d * cells[i, j + 1, k].s / cells[i, j, k].divCount);
                                    flows[cells[i, j, k].flowIndex[4]].power += (cells[i, j, k].d * cells[i, j, k - 1].s / cells[i, j, k].divCount);
                                    flows[cells[i, j, k].flowIndex[5]].power += (cells[i, j, k].d * cells[i, j, k + 1].s / cells[i, j, k].divCount);
                                }
                                else
                                {
                                    changed = false;
                                }
                            }
                        }
                    }
                }
            }
        }
        
    }

    public void MoveVelocityFeild()
    {
        Flow[] newflows = flows;
        Vector3 startPos = transform.position + new Vector3(-weidth / 2, -height / 2, -depth / 2);
        for (int i = 1; i < weidth + 1; i++)
        {
            for (int j = 1; j < height + 1; j++)
            {
                for (int k = 1; k < depth + 1; k++)//order: left right down up front back
                {
                    Vector3 velocity = new Vector3(flows[cells[i,j,k].flowIndex[1]].power - flows[cells[i, j, k].flowIndex[0]].power,
                        flows[cells[i, j, k].flowIndex[3]].power - flows[cells[i, j, k].flowIndex[2]].power,
                        flows[cells[i, j, k].flowIndex[5]].power - flows[cells[i, j, k].flowIndex[4]].power);
                    Vector3 PullLocation = startPos + new Vector3(i, j, k) - (velocity * dt);
                    float dx, dy, dz;
                    dx = PullLocation.x % 1;
                    dy = PullLocation.y % 1;
                    dz = PullLocation.z % 1;
                    int nx, ny, nz;
                    nx = (int)(PullLocation.x - dx);
                    ny = (int)(PullLocation.y - dy);
                    nz = (int)(PullLocation.z - dz);

                    Vector3 v000 = new Vector3(flows[cells[nx, ny, nz].flowIndex[1]].power - flows[cells[nx, ny, nz].flowIndex[0]].power,
                        flows[cells[nx, ny, nz].flowIndex[3]].power - flows[cells[nx, ny, nz].flowIndex[2]].power,
                        flows[cells[nx, ny, nz].flowIndex[5]].power - flows[cells[nx, ny, nz].flowIndex[4]].power);

                    Vector3 v100 = new Vector3(flows[cells[nx + 1, ny, nz].flowIndex[1]].power - flows[cells[nx + 1, ny, nz].flowIndex[0]].power, 0, 0); //why waist the processing power
                        //flows[cells[nx + 1, ny, nz].flowIndex[3]].power - flows[cells[nx + 1, ny, nz].flowIndex[2]].power,
                        //flows[cells[nx + 1, ny, nz].flowIndex[5]].power - flows[cells[nx + 1, ny, nz].flowIndex[4]].power);

                    Vector3 v010 = new Vector3(/*flows[cells[nx, ny+1, nz].flowIndex[1]].power - flows[cells[nx, ny+1, nz].flowIndex[0]].power*/0,
                        flows[cells[nx, ny+1, nz].flowIndex[3]].power - flows[cells[nx, ny+1, nz].flowIndex[2]].power,
                        /*flows[cells[nx, ny+1, nz].flowIndex[5]].power - flows[cells[nx, ny+1, nz].flowIndex[4]].power*/0);

                    Vector3 v001 = new Vector3(/*flows[cells[nx, ny, nz+1].flowIndex[1]].power - flows[cells[nx, ny, nz+1].flowIndex[0]].power*/0,
                        /*flows[cells[nx, ny, nz+1].flowIndex[3]].power - flows[cells[nx, ny, nz+1].flowIndex[2]].power*/0,
                        flows[cells[nx, ny, nz+1].flowIndex[5]].power - flows[cells[nx, ny, nz+1].flowIndex[4]].power);

                    Vector3 final = new Vector3((v000.x * dx) +(v100.x * (1-dx)), (v000.y * dy) + (v010.y * (1 - dy)), (v000.z * dz) + (v001.z * (1 - dz)));

                }
            }
        }
    }
}
