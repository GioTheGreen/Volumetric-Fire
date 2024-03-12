using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class sc_Fuid_Flow : MonoBehaviour
{
    public int weidth = 10;
    public int height = 10;
    public Vector3[] cnt;
    public float cnt_temp = 100;

    private struct Cell
    {
        public Vector3 pos;
        public Vector3 dir;
        public float d_Up;//y
        public float d_Down;
        public float d_Left;//x
        public float d_Right;
        public float d_Front;//z
        public float d_Back;

        public float temp;
        public bool constent;

        public void dirCal()
        {
            d_Up = dir.y;
            d_Right = dir.x;
            d_Front = dir.z;
        }
    }
    private Cell[] cells;



    void Start()
    {
        Vector3 startPos = transform.position + new Vector3(-weidth/2,-height/2, 0);
        cells = new Cell[weidth*height];
        for (int i = 0; i < weidth; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Cell c = new Cell();
                c.pos = startPos + new Vector3(i,j, 0);
                c.dir = Vector3.zero;
                if (cnt.Contains(new Vector3(i,j,0)))
                {
                    c.temp = 100;
                    c.constent = true;
                }
                else
                {
                    c.temp = 0;
                    c.constent = false;
                }
                cells[(i*height) + j] = c;
            }
        }
    }

    void Update()
    {
        Debug.DrawRay(transform.position, Vector3.up, Color.gray);
    }
}
