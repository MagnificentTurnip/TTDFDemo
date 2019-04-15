using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followMouse : MonoBehaviour {

    public float xOffset;
    public float yOffset;
    public float zOffset;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = new Vector3(Input.mousePosition.x + xOffset, Input.mousePosition.y + yOffset, Input.mousePosition.z + zOffset);
	}
}
