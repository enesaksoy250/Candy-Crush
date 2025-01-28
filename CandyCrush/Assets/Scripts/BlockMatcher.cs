using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BlockMatcher : MonoBehaviour
{
    private GameObject[,] board;
    private int columns;
    private int rows;

   
    public BlockMatcher(GameObject[,] board, int columns, int rows)
    {
        this.board = board;
        this.columns = columns;
        this.rows = rows;

    }
     
    public List<GameObject> FindMatchingBlocks(int startX, int startY) //Yan yana duran bloklarý tespit etme
    {
       
        List<GameObject> matches = new List<GameObject>(); // eþleþen bloklar bunun içinde tutulucak
        GameObject startBlock = board[startX, startY];
        if (startBlock == null) return matches;
        
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(new Vector2Int(startX, startY));

        while (queue.Count > 0)
        {
            Vector2Int pos = queue.Dequeue();
            if (!matches.Contains(board[pos.x, pos.y]))
            {
                matches.Add(board[pos.x, pos.y]);

                foreach (var neighbor in GetNeighbors(pos.x, pos.y))
                {
                    if (board[neighbor.x, neighbor.y] != null &&
                        board[neighbor.x, neighbor.y].name == startBlock.name &&
                        !matches.Contains(board[neighbor.x, neighbor.y]))
                    {
                        queue.Enqueue(neighbor);
                    }
                }
            }
        }
        return matches;
    }

    public List<Vector2Int> GetNeighbors(int x, int y) //seçilen bloðun dört bir tarafýný kontrol etme
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        if (x > 0) neighbors.Add(new Vector2Int(x - 1, y));
        if (x < columns - 1) neighbors.Add(new Vector2Int(x + 1, y));
        if (y > 0) neighbors.Add(new Vector2Int(x, y - 1));
        if (y < rows - 1) neighbors.Add(new Vector2Int(x, y + 1));

        return neighbors;
    }

}


