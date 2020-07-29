using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragDrop : MonoBehaviour {
  private bool isDragging;

  private void OnMouseDown()
  {
    isDragging = true;

  }

  private void OnMouseUp()
  {
    isDragging = false;
  }

  private void Update()
  {

    if (isDragging)
    {

      Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
      transform.Translate(mousePosition,Space.World);
      if (Input.GetMouseButtonDown(1))
      {
        transform.Rotate(0,0,90);
        Debug.Log(mousePosition);
      }

    }
  }
}
