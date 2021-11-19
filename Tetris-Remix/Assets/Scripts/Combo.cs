using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Combo
{
    public abstract List<Combo> PreActivate(Grid grid, int row, int col);
    // returns other combos that get activated when this combo is activated
    
    public abstract bool PostActivate(Grid grid, List<Combo> combos);
    //gets called in update return false when done
    
    public abstract void InstantiateCellLabel(Transform cellBlock);
    public abstract void SetUIText(Text text);
    public virtual void Cancel() { }
}

public class ExplosiveCombo : Combo
{
    static GameObject labelObject = Resources.Load("Prefabs/Grenade") as GameObject;
    List<GridCell> cellsToDestroy;
    bool isDone = true;

    public override List<Combo> PreActivate(Grid grid, int row, int col)
    {
        var combos = new List<Combo>();
        PreActivateUtil(combos, grid, row, col);
        return combos;
    }
    public void PreActivateUtil(List<Combo> combos, Grid grid, int row, int col)
    {
        if(!isDone) return;
        combos.Add(this);
        isDone = false;
        cellsToDestroy = new List<GridCell>(9);
        
        var (fromRow, toRow, fromCol, toCol) = CalcBlastRange(grid, row, col);

        for (int i = fromRow; i <= toRow; i++)
        {
            for(int j = fromCol; j <= toCol; j++)
                if(grid.IsFilled(i, j))
                {
                    var cellCombo = grid[i][j].GetCombo();
                    if(cellCombo != null && cellCombo is ExplosiveCombo && (i!=row || j!=col))
                        ((ExplosiveCombo)cellCombo).PreActivateUtil(combos, grid, i, j);
                    else
                        cellsToDestroy.Add(grid[i][j]);
                }
        }
    }

    public override bool PostActivate(Grid grid, List<Combo> combos)
    {
        if(isDone) return false;

        if(cellsToDestroy.Count > 0)
        {
            for(int i = 0; i < cellsToDestroy.Count; i++)
                if(cellsToDestroy[i].Destroy())
                {
                    var col = cellsToDestroy[i].x;
                    var row = grid.Height - cellsToDestroy[i].y;
                    grid.SubsideColumn(col, row - 1, 1);
                }
            grid.Show();
        }

        isDone = true;
        return true;
    }

    (int, int, int, int) CalcBlastRange(Grid grid, int row, int col)
    {
        int fromRow = row - 1; if(fromRow < 0) fromRow = 0;
        int fromCol = col - 1; if(fromCol < 0) fromCol = 0;
        int toRow = row + 1; if(toRow >= grid.Height) toRow = grid.Height - 1;
        int toCol = col + 1; if(toCol >= grid.Width) toCol = grid.Width - 1;
        return (fromRow, toRow, fromCol, toCol);
    }

    public override void InstantiateCellLabel(Transform cellBlock)
    {
        GameObject.Instantiate(labelObject, cellBlock);
    }

    public override void SetUIText(Text text)
    {
        return;
    }
}
