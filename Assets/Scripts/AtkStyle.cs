using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtkStyle : MonoBehaviour {

    public enum attackStates { idle }
    public attackStates state;
    public int idleCounter; //the current number of frames until the entity returns to idle after a move

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
