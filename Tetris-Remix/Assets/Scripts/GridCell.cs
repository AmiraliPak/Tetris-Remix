using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell
{
    static GameObject gridObject = GameObject.Find("Grid");
    GameObject cellBlock;
    Color color;
    int x, y;
    public GridCell(Color color)
    {
        this.color = color;
        cellBlock = null;
        x = y = -1;
    }

    public bool IsVisible() => cellBlock != null;
    public void Show(int y, int x)
    {
        if (IsVisible())
        {
            if(this.y==y && this.x==x)
                return;
        }
        else
        {
            cellBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cellBlock.transform.SetParent(gridObject.transform);
            cellBlock.GetComponent<Renderer>().material.color = color;
        }
        cellBlock.transform.position = new Vector3(x, y);
        this.x = x; this.y = y;
    }
    public void Destroy()
    {
        GameObject.Destroy(cellBlock);
        cellBlock = null;
    }
    ~GridCell()
    {
        Destroy();
    }
}
