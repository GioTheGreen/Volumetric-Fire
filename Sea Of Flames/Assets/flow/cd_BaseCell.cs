using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cd_BaseCell : MonoBehaviour
{
    public int s;//1 for flow area, 0 for wall area
    public float d;//divergense 
    public float temp;
    public bool[] addedFlow = new bool[6] { false,false,false,false,false,false};
    public int[] flowIndex = new int[6]; //order: left right down up front back

    public enum EType 
    {
        eNone,
        eFluid,
        eWall,
        eJet,
        eDeletor
    }
    EType type = EType.eNone;
    public virtual void inisate()
    {
        s = 0;
        temp = 0;
    }
}
