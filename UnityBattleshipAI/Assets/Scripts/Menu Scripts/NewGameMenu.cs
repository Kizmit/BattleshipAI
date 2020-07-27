using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class NewGameMenu : MonoBehaviour
{
  public void NewGame() //New Game button
   {
       Debug.Log("New Game clicked; loading next scene in build index");
       SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); //loads the next scene in the build index on click
   }
}
