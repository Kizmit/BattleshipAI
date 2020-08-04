using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

//This script manages the enemy AI behavior
public class GameManager : MonoBehaviour, IComparer
{
    [SerializeField]
    public const int GRID_SIZE = 99; //100-1 for 0 index
    public const int FRIGATE_HOLES = 5;
    public const int LARGE_CARRIER_HOLES = 7;
    public const int SUB_HOLES = 3;
    public const int CRUISER_HOLES = 3;
    public const int CARRIER_HOLES = 5;
    public const int TOTAL_SHIP_HOLES = FRIGATE_HOLES + LARGE_CARRIER_HOLES + SUB_HOLES + CRUISER_HOLES + CARRIER_HOLES;
    private int[] shipSizes;
    
    private List<int> usedIndices; //list of indices checked by AI algorithm during game
       
    private List<GameObject> enemyShipLocations; //the gameObjects that the enemy ships are positioned over
    private List<GameObject> playerShipLocations; //the gameObjects that the player ships are positioned over
    private GameObject[] enemyGridCells; //array of all enemy grid cell objects
    private GameObject[] playerGridCells; //array of all player grid cell objects
    private GameObject[] ships; //references to all player ship tiles

    private GameObject shipPlacementWarningText;

    private bool easy, medium, hard, impossible; //difficulties
    private int totalAIHits, totalPlayerHits, impossibleIndex; //game control
    private bool gameRunning; //game control
    
    void Awake()
    {
        /*Set difficulties based on menu choice*/
        easy = NewGameMenu.easy;
        medium = NewGameMenu.medium;
        hard = NewGameMenu.hard;
        impossible = NewGameMenu.impossible;

        if(!easy && !medium && !hard && !impossible) easy = true; //set a default difficulty if not selected in menu
        
        /*Initialize shipLocation lists*/
        playerShipLocations = new List<GameObject>();
        enemyShipLocations = new List<GameObject>();
        
        /*Get reference to player ships and store their sizes in an array*/
        ships = GameObject.FindGameObjectsWithTag("Tile");
        shipSizes = new int[]{FRIGATE_HOLES, LARGE_CARRIER_HOLES, SUB_HOLES, CRUISER_HOLES, CARRIER_HOLES};
        
        usedIndices = new List<int>();
        
        /*Add all grid objects to an array and sort them by their positions*/
        SetCellArray();
        
        /*Initialize count variables*/
        totalAIHits = 0;
        totalPlayerHits = 0;
        impossibleIndex = 0;

        shipPlacementWarningText = GameObject.FindGameObjectWithTag("WarningMessage");
        shipPlacementWarningText.SetActive(false);
    }

