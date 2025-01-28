using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using System;

public class GameBoard : MonoBehaviour
{

    [Header("Game Settings")]
    public int rows = 10;
    public int columns = 12;
    public int colorCount = 6;

    public int smallGroupThreshold = 4;
    public int mediumGroupThreshold = 7;
    public int largeGroupThreshold = 10;

    [Header("Others")]
    [SerializeField] Transform boardParent;
    [SerializeField] SpriteData spriteData;
    [SerializeField] BlockPrefabs blockPrefabs;
    [SerializeField] ParticleSystem particlePrefab;

    private bool isCreateParticle=false;
    private ParticleSystem createdParticle;

    public GameObject[,] board;

    private BlockMatcher blockMatcher;
    private ShuffleManager shuffleManager;

    public static GameBoard instance;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        InitializeBoard();
        CameraAndUIHandler.instance.AdjustCameraAndCollider(rows, columns);
    }



    private void InitializeBoard()
    {

        board = new GameObject[columns, rows];

        for (int col = 0; col < columns; col++)
        {
            for (int row = 0; row < rows; row++)
            {
                int colorIndex = UnityEngine.Random.Range(0, colorCount);
                Vector3 position = new Vector3(col, -row, 0);
                GameObject block = Instantiate(blockPrefabs.blockPrefabs[colorIndex], position, Quaternion.identity, boardParent);
                board[col, row] = block;
            }
        }
       
        blockMatcher = new BlockMatcher(board, columns, rows);
        shuffleManager = new ShuffleManager(board, columns, rows, blockMatcher);
        InitializeIcons();

    }

    private void InitializeIcons()
    {

        for (int col = 0; col < columns; col++)
        {
            for (int row = 0; row < rows; row++)
            {
                if (board[col, row] != null)
                {
                    List<GameObject> matchingBlocks =blockMatcher.FindMatchingBlocks(col, row); 
                    if (matchingBlocks.Count >= 2)
                    {
                        UpdateIcons(matchingBlocks);
                    }
                }
            }
        }
    }

    public void OnBlockClicked(GameObject block)
    {
        Vector2Int position = GetBlockPosition(block);
        List<GameObject> matchingBlocks = blockMatcher.FindMatchingBlocks(position.x, position.y);

        if (matchingBlocks.Count >= 2)
        {

            foreach (var match in matchingBlocks)
            {
                Vector2Int matchPosition = GetBlockPosition(match);
                board[matchPosition.x, matchPosition.y] = null;
                Destroy(match);
            }

            CollapseBoard();
            FillEmptySpaces();
            InitializeIcons();
            shuffleManager.CheckAndShuffleBoard();

            if(matchingBlocks.Count >= smallGroupThreshold)
            {

                CreateParticleEffect(block);
             
            }          

        }
    }

    Vector2Int GetBlockPosition(GameObject block)
    {
        for (int col = 0; col < columns; col++)
        {
            for (int row = 0; row < rows; row++)
            {
                if (board[col, row] == block)
                {
                    return new Vector2Int(col, row);
                }

            }
        }
        return Vector2Int.zero;
    }

    private void CollapseBoard()
    {
        for (int col = 0; col < columns; col++)
        {
            int emptyRow = -1;
            for (int row = rows - 1; row >= 0; row--)
            {
                if (board[col, row] == null && emptyRow == -1)
                    emptyRow = row;

                if (board[col, row] != null && emptyRow != -1)
                {
                    board[col, emptyRow] = board[col, row];
                    board[col, row] = null;                 
                    emptyRow--;
                }
            }
        }
    }

    private void FillEmptySpaces()
    {
        for (int col = 0; col < columns; col++)
        {
            List<int> emptyRows = new List<int>();
            int highestFilledRow = -1;


            for (int row = rows - 1; row >= 0; row--)
            {
                if (board[col, row] == null)
                {
                    emptyRows.Add(row);
                }
                else if (highestFilledRow == -1)
                {
                    highestFilledRow = row;
                }
            }



            for (int i = 0; i < emptyRows.Count; i++)
            {

                Vector3 spawnPosition = new Vector3(col, 2.5f + i, 0);
                int colorIndex = UnityEngine.Random.Range(0, colorCount);
                GameObject newBlock = Instantiate(blockPrefabs.blockPrefabs[colorIndex], spawnPosition, Quaternion.identity, boardParent);
                int targetRow = emptyRows[i];
                board[col, targetRow] = newBlock;

            }
        }
    }

    private void UpdateIcons(List<GameObject> matchingBlocks)
    {

        int blockCount = matchingBlocks.Count;
        string blockColor = matchingBlocks[0].GetComponent<Block>().blockColor;
        Sprite sprite = GetSpriteForBlock(blockColor, blockCount);

        foreach (GameObject block in matchingBlocks)
        {
            Block blockScript = block.GetComponent<Block>();
            blockScript.SetSprite(sprite);
        }

    }

    private Sprite GetSpriteForBlock(string blockColor, int blockCount)
    {
        int spriteIndex = 0;


        if (blockCount < smallGroupThreshold)
            spriteIndex = 0;
        else if (blockCount >= smallGroupThreshold && blockCount < mediumGroupThreshold)
            spriteIndex = 1;
        else if (blockCount >= mediumGroupThreshold && blockCount < largeGroupThreshold)
            spriteIndex = 2;
        else if (blockCount >= largeGroupThreshold)
            spriteIndex = 3;


        return blockColor switch
        {
            "Red" => spriteData.redSprites[spriteIndex],
            "Blue" => spriteData.blueSprites[spriteIndex],
            "Green" => spriteData.greenSprites[spriteIndex],
            "Yellow" => spriteData.yellowSprites[spriteIndex],
            "Purple" => spriteData.purpleSprites[spriteIndex],
            "Pink" => spriteData.pinkSprites[spriteIndex],
            _ => null,
        };
    }

    private void CreateParticleEffect(GameObject block)
    {

        if (!isCreateParticle)
        {
            createdParticle = Instantiate(particlePrefab, block.transform.position, Quaternion.identity);
            isCreateParticle = true;
        }
        else
        {
            createdParticle.transform.position = block.transform.position;
            createdParticle.Play();
        }

    }
}
