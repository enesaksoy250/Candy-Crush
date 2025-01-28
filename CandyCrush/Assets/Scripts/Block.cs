using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{

    public string blockColor;

    private int maxSpeed=5;

    private Rigidbody2D rb;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }


    public void SetSprite(Sprite sprite)
    {

        spriteRenderer.sprite = sprite;

    }

    private void Update()
    {
        if(rb.velocity.y < -maxSpeed)
        {
            rb.velocity = new Vector2(0, -maxSpeed);
        }   
    }



}
