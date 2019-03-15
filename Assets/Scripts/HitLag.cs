using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitLag : MonoBehaviour {

    public int counter;

	// Use this for initialization
	void Start () {
        counter = 0;
	}
	
	// FixedUpdate is called once per physics frame
	void FixedUpdate () {
        counter--;
        if (counter > 0) {
            Time.timeScale = 0.1f;
        }
        else {
            Time.timeScale = 1;
        }
	}
}
