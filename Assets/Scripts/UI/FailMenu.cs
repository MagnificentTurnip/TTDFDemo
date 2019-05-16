using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FailMenu : MonoBehaviour {

    public GameObject areYouSureMenu;

    public void RetryBoss() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToHamlet () {
        SceneManager.LoadScene("ClearingHamlet");
    }

    public void QuitAreYouSure() {
        areYouSureMenu.SetActive(true);
        Debug.Log("Cannot quit while in the Unity Editor");
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
            
	}
}
