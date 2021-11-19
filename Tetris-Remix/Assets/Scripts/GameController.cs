using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameObject gameOver;
    public Text scoreText;
    public float InitialMoveRate;
    [SerializeField] float fallRate;
    const int GRID_HEIGHT = 20;
    const int GRID_WIDTH = 10;
    Grid grid;
    GameBlock fallingBlock;
    (int, int) fallingBlockPosition;
    IEnumerator rightCoroutine, leftCoroutine;
    GameState state;
    GameState? savedState = null;
    bool fallRateReset = false;
    ComboController comboController;

    void Start()
    {
        scoreText.text = "0";
        EventSystem.OnCellDestroy.AddListener(() => scoreText.text = (int.Parse(scoreText.text) + 1).ToString());

        grid = new Grid(GRID_HEIGHT, GRID_WIDTH);

        comboController = GameObject.Find("ComboController").GetComponent<ComboController>();
        comboController.SetGrid(grid);

        fallRate = InitialMoveRate;
        state = GameState.DropingNextBlock;
    }
    void Update()
    {
        if(fallRate < InitialMoveRate) fallRate *= 10;

        switch (state)
        {
            case GameState.DropingNextBlock:
                DisableMovement();
                fallRate += 0.1f;
                var success = TryDropNextBlock();
                if (success)
                {
                    comboController.SetRandomCombo(fallingBlock);
                    StartCoroutine(Movement(1, 0));
                    state = GameState.BlockFalling;
                }
                else state = GameState.GameOver;
                break;

            case GameState.BlockFalling:
                HandleMovement();
                // state change in Movement Coroutine
                break;

            case GameState.BlockBeingPlaced:
                DisableMovement();
                var fullRows = 
                    grid.FindFullRows(
                        fallingBlockPosition.Item1,
                        fallingBlockPosition.Item1 + fallingBlock.GetLength(0)
                        );
                comboController.ManageFullRows(fullRows);
                state = GameState.ComboController;
                break;
            
            case GameState.ComboController:
                DisableMovement();
                if(!comboController.PostActivate)
                    state = GameState.DropingNextBlock;
                break;

            case GameState.GameOver:
                DisableMovement();
                EndGame();
                break;

            case GameState.Pause:
                DisableMovement();
                break;

            case GameState.Restart:
                DisableMovement();
                StopAllCoroutines();
                grid.Destroy();
                Start();
                break;
        }

        GlobalInput();
    }

    void GlobalInput()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(savedState == null)
            {
                savedState = state;
                state = GameState.Pause;
                Time.timeScale = 0;
            }
            else
            {
                state = savedState ?? GameState.Pause;
                savedState = null;
                Time.timeScale = 1;
            }
        }
    }
    void HandleMovement()
    {
        fallRateReset = false;
        // rotation
        if (Input.GetKeyDown(KeyCode.W))
            if (TryRotateFallingBlock())
                grid.Show();
        // move right
        if (Input.GetKeyDown(KeyCode.D))
        {
            rightCoroutine = Movement(0, 1, 10 * InitialMoveRate);
            StartCoroutine(rightCoroutine);
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            StopCoroutine(rightCoroutine);
            rightCoroutine = null;
        }
        // move left
        if (Input.GetKeyDown(KeyCode.A))
        {
            leftCoroutine = Movement(0, -1, 10 * InitialMoveRate);
            StartCoroutine(leftCoroutine);
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            StopCoroutine(leftCoroutine);
            leftCoroutine = null;
        }
        // fast drop
        if (Input.GetKeyDown(KeyCode.S))
            fallRate *= 10f;
        if (Input.GetKeyUp(KeyCode.S))
            fallRate /= 10f;
    }
    void DisableMovement()
    {
        if(rightCoroutine!=null) StopCoroutine(rightCoroutine);
        if(leftCoroutine!=null) StopCoroutine(leftCoroutine);
        if (!fallRateReset && (Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.DownArrow)))
        {
            fallRate /= 10f;
            fallRateReset = true;
        }
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
                    state = GameState.BlockBeingPlaced;
                break;
            }
            grid.Show();
            yield return new WaitForSeconds(1f/(customRate==0 ? fallRate : customRate));
        }
    }

    bool TryDropNextBlock()
    {
        fallingBlock = GameBlock.GetRandomBlock();
        fallingBlockPosition = (0, (GRID_WIDTH -1) / 2);
        return
            grid.TryPlaceBlock(fallingBlock, fallingBlockPosition.Item1, fallingBlockPosition.Item2);
    }

    void EndGame()
    {
        grid.Show();
        Debug.Log("GAME OVER!");
        Time.timeScale = 0;
        gameOver.SetActive(true);
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

    public void RestartGame()
    {
        state = GameState.Restart;
        
    }
}

public enum GameState
{
    BlockFalling, BlockBeingPlaced, DropingNextBlock, GameOver, Pause, ComboController, Restart
}
