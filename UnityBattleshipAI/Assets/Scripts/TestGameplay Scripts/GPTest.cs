using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPTest : MonoBehaviour
{
    // playerShips keeps track of how many unsunk ships are on the players field. it increments when the player first sets their ships and once it = 5 the game will start. 
    private int playerShips = 0;
    // this will be set to true when the player places all of their ships.
    private bool playerReady = false;
    // these keep track of how many ships have been sunk.
    private int playerShipsSunk = 0;
    private int enemyShipsSunk = 0;
    // this will be set to true when either the player or oponents sunk values = 5. 
    private bool gameOver = false;
    // Start is called before the first frame update

    public void shipSet()
    {
        this.playerShips++;
        Debug.Log(this.playerShips);
    }
    public int shipGet()
    {
        return this.playerShips;
    }

    public void Start()
    {

    }

    public void Update()
    {


    }
}
