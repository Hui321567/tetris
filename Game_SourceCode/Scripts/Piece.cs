using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{   
    public Board board {  get; private set; }
    public  TetrominoData data {  get; private set; }
    public Vector3Int[] Cells { get; private set; }
    public Vector3Int position { get; private set; }

    public int RotationIndex { get; private set; }
    public float StepDelay = 1f;
    public float LockDelay = 0.5f;
    private float StepTime;
    private float LockTime;


    public void Initialize(Board board,Vector3Int postion,TetrominoData data)
    {
        this.board = board;
        this.position = postion;
        this.data = data;
        this.RotationIndex = 0;
        this.StepTime = Time.time + this.StepDelay;
        this.LockTime = 0f;

        if(this.Cells == null)
        {
            this.Cells = new Vector3Int[data.Cells.Length];
        }
        for(int i = 0; i < data.Cells.Length; i++)
        {
            
            this.Cells[i]=(Vector3Int)data.Cells[i];
            
        }
    }

    private void Update()
    {
        this.board.Clear(this);
        this.LockTime += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Rotate(-1);
            
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            Rotate(1);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            Move(Vector2Int.left);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            Move(Vector2Int.right);
        }
        else if(Input.GetKeyDown(KeyCode.S))
        {
            Move(Vector2Int.down);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            HardDrop();
        }
        if(Time.time >= this.StepTime)
        {
            Step();
        }

        this.board.Set(this);
    }

    private void Step()
    {
        this.StepTime = Time.time + this.StepDelay;
        Move(Vector2Int.down);
        if (this.LockTime >= this.LockDelay)
        {
            Lock () ;
        }
    }

    private void HardDrop()
    {
        while (Move(Vector2Int.down))
        {
            continue;
        }
        Lock ();
    }

    private void Lock()
    {
        this.board.Set(this);
        this.board.ClearLines();
        this.board.SpawnPiece();
        
    }
    private bool Move(Vector2Int translation)
    {
        Vector3Int newPosition = this.position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;

        bool valid = this.board.IsValidPosition(this,newPosition);

        if (valid)
        { 
            this.position = newPosition;
            this.LockTime = 0f;
        }
        return valid;
    }
    private void Rotate(int direction)
    {   
        int OriginalRotation = this.RotationIndex;
        this.RotationIndex = Wrap(this.RotationIndex + direction,0,4);

        ApplyRotationMatrix(direction);

        if (!TestWallKicks(this.RotationIndex,direction))
        {
            this.RotationIndex = OriginalRotation;
            ApplyRotationMatrix(-direction);
        }
    }

    private void ApplyRotationMatrix(int direction)
    {
        for (int i = 0; i < this.Cells.Length; i++)
        {
            Vector3 cell = this.Cells[i];
            int x, y;
            switch (this.data.tetromino)
            {
                case Tetromino.I:
                case Tetromino.O:
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;
                default:
                    x = Mathf.RoundToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.RoundToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;
            }
            this.Cells[i] = new Vector3Int(x, y, 0);
        }
    }

    private bool TestWallKicks(int RotationIndex , int RotationDirection)
    {
        int WallKickIndex = GetWallKickIndex(RotationIndex, RotationDirection);

        for (int i = 0; i < this.data.wallKicks.GetLength(1); i++)
        {
            Vector2Int translation = this.data.wallKicks[WallKickIndex, i];

            if (Move(translation))
            {
                return true;
            }
        }
        return false;
        
    }

    private int GetWallKickIndex(int RotationIndex, int RotationDirection)
    {
        int WallKickIndex = RotationIndex * 2;
        if(RotationDirection < 0 )
        {
            WallKickIndex --;
        }
        
        return Wrap(WallKickIndex,0,this.data.wallKicks.GetLength(0));
        
    }

    private int Wrap(int input, int min, int max)
    {
        if (input < min)
        {
            return max - (min - input) % (max - min);
        }
        else
        {
            return min + (input - min) % (max - min);
        }
    }

   
}
