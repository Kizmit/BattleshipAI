using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//This script manages the enemy AI behavior
public class GameManager : MonoBehaviour, IComparer
{
    [SerializeField]
    public Sprite newSprite;

    private const int TURN_LIMIT = 1; //turn limit? not sure if turn control will be implemented here
    public const int GRID_SIZE = 99; //100-1 for 0 index
    public const int FRIGATE_HOLES = 5;
    public const int LARGE_CARRIER_HOLES = 7; //used to be seabase
    public const int SUB_HOLES = 3;
    public const int CRUISER_HOLES = 3;
    public const int CARRIER_HOLES = 5;
    public const int TOTAL_SHIP_HOLES = FRIGATE_HOLES + LARGE_CARRIER_HOLES + SUB_HOLES + CRUISER_HOLES + CARRIER_HOLES;

    private int[] shipSizes = new int[]{FRIGATE_HOLES, LARGE_CARRIER_HOLES, SUB_HOLES, CRUISER_HOLES, CARRIER_HOLES};
    private List<GameObject> enemyShipLocations;
    private List<Transform> ship1Transforms;
    private List<Transform> ship2Transforms;
    private List<Transform> ship3Transforms;
    private List<Transform> ship4Transforms;
    private List<Transform> ship5Transforms;

    private List<GameObject> playerShipLocations; //the transforms of randomly chosen cells from enemyGridCells
    private GameObject[] enemyGridCells; //array of all enemy grid cell objects

    private GameObject[] playerGridCells;
    private GameObject[] ships;
    private IEnumerator coroutine;

    private int totalAIHits, totalPlayerHits;

    private bool gameRunning, gameOver;
    void Awake()
    {
        playerShipLocations = new List<GameObject>();
        ships = GameObject.FindGameObjectsWithTag("Tile");
        enemyShipLocations = new List<GameObject>();
        SetCellArray();
        totalAIHits = 0;
        totalPlayerHits = 0;
        gameOver = false;
    }
    void Start()
    {
        //PrintEnemyGridCells();
        GenerateShipPositions();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (gameRunning)
        {
            Debug.Log("managing turns");
        
            if(!GetComponent<InputManager>().GetPlayerTurn())
            {
                Debug.Log("pre-AIPureRNG");
                AIPureRNG();
                Debug.Log("post-AIPureRNG");

            }
            if(totalAIHits == TOTAL_SHIP_HOLES || totalPlayerHits == TOTAL_SHIP_HOLES)
            {
                gameOver = true;
            }
        } 
    
       
    }
    
    int IComparer.Compare(object x, object y)
    {
        return( (new CaseInsensitiveComparer()).Compare(((GameObject)x).name, ((GameObject)y).name));
    }

