using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    // Start is called before the first frame update
  public void OnMouseOver()
  {
    if (Input.GetMouseButtonUp(0))
    {
      PlaceTower();
    }
  }

  private void PlaceTower()
  {
    Debug.Log("placing tower");
  }
}
