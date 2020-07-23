using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
   public void NewGame() //New Game button
   {
       Debug.Log("New Game clicked; loading next scene in build index");
       SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); //loads the next scene in the build index on click
   }
   public void AIvAIGame() //AIvAI button
   {
       Debug.Log("AI vs AI Game button clicked; no implementation");
       //to be implemented later
       //load buildIndex + 2 possibly? unless AIvAI gameplay scene = playervAI gameplay scene
   }
   public void Statistics() //statistics button
   {
       Debug.Log("Statistics button clicked; no implementation");
       //to be implemented later
   }

   //Options excluded; unity library includes setActive() function for switching UI objects

   public void QuitGame() //Quit button
   {
       Debug.Log("Quit() called on click");
       Application.Quit();
   }

}

 
