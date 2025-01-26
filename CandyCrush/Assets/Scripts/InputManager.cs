using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{

    [SerializeField] Camera mainCamera;
 
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))  
        {
            Vector3 mousePosition = Input.mousePosition; 
            Ray ray = mainCamera.ScreenPointToRay(mousePosition);  
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.collider != null)
            {
                GameObject clickedBlock = hit.collider.gameObject;  
                GameBoard.instance.OnBlockClicked(clickedBlock);  
            }
        }
    }
  
}
