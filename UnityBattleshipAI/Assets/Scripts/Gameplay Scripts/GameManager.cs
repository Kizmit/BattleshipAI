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
    public const int LARGE_CARRIER_HOLES = 7; //used to be seabase
    public const int SUB_HOLES = 3;
    public const int CRUISER_HOLES = 3;
    public const int CARRIER_HOLES = 5;
    public const int TOTAL_SHIP_HOLES = FRIGATE_HOLES + LARGE_CARRIER_HOLES + SUB_HOLES + CRUISER_HOLES + CARRIER_HOLES;
    private int[] shipSizes = new int[]{FRIGATE_HOLES, LARGE_CARRIER_HOLES, SUB_HOLES, CRUISER_HOLES, CARRIER_HOLES};
    private List<GameObject> enemyShipLocations; //the gameObjects that 
    private List<Transform> ship1Transforms;
    private List<Transform> ship2Transforms;
    private List<Transform> ship3Transforms;
    private List<Transform> ship4Transforms;
    private List<Transform> ship5Transforms;
    private List<GameObject> playerShipLocations; //the gameObjects that player ships are touching
    private GameObject[] enemyGridCells; //array of all enemy grid cell objects
    private GameObject[] playerGridCells; //array of all player grid cell objects
    private GameObject[] ships; //references to all player ship tiles

    private bool easy, medium, hard, impossible;
    private int totalAIHits, totalPlayerHits;
    private bool gameRunning;
    
    void Awake()
    {
        easy = false;
        medium = true;
        hard = false;
        impossible = false;
        /*easy = GetComponent<NewGameMenu>().easy;
        medium = GetComponent<NewGameMenu>().medium;
        hard = GetComponent<NewGameMenu>().hard;
        impossible = GetComponent<NewGameMenu>().impossible;*/
        playerShipLocations = new List<GameObject>();
        ships = GameObject.FindGameObjectsWithTag("Tile");
        enemyShipLocations = new List<GameObject>();
        SetCellArray();
        totalAIHits = 0;
        totalPlayerHits = 0;
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
                if(medium)
                {
                    AIMedium();
                }
                if(hard)
                {
                    AIHard();
                }
                if(impossible)
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
        GameObject[] openCells = new GameObject[10];
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
    /********************************************** ALGORITHMS *******************************************************/
    public void AIPureRNG()
    {
        int index;
        index = RandomNumberGenerator(GRID_SIZE);
        CheckAIHit(playerGridCells[index]);
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

        if (hit)
        {
            // Check that the new index has not been previously called.
            // Check that the new index is in-bounds by rows (0-9, 10-19, 20-29, ..., 90-99).
            // Check the the new index is in-bounds by columns (??)
            // NOTE: What would happen if we didn't check bounds? Obviously, it would be slightly less efficient
            // (such as continuing on to the next row, even though it's not possible for the ship to wrap to the next row).
            // Ignoring NOTE for now.

            // ABOVE: up one space (index - 10), up two spaces (index - 20), up three spaces (index - 30)
            int newIndex = index;
            while ((hit) && (newIndex <= (index - 30))) // While there is a hit and we are no more than three spaces away from the original hit...
            {
                newIndex = index - 10;
                // Check that newIndex is in bounds (implement later) and has not been previously attacked.
                hit = CheckAIHit(playerGridCells[newIndex]);
            }

            // BELOW: down one (index + 10), down two (index + 20), down three (index + 30)
            newIndex = index;
            while ((hit) && (newIndex <= (index + 30)))
            {
                newIndex = index + 10;
                // Check that newIndex is in bounds (implement later) and has not been previously attacked.
                hit = CheckAIHit(playerGridCells[newIndex]);
            }

            // LEFT: (index - 1), (index - 2), (index -3)
            newIndex = index;
            while ((hit) && (newIndex <= (index - 3)))
            {
                newIndex = index - 10;
                // Check that newIndex is in bounds (implement later) and has not been previously attacked.
                hit = CheckAIHit(playerGridCells[newIndex]);
            }

            // RIGHT: (index + 1), (index + 2), (index + 3)
            newIndex = index;
            while ((hit) && (newIndex <= (index + 3)))
            {
                newIndex = index + 1;
                // Check that newIndex is in bounds (implement later) and has not been previously attacked.
                hit = CheckAIHit(playerGridCells[newIndex]);
            }

        }

    }

    private void AIHard()
    {
        /* Algorithm: Checks every other space - similair to only checking the black spaces on a checkerboard.
         * When a ship is hit, the algorithm checks the three spaces above the original hit, three spaces below,
         * three spaces left, and three spaces right.
         * After, the algorithm resumes checking every other space (where it left off) until another hit.
        */
        int index = 0;
        bool hit;
        for (int i = 0; i <= GRID_SIZE; i++)
        {
            hit = CheckAIHit(playerGridCells[index]);
            if (hit)
            {
                // Implement the algorithm from AIMedium.
            }
            else
            {
                index += 2;
            }
            
        }
    }

    private void AIImpossible()
    {
        /* Algorithm: The enemy AI already knows the player's ships' locations and gets a hit every time. */

    }
    /*****************************************************************************************************************/
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

    public bool CheckAIHit(GameObject cell)
    {
        if(playerShipLocations.Contains(cell.gameObject)) 
        {
            cell.GetComponent<GridChanges>().ChangeSpriteRed();
            totalAIHits++;
            GetComponent<InputManager>().SetPlayerTurn();
            return true;
        } //hit
        else
        {
            cell.GetComponent<GridChanges>().ChangeSpriteWhite(); //miss
            GetComponent<InputManager>().SetPlayerTurn();
            return false;
        }
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
    }
}
