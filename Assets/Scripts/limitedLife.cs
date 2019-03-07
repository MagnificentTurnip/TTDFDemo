using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class limitedLife : MonoBehaviour {

    public int framesOfLife;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        framesOfLife--;
        if (framesOfLife <= 0) {
            Destroy(this.gameObject);
        }
	}
}
