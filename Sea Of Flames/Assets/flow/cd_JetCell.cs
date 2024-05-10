using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cd_JetCell : cd_BaseCell
{
    public Vector3 dir;
    override public void inisate()
    {
        s = 1;
        temp = 100;
        p_count = 600;
        EType type = EType.eJet;
    }
    public void setDir(Vector3 d)
    {
        dir = d;
    }
}
