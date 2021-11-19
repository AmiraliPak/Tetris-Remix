using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class ShootingCombo : Combo
{
    static GameObject defaultTargetObject = Resources.Load("Prefabs/Target") as GameObject;
    static Text textUI;
    public override void InstantiateCellLabel(Transform cellBlock) { }
    protected static int totalAmmoCount=0;
    protected static int initialAmmoCount;
    static int combosCount = 0;
    bool isDone = true;
    static bool activated = false;
    GameObject gun, target;
    GridCell targetCell;


    public ShootingCombo(int ammoCount)
    {
        initialAmmoCount = ammoCount;
    }

    public override sealed List<Combo> PreActivate(Grid grid, int row, int col)
    {
        activated = false;
        isDone = false;
        combosCount++;
        totalAmmoCount += initialAmmoCount;
        return new List<Combo>() { this };
    }

    public override sealed bool PostActivate(Grid grid, List<Combo> combos)
    {
        if(isDone) return false;

        if(combosCount <= 1) // last shooting combo enters here
        {
            if(!activated) // called once
            {
                activated = true;
                combosCount = 0;
                if(!InitializeCombo(grid))
                {
                    isDone = true;
                    return false;
                }
            }
            Update(grid, combos);
        }
        else // other shooting combos but last enter here
        {
            combosCount--;
            isDone = true;
        }
        return true;
    }

    bool InitializeCombo(Grid grid)
    {
        targetCell = grid.GetHighestCell();
        if(targetCell == null)
            return false;

        target = InstantiateTarget(targetCell.transform);
        gun = InstantiateGun();
        return true;
    }

    protected virtual void Update(Grid grid, List<Combo> combos)
    {
        if(totalAmmoCount == 0 || targetCell == null)
        {
            isDone = true;
            totalAmmoCount = 0;
            GameObject.Destroy(target);
            GameObject.Destroy(gun);
            return;
        }

        var targetRow = grid.Height - targetCell.y;
        var targetCol = targetCell.x;
        GridCell nextCell = null;
        if(Input.GetKeyDown(KeyCode.D))
        {
            nextCell = grid.FindClosestCell(
                targetRow, targetCol,
                (row, col) => col >= -row + (targetCol + targetRow) && col >= row + (targetCol - targetRow)
                );
        }
        else if(Input.GetKeyDown(KeyCode.A))
        {
            nextCell = grid.FindClosestCell(
                targetRow, targetCol,
                (row, col) => col <= -row + (targetCol + targetRow) && col <= row + (targetCol - targetRow)
                );
        }
        else if(Input.GetKeyDown(KeyCode.W))
        {
            nextCell = grid.FindClosestCell(
                targetRow, targetCol,
                (row, col) => row <= -col + (targetCol + targetRow) && row <= col + (targetRow - targetCol)
                );
        }
        else if(Input.GetKeyDown(KeyCode.S))
        {
            nextCell = grid.FindClosestCell(
                targetRow, targetCol,
                (row, col) => row >= -col + (targetCol + targetRow) && row >= col + (targetRow - targetCol)
                );
        }
        else if(Input.GetKeyDown(KeyCode.Space))
        {
            // start shoot coroutine
            // do next lines on bullet impact
            totalAmmoCount--;
            SetUIText(textUI);
            combos.AddRange(DestroyCell(targetCell, grid));
            grid.Show();

            if(grid.IsFilled(targetRow, targetCol))
                nextCell = grid[targetRow][targetCol];
            else nextCell = grid.FindClosestCell(
                targetRow, targetCol, (row, col) => true
                );
            if(nextCell == null) targetCell = null;
        }


        if(target == null)
        {
            nextCell = grid.FindClosestCell(
                targetRow, targetCol, (row, col) => true
            );
            if(nextCell != null) target = InstantiateTarget(targetCell.transform);
        }
        if(nextCell != null)
        {
            targetCell = nextCell;
            target.transform.SetParent(targetCell.transform, false);
        }
    }

    protected abstract List<Combo> DestroyCell(GridCell cell, Grid grid); // returns preactivated combos to postactivate
    protected abstract GameObject InstantiateGun();
    protected virtual GameObject InstantiateTarget(Transform cell)
    {
        var obj = GameObject.Instantiate(defaultTargetObject, cell);
        return obj;
    }

    public override void SetUIText(Text text)
    {
        textUI = text;
        text.text = "Move the target and press Space to shoot.\nRemaining Shots: " + totalAmmoCount;
    }
}
