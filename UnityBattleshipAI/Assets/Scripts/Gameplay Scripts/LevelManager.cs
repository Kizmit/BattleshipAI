using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LevelManager : MonoBehaviour
{
    [SerializeField]    //This allows for the object below to be edited using the inspector in unity
    private GameObject[] tilePrefabs;

    public float TileSize
    {
        get {return tilePrefabs[0].GetComponent<SpriteRenderer>().sprite.bounds.size.x; }
    }

    // Start is called before the first frame update
    void Start()
    {

        CreateLevel();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //***********************************************************************************************************
    //This function will create a grid using for loops and info it reads in from a text file named as Level in 
    //the Asset's Resources folder.
    //***********************************************************************************************************
    private void CreateLevel()
    {

        string[] mapData = ReadLevelText();

        int mapXSize = mapData[0].ToCharArray().Length;
        int mapYSize = mapData.Length;

        //Camera stuff to make the grid line up
        Vector3 worldStart = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height));

        //For loops to print the grid, newTiles is for setting what kind of square you want printed. 0's for 
        //blue, 1's for red.
        for (int y = 0; y < mapYSize; y++)
        {
            char[] newTiles = mapData[y].ToCharArray();

            for (int x = 0; x < mapXSize; x++)
            {
                PlaceTile(newTiles[x].ToString(), x, y, worldStart);
            }
        }
    }

    //***************************************************************************************************************
    //This function is what clones and places tiles. It hooks up with the for loop above and prints whatever type is
    //given to it in its first parameter.
    //***************************************************************************************************************
    private void PlaceTile(string tileType, int x, int y, Vector3 worldStart)
    {
        int tileIndex = int.Parse(tileType);

        GameObject newTile = Instantiate(tilePrefabs[tileIndex]);
        newTile.transform.position = new Vector3(worldStart.x + (TileSize * x), worldStart.y - (TileSize * y), 0);
    }

    //***************************************************************************************************************
    //This function reads in the text file Level and makes a map according to what numbers are in what position.
    //Right now it's a 10x10 grid, the 0's make the board blue, while switching any 0 with a 1 will turn that 
    //square red.  The rest of the function deals with reading in the map.
    //***************************************************************************************************************
    private string[] ReadLevelText()
    {

        TextAsset bindData = Resources.Load("Level") as TextAsset;

        string data = bindData.text.Replace(Environment.NewLine, string.Empty);

        return data.Split('-');

    }

}
