using System.Collections.Generic;
using UnityEngine;

public abstract class Combo
{
    public abstract List<Combo> PreActivate(Grid grid, int row, int col);
    // returns other combos that get activated when this combo is activated
    
    public abstract bool PostActivate(Grid grid);
    //gets called in update return false when done
    
    public abstract void InstantiateCellLabel(Transform cellBlock);
}

}
