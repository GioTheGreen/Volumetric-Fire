using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cd_BaseCell : MonoBehaviour
{
    public int s;//1 for flow area, 0 for wall area
    public float d;//divergense 
    public float temp;
    public int[] flowAddress = new int[6];

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
