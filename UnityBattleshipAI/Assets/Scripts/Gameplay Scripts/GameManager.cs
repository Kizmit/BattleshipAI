using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script manages the enemy AI behavior
public class GameManager : MonoBehaviour
{
    [SerializeField]
    public Sprite newSprite;
    
    public const int GRID_SIZE = 99; //100-1 for 0 index
    public const int FRIGATE_HOLES = 5;
    public const int SEABASE_HOLES = 9;
    public const int SUB_HOLES = 3;
    public const int CRUISER_HOLES = 3;
    public const int CARRIER_HOLES = 5;
    public const int TOTAL_SHIP_HOLES = FRIGATE_HOLES + SEABASE_HOLES + SUB_HOLES + CRUISER_HOLES + CARRIER_HOLES;

    private List<Transform> enemyShipCoordinates; //the transforms of randomly chosen cells from enemyGridCells
    private GameObject[] enemyGridCells; //array of all enemy grid cell objects
    private const int TURN_LIMIT = 1; //turn limit? not sure if turn control will be implemented here
    
    private void GenerateShipPositions()
    {
       //Add logic here to place enemy ships
       //1. Use random number generator to determine vertical or horizontal placement (if (RNG(1) = 1) vertical = true)
       //2. Use random number generator to determine starting index (index = RNG(GRID_SIZE))
       //3. Check generated index for an already existing placement in enemyShipCoordinates
       //4. Check surrounding cells (in direction based on vertical boolean) to make sure it doesnt go off edge/overlap another ship
       //5. If check succeeds, place the ship (add the transform of each cell used to enemyShipCoordinates)
       //   If check fails, restart from step 2
       //6. Repeat for all of the ships
    }
    void Awake()
    {
        enemyGridCells = GameObject.FindGameObjectsWithTag("EnemyCell"); //Puts all of the enemy grid cells into an array
    }
    void Start()
    {
        PrintEnemyGridCells();
        //generateShipPositions();
    }

    // Update is called once per frame
    void Update()
    {
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
        //add logic here to check enemyShipCoordinates for hit (if (enemyShipCoordinates.Contains(cell.transform)) is hit)
        cell.GetComponent<EnemyGridChanges>().ChangeSprite(); //this will change the sprite of the tile to red
        //we will also increment the total hits counter here to check for end of game
    }
}