    public void SetCellArray()
    {
        IComparer myComparer = new GameManager();
        enemyGridCells = GameObject.FindGameObjectsWithTag("EnemyCell"); //Puts all of the enemy grid cells into an array
        playerGridCells = GameObject.FindGameObjectsWithTag("Cell"); //Puts all of the player grid cells into an array
        Array.Sort(enemyGridCells, myComparer);
        /*foreach (UnityEngine.Object obj in playerGridCells)
        {
            Debug.Log("obj = " + obj.name);
        }*/
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
            //Debug.Log("Start cell is " + (startIndex + 1));
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
                    /*for(int i = 0; i< shipSizes[sizeIndex]; i++)
                    {
                        Debug.Log(openCells[i]);
                    }*/
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
                    /*for(int i = 0; i < shipSizes[sizeIndex]; i++)
                    {
                        Debug.Log(openCells[i]);
                    }*/
                    for(int i = 0; i < shipSizes[sizeIndex]; i++)
                    {
                       enemyShipLocations.Add(openCells[i]);
                    }
                    placed = true;
                }
            }
        }
    }

    public void AIPureRNG()
    {
        System.Threading.Thread.Sleep(100);
        int index;
        index = RandomNumberGenerator(GRID_SIZE);
        CheckAIHit(playerGridCells[index]);
    }

    private void AIMedium()
    {
        int index;
        index = RandomNumberGenerator(GRID_SIZE);
        CheckAIHit(playerGridCells[index]);
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
        
        if(enemyShipLocations.Contains(cell.gameObject))
        {
            cell.GetComponent<GridChanges>().ChangeSpriteRed();
            totalPlayerHits++;
        } //hit
        else
        {
            cell.GetComponent<GridChanges>().ChangeSpriteWhite(); //miss
        }
        GetComponent<InputManager>().SetPlayerTurn();
    }

    public void CheckAIHit(GameObject cell)
    {
        if(playerShipLocations.Contains(cell.gameObject)) 
        {
            cell.GetComponent<GridChanges>().ChangeSpriteRed();
            totalAIHits++;
        } //hit
        else
        {
            cell.GetComponent<GridChanges>().ChangeSpriteWhite(); //miss
        }
        GetComponent<InputManager>().SetPlayerTurn();
    }
    public void SetCoordinatesOfShip()
    {
        ship1Transforms = ships[0].GetComponent<Tile>().PassShipCoordinates();
        ship2Transforms = ships[1].GetComponent<Tile>().PassShipCoordinates();
        ship3Transforms = ships[2].GetComponent<Tile>().PassShipCoordinates();
        ship4Transforms = ships[3].GetComponent<Tile>().PassShipCoordinates();
        ship5Transforms = ships[4].GetComponent<Tile>().PassShipCoordinates();
        Transform[] temp1 = new Transform[ship1Transforms.Count];
        Transform[] temp2 = new Transform[ship2Transforms.Count];
        Transform[] temp3 = new Transform[ship3Transforms.Count];
        Transform[] temp4 = new Transform[ship4Transforms.Count];
        Transform[] temp5 = new Transform[ship5Transforms.Count];
        temp1 = ship1Transforms.ToArray();
        temp2 = ship2Transforms.ToArray();
        temp3 = ship3Transforms.ToArray();
        temp4 = ship4Transforms.ToArray();
        temp5 = ship5Transforms.ToArray();
        for(int i = 0; i < ship1Transforms.Count; i++)
        {
            playerShipLocations.Add(temp1[i].gameObject);
        }
        for(int i = 0; i < ship2Transforms.Count; i++)
        {
            playerShipLocations.Add(temp2[i].gameObject);
        }
        for(int i = 0; i < ship3Transforms.Count; i++)
        {
            playerShipLocations.Add(temp3[i].gameObject);
        }
        for(int i = 0; i < ship4Transforms.Count; i++)
        {
            playerShipLocations.Add(temp4[i].gameObject);
        }
        for(int i = 0; i < ship5Transforms.Count; i++)
        {
            playerShipLocations.Add(temp5[i].gameObject);
        }
        /*foreach (UnityEngine.Object obj in playerShipLocations)
        {
            Debug.Log("obj = " + obj.name);
        }*/
        GetComponent<InputManager>().SetLockedIn();
    }   

    public void Confirm()
    {
        SetCoordinatesOfShip();
        if(RandomNumberGenerator(2) == 1) GetComponent<InputManager>().SetPlayerTurn();
        gameRunning = true;
        if(gameOver)
        {
            gameRunning = false;
        }
    }

    /*private void ManageTurns()
    {
        Debug.Log("managing turns");
        if(RandomNumberGenerator(2) == 1) GetComponent<InputManager>().SetPlayerTurn();
        
        if(!GetComponent<InputManager>().GetPlayerTurn())
        {
            Debug.Log("pre-AIPureRNG");
            AIPureRNG();
            Debug.Log("post-AIPureRNG");
            GetComponent<InputManager>().SetPlayerTurn();

        }
        if(totalAIHits == TOTAL_SHIP_HOLES || totalPlayerHits == TOTAL_SHIP_HOLES)
        {
        gameOver = true;
        }
    
    }*/
}
