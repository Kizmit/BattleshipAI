using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attackable : MonoBehaviour
{
    //Serialize field to make it changable through the unity inspector
    [SerializeField]
    public Sprite newSprite;
    
    //Declare object sprite Renderer for current sprite
    public SpriteRenderer spriteRenderer;
   

    void ChangeSprite()
    {
        //Changes the sprite to the new one you can set in the inspector
        spriteRenderer.sprite = newSprite;

    }
    

    void Start()
    {
        //Set the first sprite as the water tile that is already there
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

    }

    // Update here detects if the player touches the tile, if they 
    void Update()
    {
        //If this tile is clicked, change it to the new one.
        if(Input.GetMouseButtonDown(0))
        {
            ChangeSprite();
        }
    }
}
