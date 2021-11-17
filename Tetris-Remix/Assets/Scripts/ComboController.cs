using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboController : MonoBehaviour
{
    Grid grid;
    List<Combo> combos;
    public bool PostActivate { get; private set; } = false;

    void Update()
    {
        if(PostActivate)
        {
            bool isDone = true;
            for(int i = 0; i < combos.Count; i++)
                if(combos[i].PostActivate(grid))
                    isDone = false;
            if(isDone) PostActivate = false;
            grid.Show();
        }
    }

    public void SetGrid(Grid grid) => this.grid = grid;

    public void ManageFullRows(List<int> rows)
    {
        combos = new List<Combo>();
        for (int i = 0; i < rows.Count; i++)
        {
            var row = rows[i];
            for (int col = 0; col < grid.Width; col++)
            {
                var combo = grid[row][col].GetCombo();
                if (combo != null)
                    combos.AddRange(combo.PreActivate(grid, row, col));
            }
        }
        RemoveFullRows(rows);
        grid.Show(); //also updates gridcells x and y

        if(combos.Count > 0)
            PostActivate = true;
    }

    void RemoveFullRows(List<int> rows)
    {
        for (int i = 0; i < rows.Count; i++)
            grid.RemoveRow(rows[i]);
    }
}
