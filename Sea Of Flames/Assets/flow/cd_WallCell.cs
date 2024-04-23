using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cd_WallCell : cd_BaseCell
{
    override public void inisate()
    {
        s = 0;
        temp = 0;
        p_count = 100;
        EType type = EType.eWall;
    }
}
