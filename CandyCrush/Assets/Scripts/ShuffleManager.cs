using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShuffleManager : MonoBehaviour
{

    private GameObject[,] board;
    private int columns;
    private int rows;
    private BlockMatcher blockMatcher;

    public ShuffleManager(GameObject[,] board,int columns,int rows,BlockMatcher blockMatcher)
    {

        this.board = board;
        this.columns = columns;
        this.rows = rows;
        this.blockMatcher = blockMatcher;

    }


    private void ShuffleBoard()
    {

        List<GameObject> allBlocks = new List<GameObject>();
        for (int col = 0; col < columns; col++)
        {
            for (int row = 0; row < rows; row++)
            {
                if (board[col, row] != null)
                {
                    allBlocks.Add(board[col, row]);
                    board[col, row] = null;
                }
            }
        }


        System.Random rng = new System.Random();
        int n = allBlocks.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            GameObject value = allBlocks[k];
            allBlocks[k] = allBlocks[n];
            allBlocks[n] = value;
        }


        int index = 0;
        for (int col = 0; col < columns; col++)
        {
            for (int row = 0; row < rows; row++)
            {
                if (index < allBlocks.Count)
                {
                    board[col, row] = allBlocks[index];
                    board[col, row].transform.position = new Vector3(col, -row, 0);
                    index++;
                }
            }
        }


        if (!HasMatchingBlocks())
        {
            ShuffleBoard();
        }

        else
        {
            CameraAndUIHandler.instance.ShowShuffleText();
        }
    }

    private bool HasMatchingBlocks()
    {
        for (int col = 0; col < columns; col++)
        {
            for (int row = 0; row < rows; row++)
            {
                if (board[col, row] != null)
                {

                    if (blockMatcher.FindMatchingBlocks(col, row).Count >= 2)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public void CheckAndShuffleBoard()
    {
        if (!HasMatchingBlocks())
        {
            ShuffleBoard();
        }
    }



}
