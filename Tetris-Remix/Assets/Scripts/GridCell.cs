using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell
{
    static GameObject gridObject = GameObject.Find("Grid");
    static GameObject gridCellObject = Resources.Load("Prefabs/GridCell") as GameObject;
    Combo combo;
    GameObject cellBlock;
    public Transform transform 
    {
        get 
        {
            if(cellBlock == null) return null;
            else return cellBlock.transform;
        } 
    }
    Color color;
    public int x { get; private set; }
    public int y { get; private set; }
    public GridCell(Color color)
    {
        this.color = color;
        cellBlock = null;
        x = y = -1;
    }

    public void SetCombo(Combo combo)
    {
        this.combo = combo;
    }
    public Combo GetCombo() => this.combo;

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
            cellBlock = GameObject.Instantiate(gridCellObject, gridObject.transform);
            cellBlock.GetComponent<Renderer>().material.color = color;
            if(combo != null) combo.InstantiateCellLabel(cellBlock.transform);
        }
        cellBlock.transform.position = new Vector3(x, y);
        this.x = x; this.y = y;
    }
    public bool Destroy()
    {
        if(cellBlock == null) return false;
        // GameObject.Destroy(cellBlock);
        cellBlock.SetActive(false);
        cellBlock = null;
        EventSystem.OnCellDestroy.Invoke();
        return true;
    }
}
