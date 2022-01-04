using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu_Pause : MonoBehaviour
{
    public static bool IsPaused = false;//Pause state
    public GameObject pauseMenuUI;//Pause Menu
    public GameObject optionsMenuUI;//Options Menu
    public GameObject KeyBindingUI;//Key Binding Menu
    public GameObject keyBindingOption; //KB menu
    public GameObject gamePlayUI;//Game play menue
    public Game_Master game_Master;

    public void Start()
    {
       /* if (!game_Master.premiumUser)
        {
            keyBindingOption.SetActive(false);
        }
        else
        {
            keyBindingOption.SetActive(true);
        }*/
    }

    void Update()
    {
       if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(IsPaused)
                Resume();
            else
                Pause();
        }
    }

    //Resume the game.
    public void Resume()
    {
        //Deactivate all panels when key pressed.
        if(optionsMenuUI.activeSelf)
            optionsMenuUI.SetActive(false);
        if (KeyBindingUI.activeSelf)
            KeyBindingUI.SetActive(false);

        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        IsPaused = false;
    }

    //Pause the game.
    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        IsPaused = true;
    }

    //Loading the main menu.
    public void LoadMenu()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
        Debug.Log("Loading menu...please wait");
    }
}
