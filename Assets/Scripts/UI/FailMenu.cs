using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FailMenu : MonoBehaviour {

    public void RetryBoss() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToHamlet () {
        print("Not yet implemented");
    }

    public void quitAreYouSure() {
        Debug.Log("Are you sure? Well you can't answer, you donut.");
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
            
	}
}
