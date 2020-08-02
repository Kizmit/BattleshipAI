using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class manages each individual tile of the enemy grid
public class EnemyGridChanges : MonoBehaviour
{

    [SerializeField]
    public Sprite newSprite;
    public SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Awake()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }
    void Start()
    {
    }
    // Update is called once per frame
    void Update()
    {
    }
    public void ChangeSprite()
    {
        spriteRenderer.sprite = newSprite;
    }
}
