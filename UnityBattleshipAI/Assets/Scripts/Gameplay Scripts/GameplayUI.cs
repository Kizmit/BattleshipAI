using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayUI : MonoBehaviour
{
    public void QuitGame() //Quit button
    {
        Debug.Log("Quit() called on click");
        Application.Quit();
    }

    public void ReturnToMenu() //Quit button
    {
        Debug.Log("ReturnToMenu() called on click");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
