using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour { //a class to handle the player's animation events

    public Animator animator;
    public StatusManager status;
    public PlayerMovement movement;

    public GameObject stumpo;

    public bool stickToStumpo;

    public void SetStumpoStick() {
        stickToStumpo = true;
    }

	// Use this for initialization
	void Start () {
        stickToStumpo = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (stickToStumpo) {
            animator.SetBool("sitting", true);
            transform.LookAt(stumpo.transform);
            transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y + 180, 0);
            if (Vector3.Distance(transform.position, stumpo.transform.position) > 3) {
                transform.Translate(Vector3.Normalize(stumpo.transform.position - transform.position) * 0.05f, Space.World);
            }
        }


    }
}
