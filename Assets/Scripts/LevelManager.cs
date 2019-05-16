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
		if (Input.GetKey("o") && Input.GetKey("p")) { //commands for manually loading scenes, o+p+sceneNo
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
