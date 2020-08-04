using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Linq;

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
/********************************** ALGORITHMS ***********************************************/
    public void AIPureRNG()
    {
        /* Algorithm: Randomly searches the board for hits. */
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
        /* Algorithm: Randomly searches the board until it hits a ship.
         * When a ship is hit, the algorithm will check three spaces above the original hit, three below, 
         * three to the left, and three to the right. 
         * After, the algorithm continues randomly searching for another hit.
        */
        int index;
        bool hit;
        index = RandomNumberGenerator(GRID_SIZE);
        hit = CheckAIHit(playerGridCells[index]);
        int newIndex = index;

        // Check that the new index has not been previously called.
        // Check that the new index is in-bounds by rows (0-9, 10-19, 20-29, ..., 90-99).
        // Check the the new index is in-bounds by columns (??)
        // NOTE: What would happen if we didn't check bounds? Obviously, it would be slightly less efficient
        // (such as continuing on to the next row, even though it's not possible for the ship to wrap to the next row).
        // Ignoring NOTE for now.

        // ABOVE: up one space (index - 10), up two spaces (index - 20), up three spaces (index - 30)
        while ((hit) && (newIndex <= (index - 30))) // While there is a hit and we are no more than three spaces away from the original hit...
        {
            newIndex -= 10;
            if (inBoundsRow(index, newIndex) || inBoundsCol(index, newIndex))
            {
                hit = CheckAIHit(playerGridCells[newIndex]);
            }
            else
            {
                hit = false;
            }
        }

        // BELOW: down one (index + 10), down two (index + 20), down three (index + 30)
        newIndex = index;
        while ((hit) && (newIndex <= (index + 30)))
        {
            newIndex += 10;
            if (inBoundsRow(index, newIndex) || inBoundsCol(index, newIndex))
            {
                hit = CheckAIHit(playerGridCells[newIndex]);
            }
            else
            {
                hit = false;
            }
        }

        // LEFT: (index - 1), (index - 2), (index -3)
        newIndex = index;
        while ((hit) && (newIndex <= (index - 3)))
        {
            newIndex -= 1;
            if (inBoundsRow(index, newIndex) || inBoundsCol(index, newIndex))
            {
                hit = CheckAIHit(playerGridCells[newIndex]);
            }
            else
            {
                hit = false;
            }
        }

        // RIGHT: (index + 1), (index + 2), (index + 3)
        newIndex = index;
        while ((hit) && (newIndex <= (index + 3)))
        {
            newIndex += 1;
            if (inBoundsRow(index, newIndex) || inBoundsCol(index, newIndex))
            {
                hit = CheckAIHit(playerGridCells[newIndex]);
            }
            else
            {
                hit = false;
            }
        }
    }

    private void AIHard()
    {
        /* Algorithm: Starts at index 0 and searches every other space of the board (like only checking 
         * the black spaces of a checkerboard) until a ship is hit.
         * When a ship is hit, the algorithm will check three spaces above the original hit, three below, 
         * three to the left, and three to the right. 
         * After, the algorithm continues searching every other space (from where it left off) until
         * another ship is hit.
        */
        int index;
        bool hit;
        index = RandomNumberGenerator(GRID_SIZE);
        hit = CheckAIHit(playerGridCells[index]);
        int newIndex = index;
    }

    private void AIImpossible()
    {
        /* Algorithm: The enemyAI already knows every location of the player's ships and gets a hit every turn. */
        CheckAIHit(playerShipLocations[impossibleIndex]);
        impossibleIndex++;
    }

