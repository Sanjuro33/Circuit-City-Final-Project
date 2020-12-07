using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelController : MonoBehaviour
{
    //Loads the Start Menu
    public void LoadStartMenuScene()
    {
        SceneManager.LoadScene(0);
    }

    //Loads the Game Scene
    public void LoadGameScene()
    {
        SceneManager.LoadScene(1);
    }

    //Loads the Win Screen
    public void LoadWinScreen()
    {
        SceneManager.LoadScene(2);
    }

    //Exits the application
    public void ExitGame()
    {
        Application.Quit();
    }
}
