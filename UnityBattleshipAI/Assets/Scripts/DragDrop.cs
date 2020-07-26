using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class DragDrop : MonoBehaviour
{
  private bool isDragging;

  public void OnMouseDown()
  {
    isDragging = true;
  }

  public void OnMouseUp()
  {
    isDragging = false;
  }

  void Update()
  {
    if (isDragging)
    {
      Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
      transform.Translate(mousePosition);
    }
  }
}
