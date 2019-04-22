using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oscillate : MonoBehaviour {

    public float x;
    public float y;
    public float z;

    public int delayBeforeBegin;

    public int delay;
    private int counter;

	// Use this for initialization
	void Start () {
        counter = 0;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (delayBeforeBegin <= 0) {
            counter++;
            if (counter >= delay) {
                counter = 0;
                x = -x;
                y = -y;
                z = -z;
            }
            transform.Translate(x, y, z);
        } else {
            delayBeforeBegin--;
        }
        
	}
}
