using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour
{
    private bool draggingItem = false;
    private bool lockedIn = false;
    private bool playerTurn;
    private GameObject draggedObject;
    private Vector2 touchOffset;

    void Update()
    {
        if (HasInput)
        {
            DragOrPickUp();
        }
        else
        {
            if (draggingItem)
                DropItem();
        }
    }
    Vector2 CurrentTouchPosition
    {
        get
        {
            return Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }
    private void DragOrPickUp()
    {
        var inputPosition = CurrentTouchPosition;
        if (draggingItem)
        {
            draggedObject.transform.position = inputPosition + touchOffset;
            if (Input.GetMouseButtonDown(1))
            {
                draggedObject.transform.Rotate(0,0,90);
            }
        }
        else
        {
            var layerMask = 1 << 0;
            RaycastHit2D[] touches = Physics2D.RaycastAll(inputPosition, inputPosition, 0.5f, layerMask);
            if (touches.Length > 0)
            {
                var hit = touches[0];
                if (hit.transform != null && hit.transform.tag == "Tile" && !lockedIn) //monitors for click on ship tiles
                {
                    draggingItem = true;
                    draggedObject = hit.transform.gameObject;
                    touchOffset = (Vector2)hit.transform.position - inputPosition;
                    hit.transform.GetComponent<Ship>().PickUp();
                }
                else if (hit.transform != null && hit.transform.tag == "EnemyCell" && playerTurn && lockedIn) //monitors for clicks on enemy grid
                {
                    touchOffset = (Vector2)hit.transform.position - inputPosition;
                    GetComponent<GameManager>().CheckHit(hit.transform.gameObject);
                    playerTurn = false;
                    Debug.Log(playerTurn + " in raycast");
                    //Debug.Log("Component that is grabbed is " + hit.transform.gameObject);
                }
            }
        }
    }

    public void SetLockedIn()
    {
        lockedIn = true;
    }

    public void SetPlayerTurn()
    {
       
        if(playerTurn == false) 
        {
            playerTurn = true;
            Debug.Log(playerTurn + " SetPlayerTurn if");
        }
        else 
        {
            playerTurn = false;
            Debug.Log(playerTurn + " SetPlayerTurn else");
        }
    }
    public bool GetPlayerTurn()
    {
        Debug.Log(playerTurn + " GetPlayerTurn");
        return playerTurn;
    }

    private bool HasInput
    {
        get
        {
            return Input.GetMouseButton(0);
        }
    }
    void DropItem()
    {
        draggingItem = false;
        draggedObject.transform.localScale = new Vector3(1, 1, 1);
        draggedObject.GetComponent<Ship>().Drop();
    }
}
