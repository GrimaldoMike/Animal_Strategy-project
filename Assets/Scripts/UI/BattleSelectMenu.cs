using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleSelectMenu : MonoBehaviour
{
    public string mainMenu;

    public void ReturnMainMenu()
    {
        SceneManager.LoadScene(mainMenu);
    }

    public void LoadBattle(string battleToLoad)
    {
        SceneManager.LoadScene(battleToLoad);
    }
}
