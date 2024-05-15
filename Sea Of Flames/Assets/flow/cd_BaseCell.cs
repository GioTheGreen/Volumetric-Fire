using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cd_BaseCell : MonoBehaviour
{
    public int s;//1 for flow area, 0 for wall area
    public float d;//divergense 
    public int divCount = 0;
    public float p_count;// partical count
    public float temp;
    public bool[] addedFlow = new bool[]{ false,false,false,false,false,false};
    public int[] flowIndex = new int[]{0,0,0,0,0,0 }; //order: left right down up front back

    public enum EType 
    {
        eNone,
        eFluid,
        eWall,
        eJet,
        eDeletor
    }
    public EType type = EType.eNone;
    public virtual void inisate()
    {
        s = 0;
        temp = 0;
        p_count = 0;
    }
}
