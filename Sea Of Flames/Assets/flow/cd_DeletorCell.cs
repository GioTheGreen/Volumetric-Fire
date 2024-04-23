using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cd_DeletorCell : cd_BaseCell
{
    override public void inisate()
    {
        s = 1;
        temp = 0;
        p_count = 0; //maybe negative?
        EType type = EType.eDeletor;
    }
}
