using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//This script manages the enemy AI behavior
public class GameManager : MonoBehaviour
{
    [SerializeField]
    public Sprite newSprite;

    private const int TURN_LIMIT = 1; //turn limit? not sure if turn control will be implemented here
    public const int GRID_SIZE = 99; //100-1 for 0 index
    public const int FRIGATE_HOLES = 5;
    public const int LANDINGCRAFT_HOLES = 1; //used to be seabase
    public const int SUB_HOLES = 3;
    public const int CRUISER_HOLES = 3;
    public const int CARRIER_HOLES = 5;
    public const int TOTAL_SHIP_HOLES = FRIGATE_HOLES + LANDINGCRAFT_HOLES + SUB_HOLES + CRUISER_HOLES + CARRIER_HOLES;

    private int[] shipSizes = new int[]{FRIGATE_HOLES, LANDINGCRAFT_HOLES, SUB_HOLES, CRUISER_HOLES, CARRIER_HOLES};
    private List<GameObject> enemyShipLocations; //the transforms of randomly chosen cells from enemyGridCells
    private GameObject[] enemyGridCells; //array of all enemy grid cell objects

    void Awake()
    {
        enemyGridCells = GameObject.FindGameObjectsWithTag("EnemyCell"); //Puts all of the enemy grid cells into an array
        enemyShipLocations = new List<GameObject>();
    }
    void Start()
    {
        //PrintEnemyGridCells();
        GenerateShipPositions();
    }

    // Update is called once per frame
    void Update()
    {
    }
    
    private void GenerateShipPositions()
    {
      for(int i = 0; i < shipSizes.Length; i++){
          PlaceShip(i);
        }
    }

    private void PlaceShip(int sizeIndex)
    {
        GameObject[] openCells = new GameObject[11];
        bool vertical = false;
        bool placed = false;
        int startIndex;
        int count;
        while(!placed)
        {
            count = 0;
            if(RandomNumberGenerator(2) == 1) 
            {
               vertical = true; 
               Debug.Log("Placing Vertical");
            }
            startIndex = RandomNumberGenerator(GRID_SIZE);
            Debug.Log("Start cell is " + (startIndex + 1));
            if(enemyShipLocations.Contains(enemyGridCells[startIndex])){continue;}
            if(vertical)
            {
                for(int i = startIndex, j = 0; i < GRID_SIZE; i += 10, j++) //check above start index
                {
                    if(enemyShipLocations.Contains(enemyGridCells[i]))
                    {
                        break;
                    }
                    else
                    {
                        openCells[j] = enemyGridCells[i];
                        count++;
                    }
                }
                for(int i = startIndex - 10, j = openCells.Length - (openCells.Length - count); i > 0; i -= 10, j++) //check below start index
                {
                    if(enemyShipLocations.Contains(enemyGridCells[i]))
                    {
                        break;
                    }
                    else
                    {
                        openCells[j] = enemyGridCells[i];
                        count++;
                    }
                }
                
                if(openCells.Length - (openCells.Length - count) < shipSizes[sizeIndex]) placed = false; 
                else
                {
                    for(int i = 0; i< shipSizes[sizeIndex]; i++)
                    {
                        Debug.Log(openCells[i]);
                    }
                   for(int i = 0; i < shipSizes[sizeIndex]; i++)
                   {
                       enemyShipLocations.Add(openCells[i]);
                   }
                   placed = true;
                }
            }
            else
            {
    
                for(int i = startIndex, j = 0; i > startIndex - ((startIndex % 10)); i--, j++) //check to the left of start
                {
                    if(enemyShipLocations.Contains(enemyGridCells[i]))
                    {
                        break;
                    }
                    else
                    {
                        openCells[j] = enemyGridCells[i];
                        count++;
                    }
                }
                for(int i = startIndex + 1, j = openCells.Length  - (openCells.Length - count); i < startIndex + (10 - (startIndex % 10)); i++, j++) //check to the right of start index
                {
                    if(enemyShipLocations.Contains(enemyGridCells[i]))
                    {
                        break;
                    }
                    else
                    {
                        openCells[j] = enemyGridCells[i];
                        count++;
                    }
                }

                if(openCells.Length - (openCells.Length - count) < shipSizes[sizeIndex]) placed = false; 
                else
                {
                    for(int i = 0; i < shipSizes[sizeIndex]; i++)
                    {
                        Debug.Log(openCells[i]);
                    }
                    for(int i = 0; i < shipSizes[sizeIndex]; i++)
                    {
                       enemyShipLocations.Add(openCells[i]);
                    }
                    placed = true;
                }
            }
        }
    }


    private void PrintEnemyGridCells() //for debugging
    {
        for (int i = 0; i < enemyGridCells.Length; i++)
        {
            Debug.Log(enemyGridCells[i]);
        }
    }
    private int RandomNumberGenerator(int bound) //random number generator for placing ships
    {
        int number;
        number = UnityEngine.Random.Range(0, bound);
        return number;
    }
    public void CheckHit(GameObject cell)
    {
        
        if(enemyShipLocations.Contains(cell.gameObject)) cell.GetComponent<EnemyGridChanges>().ChangeSpriteRed(); //hit
        else
        {
            cell.GetComponent<EnemyGridChanges>().ChangeSpriteWhite(); //miss
        }
    }
}
