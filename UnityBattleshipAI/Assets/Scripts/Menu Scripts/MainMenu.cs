using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
   public void QuitGame() //Quit button
   {
       Debug.Log("Quit() called on click");
       Application.Quit();
   }

}

 
