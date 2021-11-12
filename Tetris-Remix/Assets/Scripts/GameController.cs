using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public float InitialMoveRate;
    float fallRate;
    const int GRID_HEIGHT = 20;
    const int GRID_WIDTH = 10;
    Grid grid;
    GameBlock fallingBlock;
    (int, int) fallingBlockPosition;
    IEnumerator rightCoroutine, leftCoroutine;

    void Start()
    {
        grid = new Grid(GRID_HEIGHT, GRID_WIDTH);
        fallRate = InitialMoveRate;
        DropNextBlock();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            if (TryRotateFallingBlock())
                grid.Show();
        
        HandleMovement();
    }

    void HandleMovement()
    {
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            rightCoroutine = Movement(0, 1, 10 * InitialMoveRate);
            StartCoroutine(rightCoroutine);
        }
        if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow))
        {
            StopCoroutine(rightCoroutine);
            rightCoroutine = null;
        }

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            leftCoroutine = Movement(0, -1, 10 * InitialMoveRate);
            StartCoroutine(leftCoroutine);
        }
        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow))
        {
            StopCoroutine(leftCoroutine);
            leftCoroutine = null;
        }

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            fallRate *= 10f;
        if (Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.DownArrow))
            fallRate /= 10f;
    }

    IEnumerator Movement(int v1, int v2, float customRate = 0)
    {
        bool downward = (v1 == 1 && v2 == 0);
        if(downward)
        {
            grid.Show();
            yield return new WaitForSeconds(1f/(customRate==0 ? fallRate : customRate));
        }
        while (true)
        {
            if(!TryMoveFallingBlock(v1, v2))
            {
                if(downward) // block is placed
                {
                    grid.RemoveFullRows(fallingBlockPosition.Item1, fallingBlockPosition.Item1 + fallingBlock.GetLength(0));
                    grid.Show();
                    grid.LogGrid();
                    DropNextBlock();
                }
                break;
            }
            grid.Show();
            yield return new WaitForSeconds(1f/(customRate==0 ? fallRate : customRate));
        }
    }

    void DropNextBlock()
    {
        fallRate += 0.1f;
        fallingBlock = GameBlock.GetRandomBlock();
        fallingBlockPosition = (0, GRID_WIDTH / 2);
        if(grid.TryPlaceBlock(fallingBlock, fallingBlockPosition.Item1, fallingBlockPosition.Item2))
            StartCoroutine(Movement(1, 0)); // stop previous routine?
        else
            EndGame();
    }

    void EndGame()
    {
        grid.Show();
        Debug.Log("GAME OVER!");
        Time.timeScale = 0;
    }

    bool TryMoveFallingBlock(int addRow, int addCol)
    {
        var newRow = fallingBlockPosition.Item1 + addRow;
        var newCol = fallingBlockPosition.Item2 + addCol;
        if(grid.TryMoveBlock(fallingBlock, newRow, newCol))
        {
            fallingBlockPosition = (newRow, newCol);
            return true;
        }
        return false;
    }

    bool TryRotateFallingBlock()
    {
        var (row, col) = fallingBlockPosition;
        grid.RemoveBlock(fallingBlock);
        fallingBlock.Rotate();

        if(!grid.TryPlaceBlock(fallingBlock, row, col))
        {
            fallingBlock.Rotate(toLeft: true);
            Debug.Assert(grid.TryPlaceBlock(fallingBlock, row, col));
            return false;
        }
        fallingBlockPosition = (row, col);
        return true;
    }
}