    void Start()
    {
        GenerateShipPositions();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameRunning)
        {
            System.Threading.Thread.Sleep(100);
        
            if(!GetComponent<InputManager>().GetPlayerTurn())
            {
                if(easy)
                {
                    AIPureRNG();
                }
                else if(medium)
                {
                    AIMedium();
                }
                else if(hard)
                {
                    AIHard();
                }
                else if(impossible)
                {
                    AIImpossible();
                }
            }
            if(totalAIHits == TOTAL_SHIP_HOLES || totalPlayerHits == TOTAL_SHIP_HOLES)
            {
                gameRunning = false;
                DetermineWinner();
            }
        } 
    }

    private void DetermineWinner()
    {
        bool playerWin;
        if (totalAIHits == TOTAL_SHIP_HOLES)
        {
            playerWin = false;
        }
        else
        {
            playerWin = true;
        }
        WinBehavior(playerWin);
    }

    private void WinBehavior(bool playerWin)
    {
        if(playerWin)
        {
            SceneManager.LoadScene("PlayerWin", LoadSceneMode.Single);
        }
        else
        {
            SceneManager.LoadScene("PlayerLoss", LoadSceneMode.Single);
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
        Array.Sort(enemyGridCells, myComparer); //sorts the cells from position 1 to 100 for build
        Array.Sort(playerGridCells, myComparer);
    }
    private void GenerateShipPositions()
    {
      for(int i = 0; i < shipSizes.Length; i++){
          PlaceShip(i);
        }
    }

    private void PlaceShip(int sizeIndex)
    {
        GameObject[] openCells = new GameObject[10];
        bool vertical = false;
        bool placed = false;
        int startIndex, count;

        while(!placed)
        {
            count = 0;
            if(RandomNumberGenerator(2) == 1) //determine ship orientation 
            {
               vertical = true; 
               Debug.Log("Placing Vertical");
            }
            
            startIndex = RandomNumberGenerator(GRID_SIZE); //get random position of grid
            
            if(enemyShipLocations.Contains(enemyGridCells[startIndex])) continue; //if random grid position occupied, retry loop
            
            if(vertical) //if the ship should be oriented vertically
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
            }
            
            else //ship oriented horizontally
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
            }

            if(openCells.Length - (openCells.Length - count) < shipSizes[sizeIndex]) placed = false;  //Is there space for the ship?
            else //There is space for the ship
            {
                for(int i = 0; i < shipSizes[sizeIndex]; i++)
                {
                    enemyShipLocations.Add(openCells[i]);
                }
                placed = true;
            }
        }
    }

    public void AIPureRNG()
    {
        bool foundIndex = false;
        int index;
        while(!foundIndex)
        {
            index = RandomNumberGenerator(GRID_SIZE);
            if(!usedIndices.Contains(index))
            {   
                usedIndices.Add(index);
                CheckAIHit(playerGridCells[index]);
                foundIndex = true;
            }
            else continue;
        }
    }

    private void AIMedium()
    {
        int index;
        index = RandomNumberGenerator(GRID_SIZE);
        CheckAIHit(playerGridCells[index]);
    }

    private void AIHard()
    {

    }

    private void AIImpossible()
    {
        CheckAIHit(playerShipLocations[impossibleIndex]);
        impossibleIndex++;
    }

    private int RandomNumberGenerator(int bound) //random number generator for placing ships
    {
        int number;
        number = UnityEngine.Random.Range(0, bound);
        return number;
    }

    public void CheckHit(GameObject cell)
    {
        
        if(enemyShipLocations.Contains(cell.gameObject)) //hit
        {
            cell.GetComponent<GridChanges>().ChangeSpriteRed();
            cell.GetComponent<BoxCollider2D>().enabled = false;
            totalPlayerHits++;
        } 
        else //miss
        {
            cell.GetComponent<GridChanges>().ChangeSpriteWhite(); 
            cell.GetComponent<BoxCollider2D>().enabled = false;
        }
        GetComponent<InputManager>().SetPlayerTurn();
    }

    public void CheckAIHit(GameObject cell)
    {
        if(playerShipLocations.Contains(cell.gameObject)) //hit
        {
            cell.GetComponent<GridChanges>().ChangeSpriteRed();
            totalAIHits++;
        } 
        else //miss
        {
            cell.GetComponent<GridChanges>().ChangeSpriteWhite(); 
        }
        GetComponent<InputManager>().SetPlayerTurn();
    }

    public void SetCoordinatesOfShip()
    {
        List<Transform> touchingTiles = new List<Transform>();

        if(playerShipLocations.Count > 0) playerShipLocations.Clear();

        for (int i = 0; i < ships.Length; i++)
        {
            touchingTiles.AddRange(ships[i].GetComponent<Tile>().GetShipCoordinates());
        }
        foreach(Transform obj in touchingTiles)
        {
            playerShipLocations.Add(obj.gameObject);
        }

        if(playerShipLocations.Count < TOTAL_SHIP_HOLES) 
        {
        playerShipLocations.Clear();
        return;
        }

        else GetComponent<InputManager>().SetLockedIn();
    }   

    public void Confirm()
    {
        SetCoordinatesOfShip();

        if(playerShipLocations.Count < TOTAL_SHIP_HOLES) 
        {
            shipPlacementWarningText.SetActive(true);
            return;
        }
        else
        {
            GameObject.FindGameObjectWithTag("ConfirmButton").SetActive(false);
            shipPlacementWarningText.SetActive(false);
            if(RandomNumberGenerator(2) == 1) GetComponent<InputManager>().SetPlayerTurn();
            gameRunning = true;

        }
    }
}