/*********************************************************************************************/

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

    public bool CheckAIHit(GameObject cell)
    {
        if(playerShipLocations.Contains(cell.gameObject)) //hit
        {
            cell.GetComponent<GridChanges>().ChangeSpriteRed();
            totalAIHits++;
            GetComponent<InputManager>().SetPlayerTurn();
            return true;
        } 
        else //miss
        {
            cell.GetComponent<GridChanges>().ChangeSpriteWhite();
            GetComponent<InputManager>().SetPlayerTurn();
            return false;
        }
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

    // Takes the original index and newIndex and checks if they are in the same row.
    public bool inBoundsRow(int i, int ni)
    {
        int index = i;
        int newIndex = ni;

        int[] row1 = { 0, 1, 2, 9, 4, 5, 6, 7, 8, 9 };
        int[] row2 = { 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 };
        int[] row3 = { 20, 21, 22, 23, 24, 25, 26, 27, 28, 29 };
        int[] row4 = { 30, 31, 32, 33, 34, 35, 36, 37, 38, 39 };
        int[] row5 = { 40, 41, 42, 43, 44, 45, 46, 47, 48, 49 };
        int[] row6 = { 50, 51, 52, 53, 54, 55, 56, 57, 58, 59 };
        int[] row7 = { 60, 61, 62, 63, 64, 65, 66, 67, 68, 69 };
        int[] row8 = { 70, 71, 72, 73, 74, 75, 76, 77, 78, 79 };
        int[] row9 = { 80, 81, 82, 83, 84, 85, 86, 87, 88, 89 };
        int[] row10 = { 90, 91, 92, 93, 94, 95, 96, 97, 98, 99 };

        if (row1.Contains(index) && row1.Contains(newIndex))
        {
            return true;
        }
        else if (row2.Contains(index) && row2.Contains(newIndex))
        {
            return true;
        }
        else if (row3.Contains(index) && row3.Contains(newIndex))
        {
            return true;
        }
        else if (row4.Contains(index) && row4.Contains(newIndex))
        {
            return true;
        }
        else if (row5.Contains(index) && row5.Contains(newIndex))
        {
            return true;
        }
        else if (row6.Contains(index) && row6.Contains(newIndex))
        {
            return true;
        }
        else if (row7.Contains(index) && row7.Contains(newIndex))
        {
            return true;
        }
        else if (row8.Contains(index) && row8.Contains(newIndex))
        {
            return true;
        }
        else if (row9.Contains(index) && row9.Contains(newIndex))
        {
            return true;
        }
        else if (row10.Contains(index) && row10.Contains(newIndex))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Takes the original index and newIndex and checks if they are in the same column.
    public bool inBoundsCol(int i, int ni)
    {
        int index = i;
        int newIndex = ni;

        int[] col1 = { 0, 10, 20, 30, 40, 50, 60, 70, 80, 90 };
        int[] col2 = { 1, 11, 21, 31, 41, 51, 61, 71, 81, 91 };
        int[] col3 = { 2, 12, 22, 32, 42, 52, 62, 72, 82, 92 };
        int[] col4 = { 3, 13, 23, 33, 43, 53, 63, 73, 83, 93 };
        int[] col5 = { 4, 14, 24, 34, 44, 54, 64, 74, 84, 94 };
        int[] col6 = { 5, 15, 25, 35, 45, 55, 65, 75, 85, 95 };
        int[] col7 = { 6, 16, 26, 36, 46, 56, 66, 76, 86, 96 };
        int[] col8 = { 7, 17, 27, 37, 47, 57, 67, 77, 87, 97 };
        int[] col9 = { 8, 18, 28, 38, 48, 58, 68, 78, 88, 98 };
        int[] col10 = { 9, 19, 29, 39, 49, 59, 69, 79, 89, 99 };

        if (col1.Contains(index) && col1.Contains(newIndex))
        {
            return true;
        }
        else if (col2.Contains(index) && col2.Contains(newIndex))
        {
            return true;
        }
        else if (col3.Contains(index) && col3.Contains(newIndex))
        {
            return true;
        }
        else if (col4.Contains(index) && col4.Contains(newIndex))
        {
            return true;
        }
        else if (col5.Contains(index) && col5.Contains(newIndex))
        {
            return true;
        }
        else if (col6.Contains(index) && col6.Contains(newIndex))
        {
            return true;
        }
        else if (col7.Contains(index) && col7.Contains(newIndex))
        {
            return true;
        }
        else if (col8.Contains(index) && col8.Contains(newIndex))
        {
            return true;
        }
        else if (col9.Contains(index) && col9.Contains(newIndex))
        {
            return true;
        }
        else if (col10.Contains(index) && col10.Contains(newIndex))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
