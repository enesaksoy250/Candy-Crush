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

    [Header("Sprites")]
    public Sprite[] redSprites;
    public Sprite[] blueSprites;
    public Sprite[] greenSprites;
    public Sprite[] yellowSprites;
    public Sprite[] purpleSprites;
    public Sprite[] pinkSprites;

    [Header("Others")]
    public GameObject[] blockPrefabs;
    public Transform boardParent;

    public GameObject[,] board;

    [SerializeField] BoxCollider2D groundCollider;
    [SerializeField] TextMeshProUGUI shuffleText;

    public static GameBoard instance;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        InitializeBoard();
        AdjustCameraAndCollider(rows, columns, groundCollider);
    }



    void InitializeBoard()
    {
        board = new GameObject[columns, rows];

        for (int col = 0; col < columns; col++)
        {
            for (int row = 0; row < rows; row++)
            {
                int colorIndex = UnityEngine.Random.Range(0, colorCount);
                Vector3 position = new Vector3(col, -row, 0);
                GameObject block = Instantiate(blockPrefabs[colorIndex], position, Quaternion.identity, boardParent);
                board[col, row] = block;
            }
        }

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
                    List<GameObject> matchingBlocks = FindMatchingBlocks(col, row);
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
        List<GameObject> matchingBlocks = FindMatchingBlocks(position.x, position.y);

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
            CheckAndShuffleBoard();
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

    List<GameObject> FindMatchingBlocks(int startX, int startY) //Yan yana duran bloklarý tespit etme
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

    List<Vector2Int> GetNeighbors(int x, int y) //seçilen bloðun dört bir tarafýný kontrol etme
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        if (x > 0) neighbors.Add(new Vector2Int(x - 1, y));
        if (x < columns - 1) neighbors.Add(new Vector2Int(x + 1, y));
        if (y > 0) neighbors.Add(new Vector2Int(x, y - 1));
        if (y < rows - 1) neighbors.Add(new Vector2Int(x, y + 1));

        return neighbors;
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
                GameObject newBlock = Instantiate(blockPrefabs[colorIndex], spawnPosition, Quaternion.identity, boardParent);
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

            SpriteRenderer spriteRenderer = block.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = sprite;

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
            "Red" => redSprites[spriteIndex],
            "Blue" => blueSprites[spriteIndex],
            "Green" => greenSprites[spriteIndex],
            "Yellow" => yellowSprites[spriteIndex],
            "Purple" => purpleSprites[spriteIndex],
            "Pink" => pinkSprites[spriteIndex],
            _ => null,
        };
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
            StartCoroutine(ShowShuffleText());
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

                    if (FindMatchingBlocks(col, row).Count >= 2)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private void CheckAndShuffleBoard()
    {
        if (!HasMatchingBlocks())
        {
            ShuffleBoard();
        }
    }

    public void AdjustCameraAndCollider(int rows, int columns, BoxCollider2D groundCollider, float padding = 1f)
    {

        float cameraX = (columns - 1) / 2f;
        float cameraY = -(rows - 1) / 2f;
        Vector3 cameraPosition = new Vector3(cameraX, cameraY, -10f);


        float verticalSize = rows / 2f + padding;
        float horizontalSize = (columns / 2f + padding) / Camera.main.aspect;
        float orthographicSize = Mathf.Max(verticalSize, horizontalSize);


        Camera.main.transform.position = cameraPosition;
        Camera.main.orthographicSize = orthographicSize;

        if (groundCollider != null)
        {

            float colliderY = board[columns - 1, rows - 1].transform.position.y;
            groundCollider.offset = new Vector2(0, colliderY + 10);

        }

    }

    IEnumerator ShowShuffleText()
    {

        shuffleText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        shuffleText.gameObject.SetActive(false);

    }

}
