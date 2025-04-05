using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class playerController : MonoBehaviour
{
    public float speed = 5f; // Speed of player movement
    public Sprite textureUp; // Texture for moving up
    public Sprite textureDown; // Texture for moving down
    public Sprite textureLeft; // Texture for moving left
    public Sprite textureRight; // Texture for moving right

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Movement input
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // Calculate movement vector
        Vector2 movement = new Vector2(moveHorizontal, moveVertical);

        // Move the player
        rb.velocity = movement * speed;

        // Texture switching based on movement direction
        if (Input.GetKey(KeyCode.D))
        {
            // Moving right
            spriteRenderer.sprite = textureRight;
        }
        if (Input.GetKey(KeyCode.A))
        {
            // Moving left
            spriteRenderer.sprite = textureLeft;
        }
        if (Input.GetKey(KeyCode.W))
        {
            // Moving up
            spriteRenderer.sprite = textureUp;
        }
        if (Input.GetKey(KeyCode.S))
        {
            // Moving down
            spriteRenderer.sprite = textureDown;
        }
    }
}