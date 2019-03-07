using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour {

    public static bool paused = false;
    public GameObject pauseMenu;

    public void resumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        paused = false;
    }

    public void pauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        paused = true;
    }

    public void settingsMenu()
    {
        Debug.Log("This is where I'd put the settings menu. IF I HAD ONE.");
    }

    public void quitAreYouSure()
    {
        Debug.Log("Are you sure? Well you can't answer, you donut.");
    }

    public void quitGame()
    {

    }

    public void noQuit()
    {

    }

	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused)
            {
                resumeGame();
            } else
            {
                pauseGame();
            }

        }
	}
}
