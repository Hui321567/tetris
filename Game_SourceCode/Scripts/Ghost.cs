using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Ghost : MonoBehaviour
{
    public Tile tile;
    public Board board;
    public Piece trackingPiece;


    public Tilemap tilemap {  get; private set; }
    public Vector3Int[] Cells {  get; private set; }
    public Vector3Int position {  get; private set; }

    private void Awake()
    {
        this.tilemap = GetComponentInChildren<Tilemap>();
        this.Cells = new Vector3Int[4];
    }

    private void LateUpdate()
    {
        Clear();
        Copy();
        Drop();
        Set();
    }
    private void Clear()
    {
        for (int i = 0; i < this.Cells.Length; i++)
        {
            Vector3Int tilePosition = this.Cells[i] + this.position;
            this.tilemap.SetTile(tilePosition, null);
        }
    }
    private void Copy()
    {
        for (int i = 0; i < this.Cells.Length; i++)
        {
            this.Cells[i] = this.trackingPiece.Cells[i];
        }
    }
    private void Drop()
    {
        Vector3Int position = this.trackingPiece.position;

        int Current = position.y;
        int bottom = -this.board.BoardSize.y / 2 - 1;

        this.board.Clear(this.trackingPiece);

        for(int row = Current; row >= bottom; row--)
        {
            position.y = row;
            if(this.board.IsValidPosition(this.trackingPiece , position))
            {
                this.position = position;
            }
            else
            {
                break;
            }
        }
        this.board.Set(this.trackingPiece);
    }
    private void Set()
    {
        for (int i = 0; i < this.Cells.Length; i++)
        {
            Vector3Int tilePosition = this.Cells[i] + this.position;
            this.tilemap.SetTile(tilePosition, this.tile);
        }
    }
}
