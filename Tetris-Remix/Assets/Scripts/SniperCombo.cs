using System.Collections.Generic;
using UnityEngine;

public class SniperCombo : ShootingCombo
{
    static GameObject labelObject = Resources.Load("Prefabs/SniperAmmoLabel") as GameObject;
    static GameObject gunObject = Resources.Load("Prefabs/Sniper") as GameObject;
    const int AMMO_COUNT = 5;

    public SniperCombo() : base(AMMO_COUNT) { }

    public override void InstantiateCellLabel(Transform cellBlock)
    {
        GameObject.Instantiate(labelObject, cellBlock);
    }

    protected override GameObject InstantiateGun()
    {
        return GameObject.Instantiate(gunObject);
    }

    protected override List<Combo> DestroyCell(GridCell cell, Grid grid)
    {
        var cellRow = grid.Height - cell.y;
        var cellCol = cell.x;
        var cellCombo = cell.GetCombo();

        List<Combo> combos = new List<Combo>();
        if(cellCombo == null || cellCombo is SniperCombo)
        {
            if(cellCombo is SniperCombo)
                totalAmmoCount += initialAmmoCount;
            cell.Destroy();
            grid.SubsideColumn(cellCol, cellRow-1, 1);
        }
        else
            combos = cellCombo.PreActivate(grid, cellRow, cellCol);
        return combos;
    }
}
