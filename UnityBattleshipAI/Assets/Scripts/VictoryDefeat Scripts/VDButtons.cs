using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VDButtons : MonoBehaviour
{
  public void PlayAgain()
  {
      SceneManager.LoadScene("Gameplay", LoadSceneMode.Single);
  }

  public void ToMenu()
  {
      SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
  }

  public void QuitGame()
  {
      Application.Quit();
  }
}
