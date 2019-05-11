using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {

    public void LoadClearingHamlet() {
        SceneManager.LoadScene("ClearingHamlet");
    }

    public void LoadWildwoodArena() {
        SceneManager.LoadScene("WildwoodArena");
    }

    public void LoadFieldOfEndlessGreen() {
        SceneManager.LoadScene("FieldOfEndlessGreen");
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetAxis("LCtrl") > 0 && Input.GetKey("s") && Input.GetKey("l")) { //commands for manually loading scenes, Ctrl+s+l+sceneNo
            if (Input.GetKey("1")) {
                LoadClearingHamlet();
            }
            if (Input.GetKey("2")) {
                LoadWildwoodArena();
            }
            if (Input.GetKey("3")) {
                LoadFieldOfEndlessGreen();
            }
        }
	}
}
