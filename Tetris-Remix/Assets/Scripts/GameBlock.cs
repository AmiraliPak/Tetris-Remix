using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class GameBlock
{
    protected Color color;
    int rotationIndex = 0;
    protected List<GridCell[,]> rotations = new List<GridCell[,]>();

    public GridCell this[int row, int col]
    {
        get { return rotations[rotationIndex][row, col]; }
        set { rotations[rotationIndex][row, col] = value; }
    }
    public int GetLength(int dimension) => rotations[rotationIndex].GetLength(dimension);

    public void Rotate(bool toLeft = false)
    {
        if(!toLeft)
            rotationIndex = (rotationIndex + 1) % rotations.Count;
        else
            rotationIndex = (rotationIndex - 1 + rotations.Count) % rotations.Count;
    }

    public List<(int,int)> ToList()
    {
        var result = new List<(int,int)>();
        var grid = rotations[rotationIndex];
        for (int i = 0; i < grid.GetLength(0); i++)
            for(int j = 0; j < grid.GetLength(1); j++)
                if(grid[i, j] != null)
                    result.Add((i, j));
        return result;
    }

    static public GameBlock GetRandomBlock()
    {
        var rand = UnityEngine.Random.Range(1, 7);
        GameBlock block = null;
        switch(rand)
        {
            case 1: block = new IBlock(); break;
            case 2: block = new JBlock(); break;
            case 3: block = new LBlock(); break;
            case 4: block = new OBlock(); break;
            case 5: block = new SBlock(); break;
            case 6: block = new ZBlock(); break;
            case 7: block = new TBlock(); break;
        }
        return block;
    }
}

public class IBlock: GameBlock
{
    static Color cyan = new Color32(0, 255, 255, 255);
    public IBlock()
    {
        color = cyan;
        var cells = new GridCell[4];
        for (int i = 0; i < 4; i++) cells[i] = new GridCell(color);

        rotations.Add(new GridCell[,]
        {
            {cells[0]},
            {cells[1]},
            {cells[2]},
            {cells[3]}
        });
        rotations.Add(new GridCell[,]
        {
            {cells[3], cells[2], cells[1], cells[0]}
        });
        rotations.Add(new GridCell[,]
        {
            {cells[3]},
            {cells[2]},
            {cells[1]},
            {cells[0]}
        });
        rotations.Add(new GridCell[,]
        {
            {cells[0], cells[1], cells[2], cells[3]}
        });
    }
}

public class JBlock: GameBlock
{
    static Color blue = new Color32(0, 0, 255, 255);
    public JBlock()
    {
        color = blue;
        var cells = new GridCell[4];
        for (int i = 0; i < 4; i++) cells[i] = new GridCell(color);

        rotations.Add(new GridCell[,]
        {
            {null,       cells[3]},
            {null,       cells[2]},
            {cells[0],   cells[1]}
        });
        rotations.Add(new GridCell[,]
        {
            {cells[0], null,     null},
            {cells[1], cells[2], cells[3]}
        });
        rotations.Add(new GridCell[,]
        {
            {cells[1], cells[0]},
            {cells[2], null},
            {cells[3], null}
        });
        rotations.Add(new GridCell[,]
        {
            {cells[3], cells[2], cells[1]},
            {null,     null,     cells[0]}
        });
    }
}

public class LBlock: GameBlock
{
    static Color orange = new Color32(255, 127, 0, 255);
    public LBlock()
    {
        color = orange;
        var cells = new GridCell[4];
        for (int i = 0; i < 4; i++) cells[i] = new GridCell(color);

        rotations.Add(new GridCell[,]
        {
            {cells[3], null},
            {cells[2], null},
            {cells[1], cells[0]}
        });
        rotations.Add(new GridCell[,]
        {
            {cells[1], cells[2], cells[3]},
            {cells[0], null,     null}
        });
        rotations.Add(new GridCell[,]
        {
            {cells[0],   cells[1]},
            {null,       cells[2]},
            {null,       cells[3]}
        });
        rotations.Add(new GridCell[,]
        {
            {null,     null,     cells[0]},
            {cells[3], cells[2], cells[1]}
        });
    }
}

public class OBlock: GameBlock
{
    static Color yellow = new Color32(255, 255, 0, 255);
    public OBlock()
    {
        color = yellow;
        var cells = new GridCell[4];
        for (int i = 0; i < 4; i++) cells[i] = new GridCell(color);

        rotations.Add(new GridCell[,]
        {
            {cells[0], cells[1]},
            {cells[3], cells[2]}
        });
        rotations.Add(new GridCell[,]
        {
            {cells[3], cells[0]},
            {cells[2], cells[1]}
        });
        rotations.Add(new GridCell[,]
        {
            {cells[2], cells[3]},
            {cells[1], cells[0]}
        });
        rotations.Add(new GridCell[,]
        {
            {cells[1], cells[2]},
            {cells[0], cells[3]}
        });
    }
}

public class SBlock: GameBlock
{
    static Color green = new Color32(0, 255, 0, 255);
    public SBlock()
    {
        color = green;
        var cells = new GridCell[4];
        for (int i = 0; i < 4; i++) cells[i] = new GridCell(color);

        rotations.Add(new GridCell[,]
        {
            {null,     cells[1], cells[0]},
            {cells[3], cells[2], null}
        });
        rotations.Add(new GridCell[,]
        {
            {cells[3], null},
            {cells[2], cells[1]},
            {null,     cells[0]}
        });
        rotations.Add(new GridCell[,]
        {
            {null,     cells[2], cells[3]},
            {cells[0], cells[1], null}
        });
        rotations.Add(new GridCell[,]
        {
            {cells[0], null},
            {cells[1], cells[2]},
            {null,     cells[3]}
        });
    }
}

public class ZBlock: GameBlock
{
    static Color red = new Color32(255, 0, 0, 255);
    public ZBlock()
    {
        color = red;
        var cells = new GridCell[4];
        for (int i = 0; i < 4; i++) cells[i] = new GridCell(color);

        rotations.Add(new GridCell[,]
        {
            {cells[0], cells[1], null},
            {null,     cells[2], cells[3]}
        });
        rotations.Add(new GridCell[,]
        {
            {null,     cells[0]},
            {cells[2], cells[1]},
            {cells[3], null}
        });
        rotations.Add(new GridCell[,]
        {
            {cells[3], cells[2], null},
            {null,     cells[1], cells[0]}
        });
        rotations.Add(new GridCell[,]
        {
            {null,     cells[3]},
            {cells[1], cells[2]},
            {cells[0], null}
        });
    }
}

public class TBlock: GameBlock
{
    static Color purple = new Color32(128, 0, 128, 255);
    public TBlock()
    {
        color = purple;
        var cells = new GridCell[4];
        for (int i = 0; i < 4; i++) cells[i] = new GridCell(color);

        rotations.Add(new GridCell[,]
        {
            {cells[0], cells[1], cells[2]},
            {null,     cells[3], null}
        });
        rotations.Add(new GridCell[,]
        {
            {null,       cells[0]},
            {cells[3],   cells[1]},
            {null,       cells[2]}
        });
        rotations.Add(new GridCell[,]
        {
            {null,     cells[3], null},
            {cells[2], cells[1], cells[0]}
        });
        rotations.Add(new GridCell[,]
        {
            {cells[2], null},
            {cells[1], cells[3]},
            {cells[0], null}
        });
    }
}
