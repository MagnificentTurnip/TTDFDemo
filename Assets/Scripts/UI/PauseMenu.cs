using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour {

    public static bool menuPaused = false;
    public static bool miniMenuPaused = false;
    public static bool paused = false;
    public GameObject pauseMenu;
    public GameObject miniPauseMenu;
    public GameObject areYouSureMenu;

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
        Debug.Log("Settings Menu Not Yet Implemented");
    }

    public void QuitAreYouSure()
    {
        areYouSureMenu.SetActive(true);
        Debug.Log("Cannot quit while in the Unity Editor");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void NoQuit()
    {
        areYouSureMenu.SetActive(false);
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
