using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour {

    public static bool menuPaused = false;
    public static bool miniMenuPaused = false;
    public static bool paused = false;
    public GameObject pauseMenu;
    public GameObject miniPauseMenu;

    public void MenuResumeGame()
    {
        menuPaused = false;
        pauseMenu.SetActive(false);
        Resume();
    }

    public void MenuPauseGame()
    {
        menuPaused = true;
        pauseMenu.SetActive(true);
        Pause();
    }

    public void MiniResumeGame() {
        miniMenuPaused = false;
        miniPauseMenu.SetActive(false);
        Resume();
    }

    public void MiniPauseGame() {
        miniMenuPaused = true;
        miniPauseMenu.SetActive(true);
        Pause();
    }

    public void Pause() {
        Time.timeScale = 0f;
        paused = true;
    }

    public void Resume() {
        Time.timeScale = 1f;
        paused = false;
    }

    public void SettingsMenu()
    {
        Debug.Log("This is where I'd put the settings menu. IF I HAD ONE.");
    }

    public void QuitAreYouSure()
    {
        Debug.Log("Are you sure? Well you can't answer, you donut.");
    }

    public void QuitGame()
    {

    }

    public void NoQuit()
    {

    }

	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (menuPaused)
            {
                MenuResumeGame();
            } else
            {
                MenuPauseGame();
            }

        }
	}
}
