using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class AIvAIMenu : MonoBehaviour
{
public void AIvAIGame() //AIvAI button
   {
       SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
       Debug.Log("AI vs AI Game button clicked; scene loaded");
       //to be implemented later
       //load buildIndex + 2 possibly? unless AIvAI gameplay scene = playervAI gameplay scene
   }
   
   //this is where we'd determine which AI algorithm is loaded based on difficulty selection in AIvAI menu
}
