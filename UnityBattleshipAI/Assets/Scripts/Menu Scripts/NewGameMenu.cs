using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class NewGameMenu : MonoBehaviour
{
  public static bool easy, medium, hard, impossible;

  public void NewGame() //New Game button
  {
      Debug.Log("New Game clicked; loading next scene in build index");
      SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); //loads the next scene in the build index on click
  }
  public void SetEasy()
  { 
    Debug.Log("SetEasy");
    easy = true;
    medium = false;
    hard = false;
    impossible = false;
  }
  public void SetMedium()
  {
    Debug.Log("SetMedium");
    easy = false;
    medium = true;
    hard = false;
    impossible = false;
  }

  public void SetHard()
  {
    Debug.Log("SetHard");
    easy = false;
    medium = false;
    hard = true;
    impossible = false;
  }

  public void SetImpossible()
  {
    Debug.Log("SetImpossible");
    easy = false;
    medium = false;
    hard = false;
    impossible = true;
  }
}
