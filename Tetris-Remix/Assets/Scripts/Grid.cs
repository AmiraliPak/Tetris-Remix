using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    public int Width { get; }
    public int Height { get; }
    List<GridCell[]> grid;
    Dictionary<GameBlock, (int, int)> blockToPoint = new Dictionary<GameBlock, (int, int)>();

    public Grid(int height, int width)
    {
        Height = height;
        Width = width;
        grid = new List<GridCell[]>();
        for(int i = 0; i < height; i++)
            grid.Add(new GridCell[width]);                
    }

    public GridCell[] this[int row]
    {
        get { return grid[row]; }
        set { grid[row] = value; }
    }

    public void RemoveRow(int row)
    {
        for(int j=0 ; j < Width; j++)
            grid[row][j].Destroy();
        grid.RemoveAt(row);
        grid.Insert(0, new GridCell[Width]);
    }

    public bool IsFilled(int row, int col) => grid[row][col] != null;

    public bool TryPlaceBlock(GameBlock block, int row, int col)
    {
        if(blockToPoint.ContainsKey(block))
            throw new System.Exception("block already in grid. use TryMoveBlock instead");

        if (!CheckBlockBounds(block, row, col))
            return false;

        var points = block.ToList();
        for (int i = 0; i < points.Count; i++)
        {
            var (v1, v2) = points[i];
            if(IsFilled(row + v1, col + v2))
                return false;
        }

        for (int i = 0; i < points.Count; i++)
        {
            var (v1, v2) = points[i];
            grid[row + v1][col + v2] = block[v1, v2];
        }
        blockToPoint.Add(block, (row, col));
        return true;
    }

    public void RemoveBlock(GameBlock block)
    {
        if(!blockToPoint.ContainsKey(block))
            throw new System.Exception("block not in grid. cant remove");
        var (row,col) = blockToPoint[block];

        var points = block.ToList();
        for (int i = 0; i < points.Count; i++)
        {
            var (v1, v2) = points[i];
            grid[row + v1][col + v2] = null;
        }
        blockToPoint.Remove(block);
    } 

    public bool TryMoveBlock(GameBlock block, int toRow, int toCol)
    {
        if(!blockToPoint.ContainsKey(block))
            throw new System.Exception("block not in grid. use TryPlaceBlock instead");

        if (!CheckBlockBounds(block, toRow, toCol))
            return false;
        var (fromRow,fromCol) = blockToPoint[block];

        RemoveBlock(block);
        if(!TryPlaceBlock(block, toRow, toCol))
        {
            Debug.Assert(TryPlaceBlock(block, fromRow, fromCol));
            return false;
        }
        return true;
    }

    public bool CheckBlockBounds(GameBlock block, int toRow, int toCol)
    {
        return !(toRow < 0 || toCol < 0 || toRow + block.GetLength(0) > Height || toCol + block.GetLength(1) > Width);
    }

    public void LogGrid()
    {
        string log = "Grid:\n";
        for(int i = 0; i < Height; i++)
        {
            for(int j=0 ; j < Width; j++)
                log += (IsFilled(i, j)) ? '0' : '.';
            log += '\n';
        }
        Debug.Log(log);
    }

    public void Show()
    {
        for(int i = 0; i < Height; i++)
            for(int j=0 ; j < Width; j++)
                if(IsFilled(i, j))
                    grid[i][j].Show(Height - i, j);
    }

    public List<int> FindFullRows(int fromRow, int toRow)
    {
        var fullRows = new List<int>(toRow - fromRow + 1);
        if(toRow >= Height) toRow = Height - 1;
        for(int i = fromRow; i <= toRow; i++)
        {
            bool isFull = true;
            for(int j = 0; j < Width; j++)
                if(!IsFilled(i, j)){
                    isFull = false;
                    break;
                }
            if(isFull) fullRows.Add(i);
        }
        return fullRows;
    }

    public void DestroyCell(int row, int col)
    {
        grid[row][col].Destroy();
        grid[row][col] = null;
    }

    public void SubsideColumn(int col, int baseRow, int amount)
    {
        for(int row = baseRow; row >= 0; row--)
            grid[row + amount][col] = grid[row][col];
        
        for (int row = 0; row < amount; row++)
            grid[row][col] = null;
    }

    public GridCell GetHighestCell()
    {
        for(int row = 0; row < Height; row++)
            for(int col = 0; col < Width; col++)
                if(IsFilled(row, col))
                    return grid[row][col];
        return null;
    }

    public GridCell FindClosestCell(int row, int col, Func<int,int,bool> ValidatePoint)
    {
        var visited = new HashSet<(int, int)>();
        Queue<(int,int)> queue = new Queue<(int,int)>();
        queue.Enqueue((row, col));
        visited.Add((row, col));
        bool flag = false;
        while(queue.Count != 0)
        {
            (row, col) = queue.Dequeue();
            if(IsFilled(row, col) && flag)
                return grid[row][col];
            flag = true;

            if(row-1 >= 0 && ValidatePoint(row-1,col) && !visited.Contains((row-1,col)))
            {
                queue.Enqueue((row - 1, col));
                visited.Add((row-1, col));
            }
            if(row+1 < Height && ValidatePoint(row+1,col) && !visited.Contains((row+1,col)))
            {
                queue.Enqueue((row + 1, col));
                visited.Add((row+1, col));
            }
            if(col-1 >= 0 && ValidatePoint(row,col-1) && !visited.Contains((row,col-1)))
            {
                queue.Enqueue((row, col-1));
                visited.Add((row, col-1));
            }
            if(col+1 < Width && ValidatePoint(row,col+1) && !visited.Contains((row,col+1)))
            {
                queue.Enqueue((row, col+1));
                visited.Add((row, col+1));
            }
        }
        return null;
    }

    public void Destroy()
    {
        for(int i = 0; i < Height; i++)
            for(int j = 0 ; j < Width; j++)
            {
                if(IsFilled(i,j))
                    grid[i][j].Destroy();
            }
    }
}
