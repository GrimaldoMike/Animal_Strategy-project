using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string levelToLoad;
    public GameObject optionsScreen, creditsScreen;

    public void StartGame()
    {
        SceneManager.LoadScene(levelToLoad);
    }

    public void OpenCredits()
    {
        creditsScreen.SetActive(true);
    }
    public void CloseCredits()
    {
        creditsScreen.SetActive(false);
    }
    public void OpenOptions()
    {
        optionsScreen.SetActive(true);
    }
    public void CloseOptions()
    {
        optionsScreen.SetActive(false);
    }
    public void QuitGame()
    {
        Application.Quit();

        Debug.Log("Quiting Game");
    }
}
