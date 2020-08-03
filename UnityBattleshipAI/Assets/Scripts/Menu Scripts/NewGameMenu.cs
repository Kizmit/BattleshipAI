using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class NewGameMenu : MonoBehaviour
{
  public bool easy, medium, hard, impossible;
  public void NewGame() //New Game button
  {
      Debug.Log("New Game clicked; loading next scene in build index");
      SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); //loads the next scene in the build index on click
  }
  public void SetEasy()
  { 
    easy = true;
    medium = false;
    hard = false;
    impossible = false;
  }
  public void SetMedium()
  {
    easy = false;
    medium = true;
    hard = false;
    impossible = false;
  }

  public void SetHard()
  {
    easy = false;
    medium = false;
    hard = true;
    impossible = false;
  }

  public void SetImpossible()
  {
    easy = false;
    medium = false;
    hard = false;
    impossible = true;
  }
}
