using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComboController : MonoBehaviour
{
    Grid grid;
    List<Combo> combos;
    Text UIText;
    public bool PostActivate { get; private set; } = false;


    void Start()
    {
        UIText = GetComponentInChildren<Text>();
    }
    void Update()
    {
        if(PostActivate)
        {
            bool isDone = true;
            for(int i = 0; i < combos.Count; i++)
                if(combos[i].PostActivate(grid, combos))
                {
                    combos[i].SetUIText(UIText);
                    isDone = false;
                }
            if(isDone){
                PostActivate = false;
                UIText.text = "";
            }
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

    public void SetRandomCombo(GameBlock block)
    {
        var rand = UnityEngine.Random.Range(1, 10);
        if(rand == 1 || rand == 5 || rand == 9)
            block.SetCombo(new ExplosiveCombo());
        else if(rand == 3 || rand == 7)
            block.SetCombo(new SniperCombo());
    }
}
