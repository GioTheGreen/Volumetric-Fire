using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cd_FluidCell : cd_BaseCell
{
    override public void inisate()
    {
        s = 1;
        temp = 0;
        EType type = EType.eFluid;
    }
}
