using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CameraAndUIHandler : MonoBehaviour
{
  
    [SerializeField] TextMeshProUGUI shuffleText;
    [SerializeField] BoxCollider2D groundCollider;

    public static CameraAndUIHandler instance;

    private void Awake()
    {
        instance = this;
    }

    public void AdjustCameraAndCollider(int rows, int columns, float padding = 1f)
    {

        float cameraX = (columns - 1) / 2f;
        float cameraY = -(rows - 1) / 2f;
        Vector3 cameraPosition = new Vector3(cameraX, cameraY, -10f);


        float verticalSize = rows / 2f + padding;
        float horizontalSize = (columns / 2f + padding) / Camera.main.aspect;
        float orthographicSize = Mathf.Max(verticalSize, horizontalSize);


        Camera.main.transform.position = cameraPosition;
        Camera.main.orthographicSize = orthographicSize;

        float colliderY = GameBoard.instance.board[columns - 1, rows - 1].transform.position.y;
        groundCollider.offset = new Vector2(0, colliderY + 10);


    }

    private IEnumerator ShowShuffleTextCoroutine()
    {

        shuffleText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2);
        shuffleText.gameObject.SetActive(false);

    }

    public void ShowShuffleText()
    {
        StartCoroutine(ShowShuffleTextCoroutine());
    }
}
