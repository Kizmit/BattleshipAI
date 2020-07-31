using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardScript : MonoBehaviour
{
    public bool OCCUPIED;   // Whether the unit contains a ship or not.
    // Start is called before the first frame update.
    void Start()
    {
        this.OCCUPIED = false;  // Empty board -> no units contain ships.
    }

    // Update is called once per frame.
    void Update()
    {
        if (Input.mousePosition != null)    // Has mouse position.
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);    // Convert that mouse position to a Ray object.
            Vector3 worldPoint = ray.GetPoint(-ray.origin.z / ray.direction.z);
            BoardScript grid = transform.GetComponent<BoardScript>();
            Vector3Int position = grid.WorldToCell(worldPoint); // Get xy coordinate location of tile on tilemap.
            }
        }
    }
}
